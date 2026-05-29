// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Text.Json.Nodes;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OpenApi.Reader.V2
{
    /// <summary>
    /// Class containing logic to deserialize Open API V2 document into
    /// runtime Open API object model.
    /// </summary>
    internal static partial class OpenApiV2Deserializer
    {
        private static readonly FixedFieldMap<OpenApiResponse> _responseFixedFields = new()
        {
            {
                "description",
                (o, n, _, _) => o.Description = n.GetScalarValue()
            },
            {
                "headers",
                (o, n, t, c) => o.Headers = n.CreateMap(LoadHeader, t, c)
            },
            {
                "examples", LoadExamples
            },
            {
                "x-examples", LoadResponseExamplesExtension
            },
            {
                "schema",
                (o, n, t, c) => c.SetTempStorage(TempStorageKeys.ResponseSchema, LoadSchema(n, t, c), o)
            },
        };

        private static readonly PatternFieldMap<OpenApiResponse> _responsePatternFields =
            new()
            {
                {s => s.StartsWith(OpenApiConstants.ExtensionFieldNamePrefix, StringComparison.OrdinalIgnoreCase) && !s.Equals(OpenApiConstants.ExamplesExtension, StringComparison.OrdinalIgnoreCase), 
                    (o, p, n, _, c) => o.AddExtension(p, LoadExtension(p, n, c))}
            };

        private static readonly AnyFieldMap<OpenApiMediaType> _mediaTypeAnyFields =
            new()
            {
                {
                    OpenApiConstants.Example,
                    new(
                        m => m.Example,
                        (m, v) => m.Example = v,
                        m => m.Schema)
                }
            };

        private static void ProcessProduces(OpenApiResponse response, ParsingContext context)
        {
            if (response.Content == null)
            {
                response.Content = new Dictionary<string, OpenApiMediaType>();
            }
            else if (context.GetFromTempStorage<bool>(TempStorageKeys.ResponseProducesSet, response))
            {
                // Process "produces" only once since once specified at operation level it cannot be overridden.
                return;
            }

            var produces = context.GetFromTempStorage<List<string>>(TempStorageKeys.OperationProduces)
                ?? context.GetFromTempStorage<List<string>>(TempStorageKeys.GlobalProduces)
                ?? context.DefaultContentType ?? ["application/octet-stream"];

            var schema = context.GetFromTempStorage<IOpenApiSchema>(TempStorageKeys.ResponseSchema, response);
            var examples = context.GetFromTempStorage<Dictionary<string, IOpenApiExample>>(TempStorageKeys.Examples, response);

            foreach (var produce in produces)
            {
                if (response.Content.TryGetValue(produce, out var produceValue) && produceValue is OpenApiMediaType openApiMediaType)
                {
                    if (schema != null)
                    {
                        openApiMediaType.Schema = schema;
                        ProcessAnyFields(openApiMediaType, _mediaTypeAnyFields, context);
                    }
                }
                else
                {
                    var mediaType = new OpenApiMediaType
                    {
                        Schema = schema,
                        Examples = examples
                    };

                    response.Content.Add(produce, mediaType);
                }
            }

            context.SetTempStorage(TempStorageKeys.ResponseSchema, null, response);
            context.SetTempStorage(TempStorageKeys.Examples, null, response);
            context.SetTempStorage(TempStorageKeys.ResponseProducesSet, true, response);
        }

        private static void LoadResponseExamplesExtension(OpenApiResponse response, JsonNode node, OpenApiDocument? hostDocument, ParsingContext context)
        {
            var examples = LoadExamplesExtension(node, context);
            context.SetTempStorage(TempStorageKeys.Examples, examples, response);
        }

        private static Dictionary<string, IOpenApiExample> LoadExamplesExtension(JsonNode node, ParsingContext context)
        {
            var jsonObject = node.CheckMapNode(OpenApiConstants.ExamplesExtension, context);
            var examples = new Dictionary<string, IOpenApiExample>();

            foreach (var examplesNode in jsonObject)
            {
                // Load the media type node as an OpenApiExample object
                var example = new OpenApiExample();
                var exampleNode = examplesNode.Value.CheckMapNode(examplesNode.Key, context);
                foreach (var jsonNode in exampleNode)
                {
                    switch (jsonNode.Key.ToLowerInvariant())
                    {
                        case "summary":
                            example.Summary = jsonNode.Value.GetScalarValue();
                            break;
                        case "description":
                            example.Description = jsonNode.Value.GetScalarValue();
                            break;
                        case "value":
                            example.Value = jsonNode.Value ?? JsonNullSentinel.JsonNull;
                            break;
                        case "externalValue":
                            example.ExternalValue = jsonNode.Value.GetScalarValue();
                            break;
                    }
                }

                examples.Add(examplesNode.Key, example);
            }

            return examples;
        }

        private static void LoadExamples(OpenApiResponse response, JsonNode node, OpenApiDocument? hostDocument, ParsingContext context)
        {
            var jsonObject = node.CheckMapNode("examples", context);

            foreach (var mediaTypeNode in jsonObject)
            {
                LoadExample(response, mediaTypeNode.Key, mediaTypeNode.Value, context);
            }
        }

        private static void LoadExample(OpenApiResponse response, string mediaType, JsonNode? node, ParsingContext context)
        {
            var exampleNode = node ?? JsonNullSentinel.JsonNull;

            response.Content ??= new Dictionary<string, OpenApiMediaType>();

            OpenApiMediaType mediaTypeObject;
            if (response.Content.TryGetValue(mediaType, out var value) && value is OpenApiMediaType mediaTypeValue)
            {
                mediaTypeObject = mediaTypeValue;
            }
            else
            {
                mediaTypeObject = new()
                {
                    Schema = context.GetFromTempStorage<IOpenApiSchema>(TempStorageKeys.ResponseSchema, response)
                };
                response.Content.Add(mediaType, mediaTypeObject);
            }

            mediaTypeObject.Example = exampleNode;
        }

        public static IOpenApiResponse LoadResponse(JsonNode node, OpenApiDocument hostDocument, ParsingContext context)
        {
            var jsonObject = node.CheckMapNode("response", context);

            var pointer = jsonObject.GetReferencePointer();
            if (pointer != null)
            {
                var reference = GetReferenceIdAndExternalResource(pointer);
                return new OpenApiResponseReference(reference.Item1, hostDocument, reference.Item2);
            }

            var response = new OpenApiResponse();

            ParseMap(jsonObject, response, _responseFixedFields, _responsePatternFields, hostDocument, context);
            if (response.Content?.Values is not null)
            {
                foreach (var mediaType in response.Content.Values.OfType<OpenApiMediaType>())
                {
                    if (mediaType.Schema != null)
                    {
                        ProcessAnyFields(mediaType, _mediaTypeAnyFields, context);
                    }
                }
            }

            return response;
        }
    }
}
