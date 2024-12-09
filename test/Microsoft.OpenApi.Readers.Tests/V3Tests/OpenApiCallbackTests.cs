// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.IO;
using System.Linq;
using FluentAssertions;
using Microsoft.OpenApi.Expressions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Reader;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.V3Tests
{
    [Collection("DefaultSettings")]
    public class OpenApiCallbackTests
    {
        private const string SampleFolderPath = "V3Tests/Samples/OpenApiCallback/";
        public OpenApiCallbackTests()
        {
            OpenApiReaderRegistry.RegisterReader(OpenApiConstants.Yaml, new OpenApiYamlReader());
        }

        [Fact]
        public void ParseBasicCallbackShouldSucceed()
        {
            // Act
            var callback = OpenApiModelFactory.Load<OpenApiCallback>(Path.Combine(SampleFolderPath, "basicCallback.yaml"), OpenApiSpecVersion.OpenApi3_0, out var diagnostic);

            // Assert
            diagnostic.Should().BeEquivalentTo(new OpenApiDiagnostic());

            callback.Should().BeEquivalentTo(
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

        [Fact]
        public void ParseCallbackWithReferenceShouldSucceed()
        {
            using var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "callbackWithReference.yaml"));

            // Act
            var result = OpenApiModelFactory.Load(stream, OpenApiConstants.Yaml);

            // Assert
            var path = result.Document.Paths.First().Value;
            var subscribeOperation = path.Operations[OperationType.Post];

            var callback = subscribeOperation.Callbacks["simpleHook"];

            result.Diagnostic.Should().BeEquivalentTo(
                new OpenApiDiagnostic() { SpecificationVersion = OpenApiSpecVersion.OpenApi3_0 });

            callback.Should().BeEquivalentTo(
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
                                                Schema = new()
                                                {
                                                    Type = JsonSchemaType.Object
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
                        HostDocument = result.Document
                    }
                });
        }

        [Fact]
        public void ParseMultipleCallbacksWithReferenceShouldSucceed()
        {
            // Act
            var result = OpenApiModelFactory.Load(Path.Combine(SampleFolderPath, "multipleCallbacksWithReference.yaml"));

            // Assert
            var path = result.Document.Paths.First().Value;
            var subscribeOperation = path.Operations[OperationType.Post];

            result.Diagnostic.Should().BeEquivalentTo(
                new OpenApiDiagnostic() { SpecificationVersion = OpenApiSpecVersion.OpenApi3_0 });

            var callback1 = subscribeOperation.Callbacks["simpleHook"];

            callback1.Should().BeEquivalentTo(
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
                                                Schema = new()
                                                {
                                                    Type = JsonSchemaType.Object
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
                        HostDocument = result.Document
                    }
                });

            var callback2 = subscribeOperation.Callbacks["callback2"];
            callback2.Should().BeEquivalentTo(
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
                                                Schema = new()
                                                {
                                                    Type = JsonSchemaType.String
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
                });

            var callback3 = subscribeOperation.Callbacks["callback3"];
            callback3.Should().BeEquivalentTo(
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
                                                Schema = new()
                                                {
                                                    Type = JsonSchemaType.Object
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
                });
        }
    }
}
