// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Reader.ParseNodes;

namespace Microsoft.OpenApi.Reader.V3
{
    /// <summary>
    /// Class containing logic to deserialize Open API V3 document into
    /// runtime Open API object model.
    /// </summary>
    internal static partial class OpenApiV3Deserializer
    {
        private static readonly FixedFieldMap<OpenApiDocument> _openApiFixedFields = new()
        {
            {
                "openapi", (_, _) =>
                {
                } /* Version is valid field but we already parsed it */
            },
            {"info", (o, n) => o.Info = LoadInfo(n)},
            {"servers", (o, n) => o.Servers = n.CreateList(LoadServer)},
            {"paths", (o, n) => o.Paths = LoadPaths(n)},
            {"components", (o, n) => o.Components = LoadComponents(n)},
            {"tags", (o, n) => {o.Tags = n.CreateList(LoadTag);
                foreach (var tag in o.Tags)
                {
                    tag.Reference = new()
                    {
                        Id = tag.Name,
                        Type = ReferenceType.Tag
                    };
                }
            } },
            {"externalDocs", (o, n) => o.ExternalDocs = LoadExternalDocs(n)},
            {"security", (o, n) => o.SecurityRequirements = n.CreateList(LoadSecurityRequirement)}
        };

        private static readonly PatternFieldMap<OpenApiDocument> _openApiPatternFields = new PatternFieldMap<OpenApiDocument>
        {
            // We have no semantics to verify X- nodes, therefore treat them as just values.
            {s => s.StartsWith("x-"), (o, p, n) => o.AddExtension(p, LoadExtension(p, n))}
        };

        public static OpenApiDocument LoadOpenApi(RootNode rootNode)
        {
            var openApidoc = new OpenApiDocument();
            var openApiNode = rootNode.GetMap();

            ParseMap(openApiNode, openApidoc, _openApiFixedFields, _openApiPatternFields);

            //if (openApidoc.Components != null)
            //{
            //    openApidoc.Workspace.RegisterComponents(openApidoc);
            //}

            return openApidoc;
        }
    }
}
