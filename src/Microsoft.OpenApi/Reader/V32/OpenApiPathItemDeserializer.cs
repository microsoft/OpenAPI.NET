using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace Microsoft.OpenApi.Reader.V32
{
    /// <summary>
    /// Class containing logic to deserialize Open API V32 document into
    /// runtime Open API object model.
    /// </summary>
    internal static partial class OpenApiV32Deserializer
    {
        private static readonly FixedFieldMap<OpenApiPathItem> _pathItemFixedFields = new()
        {

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
            {"get", (o, n, t) => o.AddOperation(HttpMethod.Get, LoadOperation(n, t))},
            {"put", (o, n, t) => o.AddOperation(HttpMethod.Put, LoadOperation(n, t))},
            {"post", (o, n, t) => o.AddOperation(HttpMethod.Post, LoadOperation(n, t))},
            {"delete", (o, n, t) => o.AddOperation(HttpMethod.Delete, LoadOperation(n, t))},
            {"options", (o, n, t) => o.AddOperation(HttpMethod.Options, LoadOperation(n, t))},
            {"head", (o, n, t) => o.AddOperation(HttpMethod.Head, LoadOperation(n, t))},
#if NETSTANDARD2_1_OR_GREATER
            {"patch", (o, n, t) => o.AddOperation(HttpMethod.Patch, LoadOperation(n, t))},
#else
            {"patch", (o, n, t) => o.AddOperation(new HttpMethod("PATCH"), LoadOperation(n, t))},
#endif
            {"query", (o, n, t) => o.AddOperation(new HttpMethod("QUERY"), LoadOperation(n, t))},
            {"trace", (o, n, t) => o.AddOperation(HttpMethod.Trace, LoadOperation(n, t))},
            {"servers", (o, n, t) => o.Servers = n.CreateList(LoadServer, t)},
            {"parameters", (o, n, t) => o.Parameters = n.CreateList(LoadParameter, t)},
            {OpenApiConstants.AdditionalOperations, LoadAdditionalOperations }
        };

        

        private static void LoadAdditionalOperations(OpenApiPathItem o, ParseNode n, OpenApiDocument t)
        {
            if (n is null)
            {
                return;
            }

            var mapNode = n.CheckMapNode(OpenApiConstants.AdditionalOperations);

            foreach (var property in mapNode.Where(p => !OpenApiPathItem._standardHttp32MethodsNames.Contains(p.Name)))
            {
                var operationType = property.Name;

                var httpMethod = new HttpMethod(operationType);
                o.AddOperation(httpMethod, LoadOperation(property.Value, t));
            }
        }

        private static readonly PatternFieldMap<OpenApiPathItem> _pathItemPatternFields =
            new()
            {
                {s => s.StartsWith(OpenApiConstants.ExtensionFieldNamePrefix, StringComparison.OrdinalIgnoreCase), (o, p, n, _) => o.AddExtension(p, LoadExtension(p,n))}
            };

        public static IOpenApiPathItem LoadPathItem(ParseNode node, OpenApiDocument hostDocument)
        {
            var mapNode = node.CheckMapNode("PathItem");

            var pointer = mapNode.GetReferencePointer();

            if (pointer != null)
            {
                var reference = GetReferenceIdAndExternalResource(pointer);
                var pathItemReference = new OpenApiPathItemReference(reference.Item1, hostDocument, reference.Item2);
                pathItemReference.Reference.SetMetadataFromMapNode(mapNode);
                return pathItemReference;
            }

            var pathItem = new OpenApiPathItem();

            ParseMap(mapNode, pathItem, _pathItemFixedFields, _pathItemPatternFields, hostDocument);

            return pathItem;
        }
    }
}

