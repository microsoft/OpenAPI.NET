// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
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
        private static readonly FixedFieldMap<OpenApiOperation> _operationFixedFields =
            new FixedFieldMap<OpenApiOperation>
            {
                {
                    "tags", (o, n) => o.Tags = n.CreateSimpleList(
                        valueNode =>
                            LoadTagByReference(
                                valueNode.Context,
                                valueNode.Diagnostic,
                                valueNode.GetScalarValue()))
                },
                {
                    "summary", (o, n) =>
                    {
                        o.Summary = n.GetScalarValue();
                    }
                },
                {
                    "description", (o, n) =>
                    {
                        o.Description = n.GetScalarValue();
                    }
                },
                {
                    "externalDocs", (o, n) =>
                    {
                        o.ExternalDocs = LoadExternalDocs(n);
                    }
                },
                {
                    "operationId", (o, n) =>
                    {
                        o.OperationId = n.GetScalarValue();
                    }
                },
                {
                    "parameters", (o, n) =>
                    {
                        o.Parameters = n.CreateList(LoadParameter);
                    }
                },
                {
                    "consumes", (o, n) => n.Context.SetTempStorage(
                        TempStorageKeys.OperationConsumes,
                        n.CreateSimpleList(s => s.GetScalarValue()))
                },
                {
                    "produces", (o, n) => n.Context.SetTempStorage(
                        TempStorageKeys.OperationProduces,
                        n.CreateSimpleList(s => s.GetScalarValue()))
                },
                {
                    "responses", (o, n) =>
                    {
                        o.Responses = LoadResponses(n);
                    }
                },
                {
                    "deprecated", (o, n) =>
                    {
                        o.Deprecated = bool.Parse(n.GetScalarValue());
                    }
                },
                {
                    "security", (o, n) =>
                    {
                        o.Security = n.CreateList(LoadSecurityRequirement);
                    }
                },
            };

        private static readonly PatternFieldMap<OpenApiOperation> _operationPatternFields =
            new PatternFieldMap<OpenApiOperation>
            {
                {s => s.StartsWith("x-"), (o, p, n) => o.AddExtension(p, LoadExtension(p, n))}
            };

        private static readonly FixedFieldMap<OpenApiResponses> _responsesFixedFields =
            new FixedFieldMap<OpenApiResponses>();

        private static readonly PatternFieldMap<OpenApiResponses> _responsesPatternFields =
            new PatternFieldMap<OpenApiResponses>
            {
                {s => !s.StartsWith("x-"), (o, p, n) => o.Add(p, LoadResponse(n))},
                {s => s.StartsWith("x-"), (o, p, n) => o.AddExtension(p, LoadExtension(p, n))}
            };

        internal static OpenApiOperation LoadOperation(ParseNode node)
        {
            // Reset these temp storage parameters for each operation.
            node.Context.SetTempStorage(TempStorageKeys.BodyParameter, null);
            node.Context.SetTempStorage(TempStorageKeys.FormParameters, null);
            node.Context.SetTempStorage(TempStorageKeys.OperationProduces, null);
            node.Context.SetTempStorage(TempStorageKeys.OperationConsumes, null);

            var mapNode = node.CheckMapNode("Operation");

            var operation = new OpenApiOperation();

            ParseMap(mapNode, operation, _operationFixedFields, _operationPatternFields);

            // Build request body based on information determined while parsing OpenApiOperation
            var bodyParameter = node.Context.GetFromTempStorage<OpenApiParameter>(TempStorageKeys.BodyParameter);
            if (bodyParameter != null)
            {
                operation.RequestBody = CreateRequestBody(node.Context, bodyParameter);
            }
            else
            {
                var formParameters = node.Context.GetFromTempStorage<List<OpenApiParameter>>(TempStorageKeys.FormParameters);
                if (formParameters != null)
                {
                    operation.RequestBody = CreateFormBody(node.Context, formParameters);
                }
            }

            foreach (var response in operation.Responses.Values)
            {
                ProcessProduces(response, node.Context);
            }

            return operation;
        }

        public static OpenApiResponses LoadResponses(ParseNode node)
        {
            var mapNode = node.CheckMapNode("Responses");

            var domainObject = new OpenApiResponses();

            ParseMap(mapNode, domainObject, _responsesFixedFields, _responsesPatternFields);

            return domainObject;
        }

        private static OpenApiRequestBody CreateFormBody(ParsingContext context, List<OpenApiParameter> formParameters)
        {
            var mediaType = new OpenApiMediaType
            {
                Schema = new OpenApiSchema
                {
                    Properties = formParameters.ToDictionary(
                        k => k.Name,
                        v =>
                        {
                            var schema = v.Schema;
                            schema.Description = v.Description;
                            return schema;
                        }),
                    Required = new HashSet<string>(formParameters.Where(p => p.Required).Select(p => p.Name))
                }
            };
            
            var consumes = context.GetFromTempStorage<List<string>>(TempStorageKeys.OperationConsumes) ??
                context.GetFromTempStorage<List<string>>(TempStorageKeys.GlobalConsumes) ??
                new List<string> {"application/x-www-form-urlencoded"};

            var formBody = new OpenApiRequestBody
            {
                Content = consumes.ToDictionary(
                    k => k,
                    v => mediaType)
            };

            return formBody;
        }

        internal static OpenApiRequestBody CreateRequestBody(
            ParsingContext context,
            OpenApiParameter bodyParameter)
        {
            var consumes = context.GetFromTempStorage<List<string>>(TempStorageKeys.OperationConsumes) ??
                context.GetFromTempStorage<List<string>>(TempStorageKeys.GlobalConsumes) ??
                new List<string> {"application/json"};

            var requestBody = new OpenApiRequestBody
            {
                Description = bodyParameter.Description,
                Required = bodyParameter.Required,
                Content = consumes.ToDictionary(
                    k => k,
                    v => new OpenApiMediaType
                    {
                        Schema = bodyParameter.Schema
                    })
            };

            return requestBody;
        }

        private static OpenApiTag LoadTagByReference(
            ParsingContext context,
            OpenApiDiagnostic diagnostic,
            string tagName)
        {
            var tagObject = new OpenApiTag()
            {
                UnresolvedReference = true,
                Reference = new OpenApiReference() { Id = tagName, Type = ReferenceType.Tag }
            };

            return tagObject;
        }
    }
}