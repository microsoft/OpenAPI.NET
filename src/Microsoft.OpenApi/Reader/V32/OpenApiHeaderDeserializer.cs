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
        private static readonly FixedFieldMap<OpenApiHeader> _headerFixedFields = new()
        {
            {
                "description", (o, n, _, c) =>
                {
                    o.Description = n.GetScalarValue();
                }
            },
            {
                "required",
                (o, n, _, c) =>
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
                (o, n, _, c) =>
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
                (o, n, _, c) =>
                {
                    var allowEmptyVal = n.GetScalarValue();
                    if (allowEmptyVal != null)
                    {
                        o.AllowEmptyValue = bool.Parse(allowEmptyVal);
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
                    if(!n.GetScalarValue().TryGetEnumFromDisplayName<ParameterStyle>(c, out var style))
                    {
                        return;
                    }
                    o.Style = style;
                }
            },
            {
                "explode",
                (o, n, _, c) =>
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

        private static readonly PatternFieldMap<OpenApiHeader> _headerPatternFields = new()
        {
            {s => s.StartsWith(OpenApiConstants.ExtensionFieldNamePrefix, StringComparison.OrdinalIgnoreCase), (o, p, n, _, c) => o.AddExtension(p, LoadExtension(p, n, c))}
        };

        public static IOpenApiHeader LoadHeader(JsonNode node, OpenApiDocument hostDocument, ParsingContext context)
        {
            var JsonObject = node.CheckMapNode("header", context);

            var pointer = JsonObject.GetReferencePointer();
            if (pointer != null)
            {
                var reference = GetReferenceIdAndExternalResource(pointer);
                var headerReference = new OpenApiHeaderReference(reference.Item1, hostDocument, reference.Item2);
                headerReference.Reference.SetMetadataFromJsonObject(JsonObject);
                return headerReference;
            }

            var header = new OpenApiHeader();
            ParseMap(JsonObject, header, _headerFixedFields, _headerPatternFields, hostDocument, context);

            return header;
        }
    }
}

