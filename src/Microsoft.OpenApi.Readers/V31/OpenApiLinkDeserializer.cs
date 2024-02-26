using System.Linq;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Models.References;
using Microsoft.OpenApi.Readers.ParseNodes;

namespace Microsoft.OpenApi.Readers.V31
{
    /// <summary>
    /// Class containing logic to deserialize Open API V31 document into
    /// runtime Open API object model.
    /// </summary>
    internal static partial class OpenApiV31Deserializer
    {
        private static readonly FixedFieldMap<OpenApiLink> _linkFixedFields = new()
        {
            {
                "operationRef", (o, n) =>
                {
                    o.OperationRef = n.GetScalarValue();
                }
            },
            {
                "operationId", (o, n) =>
                {
                    o.OperationId = n.GetScalarValue();
                }
            },
            {
                "parameters", (o, n) =>
                {
                    o.Parameters = n.CreateSimpleMap(LoadRuntimeExpressionAnyWrapper);
                }
            },
            {
                "requestBody", (o, n) =>
                {
                    o.RequestBody = LoadRuntimeExpressionAnyWrapper(n);
                }
            },
            {
                "description", (o, n) =>
                {
                    o.Description = n.GetScalarValue();
                }
            },
            {"server", (o, n) => o.Server = LoadServer(n)}
        };

        private static readonly PatternFieldMap<OpenApiLink> _linkPatternFields = new()
        {
            {s => s.StartsWith("x-"), (o, p, n) => o.AddExtension(p, LoadExtension(p,n))},
        };

        public static OpenApiLink LoadLink(ParseNode node)
        {
            var mapNode = node.CheckMapNode("link");
            var link = new OpenApiLink();

            var pointer = mapNode.GetReferencePointer();
            if (pointer != null)
            {
                var refId = pointer.Split('/').Last();
                return new OpenApiLinkReference(refId, _openApiDocument);
            }

            ParseMap(mapNode, link, _linkFixedFields, _linkPatternFields);

            return link;
        }
    }
}
