// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Reader.ParseNodes;
using Microsoft.OpenApi.Services;

namespace Microsoft.OpenApi.Reader.V2
{
    /// <summary>
    /// Class containing logic to deserialize Open API V2 document into
    /// runtime Open API object model.
    /// </summary>
    internal static partial class OpenApiV2Deserializer
    {
        private static readonly FixedFieldMap<OpenApiDocument> _openApiFixedFields = new()
        {
            {
                "swagger", (_, _, _) => {}
                /* Version is valid field but we already parsed it */
            },
            {"info", (o, n, _) => o.Info = LoadInfo(n, o)},
            {"host", (_, n, _) => n.Context.SetTempStorage("host", n.GetScalarValue())},
            {"basePath", (_, n, _) => n.Context.SetTempStorage("basePath", n.GetScalarValue())},
            {
                "schemes", (_, n, _) => n.Context.SetTempStorage(
                    "schemes",
                    n.CreateSimpleList(
                        (s, p) => s.GetScalarValue()))
            },
            {
                "consumes",
                (_, n, _) =>
                {
                    var consumes = n.CreateSimpleList((s, p) => s.GetScalarValue());
                    if (consumes.Count > 0)
                    {
                        n.Context.SetTempStorage(TempStorageKeys.GlobalConsumes, consumes);
                    }
                }
            },
            {
                "produces", (_, n, _) => {
                    var produces = n.CreateSimpleList((s, p) => s.GetScalarValue());
                    if (produces.Count > 0)
                    {
                        n.Context.SetTempStorage(TempStorageKeys.GlobalProduces, produces);
                    }
                }
            },
            {"paths", (o, n, _) => o.Paths = LoadPaths(n, o)},
            {
                "definitions",
                (o, n, _) =>
                {
                    o.Components ??= new();
                    o.Components.Schemas = n.CreateMap(LoadSchema, o);
                }
            },
            {
                "parameters",
                (o, n, _) =>
                {
                    if (o.Components == null)
                    {
                        o.Components = new();
                    }

                    o.Components.Parameters = n.CreateMap(LoadParameter, o);

                    o.Components.RequestBodies = n.CreateMap((p, d) =>
                            {
                                var parameter = LoadParameter(node: p, loadRequestBody: true, hostDocument: d);
                                return parameter != null ? CreateRequestBody(p.Context, parameter) : null;
                            }
                      );
                }
            },
            {
                "responses", (o, n, _) =>
                {
                    if (o.Components == null)
                    {
                        o.Components = new();
                    }

                    o.Components.Responses = n.CreateMap(LoadResponse, o);
                }
            },
            {
                "securityDefinitions", (o, n, _) =>
                {
                    if (o.Components == null)
                    {
                        o.Components = new();
                    }

                    o.Components.SecuritySchemes = n.CreateMap(LoadSecurityScheme, o);
                }
            },
            {"security", (o, n, _) => o.SecurityRequirements = n.CreateList(LoadSecurityRequirement, o)},
            {"tags", (o, n, _) => o.Tags = n.CreateList(LoadTag, o)},
            {"externalDocs", (o, n, _) => o.ExternalDocs = LoadExternalDocs(n, o)}
        };

        private static readonly PatternFieldMap<OpenApiDocument> _openApiPatternFields = new()
        {
            // We have no semantics to verify X- nodes, therefore treat them as just values.
            {s => s.StartsWith("x-"), (o, p, n, _) => o.AddExtension(p, LoadExtension(p, n))}
        };

        private static void MakeServers(IList<OpenApiServer> servers, ParsingContext context, RootNode rootNode)
        {
            var host = context.GetFromTempStorage<string>("host");
            var basePath = context.GetFromTempStorage<string>("basePath");
            var schemes = context.GetFromTempStorage<List<string>>("schemes");
            var defaultUrl = rootNode.Context.BaseUrl;

            // so we don't default to the document path when a host is provided
            if (string.IsNullOrEmpty(basePath) && !string.IsNullOrEmpty(host))
            {
                basePath = "/";
            }

            // If nothing is provided, don't create a server
            if (host == null && basePath == null && schemes == null)
            {
                return;
            }

            //Validate host
            if (host != null && !IsHostValid(host))
            {
                rootNode.Context.Diagnostic.Errors.Add(new(rootNode.Context.GetLocation(), "Invalid host"));
                return;
            }

            // Fill in missing information based on the defaultUrl
            if (defaultUrl != null)
            {
                host = host ?? defaultUrl.GetComponents(UriComponents.NormalizedHost, UriFormat.SafeUnescaped);
                basePath = basePath ?? defaultUrl.GetComponents(UriComponents.Path, UriFormat.SafeUnescaped);
                schemes = schemes ?? new List<string> { defaultUrl.GetComponents(UriComponents.Scheme, UriFormat.SafeUnescaped) };
            }
            else if (String.IsNullOrEmpty(host) && String.IsNullOrEmpty(basePath))
            {
                return;  // Can't make a server object out of just a Scheme
            }

            // Create the Server objects
            if (schemes is {Count: > 0})
            {
                foreach (var scheme in schemes)
                {
                    var server = new OpenApiServer
                    {
                        Url = BuildUrl(scheme, host, basePath)
                    };

                    servers.Add(server);
                }
            }
            else
            {
                var server = new OpenApiServer
                {
                    Url = BuildUrl(null, host, basePath)
                };

                servers.Add(server);
            }

            foreach (var server in servers)
            {
                // Server Urls are always appended to Paths and Paths must start with /
                // so removing the slash prevents a double slash.
                if (server.Url.EndsWith("/"))
                {
                    server.Url = server.Url.Substring(0, server.Url.Length - 1);
                }
            }
        }

