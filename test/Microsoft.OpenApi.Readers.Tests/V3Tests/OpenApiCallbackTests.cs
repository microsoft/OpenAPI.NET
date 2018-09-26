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
        public void ParseCallbackWithReferenceShouldSucceed()
        {
            using (var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "callbackWithReference.yaml")))
            {
                // Act
                var openApiDoc = new OpenApiStreamReader().Read(stream, out var diagnostic);

                // Assert
                var path = openApiDoc.Paths.First().Value;
                var subscribeOperation = path.Operations[OperationType.Post];

                var callback = subscribeOperation.Callbacks["simpleHook"];

                diagnostic.ShouldBeEquivalentTo(
                    new OpenApiDiagnostic() { SpecificationVersion = OpenApiSpecVersion.OpenApi3_0 });

                callback.ShouldBeEquivalentTo(
                    new OpenApiCallback
                    {
                        PathItems = 
                        {
                            [RuntimeExpression.Build("$request.body#/url")]= new OpenApiPathItem {
                                Operations = {
                                    [OperationType.Post] = new OpenApiOperation()
                                    {
                                        RequestBody = new OpenApiRequestBody
                                        {
                                            Content =
                                            {
                                                ["application/json"] = new OpenApiMediaType
                                                {
                                                    Schema = new OpenApiSchema()
                                                    {
                                                        Type = "object"
                                                    }
                                                }
                                            }
                                        },
                                        Responses = {
                                            ["200"]= new OpenApiResponse
                                            {
                                                Description = "Success"
                                            }
                                        }
                                    }
                                }
                            }
                        },
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.Callback,
                            Id = "simpleHook",
                        }
                    });
            }
        }

        [Fact]
        public void ParseMultipleCallbacksWithReferenceShouldSucceed()
        {
            using ( var stream = Resources.GetStream( Path.Combine( SampleFolderPath, "multipleCallbacksWithReference.yaml" ) ) )
            {
                // Act
                var openApiDoc = new OpenApiStreamReader().Read( stream, out var diagnostic );

                // Assert
                var path = openApiDoc.Paths.First().Value;
                var subscribeOperation = path.Operations[OperationType.Post];

                diagnostic.ShouldBeEquivalentTo(
                    new OpenApiDiagnostic() { SpecificationVersion = OpenApiSpecVersion.OpenApi3_0 } );

                var callback1 = subscribeOperation.Callbacks["simpleHook"];

                callback1.ShouldBeEquivalentTo(
                    new OpenApiCallback
                    {
                        PathItems =
                        {
                            [RuntimeExpression.Build("$request.body#/url")]= new OpenApiPathItem {
                                Operations = {
                                    [OperationType.Post] = new OpenApiOperation()
                                    {
                                        RequestBody = new OpenApiRequestBody
                                        {
                                            Content =
                                            {
                                                ["application/json"] = new OpenApiMediaType
                                                {
                                                    Schema = new OpenApiSchema()
                                                    {
                                                        Type = "object"
                                                    }
                                                }
                                            }
                                        },
                                        Responses = {
                                            ["200"]= new OpenApiResponse
                                            {
                                                Description = "Success"
                                            }
                                        }
                                    }
                                }
                            }
                        },
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.Callback,
                            Id = "simpleHook",
                        }
                    } );

                var callback2 = subscribeOperation.Callbacks["callback2"];
                callback2.ShouldBeEquivalentTo(
                    new OpenApiCallback
                    {
                        PathItems =
                        {
                            [RuntimeExpression.Build("/simplePath")]= new OpenApiPathItem {
                                Operations = {
                                    [OperationType.Post] = new OpenApiOperation()
                                    {
                                        RequestBody = new OpenApiRequestBody
                                        {
                                            Description = "Callback 2",
                                            Content =
                                            {
                                                ["application/json"] = new OpenApiMediaType
                                                {
                                                    Schema = new OpenApiSchema()
                                                    {
                                                        Type = "string"
                                                    }
                                                }
                                            }
                                        },
                                        Responses = {
                                            ["400"]= new OpenApiResponse
                                            {
                                                Description = "Callback Response"
                                            }
                                        }
                                    }
                                },
                            }
                        }
                    } );

                var callback3 = subscribeOperation.Callbacks["callback3"];
                callback3.ShouldBeEquivalentTo(
                    new OpenApiCallback
                    {
                        PathItems =
                        {
                            [RuntimeExpression.Build(@"http://example.com?transactionId={$request.body#/id}&email={$request.body#/email}")] = new OpenApiPathItem {
                                Operations = {
                                    [OperationType.Post] = new OpenApiOperation()
                                    {
                                        RequestBody = new OpenApiRequestBody
                                        {
                                            Content =
                                            {
                                                ["application/xml"] = new OpenApiMediaType
                                                {
                                                    Schema = new OpenApiSchema()
                                                    {
                                                        Type = "object"
                                                    }
                                                }
                                            }
                                        },
                                        Responses = {
                                            ["200"]= new OpenApiResponse
                                            {
                                                Description = "Success"
                                            },
                                            ["401"]= new OpenApiResponse
                                            {
                                                Description = "Unauthorized"
                                            },
                                            ["404"]= new OpenApiResponse
                                            {
                                                Description = "Not Found"
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    } );
            }
        }
    }
}