// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using Microsoft.OpenApi.Expressions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers.ParseNodes;

namespace Microsoft.OpenApi.Readers.V3
{
    /// <summary>
    /// Class containing logic to deserialize Open API V3 document into
    /// runtime Open API object model.
    /// </summary>
    internal static partial class OpenApiV3Deserializer
    {
        private static readonly FixedFieldMap<OpenApiCallback> CallbackFixedFields =
            new FixedFieldMap<OpenApiCallback>();

        private static readonly PatternFieldMap<OpenApiCallback> CallbackPatternFields =
            new PatternFieldMap<OpenApiCallback>
            {
                {s => s.StartsWith("$"), (o, p, n) => o.AddPathItem(RuntimeExpression.Build(p), LoadPathItem(n))},
                {s => s.StartsWith("x-"), (o, p, n) => o.AddExtension(p, n.CreateAny())},
            };

        public static OpenApiCallback LoadCallback(ParseNode node)
        {
            var mapNode = node.CheckMapNode("callback");

            var refpointer = mapNode.GetReferencePointer();
            if (refpointer != null)
            {
                return mapNode.GetReferencedObject<OpenApiCallback>(refpointer);
            }

            var domainObject = new OpenApiCallback();

            ParseMap(mapNode, domainObject, CallbackFixedFields, CallbackPatternFields);

            return domainObject;
        }
    }
}