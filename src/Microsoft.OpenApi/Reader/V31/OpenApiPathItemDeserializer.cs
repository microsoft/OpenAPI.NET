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
        private static readonly FixedFieldMap<OpenApiPathItem> _pathItemFixedFields = new()
        {

            {
                "$ref", (o,n, _) => {
                    o.Reference = new OpenApiReference() { ExternalResource = n.GetScalarValue() };
                    o.UnresolvedReference =true;
                }
            },
            {
                "summary", (o, n, _) =>
                {
                    o.Summary = n.GetScalarValue();
                }
            },
            {
                "description", (o, n, _) =>
                {
                    o.Description = n.GetScalarValue();
                }
            },
            {"get", (o, n, t) => o.AddOperation(OperationType.Get, LoadOperation(n, t))},
            {"put", (o, n, t) => o.AddOperation(OperationType.Put, LoadOperation(n, t))},
            {"post", (o, n, t) => o.AddOperation(OperationType.Post, LoadOperation(n, t))},
            {"delete", (o, n, t) => o.AddOperation(OperationType.Delete, LoadOperation(n, t))},
            {"options", (o, n, t) => o.AddOperation(OperationType.Options, LoadOperation(n, t))},
            {"head", (o, n, t) => o.AddOperation(OperationType.Head, LoadOperation(n, t))},
            {"patch", (o, n, t) => o.AddOperation(OperationType.Patch, LoadOperation(n, t))},
            {"trace", (o, n, t) => o.AddOperation(OperationType.Trace, LoadOperation(n, t))},
            {"servers", (o, n, t) => o.Servers = n.CreateList(LoadServer, t)},
            {"parameters", (o, n, t) => o.Parameters = n.CreateList(LoadParameter, t)}
        };

        private static readonly PatternFieldMap<OpenApiPathItem> _pathItemPatternFields =
            new()
            {
                {s => s.StartsWith("x-"), (o, p, n, _) => o.AddExtension(p, LoadExtension(p,n))}
            };

        public static OpenApiPathItem LoadPathItem(ParseNode node, OpenApiDocument hostDocument = null)
        {
            var mapNode = node.CheckMapNode("PathItem");

            var pointer = mapNode.GetReferencePointer();

            if (pointer != null)
            {
                var reference = GetReferenceIdAndExternalResource(pointer);
                return new OpenApiPathItemReference(reference.Item1, hostDocument, reference.Item2);
            }

            var pathItem = new OpenApiPathItem();

            ParseMap(mapNode, pathItem, _pathItemFixedFields, _pathItemPatternFields, hostDocument);

            return pathItem;
        }
    }
}
