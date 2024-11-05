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
                "description", (o, n, _) =>
                {
                    o.Description = n.GetScalarValue();
                }
            },
            {
                "required", (o, n, _) =>
                {
                    o.Required = bool.Parse(n.GetScalarValue());
                }
            },
            {
                "deprecated", (o, n, _) =>
                {
                    o.Deprecated = bool.Parse(n.GetScalarValue());
                }
            },
            {
                "allowEmptyValue", (o, n, _) =>
                {
                    o.AllowEmptyValue = bool.Parse(n.GetScalarValue());
                }
            },
            {
                "allowReserved", (o, n, _) =>
                {
                    o.AllowReserved = bool.Parse(n.GetScalarValue());
                }
            },
            {
                "style", (o, n, _) =>
                {
                    o.Style = n.GetScalarValue().GetEnumFromDisplayName<ParameterStyle>();
                }
            },
            {
                "explode", (o, n, _) =>
                {
                    o.Explode = bool.Parse(n.GetScalarValue());
                }
            },
            {
                "schema", (o, n, t) =>
                {
                    o.Schema = LoadSchema(n, t);
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
            {s => s.StartsWith("x-"), (o, p, n, _) => o.AddExtension(p, LoadExtension(p,n))}
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
