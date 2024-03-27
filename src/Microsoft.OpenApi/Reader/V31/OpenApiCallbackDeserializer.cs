using Microsoft.OpenApi.Expressions;
using System;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Reader.ParseNodes;

namespace Microsoft.OpenApi.Reader.V31
{
    /// <summary>
    /// Class containing logic to deserialize Open API V3 document into
    /// runtime Open API object model.
    /// </summary>
    internal static partial class OpenApiV31Deserializer
    {
        private static readonly FixedFieldMap<OpenApiCallback> _callbackFixedFields =
            new();

        private static readonly PatternFieldMap<OpenApiCallback> _callbackPatternFields =
            new()
            {
            {s => !s.StartsWith("x-", StringComparison.OrdinalIgnoreCase), (o, p, n) => o.AddPathItem(RuntimeExpression.Build(p), LoadPathItem(n))},
            {s => s.StartsWith("x-", StringComparison.OrdinalIgnoreCase), (o, p, n) => o.AddExtension(p, LoadExtension(p,n))},
            };

        public static OpenApiCallback LoadCallback(ParseNode node)
        {
            var mapNode = node.CheckMapNode("callback");

            if (mapNode.GetReferencePointer() is {} pointer)
            {
                return mapNode.GetReferencedObject<OpenApiCallback>(ReferenceType.Callback, pointer);
            }

            var domainObject = new OpenApiCallback();

            ParseMap(mapNode, domainObject, _callbackFixedFields, _callbackPatternFields);

            return domainObject;
        }
    }
}
