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
        private static readonly FixedFieldMap<OpenApiHeader> _headerFixedFields = new()
        {
            {
                "description",
                (o, n, _, _) => o.Description = n.GetScalarValue()
            },
            {
                "required",
                (o, n, _, _) =>
                {
                    o.Required = n.GetScalarBoolValue();
                }
            },
            {
                "deprecated",
                (o, n, _, _) =>
                {
                    o.Deprecated = n.GetScalarBoolValue();
                }
            },
            {
                "allowEmptyValue",
                (o, n, _, _) =>
                {
                    o.AllowEmptyValue = n.GetScalarBoolValue();
                }
            },
            {
                "allowReserved",
                (o, n, _, _) =>
                {
                    o.AllowReserved = n.GetScalarBoolValue();
                }
            },
            {
                "style",
                (o, n, _, c) => 
                {
                    if(!n.GetScalarValue().TryGetEnumFromDisplayName<ParameterStyle>(c, out var style))
                    {
                        return;
                    }
                    o.Style = style;
                }
            },
            {
                "explode",
                (o, n, _, _) =>
                {
                    o.Explode = n.GetScalarBoolValue();
                }
            },
            {
                "schema",
                (o, n, t, c) => o.Schema = LoadSchema(n, t, c)
            },
            {
                "content", (o, n, t, c) =>
                {
                    o.Content = n.CreateMap(LoadMediaType, t, c);
                }
            },
            {
                "examples",
                (o, n, t, c) => o.Examples = n.CreateMap(LoadExample, t, c)
            },
            {
                "example",
                (o, n, _, _) => o.Example = n
            },
        };

        private static readonly PatternFieldMap<OpenApiHeader> _headerPatternFields = new()
        {
            {s => s.StartsWith(OpenApiConstants.ExtensionFieldNamePrefix, StringComparison.OrdinalIgnoreCase), (o, p, n, _, c) => o.AddExtension(p, LoadExtension(p, n, c))}
        };

        public static IOpenApiHeader LoadHeader(JsonNode node, OpenApiDocument hostDocument, ParsingContext context)
        {
            var jsonObject = node.CheckMapNode("header", context);

            var pointer = jsonObject.GetReferencePointer();
            if (pointer != null)
            {
                var reference = GetReferenceIdAndExternalResource(pointer);
                return new OpenApiHeaderReference(reference.Item1, hostDocument, reference.Item2);
            }

            var header = new OpenApiHeader();
            ParseMap(jsonObject, header, _headerFixedFields, _headerPatternFields, hostDocument, context);

            return header;
        }
    }
}
