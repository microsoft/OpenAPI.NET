// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Reader.ParseNodes;

namespace Microsoft.OpenApi.Reader.V2
{
    /// <summary>
    /// Class containing logic to deserialize Open API V2 document into
    /// runtime Open API object model.
    /// </summary>
    internal static partial class OpenApiV2Deserializer
    {
        private static readonly FixedFieldMap<OpenApiExternalDocs> _externalDocsFixedFields =
            new()
            {
                {
                    OpenApiConstants.Description,
                    (o, n, _) => o.Description = n.GetScalarValue()
                },
                {
                    OpenApiConstants.Url,
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

            ParseMap(mapNode, externalDocs, _externalDocsFixedFields, _externalDocsPatternFields);

            return externalDocs;
        }
    }
}
