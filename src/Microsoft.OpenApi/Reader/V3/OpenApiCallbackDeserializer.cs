// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Linq;
using Microsoft.OpenApi.Expressions;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Models.References;
using Microsoft.OpenApi.Reader.ParseNodes;

namespace Microsoft.OpenApi.Reader.V3
{
    /// <summary>
    /// Class containing logic to deserialize Open API V3 document into
    /// runtime Open API object model.
    /// </summary>
    internal static partial class OpenApiV3Deserializer
    {
        private static readonly FixedFieldMap<OpenApiCallback> _callbackFixedFields = new();

        private static readonly PatternFieldMap<OpenApiCallback> _callbackPatternFields =
            new()
            {
                {s => !s.StartsWith("x-"), (o, p, n, t) => o.AddPathItem(RuntimeExpression.Build(p), LoadPathItem(n, t))},
                {s => s.StartsWith("x-"), (o, p, n, _) => o.AddExtension(p, LoadExtension(p,n))},
            };

        public static OpenApiCallback LoadCallback(ParseNode node, OpenApiDocument hostDocument = null)
        {
            var mapNode = node.CheckMapNode("callback");

            var pointer = mapNode.GetReferencePointer();
            
            if (pointer != null)
            {
                var reference = GetReferenceIdAndExternalResource(pointer);
                return new OpenApiCallbackReference(reference.Item1, hostDocument, reference.Item2);
            }

            var domainObject = new OpenApiCallback();

            ParseMap(mapNode, domainObject, _callbackFixedFields, _callbackPatternFields, hostDocument);

            return domainObject;
        }
    }
}
