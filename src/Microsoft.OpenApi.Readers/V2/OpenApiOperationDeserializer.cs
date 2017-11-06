// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Any;
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
        private static readonly FixedFieldMap<OpenApiOperation> OperationFixedFields =
            new FixedFieldMap<OpenApiOperation>
            {
                {
                    "tags", (o, n) => o.Tags = n.CreateSimpleList(
                        valueNode =>
                            OpenApiReferenceService.LoadTagByReference(
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
                        "operationconsumes",
                        n.CreateSimpleList(s => s.GetScalarValue()))
                },
                {
                    "produces", (o, n) => n.Context.SetTempStorage(
                        "operationproduces",
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

        private static readonly PatternFieldMap<OpenApiOperation> OperationPatternFields =
            new PatternFieldMap<OpenApiOperation>
            {
                {s => s.StartsWith("x-"), (o, k, n) => o.Extensions.Add(k, new OpenApiString(n.GetScalarValue()))},
            };

        internal static OpenApiOperation LoadOperation(ParseNode node)
        {
            var mapNode = node.CheckMapNode("OpenApiOperation");

            var operation = new OpenApiOperation();

            ParseMap(mapNode, operation, OperationFixedFields, OperationPatternFields);

            // Build request body based on information determined while parsing OpenApiOperation
            var bodyParameter = node.Context.GetTempStorage<OpenApiParameter>("bodyParameter");
            if (bodyParameter != null)
            {
                operation.RequestBody = CreateRequestBody(node.Context, bodyParameter);
            }
            else
            {
                var formParameters = node.Context.GetTempStorage<List<OpenApiParameter>>("formParameters");
                if (formParameters != null)
                {
                    operation.RequestBody = CreateFormBody(formParameters);
                }
            }

            return operation;
        }
       
        public static FixedFieldMap<OpenApiResponses> ResponsesFixedFields = new FixedFieldMap<OpenApiResponses>();

        public static PatternFieldMap<OpenApiResponses> ResponsesPatternFields = new PatternFieldMap<OpenApiResponses>
        {
            {s => !s.StartsWith("x-"), (o, p, n) => o.Add(p, LoadResponse(n))},
            {s => s.StartsWith("x-"), (o, p, n) => o.AddExtension(p, n.CreateAny())}
        };

        public static OpenApiResponses LoadResponses(ParseNode node)
        {
            var mapNode = node.CheckMapNode("Responses");

            var domainObject = new OpenApiResponses();

            ParseMap(mapNode, domainObject, ResponsesFixedFields, ResponsesPatternFields);

            return domainObject;
        }

        private static OpenApiRequestBody CreateFormBody(List<OpenApiParameter> formParameters)
        {
            var mediaType = new OpenApiMediaType
            {
                Schema = new OpenApiSchema
                {
                    Properties = formParameters.ToDictionary(k => k.Name, v => v.Schema)
                }
            };

            var formBody = new OpenApiRequestBody
            {
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    {"application/x-www-form-urlencoded", mediaType}
                }
            };

            return formBody;
        }

        private static OpenApiRequestBody CreateRequestBody(ParsingContext context, OpenApiParameter bodyParameter)
        {
            var consumes = context.GetTempStorage<List<string>>("operationproduces") ??
                context.GetTempStorage<List<string>>("globalproduces") ?? new List<string> {"application/json"};

            var requestBody = new OpenApiRequestBody
            {
                Description = bodyParameter.Description,
                Required = bodyParameter.Required,
                Content = consumes.ToDictionary(
                    k => k,
                    v => new OpenApiMediaType
                    {
                        Schema = bodyParameter.Schema // Should we clone this?
                    })
            };

            return requestBody;
        }
    }
}