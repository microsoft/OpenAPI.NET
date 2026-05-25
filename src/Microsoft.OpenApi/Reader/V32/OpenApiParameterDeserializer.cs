using System;
using System.Text.Json.Nodes;

namespace Microsoft.OpenApi.Reader.V32
{
    /// <summary>
    /// Class containing logic to deserialize Open API V32 document into
    /// runtime Open API object model.
    /// </summary>
    internal static partial class OpenApiV32Deserializer
    {
        private static readonly FixedFieldMap<OpenApiParameter> _parameterFixedFields =
            new()
            {
                {
                    "name", (o, n, _, c) =>
                    {
                        o.Name = n.GetScalarValue();
                    }
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
                    "description", (o, n, _, c) =>
                    {
                        o.Description = n.GetScalarValue();
                    }
                },
                {
                    "required",
                    (o, n, t, c) =>
                    {
                        var required = n.GetScalarValue();
                        if (required != null)
                        {
                            o.Required = bool.Parse(required);
                        }
                    }
                },
                {
                    "deprecated",
                    (o, n, t, c) =>
                    {
                        var deprecated = n.GetScalarValue();
                        if (deprecated != null)
                        {
                            o.Deprecated = bool.Parse(deprecated);
                        }
                    }
                },
                {
                    "allowEmptyValue",
                    (o, n, t, c) =>
                    {
                        var allowEmptyValue = n.GetScalarValue();
                        if (allowEmptyValue != null)
                        {
                            o.AllowEmptyValue = bool.Parse(allowEmptyValue);
                        }
                    }
                },
                {
                    "allowReserved",
                    (o, n, _, c) =>
                    {
                        var allowReserved = n.GetScalarValue();
                        if (allowReserved != null)
                        {
                            o.AllowReserved = bool.Parse(allowReserved);
                        }
                    }
                },
                {
                    "style", (o, n, _, c) =>
                    {
                        if (!n.GetScalarValue().TryGetEnumFromDisplayName<ParameterStyle>(c, out var style))
                        {
                            return;
                        }
                        o.Style = style;
                    }
                },
                {
                    "explode", (o, n, _, c) =>
                    {
                        var explode = n.GetScalarValue();
                        if (explode != null)
                        {
                            o.Explode = bool.Parse(explode);
                        }
                    }
                },
                {
                    "schema", (o, n, t, c) =>
                    {
                        o.Schema = LoadSchema(n, t, c);
                    }
                },
                {
                    "content", (o, n, t, c) =>
                    {
                        o.Content = n.CreateMap(LoadMediaType, t, c);
                    }
                },
                {
                    "examples", (o, n, t, c) =>
                    {
                        o.Examples = n.CreateMap(LoadExample, t, c);
                    }
                },
                {
                    "example", (o, n, _, c) =>
                    {
                        o.Example = n.CreateAny();
                    }
                },
            };

        private static readonly PatternFieldMap<OpenApiParameter> _parameterPatternFields =
            new()
            {
                {s => s.StartsWith(OpenApiConstants.ExtensionFieldNamePrefix, StringComparison.OrdinalIgnoreCase), (o, p, n, _, c) => o.AddExtension(p, LoadExtension(p, n, c))}
            };

        private static readonly AnyFieldMap<OpenApiParameter> _parameterAnyFields = new AnyFieldMap<OpenApiParameter>
        {
            {
                OpenApiConstants.Example,
                new AnyFieldMapParameter<OpenApiParameter>(
                    s => s.Example,
                    (s, v) => s.Example = v,
                    s => s.Schema)
            }
        };

        private static readonly AnyMapFieldMap<OpenApiParameter, IOpenApiExample> _parameterAnyMapOpenApiExampleFields =
            new AnyMapFieldMap<OpenApiParameter, IOpenApiExample>
        {
            {
                OpenApiConstants.Examples,
                new AnyMapFieldMapParameter<OpenApiParameter, IOpenApiExample>(
                    m => m.Examples,
                    e => e.Value,
                    (e, v) => {if (e is OpenApiExample ex) {ex.Value = v;}},
                    m => m.Schema)
            }
        };

        public static IOpenApiParameter LoadParameter(JsonNode node, OpenApiDocument hostDocument, ParsingContext context)
        {
            var JsonObject = node.CheckMapNode("parameter", context);

            var pointer = JsonObject.GetReferencePointer();
            if (pointer != null)
            {
                var reference = GetReferenceIdAndExternalResource(pointer);
                var parameterReference = new OpenApiParameterReference(reference.Item1, hostDocument, reference.Item2);
                parameterReference.Reference.SetMetadataFromJsonObject(JsonObject);
                return parameterReference;
            }

            var parameter = new OpenApiParameter();

            ParseMap(JsonObject, parameter, _parameterFixedFields, _parameterPatternFields, hostDocument, context);
            ProcessAnyFields(JsonObject, parameter, _parameterAnyFields, context);
            ProcessAnyMapFields(JsonObject, parameter, _parameterAnyMapOpenApiExampleFields, context);

            return parameter;
        }
    }
}

