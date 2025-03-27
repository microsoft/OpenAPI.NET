// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Reader.ParseNodes;
using Microsoft.OpenApi.Models.References;
using Microsoft.OpenApi.Models.Interfaces;
using System;

namespace Microsoft.OpenApi.Reader.V2
{
    /// <summary>
    /// Class containing logic to deserialize Open API V2 document into
    /// runtime Open API object model.
    /// </summary>
    internal static partial class OpenApiV2Deserializer
    {
        private static readonly FixedFieldMap<OpenApiOperation> _operationFixedFields =
            new()
            {
                {
                    "tags", (o, n, doc) => {
                        if (n.CreateSimpleList(
                            (valueNode, doc) =>
                            {
                                var val = valueNode.GetScalarValue();
                                if (string.IsNullOrEmpty(val))
                                    return null;   // Avoid exception on empty tag, we'll remove these from the list further on
                                return LoadTagByReference(val , doc);
                                },
                            doc)
                        // Filter out empty tags instead of excepting on them
                        .OfType<OpenApiTagReference>().ToList() is {Count: > 0} tags)
                        {
                            o.Tags = new HashSet<OpenApiTagReference>(tags, OpenApiTagComparer.Instance);
                        }
                    }
                },
                {
                    "summary",
                    (o, n, _) => o.Summary = n.GetScalarValue()
                },
                {
                    "description",
                    (o, n, _) => o.Description = n.GetScalarValue()
                },
                {
                    "externalDocs",
                    (o, n, t) => o.ExternalDocs = LoadExternalDocs(n, t)
                },
                {
                    "operationId",
                    (o, n, _) => o.OperationId = n.GetScalarValue()
                },
                {
                    "parameters",
                    (o, n, t) => o.Parameters = n.CreateList(LoadParameter, t)
                        .OfType<IOpenApiParameter>()
                        .ToList()
                },
                {
                    "consumes", (_, n, doc) => {
                        var consumes = n.CreateSimpleList((s, p) => s.GetScalarValue(), doc);
                        if (consumes.Count > 0) {
                            n.Context.SetTempStorage(TempStorageKeys.OperationConsumes,consumes);
                        }
                    }
                },
                {
                    "produces", (_, n, doc) => {
                        var produces = n.CreateSimpleList((s, p) => s.GetScalarValue(), doc);
                        if (produces.Count > 0) {
                            n.Context.SetTempStorage(TempStorageKeys.OperationProduces, produces);
                        }
                    }
                },
                {
                    "responses",
                    (o, n, t) => o.Responses = LoadResponses(n, t)
                },
                {
                    "deprecated",
                    (o, n, _) =>
                    {
                        var deprecated = n.GetScalarValue();
                        if (deprecated != null)
                        {
                            o.Deprecated = bool.Parse(deprecated);
                        }
                    }
                },
                {
                    "security",
                    (o, n, t) => o.Security = n.CreateList(LoadSecurityRequirement, t)
                },
            };

        private static readonly PatternFieldMap<OpenApiOperation> _operationPatternFields =
            new()
            {
                {s => s.StartsWith(OpenApiConstants.ExtensionFieldNamePrefix, StringComparison.OrdinalIgnoreCase), (o, p, n, _) => o.AddExtension(p, LoadExtension(p, n))}
            };

        private static readonly FixedFieldMap<OpenApiResponses> _responsesFixedFields = new();

        private static readonly PatternFieldMap<OpenApiResponses> _responsesPatternFields =
            new()
            {
                {s => !s.StartsWith(OpenApiConstants.ExtensionFieldNamePrefix, StringComparison.OrdinalIgnoreCase), (o, p, n, t) => o.Add(p, LoadResponse(n, t))},
                {s => s.StartsWith(OpenApiConstants.ExtensionFieldNamePrefix, StringComparison.OrdinalIgnoreCase), (o, p, n, _) => o.AddExtension(p, LoadExtension(p, n))}
            };

