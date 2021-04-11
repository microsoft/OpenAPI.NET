// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers.ParseNodes;

namespace Microsoft.OpenApi.Readers.V3
{
    /// <summary>
    /// Class containing logic to deserialize Open API V3 document into
    /// runtime Open API object model.
    /// </summary>
    internal static partial class OpenApiV3Deserializer
    {
        private static readonly FixedFieldMap<OpenApiExternalDocs> _externalDocsFixedFields =
            new FixedFieldMap<OpenApiExternalDocs>
            {
                // $ref
                {
                    "description", (o, n) =>
                    {
                        o.Description = n.GetScalarValue();
                    }
                },
                {
                    "url", (o, n) =>
                    {
                        o.Url = new Uri(n.GetScalarValue(), UriKind.RelativeOrAbsolute);
                    }
                },
            };

    private static readonly PatternFieldMap<OpenApiExternalDocs> _externalDocsPatternFields =
            new PatternFieldMap<OpenApiExternalDocs> {

                    {s => s.StartsWith("x-"), (o, p, n) => o.AddExtension(p, LoadExtension(p, n))}
                };

    public static OpenApiExternalDocs LoadExternalDocs(ParseNode node)
        {
            var mapNode = node.CheckMapNode("externalDocs");

            var externalDocs = new OpenApiExternalDocs();

            ParseMap(mapNode, externalDocs, _externalDocsFixedFields, _externalDocsPatternFields);

            return externalDocs;
        }
    }
}
