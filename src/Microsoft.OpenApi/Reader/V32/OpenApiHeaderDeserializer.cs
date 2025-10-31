using System;

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
                "description", (o, n, _) =>
                {
                    o.Description = n.GetScalarValue();
                }
            },
            {
                "required",
                (o, n, _) =>
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
                (o, n, _) =>
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
                (o, n, _) =>
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
                (o, n, _) =>
                {
                    var allowReserved = n.GetScalarValue();
                    if (allowReserved != null)
                    {
                        o.AllowReserved = bool.Parse(allowReserved);
                    }
                }
            },
            {
                "style", (o, n, _) =>
                {
                    if(!n.GetScalarValue().TryGetEnumFromDisplayName<ParameterStyle>(n.Context, out var style))
                    {
                        return;
                    }
                    o.Style = style;
                }
            },
            {
                "explode",
                (o, n, _) =>
                {
                    var explode = n.GetScalarValue();
                    if (explode != null)
                    {
                        o.Explode = bool.Parse(explode);
                    }
                }
            },
            {
                "schema", (o, n, t) =>
                {
                    o.Schema = LoadSchema(n, t);
                }
            },
            {
                "content", (o, n, t) =>
                {
                    o.Content = n.CreateMap(LoadMediaType, t);
                }
            },
            {
                "examples", (o, n, t) =>
                {
                    o.Examples = n.CreateMap(LoadExample, t);
                }
            },
            {
                "example", (o, n, _) =>
                {
                    o.Example = n.CreateAny();
                }
            },
        };

        private static readonly PatternFieldMap<OpenApiHeader> _headerPatternFields = new()
        {
            {s => s.StartsWith(OpenApiConstants.ExtensionFieldNamePrefix, StringComparison.OrdinalIgnoreCase), (o, p, n, _) => o.AddExtension(p, LoadExtension(p,n))}
        };

        public static IOpenApiHeader LoadHeader(ParseNode node, OpenApiDocument hostDocument)
        {
            var mapNode = node.CheckMapNode("header");

            var pointer = mapNode.GetReferencePointer();
            if (pointer != null)
            {
                var reference = GetReferenceIdAndExternalResource(pointer);
                var headerReference = new OpenApiHeaderReference(reference.Item1, hostDocument, reference.Item2);
                headerReference.Reference.SetMetadataFromMapNode(mapNode);
                return headerReference;
            }

            var header = new OpenApiHeader();
            foreach (var property in mapNode)
            {
                property.ParseField(header, _headerFixedFields, _headerPatternFields, hostDocument);
            }

            return header;
        }
    }
}

