using System.Linq;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Models.References;
using Microsoft.OpenApi.Reader.ParseNodes;

namespace Microsoft.OpenApi.Reader.V31
{
    /// <summary>
    /// Class containing logic to deserialize Open API V31 document into
    /// runtime Open API object model.
    /// </summary>
    internal static partial class OpenApiV31Deserializer
    {
        private static readonly FixedFieldMap<OpenApiHeader> _headerFixedFields = new()
        {
            {
                "description", (o, n) =>
                {
                    o.Description = n.GetScalarValue();
                }
            },
            {
                "required", (o, n) =>
                {
                    o.Required = bool.Parse(n.GetScalarValue());
                }
            },
            {
                "deprecated", (o, n) =>
                {
                    o.Deprecated = bool.Parse(n.GetScalarValue());
                }
            },
            {
                "allowEmptyValue", (o, n) =>
                {
                    o.AllowEmptyValue = bool.Parse(n.GetScalarValue());
                }
            },
            {
                "allowReserved", (o, n) =>
                {
                    o.AllowReserved = bool.Parse(n.GetScalarValue());
                }
            },
            {
                "style", (o, n) =>
                {
                    o.Style = n.GetScalarValue().GetEnumFromDisplayName<ParameterStyle>();
                }
            },
            {
                "explode", (o, n) =>
                {
                    o.Explode = bool.Parse(n.GetScalarValue());
                }
            },
            {
                "schema", (o, n) =>
                {
                    o.Schema = LoadSchema(n);
                }
            },
            {
                "examples", (o, n) =>
                {
                    o.Examples = n.CreateMap(LoadExample);
                }
            },
            {
                "example", (o, n) =>
                {
                    o.Example = n.CreateAny();
                }
            },
        };

        private static readonly PatternFieldMap<OpenApiHeader> _headerPatternFields = new()
        {
            {s => s.StartsWith("x-"), (o, p, n) => o.AddExtension(p, LoadExtension(p,n))}
        };

        public static OpenApiHeader LoadHeader(ParseNode node, OpenApiDocument hostDocument = null)
        {
            var mapNode = node.CheckMapNode("header");

            var pointer = mapNode.GetReferencePointer();
            if (pointer != null)
            {
                var reference = GetReferenceIdAndExternalResource(pointer);
                return new OpenApiHeaderReference(reference.Item1, hostDocument, reference.Item2);
            }

            var header = new OpenApiHeader();
            foreach (var property in mapNode)
            {
                property.ParseField(header, _headerFixedFields, _headerPatternFields);
            }

            return header;
        }
    }
}
