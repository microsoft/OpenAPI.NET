// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using Microsoft.OpenApi.Extensions;

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
                "openapi", (_, _, _) =>
                {
                } /* Version is valid field but we already parsed it */
            },
            {"info", (o, n, _) => o.Info = LoadInfo(n, o)},
            {"servers", (o, n, _) => o.Servers = n.CreateList(LoadServer, o)},
            {"paths", (o, n, _) => o.Paths = LoadPaths(n, o)},
            {"components", (o, n, _) => o.Components = LoadComponents(n, o)},
            {"tags", (o, n, _) => { if (n.CreateList(LoadTag, o) is {Count:> 0} tags) {o.Tags = new HashSet<OpenApiTag>(tags, OpenApiTagComparer.Instance); } } },
            {"externalDocs", (o, n, _) => o.ExternalDocs = LoadExternalDocs(n, o)},
            {"security", (o, n, _) => o.Security = n.CreateList(LoadSecurityRequirement, o)}
        };

        private static readonly PatternFieldMap<OpenApiDocument> _openApiPatternFields = new PatternFieldMap<OpenApiDocument>
        {
            // We have no semantics to verify X- nodes, therefore treat them as just values.
            {s => s.StartsWith(OpenApiConstants.ExtensionFieldNamePrefix, StringComparison.OrdinalIgnoreCase), (o, p, n, _) => o.AddExtension(p, LoadExtension(p, n))}
        };

        public static OpenApiDocument LoadOpenApi(RootNode rootNode, Uri location)
        {
            var openApiDoc = new OpenApiDocument
            {
                BaseUri = location
            };
            var openApiNode = rootNode.GetMap();

            ParseMap(openApiNode, openApiDoc, _openApiFixedFields, _openApiPatternFields, openApiDoc);

            // Register components
            openApiDoc.Workspace?.RegisterComponents(openApiDoc);

            return openApiDoc;
        }
    }
}
