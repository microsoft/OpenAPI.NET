﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers.ParseNodes;

namespace Microsoft.OpenApi.Readers.V2
{
    /// <summary>
    /// Class containing logic to deserialize Open API V2 document into
    /// runtime Open API object model.
    /// </summary>
    internal static partial class OpenApiV2Deserializer
    {
        private static readonly FixedFieldMap<OpenApiPathItem> _pathItemFixedFields = new()
        {
            {
                "$ref", (o, n) =>
                {
                    o.Reference = new() { ExternalResource = n.GetScalarValue() };
                    o.UnresolvedReference =true;
                }
            },
            {"get", (o, n) => o.AddOperation(OperationType.Get, LoadOperation(n))},
            {"put", (o, n) => o.AddOperation(OperationType.Put, LoadOperation(n))},
            {"post", (o, n) => o.AddOperation(OperationType.Post, LoadOperation(n))},
            {"delete", (o, n) => o.AddOperation(OperationType.Delete, LoadOperation(n))},
            {"options", (o, n) => o.AddOperation(OperationType.Options, LoadOperation(n))},
            {"head", (o, n) => o.AddOperation(OperationType.Head, LoadOperation(n))},
            {"patch", (o, n) => o.AddOperation(OperationType.Patch, LoadOperation(n))},
            {
                "parameters",
                LoadPathParameters
            },
        };

        private static readonly PatternFieldMap<OpenApiPathItem> _pathItemPatternFields =
            new()
            {
                {s => s.StartsWith("x-"), (o, p, n) => o.AddExtension(p, LoadExtension(p, n))},
            };

        public static OpenApiPathItem LoadPathItem(ParseNode node)
        {
            var mapNode = node.CheckMapNode("PathItem");

            var pathItem = new OpenApiPathItem();

            ParseMap(mapNode, pathItem, _pathItemFixedFields, _pathItemPatternFields);

            return pathItem;
        }

        private static void LoadPathParameters(OpenApiPathItem pathItem, ParseNode node)
        {
            node.Context.SetTempStorage(TempStorageKeys.BodyParameter, null);
            node.Context.SetTempStorage(TempStorageKeys.FormParameters, null);

            pathItem.Parameters = node.CreateList(LoadParameter);

            // Build request body based on information determined while parsing OpenApiOperation
            var bodyParameter = node.Context.GetFromTempStorage<OpenApiParameter>(TempStorageKeys.BodyParameter);
            if (bodyParameter != null)
            {
                var requestBody = CreateRequestBody(node.Context, bodyParameter);
                foreach(var opPair in pathItem.Operations.Where(x => x.Value.RequestBody is null))
                {
                    switch (opPair.Key)
                    {
                        case OperationType.Post:
                        case OperationType.Put:
                        case OperationType.Patch:
                            opPair.Value.RequestBody = requestBody;
                            break;
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
                        switch (opPair.Key)
                        {
                            case OperationType.Post:
                            case OperationType.Put:
                            case OperationType.Patch:
                                opPair.Value.RequestBody = requestBody;
                                break;
                        }
                    }
                }
            }
        }
    }
}
