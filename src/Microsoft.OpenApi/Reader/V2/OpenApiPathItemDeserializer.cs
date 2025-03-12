// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Reader.ParseNodes;

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
            {"get", (o, n, t) => o.AddOperation(HttpMethod.Get, LoadOperation(n, t))},
            {"put", (o, n, t) => o.AddOperation(HttpMethod.Put, LoadOperation(n, t))},
            {"post", (o, n, t) => o.AddOperation(HttpMethod.Post, LoadOperation(n, t))},
            {"delete", (o, n, t) => o.AddOperation(HttpMethod.Delete, LoadOperation(n, t))},
            {"options", (o, n, t) => o.AddOperation(HttpMethod.Options, LoadOperation(n, t))},
            {"head", (o, n, t) => o.AddOperation(HttpMethod.Head, LoadOperation(n, t))},
            {"patch", (o, n, t) => o.AddOperation(new HttpMethod("PATCH"), LoadOperation(n, t))},
            {
                "parameters",
                LoadPathParameters
            },
        };

        private static readonly PatternFieldMap<OpenApiPathItem> _pathItemPatternFields =
            new()
            {
                {s => s.StartsWith(OpenApiConstants.ExtensionFieldNamePrefix, StringComparison.OrdinalIgnoreCase), (o, p, n, _) => o.AddExtension(p, LoadExtension(p, n))},
            };

        public static OpenApiPathItem LoadPathItem(ParseNode node, OpenApiDocument hostDocument)
        {
            var mapNode = node.CheckMapNode("PathItem");

            var pathItem = new OpenApiPathItem();

            ParseMap(mapNode, pathItem, _pathItemFixedFields, _pathItemPatternFields, doc: hostDocument);

            return pathItem;
        }

        private static void LoadPathParameters(OpenApiPathItem pathItem, ParseNode node, OpenApiDocument hostDocument)
        {
            node.Context.SetTempStorage(TempStorageKeys.BodyParameter, null);
            node.Context.SetTempStorage(TempStorageKeys.FormParameters, null);

            pathItem.Parameters = node.CreateList(LoadParameter, hostDocument);

            // Build request body based on information determined while parsing OpenApiOperation
            var bodyParameter = node.Context.GetFromTempStorage<OpenApiParameter>(TempStorageKeys.BodyParameter);
            if (bodyParameter != null)
            {
                var requestBody = CreateRequestBody(node.Context, bodyParameter);
                foreach (var opPair in pathItem.Operations.Where(x => x.Value.RequestBody is null))
                {
                    if (opPair.Key == HttpMethod.Post || opPair.Key == HttpMethod.Put || opPair.Key == new HttpMethod("PATCH"))
                    {
                        opPair.Value.RequestBody = requestBody;
                    }
                }
            }
            else
            {
                var formParameters = node.Context.GetFromTempStorage<List<OpenApiParameter>>(TempStorageKeys.FormParameters);
                if (formParameters != null)
                {
                    var requestBody = CreateFormBody(node.Context, formParameters);
                    foreach (var opPair in pathItem.Operations.Where(x => x.Value.RequestBody is null))
                    {
                        if (opPair.Key == HttpMethod.Post || opPair.Key == HttpMethod.Put || opPair.Key == new HttpMethod("PATCH"))
                        {
                            opPair.Value.RequestBody = requestBody;
                        }
                    }
                }
            }
        }
    }
}