        internal static OpenApiOperation LoadOperation(ParseNode node, OpenApiDocument hostDocument)
        {
            // Reset these temp storage parameters for each operation.
            node.Context.SetTempStorage(TempStorageKeys.BodyParameter, null);
            node.Context.SetTempStorage(TempStorageKeys.FormParameters, null);
            node.Context.SetTempStorage(TempStorageKeys.OperationProduces, null);
            node.Context.SetTempStorage(TempStorageKeys.OperationConsumes, null);

            var mapNode = node.CheckMapNode("Operation");

            var operation = new OpenApiOperation();

            ParseMap(mapNode, operation, _operationFixedFields, _operationPatternFields, doc: hostDocument);

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

            var responses = operation.Responses;
            if (responses is not null)
            {
                foreach (var response in responses.Values.OfType<OpenApiResponse>())
                {
                    ProcessProduces(node.CheckMapNode("responses"), response, node.Context);
                }
            }            

            // Reset so that it's not picked up later
            node.Context.SetTempStorage(TempStorageKeys.OperationProduces, null);

            return operation;
        }

        public static OpenApiResponses LoadResponses(ParseNode node, OpenApiDocument hostDocument)
        {
            var mapNode = node.CheckMapNode("Responses");

            var domainObject = new OpenApiResponses();

            ParseMap(mapNode, domainObject, _responsesFixedFields, _responsesPatternFields, doc: hostDocument);

            return domainObject;
        }

        private static OpenApiRequestBody CreateFormBody(ParsingContext context, List<OpenApiParameter> formParameters)
        {
            var mediaType = new OpenApiMediaType
            {
                Schema = new OpenApiSchema()
                {
                    Properties = formParameters
                    .Where(p => p.Name != null)
                    .ToDictionary(
                        k => k.Name!,
                        v =>
                        {
                            var schema = v.Schema!.CreateShallowCopy();
                            schema.Description = v.Description;
                            if (schema is OpenApiSchema openApiSchema)
                            {
                                openApiSchema.Extensions = v.Extensions;
                            }
                            return schema;
                        }),
                    Required = new HashSet<string>(formParameters.Where(static p => p.Required && p.Name is not null).Select(static p => p.Name!), StringComparer.Ordinal)
                }
            };

            var consumes = context.GetFromTempStorage<List<string>>(TempStorageKeys.OperationConsumes) ??
                context.GetFromTempStorage<List<string>>(TempStorageKeys.GlobalConsumes) ??
                new List<string> { "application/x-www-form-urlencoded" };

            var formBody = new OpenApiRequestBody
            {
                Content = consumes.ToDictionary(
                    k => k,
                    _ => mediaType)
            };

            foreach (var value in formBody.Content.Values
                .Where(static x => x.Schema is not null
                                   && x.Schema.Properties is not null
                                   && x.Schema.Properties.Any()
                                   && x.Schema.Type == null).Select(static x => x.Schema).OfType<OpenApiSchema>())
            {
                value.Type = JsonSchemaType.Object;
            }

            return formBody;
        }

        internal static IOpenApiRequestBody CreateRequestBody(
            ParsingContext context,
            IOpenApiParameter bodyParameter)
        {
            var consumes = context.GetFromTempStorage<List<string>>(TempStorageKeys.OperationConsumes) ??
                context.GetFromTempStorage<List<string>>(TempStorageKeys.GlobalConsumes) ??
                new List<string> { "application/json" };

            var requestBody = new OpenApiRequestBody
            {
                Description = bodyParameter.Description,
                Required = bodyParameter.Required,
                Content = consumes.ToDictionary(
                    k => k,
                    _ => new OpenApiMediaType
                    {
                        Schema = bodyParameter.Schema,
                        Examples = bodyParameter.Examples
                    }),
                Extensions = bodyParameter.Extensions
            };

            if (requestBody.Extensions is not null && bodyParameter.Name is not null)
            {
                requestBody.Extensions[OpenApiConstants.BodyName] = new OpenApiAny(bodyParameter.Name);
            }            
            return requestBody;
        }

        private static OpenApiTagReference LoadTagByReference(
            string tagName, OpenApiDocument? hostDocument)
        {
            return new OpenApiTagReference(tagName, hostDocument);
        }
    }
}
