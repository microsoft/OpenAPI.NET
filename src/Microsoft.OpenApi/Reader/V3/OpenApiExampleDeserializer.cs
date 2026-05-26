// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Text.Json.Nodes;

using System;

namespace Microsoft.OpenApi.Reader.V3
{
    /// <summary>
    /// Class containing logic to deserialize Open API V3 document into
    /// runtime Open API object model.
    /// </summary>
    internal static partial class OpenApiV3Deserializer
    {
        private static readonly FixedFieldMap<OpenApiExample> _exampleFixedFields = new()
        {
            {
                "summary",
                (o, n, _, c) => o.Summary = n.GetScalarValue()
            },
            {
                "description",
                (o, n, _, c) => o.Description = n.GetScalarValue()
            },
            {
                "value",
                (o, n, _, c) => o.Value = n
            },
            {
                "externalValue",
                (o, n, _, c) => o.ExternalValue = n.GetScalarValue()
            },
        };

        private static readonly PatternFieldMap<OpenApiExample> _examplePatternFields =
            new()
            {
                {s => s.Equals("x-oai-dataValue", StringComparison.OrdinalIgnoreCase), (o, p, n, _, c) => o.DataValue = n},
                {s => s.Equals("x-oai-serializedValue", StringComparison.OrdinalIgnoreCase), (o, p, n, _, c) => o.SerializedValue = n.GetScalarValue()},
                {s => s.StartsWith(OpenApiConstants.ExtensionFieldNamePrefix, StringComparison.OrdinalIgnoreCase), (o, p, n, _, c) => o.AddExtension(p, LoadExtension(p, n, c))}
            };

        public static IOpenApiExample LoadExample(JsonNode node, OpenApiDocument hostDocument, ParsingContext context)
        {
            var JsonObject = node.CheckMapNode("example", context);

            var pointer = JsonObject.GetReferencePointer();
            if (pointer != null)
            {
                var reference = GetReferenceIdAndExternalResource(pointer);
                return new OpenApiExampleReference(reference.Item1, hostDocument, reference.Item2);
            }

            var example = new OpenApiExample();
            ParseMap(JsonObject, example, _exampleFixedFields, _examplePatternFields, hostDocument, context);

            return example;
        }
    }
}
