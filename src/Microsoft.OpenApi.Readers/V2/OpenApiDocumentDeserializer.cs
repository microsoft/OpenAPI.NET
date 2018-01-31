// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Collections.Generic;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers.ParseNodes;

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
                (o, n) => n.Context.SetTempStorage("globalconsumes", n.CreateSimpleList(s => s.GetScalarValue()))
            },
            {
                "produces",
                (o, n) => n.Context.SetTempStorage("globalproduces", n.CreateSimpleList(s => s.GetScalarValue()))
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
                        "#/definitions/",
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
                        "#/parameters/",
                        LoadParameter);
                }
            },
            {
                "responses", (o, n) =>
                {
                    if (o.Components == null)
                    {
                        o.Components = new OpenApiComponents();
                    }

                    o.Components.Responses = n.CreateMap(LoadResponse);
                }
            },
            {
                "securityDefinitions", (o, n) =>
                {
                    if (o.Components == null)
                    {
                        o.Components = new OpenApiComponents();
                    }

                    o.Components.SecuritySchemes = n.CreateMap(LoadSecurityScheme);
                }
            },
            {"security", (o, n) => o.SecurityRequirements = n.CreateList(LoadSecurityRequirement)},
            {"tags", (o, n) => o.Tags = n.CreateList(LoadTag)},
            {"externalDocs", (o, n) => o.ExternalDocs = LoadExternalDocs(n)}
        };

        private static PatternFieldMap<OpenApiDocument> _openApiPatternFields = new PatternFieldMap<OpenApiDocument>
        {
            // We have no semantics to verify X- nodes, therefore treat them as just values.
            {s => s.StartsWith("x-"), (o, p, n) => o.AddExtension(p, n.CreateAny())}
        };

        private static void MakeServers(IList<OpenApiServer> servers, ParsingContext context)
        {
            var host = context.GetFromTempStorage<string>("host");
            var basePath = context.GetFromTempStorage<string>("basePath");
            var schemes = context.GetFromTempStorage<List<string>>("schemes");

            if (schemes != null)
            {
                foreach (var scheme in schemes)
                {
                    var server = new OpenApiServer();
                    server.Url = scheme + "://" + (host ?? "example.org/") + (basePath ?? "/");
                    servers.Add(server);
                }
            }
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

            MakeServers(openApidoc.Servers, openApiNode.Context);

            return openApidoc;
        }
    }
}