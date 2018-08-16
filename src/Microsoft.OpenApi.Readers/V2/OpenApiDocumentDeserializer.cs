// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers.ParseNodes;
using Microsoft.OpenApi.Services;

namespace Microsoft.OpenApi.Readers.V2
{
    /// <summary>
    /// Class containing logic to deserialize Open API V2 document into
    /// runtime Open API object model.
    /// </summary>
    internal static partial class OpenApiV2Deserializer
    {
        private static FixedFieldMap<OpenApiDocument> _openApiFixedFields = new FixedFieldMap<OpenApiDocument>
        {
            {
                "swagger", (o, n) =>
                {
                } /* Version is valid field but we already parsed it */
            },
            {"info", (o, n) => o.Info = LoadInfo(n)},
            {"host", (o, n) => n.Context.SetTempStorage("host", n.GetScalarValue())},
            {"basePath", (o, n) => n.Context.SetTempStorage("basePath", n.GetScalarValue())},
            {
                "schemes", (o, n) => n.Context.SetTempStorage(
                    "schemes",
                    n.CreateSimpleList(
                        s => s.GetScalarValue()))
            },
            {
                "consumes",
                (o, n) => n.Context.SetTempStorage(TempStorageKeys.GlobalConsumes, n.CreateSimpleList(s => s.GetScalarValue()))
            },
            {
                "produces",
                (o, n) => n.Context.SetTempStorage(TempStorageKeys.GlobalProduces, n.CreateSimpleList(s => s.GetScalarValue()))
            },
            {"paths", (o, n) => o.Paths = LoadPaths(n)},
            {
                "definitions",
                (o, n) =>
                {
                    if (o.Components == null)
                    {
                        o.Components = new OpenApiComponents();
                    }

                    o.Components.Schemas = n.CreateMapWithReference(
                        ReferenceType.Schema,
                        LoadSchema);
                }
            },
            {
                "parameters",
                (o, n) =>
                {
                    if (o.Components == null)
                    {
                        o.Components = new OpenApiComponents();
                    }

                    o.Components.Parameters = n.CreateMapWithReference(
                        ReferenceType.Parameter,
                        LoadParameter);

                    o.Components.RequestBodies = n.CreateMapWithReference(ReferenceType.RequestBody, p => 
                            {
                                var parameter = LoadParameter(p, evenBody: true);
                                if (parameter.In == null)
                                {
                                    return CreateRequestBody(n.Context,parameter); 
                                }
                                return null;
                            }
                      );
                }
            },
            {
                "responses", (o, n) =>
                {
                    if (o.Components == null)
                    {
                        o.Components = new OpenApiComponents();
                    }

                    o.Components.Responses = n.CreateMapWithReference(
                        ReferenceType.Response,
                        LoadResponse);
                }
            },
            {
                "securityDefinitions", (o, n) =>
                {
                    if (o.Components == null)
                    {
                        o.Components = new OpenApiComponents();
                    }

                    o.Components.SecuritySchemes = n.CreateMapWithReference(
                        ReferenceType.SecurityScheme,
                        LoadSecurityScheme
                        );
                }
            },
            {"security", (o, n) => o.SecurityRequirements = n.CreateList(LoadSecurityRequirement)},
            {"tags", (o, n) => o.Tags = n.CreateList(LoadTag)},
            {"externalDocs", (o, n) => o.ExternalDocs = LoadExternalDocs(n)}
        };

        private static PatternFieldMap<OpenApiDocument> _openApiPatternFields = new PatternFieldMap<OpenApiDocument>
        {
            // We have no semantics to verify X- nodes, therefore treat them as just values.
            {s => s.StartsWith("x-"), (o, p, n) => o.AddExtension(p, LoadExtension(p, n))}
        };

        private static void MakeServers(IList<OpenApiServer> servers, ParsingContext context, Uri defaultUrl)
        {
            var host = context.GetFromTempStorage<string>("host");
            var basePath = context.GetFromTempStorage<string>("basePath");
            var schemes = context.GetFromTempStorage<List<string>>("schemes");

            // If nothing is provided, don't create a server
            if (host == null && basePath == null && schemes == null)
            {
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
            if (schemes != null && schemes.Count > 0)
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
            if (String.IsNullOrEmpty(scheme) && !String.IsNullOrEmpty(host))
            {
                host = "//" + host;  // The double slash prefix creates a relative url where the scheme is defined by the BaseUrl
            }

            int? port = null;

            if (!String.IsNullOrEmpty(host) && host.Contains(":"))
            {
                var pieces = host.Split(':');
                host = pieces.First();
                port = int.Parse(pieces.Last());
            }

            var uriBuilder = new UriBuilder()
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
            var openApidoc = new OpenApiDocument();

            var openApiNode = rootNode.GetMap();

            ParseMap(openApiNode, openApidoc, _openApiFixedFields, _openApiPatternFields);


            // Post Process OpenApi Object
            if (openApidoc.Servers == null)
            {
                openApidoc.Servers = new List<OpenApiServer>();
            }

            MakeServers(openApidoc.Servers, openApiNode.Context, rootNode.Context.BaseUrl);

            FixRequestBodyReferences(openApidoc);
            return openApidoc;
        }

        private static void FixRequestBodyReferences(OpenApiDocument doc)
        {
            // Walk all unresolved parameter references
            // if id matches with request body Id, change type
            if (doc.Components?.RequestBodies != null && doc.Components?.RequestBodies.Count > 0)
            {
                var fixer = new RequestBodyReferenceFixer(doc.Components?.RequestBodies);
                var walker = new OpenApiWalker(fixer);
                walker.Walk(doc);
            }
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
            var body = operation.Parameters.Where(p => p.UnresolvedReference == true
            && _requestBodies.ContainsKey(p.Reference.Id)).FirstOrDefault();

            if (body != null)
            {
                operation.Parameters.Remove(body);
                operation.RequestBody = new OpenApiRequestBody()
                {
                    UnresolvedReference = true,
                    Reference = new OpenApiReference()
                    {
                        Id = body.Reference.Id,
                        Type = ReferenceType.RequestBody
                    }
                };
            }
        }

      
    }
}