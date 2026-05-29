// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Text.Json.Nodes;

using System;
using System.Net.Http;

namespace Microsoft.OpenApi.Reader.V3
{
    /// <summary>
    /// Class containing logic to deserialize Open API V3 document into
    /// runtime Open API object model.
    /// </summary>
    internal static partial class OpenApiV3Deserializer
    {
        private static readonly FixedFieldMap<OpenApiPathItem> _pathItemFixedFields = new()
        {
            {
                "summary",
                (o, n, _, c) => o.Summary = n.GetScalarValue()
            },
            {
                "description",
                (o, n, _, c) => o.Description = n.GetScalarValue()
            },
            {"get", (o, n, t, c) => o.AddOperation(HttpMethod.Get, LoadOperation(n, t, c))},
            {"put", (o, n, t, c) => o.AddOperation(HttpMethod.Put, LoadOperation(n, t, c))},
            {"post", (o, n, t, c) => o.AddOperation(HttpMethod.Post, LoadOperation(n, t, c))},
            {"delete", (o, n, t, c) => o.AddOperation(HttpMethod.Delete, LoadOperation(n, t, c))},
            {"options", (o, n, t, c) => o.AddOperation(HttpMethod.Options, LoadOperation(n, t, c))},
            {"head", (o, n, t, c) => o.AddOperation(HttpMethod.Head, LoadOperation(n, t, c))},
#if NETSTANDARD2_1_OR_GREATER
            {"patch", (o, n, t, c) => o.AddOperation(HttpMethod.Patch, LoadOperation(n, t, c))},
#else
            {"patch", (o, n, t, c) => o.AddOperation(new HttpMethod("PATCH"), LoadOperation(n, t, c))},
#endif
            {"trace", (o, n, t, c) => o.AddOperation(HttpMethod.Trace, LoadOperation(n, t, c))},
            {"servers", (o, n, t, c) => o.Servers = n.CreateList(LoadServer, t, c)},
            {"parameters", (o, n, t, c) => o.Parameters = n.CreateList(LoadParameter, t, c)}
        };

        private static readonly PatternFieldMap<OpenApiPathItem> _pathItemPatternFields =
            new()
            {
                {s => s.StartsWith(OpenApiConstants.ExtensionFieldNamePrefix, StringComparison.OrdinalIgnoreCase), (o, p, n, _, c) => o.AddExtension(p, LoadExtension(p, n, c))}
            };

        public static IOpenApiPathItem LoadPathItem(JsonNode node, OpenApiDocument hostDocument, ParsingContext context)
        {
            var jsonObject = node.CheckMapNode("PathItem", context);

            var pointer = jsonObject.GetReferencePointer();
            if (pointer != null)
            {
                var reference = GetReferenceIdAndExternalResource(pointer);
                return new OpenApiPathItemReference(reference.Item1, hostDocument, reference.Item2);
            }

            var pathItem = new OpenApiPathItem();

            ParseMap(jsonObject, pathItem, _pathItemFixedFields, _pathItemPatternFields, hostDocument, context);

            return pathItem;
        }
    }
}
