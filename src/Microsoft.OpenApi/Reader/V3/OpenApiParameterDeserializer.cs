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
        private static readonly FixedFieldMap<OpenApiParameter> _parameterFixedFields =
            new()
            {
                {
                    "name",
                    (o, n, _, _) => o.Name = n.GetScalarValue()
                },
                {
                    "in", (o, n, _, c) =>
                    {
                        if (!n.GetScalarValue().TryGetEnumFromDisplayName<ParameterLocation>(c, out var _in))
                        {
                            return;
                        }
                        o.In = _in;
                    }
                },
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
                        if (!n.GetScalarValue().TryGetEnumFromDisplayName<ParameterStyle>(c, out var style))
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
                    "content",
                    (o, n, t, c) => o.Content = n.CreateMap(LoadMediaType, t, c)
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

        private static readonly PatternFieldMap<OpenApiParameter> _parameterPatternFields =
            new()
            {
                {s => s.StartsWith(OpenApiConstants.ExtensionFieldNamePrefix, StringComparison.OrdinalIgnoreCase), (o, p, n, _, c) => o.AddExtension(p, LoadExtension(p, n, c))}
            };

        private static readonly AnyFieldMap<OpenApiParameter> _parameterAnyFields = new()
        {
            {
                OpenApiConstants.Example,
                new(
                    s => s.Example,
                    (s, v) => s.Example = v,
                    s => s.Schema)
            }
        };

        private static readonly AnyMapFieldMap<OpenApiParameter, IOpenApiExample> _parameterAnyMapOpenApiExampleFields =
            new()
            {
            {
                OpenApiConstants.Examples,
                new(
                    m => m.Examples,
                    e => e.Value,
                    (e, v) => {if (e is OpenApiExample ex) {ex.Value = v;}},
                    m => m.Schema)
            }
        };

        public static IOpenApiParameter LoadParameter(JsonNode node, OpenApiDocument hostDocument, ParsingContext context)
        {
            var jsonObject = node.CheckMapNode("parameter", context);

            var pointer = jsonObject.GetReferencePointer();
            if (pointer != null)
            {
                var reference = GetReferenceIdAndExternalResource(pointer);
                return new OpenApiParameterReference(reference.Item1, hostDocument, reference.Item2);
            }

            var parameter = new OpenApiParameter();

            ParseMap(jsonObject, parameter, _parameterFixedFields, _parameterPatternFields, hostDocument, context);
            ProcessAnyFields(parameter, _parameterAnyFields, context);
            ProcessAnyMapFields(parameter, _parameterAnyMapOpenApiExampleFields, context);

            return parameter;
        }
    }
}
