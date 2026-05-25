// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Text.Json.Nodes;

using System;

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
                    (o, n, _, c) =>
                    {
                        var description = n.GetScalarValue();
                        if (description != null)
                        {
                            o.Description = n.GetScalarValue(); 
                        } 
                    }
                },
                {
                    OpenApiConstants.Url,
                    (o, n, _, c) =>
                    {
                        var url = n.GetScalarValue();
                        if (url != null)
                        {
                            o.Url = new(url, UriKind.RelativeOrAbsolute); 
                        } 
                    }
                },
            };

        private static readonly PatternFieldMap<OpenApiExternalDocs> _externalDocsPatternFields =
                new()
                {
                    {s => s.StartsWith(OpenApiConstants.ExtensionFieldNamePrefix, StringComparison.OrdinalIgnoreCase), (o, p, n, _, c) => o.AddExtension(p, LoadExtension(p, n, c))}
                    };

        public static OpenApiExternalDocs LoadExternalDocs(JsonNode node, OpenApiDocument hostDocument, ParsingContext context)
        {
            var JsonObject = node.CheckMapNode("externalDocs", context);

            var externalDocs = new OpenApiExternalDocs();

            ParseMap(JsonObject, externalDocs, _externalDocsFixedFields, _externalDocsPatternFields, hostDocument, context);

            return externalDocs;
        }
    }
}
