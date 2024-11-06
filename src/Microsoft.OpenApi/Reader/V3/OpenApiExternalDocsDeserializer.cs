// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
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
        private static readonly FixedFieldMap<OpenApiExternalDocs> _externalDocsFixedFields =
            new()
            {
                // $ref
                {
                    "description",
                    (o, n, _) => o.Description = n.GetScalarValue()
                },
                {
                    "url",
                    (o, n, _) => o.Url = new(n.GetScalarValue(), UriKind.RelativeOrAbsolute)
                },
            };

    private static readonly PatternFieldMap<OpenApiExternalDocs> _externalDocsPatternFields =
            new()
            {
                    {s => s.StartsWith("x-"), (o, p, n, _) => o.AddExtension(p, LoadExtension(p, n))}
                    };

        public static OpenApiExternalDocs LoadExternalDocs(ParseNode node, OpenApiDocument hostDocument = null)
        {
            var mapNode = node.CheckMapNode("externalDocs");

            var externalDocs = new OpenApiExternalDocs();

            ParseMap(mapNode, externalDocs, _externalDocsFixedFields, _externalDocsPatternFields, hostDocument);

            return externalDocs;
        }
    }
}
