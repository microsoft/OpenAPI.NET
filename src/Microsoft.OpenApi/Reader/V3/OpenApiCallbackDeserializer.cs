// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Text.Json.Nodes;

using System;

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
                {s => !s.StartsWith(OpenApiConstants.ExtensionFieldNamePrefix, StringComparison.OrdinalIgnoreCase), (o, p, n, t, c) => o.AddPathItem(RuntimeExpression.Build(p), LoadPathItem(n, t, c))},
                {s => s.StartsWith(OpenApiConstants.ExtensionFieldNamePrefix, StringComparison.OrdinalIgnoreCase), (o, p, n, _, c) => o.AddExtension(p, LoadExtension(p, n, c))},
            };

        public static IOpenApiCallback LoadCallback(JsonNode node, OpenApiDocument hostDocument, ParsingContext context)
        {
            var jsonObject = node.CheckMapNode("callback", context);

            var pointer = jsonObject.GetReferencePointer();
            
            if (pointer != null)
            {
                var reference = GetReferenceIdAndExternalResource(pointer);
                return new OpenApiCallbackReference(reference.Item1, hostDocument, reference.Item2);
            }

            var domainObject = new OpenApiCallback();

            ParseMap(jsonObject, domainObject, _callbackFixedFields, _callbackPatternFields, hostDocument, context);

            return domainObject;
        }
    }
}
