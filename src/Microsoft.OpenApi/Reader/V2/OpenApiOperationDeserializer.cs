// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.Linq;
using System;
using System.Text.Json.Nodes;

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
                    "tags", (o, n, doc, c) => {
                        if (n.CreateSimpleList(
                            (jsonNode, document) =>
                            {
                                var val = jsonNode.GetScalarValue();
                                if (string.IsNullOrEmpty(val))
                                    return null;   // Avoid exception on empty tag, we'll remove these from the list further on
                                return LoadTagByReference(val!, document);
                            },
                            doc,
                            c)
                        // Filter out empty tags instead of excepting on them
                        .OfType<OpenApiTagReference>().ToList() is {Count: > 0} tags)
                        {
                            o.Tags = new HashSet<OpenApiTagReference>(tags, OpenApiTagComparer.Instance);
                        }
                    }
                },
                {
                    "summary",
                    (o, n, _, _) => o.Summary = n.GetScalarValue()
                },
                {
                    "description",
                    (o, n, _, _) => o.Description = n.GetScalarValue()
                },
                {
                    "externalDocs",
                    (o, n, t, c) => o.ExternalDocs = LoadExternalDocs(n, t, c)
                },
                {
                    "operationId",
                    (o, n, _, _) => o.OperationId = n.GetScalarValue()
                },
                {
                    "parameters",
                    (o, n, t, c) => o.Parameters = n.CreateList(LoadParameter, t, c)
                        .OfType<IOpenApiParameter>()
                        .ToList()
                },
                {
                    "consumes", (_, n, doc, c) => {
                        var consumes = n.CreateSimpleList((s, _) => s.GetScalarValue(), doc, c);
                        if (consumes.Count > 0) {
                            c.SetTempStorage(TempStorageKeys.OperationConsumes,consumes);
                        }
                    }
                },
                {
                    "produces", (_, n, doc, c) => {
                        var produces = n.CreateSimpleList((s, _) => s.GetScalarValue(), doc, c);
                        if (produces.Count > 0) {
                            c.SetTempStorage(TempStorageKeys.OperationProduces, produces);
                        }
                    }
                },
                {
                    "responses",
                    (o, n, t, c) => o.Responses = LoadResponses(n, t, c)
                },
                {
                    "deprecated",
                    (o, n, _, _) =>
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
                    (o, n, t, c) => { if (n is JsonArray)
                    {
                        o.Security = n.CreateList(LoadSecurityRequirement, t, c); 
                    } }
                },
            };

        private static readonly PatternFieldMap<OpenApiOperation> _operationPatternFields =
            new()
            {
                {s => s.StartsWith(OpenApiConstants.ExtensionFieldNamePrefix, StringComparison.OrdinalIgnoreCase), (o, p, n, _, c) => o.AddExtension(p, LoadExtension(p, n, c))}
            };

        private static readonly FixedFieldMap<OpenApiResponses> _responsesFixedFields = new();

        private static readonly PatternFieldMap<OpenApiResponses> _responsesPatternFields =
            new()
            {
                {s => !s.StartsWith(OpenApiConstants.ExtensionFieldNamePrefix, StringComparison.OrdinalIgnoreCase), (o, p, n, t, c) => o.Add(p, LoadResponse(n, t, c))},
                {s => s.StartsWith(OpenApiConstants.ExtensionFieldNamePrefix, StringComparison.OrdinalIgnoreCase), (o, p, n, _, c) => o.AddExtension(p, LoadExtension(p, n, c))}
            };

        internal static OpenApiOperation LoadOperation(JsonNode node, OpenApiDocument hostDocument, ParsingContext context)
        {
            // Reset these temp storage parameters for each operation.
            context.SetTempStorage(TempStorageKeys.BodyParameter, null);
            context.SetTempStorage(TempStorageKeys.FormParameters, null);
            context.SetTempStorage(TempStorageKeys.OperationProduces, null);
            context.SetTempStorage(TempStorageKeys.OperationConsumes, null);

            var jsonObject = node.CheckMapNode("Operation", context);

            var operation = new OpenApiOperation();

            ParseMap(jsonObject, operation, _operationFixedFields, _operationPatternFields, hostDocument, context);

            // Build request body based on information determined while parsing OpenApiOperation
            var bodyParameter = context.GetFromTempStorage<OpenApiParameter>(TempStorageKeys.BodyParameter);
            if (bodyParameter != null)
            {
                operation.RequestBody = CreateRequestBody(context, bodyParameter);
            }
            else
            {
                var formParameters = context.GetFromTempStorage<List<OpenApiParameter>>(TempStorageKeys.FormParameters);
                if (formParameters != null)
                {
                    operation.RequestBody = CreateFormBody(context, formParameters);
                }
            }

            var operationProduces = context.GetFromTempStorage<List<string>>(TempStorageKeys.OperationProduces);
            var responses = operation.Responses;
            if ((operationProduces is not null || jsonObject.ContainsKey("produces")) && responses is not null)
            {
                foreach (var response in responses.Values.OfType<OpenApiResponse>())
                {
                    ProcessProduces(response, context);
                }
            }            

            // Reset so that it's not picked up later
            context.SetTempStorage(TempStorageKeys.OperationProduces, null);

            return operation;
        }

        public static OpenApiResponses LoadResponses(JsonNode node, OpenApiDocument hostDocument, ParsingContext context)
        {
            var jsonObject = node.CheckMapNode("Responses", context);

            var domainObject = new OpenApiResponses();

            ParseMap(jsonObject, domainObject, _responsesFixedFields, _responsesPatternFields, hostDocument, context);

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
                ["application/x-www-form-urlencoded"];

            var formBody = new OpenApiRequestBody
            {
                Content = consumes.ToDictionary(
                    k => k,
                    _ => (IOpenApiMediaType)mediaType)
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
                ["application/json"];

            var requestBody = new OpenApiRequestBody
            {
                Description = bodyParameter.Description,
                Required = bodyParameter.Required,
                Content = consumes.ToDictionary(
                    k => k,
                    _ => (IOpenApiMediaType)new OpenApiMediaType
                    {
                        Schema = bodyParameter.Schema,
                        Examples = bodyParameter.Examples
                    }),
                Extensions = bodyParameter.Extensions
            };

            if (bodyParameter.Name is not null)
            {
                requestBody.Extensions ??= new Dictionary<string, IOpenApiExtension>();
                requestBody.Extensions[OpenApiConstants.BodyName] = new JsonNodeExtension(bodyParameter.Name);
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
