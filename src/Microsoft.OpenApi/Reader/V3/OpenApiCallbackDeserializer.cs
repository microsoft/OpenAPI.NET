// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using Microsoft.OpenApi.Expressions;
using Microsoft.OpenApi.Extensions;

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
                {s => !s.StartsWith(OpenApiConstants.ExtensionFieldNamePrefix, StringComparison.OrdinalIgnoreCase), (o, p, n, t) => o.AddPathItem(RuntimeExpression.Build(p), LoadPathItem(n, t))},
                {s => s.StartsWith(OpenApiConstants.ExtensionFieldNamePrefix, StringComparison.OrdinalIgnoreCase), (o, p, n, _) => o.AddExtension(p, LoadExtension(p,n))},
            };

        public static IOpenApiCallback LoadCallback(ParseNode node, OpenApiDocument hostDocument)
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
