// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Text.Json.Nodes;

using System;

namespace Microsoft.OpenApi.Reader.V32
{
    /// <summary>
    /// Class containing logic to deserialize Open API V32 document into
    /// runtime Open API object model.
    /// </summary>
    internal static partial class OpenApiV32Deserializer
    {
        public static readonly FixedFieldMap<OpenApiResponses> ResponsesFixedFields = new();

        public static readonly PatternFieldMap<OpenApiResponses> ResponsesPatternFields = new()
        {
            {s => !s.StartsWith(OpenApiConstants.ExtensionFieldNamePrefix, StringComparison.OrdinalIgnoreCase), (o, p, n, t, c) => o.Add(p, LoadResponse(n, t, c))},
            {s => s.StartsWith(OpenApiConstants.ExtensionFieldNamePrefix, StringComparison.OrdinalIgnoreCase), (o, p, n, _, c) => o.AddExtension(p, LoadExtension(p, n, c))}
        };

        public static OpenApiResponses LoadResponses(JsonNode node, OpenApiDocument hostDocument, ParsingContext context)
        {
            var jsonObject = node.CheckMapNode("Responses", context);

            var domainObject = new OpenApiResponses();

            ParseMap(jsonObject, domainObject, ResponsesFixedFields, ResponsesPatternFields, hostDocument, context);

            return domainObject;
        }
    }
}

