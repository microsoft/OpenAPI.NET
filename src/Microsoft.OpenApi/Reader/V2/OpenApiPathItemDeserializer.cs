// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Text.Json.Nodes;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace Microsoft.OpenApi.Reader.V2
{
    /// <summary>
    /// Class containing logic to deserialize Open API V2 document into
    /// runtime Open API object model.
    /// </summary>
    internal static partial class OpenApiV2Deserializer
    {
        private static readonly FixedFieldMap<OpenApiPathItem> _pathItemFixedFields = new()
        {
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
            {
                "parameters",
                LoadPathParameters
            },
        };

        private static readonly PatternFieldMap<OpenApiPathItem> _pathItemPatternFields =
            new()
            {
                {s => s.StartsWith(OpenApiConstants.ExtensionFieldNamePrefix, StringComparison.OrdinalIgnoreCase), (o, p, n, _, c) => o.AddExtension(p, LoadExtension(p, n, c))},
            };

        public static OpenApiPathItem LoadPathItem(JsonNode node, OpenApiDocument hostDocument, ParsingContext context)
        {
            var JsonObject = node.CheckMapNode("PathItem", context);

            var pathItem = new OpenApiPathItem();

            ParseMap(JsonObject, pathItem, _pathItemFixedFields, _pathItemPatternFields, hostDocument, context);

            return pathItem;
        }

        private static void LoadPathParameters(OpenApiPathItem pathItem, JsonNode node, OpenApiDocument hostDocument, ParsingContext context)
        {
            context.SetTempStorage(TempStorageKeys.BodyParameter, null);
            context.SetTempStorage(TempStorageKeys.FormParameters, null);

            pathItem.Parameters = node.CreateList(LoadParameter, hostDocument, context)
                                     .OfType<IOpenApiParameter>()
                                     .ToList();

            // Build request body based on information determined while parsing OpenApiOperation
            var bodyParameter = context.GetFromTempStorage<OpenApiParameter>(TempStorageKeys.BodyParameter);
            if (bodyParameter is not null && pathItem.Operations is not null)
            {
                var requestBody = CreateRequestBody(context, bodyParameter);
                foreach (var opPair in pathItem.Operations.Where(x => x.Value.RequestBody is null))
                {
                    if (opPair.Key == HttpMethod.Post || opPair.Key == HttpMethod.Put
#if NETSTANDARD2_1_OR_GREATER
            || opPair.Key == HttpMethod.Patch
#else
                        || opPair.Key == new HttpMethod("PATCH")
#endif
                       )
                    {
                        opPair.Value.RequestBody = requestBody;
                    }
                }
            }
            else
            {
                var formParameters = context.GetFromTempStorage<List<OpenApiParameter>>(TempStorageKeys.FormParameters);
                if (formParameters is not null && pathItem.Operations is not null)
                {
                    var requestBody = CreateFormBody(context, formParameters);
                    foreach (var opPair in pathItem.Operations.Where(x => x.Value.RequestBody is null))
                    {
                        if (opPair.Key == HttpMethod.Post || opPair.Key == HttpMethod.Put
#if NETSTANDARD2_1_OR_GREATER
                || opPair.Key == HttpMethod.Patch
#else
                            || opPair.Key == new HttpMethod("PATCH")
#endif
                           )
                        {
                            opPair.Value.RequestBody = requestBody;
                        }
                    }
                }
            }

        }
    }
}
