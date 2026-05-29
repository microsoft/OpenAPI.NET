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
        private static readonly FixedFieldMap<OpenApiTag> _tagFixedFields = new()
        {
            {
                OpenApiConstants.Name,
                (o, n, _, c) => o.Name = n.GetScalarValue()
            },
            {
                OpenApiConstants.Description,
                (o, n, _, c) => o.Description = n.GetScalarValue()
            },
            {
                OpenApiConstants.ExternalDocs,
                (o, n, t, c) => o.ExternalDocs = LoadExternalDocs(n, t, c)
            }
        };

        private static readonly PatternFieldMap<OpenApiTag> _tagPatternFields = new()
        {
            {s => s.StartsWith(OpenApiConstants.ExtensionFieldNamePrefix, StringComparison.OrdinalIgnoreCase), (o, p, n, _, c) => o.AddExtension(p, LoadExtension(p, n, c))}
        };

        public static OpenApiTag LoadTag(JsonNode n, OpenApiDocument hostDocument, ParsingContext context)
        {
            var jsonObject = n.CheckMapNode("tag", context);

            var domainObject = new OpenApiTag();

            ParseMap(jsonObject, domainObject, _tagFixedFields, _tagPatternFields, hostDocument, context);

            return domainObject;
        }
    }
}
