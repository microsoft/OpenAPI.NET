// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Net.Http;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Models.Interfaces;
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
        private static readonly FixedFieldMap<OpenApiPathItem> _pathItemFixedFields = new()
        {
            {
                "summary",
                (o, n, _) => o.Summary = n.GetScalarValue()
            },
            {
                "description",
                (o, n, _) => o.Description = n.GetScalarValue()
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
            {"trace", (o, n, t) => o.AddOperation(HttpMethod.Trace, LoadOperation(n, t))},
            {"servers", (o, n, t) => o.Servers = n.CreateList(LoadServer, t)},
            {"parameters", (o, n, t) => o.Parameters = n.CreateList(LoadParameter, t)}
        };

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
                return new OpenApiPathItemReference(reference.Item1, hostDocument, reference.Item2);
            }

            var pathItem = new OpenApiPathItem();

            ParseMap(mapNode, pathItem, _pathItemFixedFields, _pathItemPatternFields, hostDocument);

            return pathItem;
        }
    }
}
