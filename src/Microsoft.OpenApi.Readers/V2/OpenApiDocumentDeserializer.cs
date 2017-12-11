// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System;
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
        public static FixedFieldMap<OpenApiDocument> OpenApiFixedFields = new FixedFieldMap<OpenApiDocument>
        {
            {
                "swagger", (o, n) =>
                {
                    o.SpecVersion = new Version(2, 0);
                }
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
                (o, n) => o.Components.Schemas = n.CreateMapWithReference(
                    ReferenceType.Schema,
                    "#/definitions/",
                    LoadSchema)
            },
            {
                "parameters",
                (o, n) => o.Components.Parameters = n.CreateMapWithReference(
                    ReferenceType.Parameter,
                    "#/parameters/",
                    LoadParameter)
            },
            {"responses", (o, n) => o.Components.Responses = n.CreateMap(LoadResponse)},
            {"securityDefinitions", (o, n) => o.Components.SecuritySchemes = n.CreateMap(LoadSecurityScheme)},
            {"security", (o, n) => o.SecurityRequirements = n.CreateList(LoadSecurityRequirement)},
            {"tags", (o, n) => o.Tags = n.CreateList(LoadTag)},
            {"externalDocs", (o, n) => o.ExternalDocs = LoadExternalDocs(n)}
        };

        public static PatternFieldMap<OpenApiDocument> OpenApiPatternFields = new PatternFieldMap<OpenApiDocument>
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

            var required = new List<string> {"info", "swagger", "paths"};

            ParseMap(openApiNode, openApidoc, OpenApiFixedFields, OpenApiPatternFields, required);

            ReportMissing(openApiNode, required);

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