        private static string BuildUrl(string scheme, string host, string basePath)
        {
            if (string.IsNullOrEmpty(scheme) && !string.IsNullOrEmpty(host))
            {
                host = "//" + host;  // The double slash prefix creates a relative url where the scheme is defined by the BaseUrl
            }

            int? port = null;

#if NETSTANDARD2_1_OR_GREATER
            if (!String.IsNullOrEmpty(host) && host.Contains(':', StringComparison.OrdinalIgnoreCase))
#else
            if (!String.IsNullOrEmpty(host) && host.Contains(':'))
#endif
            {
                var pieces = host.Split(':');
                host = pieces.First();
                port = int.Parse(pieces.Last(), CultureInfo.InvariantCulture);
            }

            var uriBuilder = new UriBuilder
            {
                Scheme = scheme,
                Host = host,
                Path = basePath
            };

            if (port != null)
            {
                uriBuilder.Port = port.Value;
            }

            return uriBuilder.ToString();
        }

        public static OpenApiDocument LoadOpenApi(RootNode rootNode)
        {
            var openApiDoc = new OpenApiDocument();

            var openApiNode = rootNode.GetMap();

            ParseMap(openApiNode, openApiDoc, _openApiFixedFields, _openApiPatternFields);

            if (openApiDoc.Paths != null)
            {
                ProcessResponsesMediaTypes(
                    rootNode.GetMap(),
                    openApiDoc.Paths.Values
                        .SelectMany(path => path.Operations?.Values ?? Enumerable.Empty<OpenApiOperation>())
                        .SelectMany(operation => operation.Responses?.Values ?? Enumerable.Empty<OpenApiResponse>()),
                    openApiNode.Context);
            }

            ProcessResponsesMediaTypes(rootNode.GetMap(), openApiDoc.Components?.Responses?.Values, openApiNode.Context);

            // Post Process OpenApi Object
            if (openApiDoc.Servers == null)
            {
                openApiDoc.Servers = new List<OpenApiServer>();
            }

            MakeServers(openApiDoc.Servers, openApiNode.Context, rootNode);

            FixRequestBodyReferences(openApiDoc);

            // Register components
            openApiDoc.Workspace.RegisterComponents(openApiDoc);

            return openApiDoc;
        }

        private static void ProcessResponsesMediaTypes(MapNode mapNode, IEnumerable<OpenApiResponse> responses, ParsingContext context)
        {
            if (responses != null)
            {
                foreach (var response in responses)
                {
                    ProcessProduces(mapNode, response, context);

                    if (response.Content != null)
                    {
                        foreach (var mediaType in response.Content.Values)
                        {
                            ProcessAnyFields(mapNode, mediaType, _mediaTypeAnyFields);
                        }
                    }
                }
            }
        }

        private static void FixRequestBodyReferences(OpenApiDocument doc)
        {
            // Walk all unresolved parameter references
            // if id matches with request body Id, change type
            if (doc.Components?.RequestBodies is {Count: > 0})
            {
                var fixer = new RequestBodyReferenceFixer(doc.Components?.RequestBodies);
                var walker = new OpenApiWalker(fixer);
                walker.Walk(doc);
            }
        }

        private static bool IsHostValid(string host)
        {
            //Check if the host contains ://
            if (host.Contains(Uri.SchemeDelimiter))
            {
                return false;
            }

            //Check if the host (excluding port number) is a valid dns/ip address.
            var hostPart = host.Split(':').First();
            return Uri.CheckHostName(hostPart) != UriHostNameType.Unknown;
        }
    }

    internal class RequestBodyReferenceFixer : OpenApiVisitorBase
    {
        private IDictionary<string, OpenApiRequestBody> _requestBodies;
        public RequestBodyReferenceFixer(IDictionary<string, OpenApiRequestBody> requestBodies)
        {
            _requestBodies = requestBodies;
        }

        public override void Visit(OpenApiOperation operation)
        {
            var body = operation.Parameters.FirstOrDefault(
                p => p.UnresolvedReference
                     && _requestBodies.ContainsKey(p.Reference.Id));

            if (body != null)
            {
                operation.Parameters.Remove(body);
                operation.RequestBody = new()
                {
                    UnresolvedReference = true,
                    Reference = new()
                    {
                        Id = body.Reference.Id,
                        Type = ReferenceType.RequestBody
                    }
                };
            }
        }
    }
}
