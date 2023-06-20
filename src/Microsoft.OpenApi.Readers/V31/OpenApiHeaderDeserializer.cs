using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers.ParseNodes;

namespace Microsoft.OpenApi.Readers.V31
{
    /// <summary>
    /// Class containing logic to deserialize Open API V31 document into
    /// runtime Open API object model.
    /// </summary>
    internal static partial class OpenApiV31Deserializer
    {
        private static readonly FixedFieldMap<OpenApiHeader> _headerFixedFields = new FixedFieldMap<OpenApiHeader>
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
                    o.Schema31 = LoadSchema(n);
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

        private static readonly PatternFieldMap<OpenApiHeader> _headerPatternFields = new PatternFieldMap<OpenApiHeader>
        {
            {s => s.StartsWith("x-"), (o, p, n) => o.AddExtension(p, LoadExtension(p,n))}
        };

        public static OpenApiHeader LoadHeader(ParseNode node)
        {
            var mapNode = node.CheckMapNode("header");

            var pointer = mapNode.GetReferencePointer();
            if (pointer != null)
            {
                var description = node.Context.VersionService.GetReferenceScalarValues(mapNode, OpenApiConstants.Description);
                var summary = node.Context.VersionService.GetReferenceScalarValues(mapNode, OpenApiConstants.Summary);

                return mapNode.GetReferencedObject<OpenApiHeader>(ReferenceType.Header, pointer, summary, description);
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
