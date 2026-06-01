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
        private static readonly FixedFieldMap<OpenApiLink> _linkFixedFields = new()
        {
            {
                "operationRef",
                (o, n, _, _) => o.OperationRef = n.GetScalarValue()
            },
            {
                "operationId",
                (o, n, _, _) => o.OperationId = n.GetScalarValue()
            },
            {
                "parameters",
                (o, n, _, c) => o.Parameters = n.CreateSimpleMap(LoadRuntimeExpressionAnyWrapper, c)
            },
            {
                "requestBody",
                (o, n, _, _) => o.RequestBody = LoadRuntimeExpressionAnyWrapper(n)
            },
            {
                "description",
                (o, n, _, _) => o.Description = n.GetScalarValue()
            },
            {"server", (o, n, t, c) => o.Server = LoadServer(n, t, c)}
        };

        private static readonly PatternFieldMap<OpenApiLink> _linkPatternFields = new()
        {
            {s => s.StartsWith(OpenApiConstants.ExtensionFieldNamePrefix, StringComparison.OrdinalIgnoreCase), (o, p, n, _, c) => o.AddExtension(p, LoadExtension(p, n, c))},
        };

        public static IOpenApiLink LoadLink(JsonNode node, OpenApiDocument hostDocument, ParsingContext context)
        {
            var jsonObject = node.CheckMapNode("link", context);
            var link = new OpenApiLink();

            var pointer = jsonObject.GetReferencePointer();
            if (pointer != null)
            {
                var reference = GetReferenceIdAndExternalResource(pointer);
                return new OpenApiLinkReference(reference.Item1, hostDocument, reference.Item2);
            }

            ParseMap(jsonObject, link, _linkFixedFields, _linkPatternFields, hostDocument, context);

            return link;
        }
    }
}
