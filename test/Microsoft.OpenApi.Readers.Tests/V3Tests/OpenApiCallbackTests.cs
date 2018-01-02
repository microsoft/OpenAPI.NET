// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.IO;
using System.Linq;
using FluentAssertions;
using Microsoft.OpenApi.Expressions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers.ParseNodes;
using Microsoft.OpenApi.Readers.V3;
using SharpYaml.Serialization;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.V3Tests
{
    [Collection("DefaultSettings")]
    public class OpenApiCallbackTests
    {
        private const string SampleFolderPath = "V3Tests/Samples/OpenApiCallback/";

        [Fact]
        public void ParseBasicCallbackShouldSucceed()
        {
            using (var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "basicCallback.yaml")))
            {
                // Arrange
                var yamlStream = new YamlStream();
                yamlStream.Load(new StreamReader(stream));
                var yamlNode = yamlStream.Documents.First().RootNode;

                var context = new ParsingContext();
                var diagnostic = new OpenApiDiagnostic();

                var node = new MapNode(context, diagnostic, (YamlMappingNode)yamlNode);

                // Act
                var callback = OpenApiV3Deserializer.LoadCallback(node);

                // Assert
                diagnostic.ShouldBeEquivalentTo(new OpenApiDiagnostic());

                callback.ShouldBeEquivalentTo(
                    new OpenApiCallback
                    {
                        PathItems =
                        {
                            [RuntimeExpression.Build("$request.body#/url")]
                            = new OpenApiPathItem
                            {
                                Operations =
                                {
                                    [OperationType.Post] =
                                    new OpenApiOperation
                                    {
                                        RequestBody = new OpenApiRequestBody
                                        {
                                            Content =
                                            {
                                                ["application/json"] = null
                                            }
                                        },
                                        Responses = new OpenApiResponses
                                        {
                                            ["200"] = new OpenApiResponse
                                            {
                                                Description = "Success"
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    });
            }
        }

        [Fact]
        public void ParseAdvancedCallbackWithReferenceShouldSucceed()
        {
            using (var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "advancedCallbackWithReference.yaml")))
            {
                // Act
                var openApiDoc = new OpenApiStreamReader().Read(stream, out var context);

                // Assert
                var path = openApiDoc.Paths.First().Value;
                var subscribeOperation = path.Operations[OperationType.Post];

                var callback = subscribeOperation.Callbacks["simpleHook"];

                context.ShouldBeEquivalentTo(new OpenApiDiagnostic());

                callback.ShouldBeEquivalentTo(
                    new OpenApiCallback
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.Callback,
                            Id = "simpleHook",
                        },
                        PathItems =
                        {
                            [RuntimeExpression.Build("$request.body#/url")]
                            = new OpenApiPathItem
                            {
                                Operations =
                                {
                                    [OperationType.Post] =
                                    new OpenApiOperation
                                    {
                                        RequestBody = new OpenApiRequestBody
                                        {
                                            Content =
                                            {
                                                ["application/json"] = new OpenApiMediaType
                                                {
                                                    Schema = new OpenApiSchema
                                                    {
                                                        Type = "object"
                                                    }
                                                }
                                            }
                                        },
                                        Responses = new OpenApiResponses
                                        {
                                            ["200"] = new OpenApiResponse
                                            {
                                                Description = "Success"
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    });
            }
        }
    }
}