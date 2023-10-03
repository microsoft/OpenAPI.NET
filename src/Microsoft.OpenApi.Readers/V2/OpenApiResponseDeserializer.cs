// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
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
        private static readonly FixedFieldMap<OpenApiResponse> _responseFixedFields = new FixedFieldMap<OpenApiResponse>
        {
            {
                "description", (o, n) =>
                {
                    o.Description = n.GetScalarValue();
                }
            },
            {
                "headers", (o, n) =>
                {
                    o.Headers = n.CreateMap(LoadHeader);
                }
            },
            {
                "examples", (o, n) =>
                {
                    LoadExamples(o, n);
                }
            },
            {
                "schema", (o, n) =>
                {
                    n.Context.SetTempStorage(TempStorageKeys.ResponseSchema, LoadSchema(n), o);
                }
            },
        };

        private static readonly PatternFieldMap<OpenApiResponse> _responsePatternFields =
            new PatternFieldMap<OpenApiResponse>
            {
                {s => s.StartsWith("x-"), (o, p, n) => o.AddExtension(p, LoadExtension(p, n))}
            };

        private static readonly AnyFieldMap<OpenApiMediaType> _mediaTypeAnyFields =
            new AnyFieldMap<OpenApiMediaType>
            {
                {
                    OpenApiConstants.Example,
                    new AnyFieldMapParameter<OpenApiMediaType>(
                        m => m.Example,
                        (m, v) => m.Example = v,
                        m => m.Schema)
                }
            };

        private static void ProcessProduces(MapNode mapNode, OpenApiResponse response, ParsingContext context)
        {
            if (response.Content == null)
            {
                response.Content = new Dictionary<string, OpenApiMediaType>();
            }
            else if (context.GetFromTempStorage<bool>(TempStorageKeys.ResponseProducesSet, response))
            {
                // Process "produces" only once since once specified at operation level it cannot be overriden.
                return;
            }

            var produces = context.GetFromTempStorage<List<string>>(TempStorageKeys.OperationProduces)
                ?? context.GetFromTempStorage<List<string>>(TempStorageKeys.GlobalProduces)
                ?? context.DefaultContentType ?? new List<string> { "application/octet-stream" };

            var schema = context.GetFromTempStorage<OpenApiSchema>(TempStorageKeys.ResponseSchema, response);

            foreach (var produce in produces)
            {
                if (response.Content.TryGetValue(produce, out var produceValue))
                {
                    if (schema != null)
                    {
                        produceValue.Schema = schema;
                        ProcessAnyFields(mapNode, produceValue, _mediaTypeAnyFields);
                    }
                }
                else
                {
                    var mediaType = new OpenApiMediaType
                    {
                        Schema = schema
                    };

                    response.Content.Add(produce, mediaType);
                }
            }

            context.SetTempStorage(TempStorageKeys.ResponseSchema, null, response);
            context.SetTempStorage(TempStorageKeys.ResponseProducesSet, true, response);
        }

        private static void LoadExamples(OpenApiResponse response, ParseNode node)
        {
            var mapNode = node.CheckMapNode("examples");
            foreach (var mediaTypeNode in mapNode)
            {
                LoadExample(response, mediaTypeNode.Name, mediaTypeNode.Value);
            }
        }

        private static void LoadExample(OpenApiResponse response, string mediaType, ParseNode node)
        {
            var exampleNode = node.CreateAny();

            if (response.Content == null)
            {
                response.Content = new Dictionary<string, OpenApiMediaType>();
            }

            OpenApiMediaType mediaTypeObject;
            if (response.Content.ContainsKey(mediaType))
            {
                mediaTypeObject = response.Content[mediaType];
            }
            else
            {
                mediaTypeObject = new OpenApiMediaType
                {
                    Schema = node.Context.GetFromTempStorage<OpenApiSchema>(TempStorageKeys.ResponseSchema, response)
                };
                response.Content.Add(mediaType, mediaTypeObject);
            }

            mediaTypeObject.Example = exampleNode;
        }

        public static OpenApiResponse LoadResponse(ParseNode node)
        {
            var mapNode = node.CheckMapNode("response");

            var pointer = mapNode.GetReferencePointer();
            if (pointer != null)
            {
                return mapNode.GetReferencedObject<OpenApiResponse>(ReferenceType.Response, pointer);
            }

            var response = new OpenApiResponse();
            foreach (var property in mapNode)
            {
                property.ParseField(response, _responseFixedFields, _responsePatternFields);
            }

            foreach (var mediaType in response.Content.Values)
            {
                if (mediaType.Schema != null)
                {
                    ProcessAnyFields(mapNode, mediaType, _mediaTypeAnyFields);
                }
            }

            return response;
        }
    }
}
