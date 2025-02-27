// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.OpenApi.Expressions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Models.References;
using Microsoft.OpenApi.Reader;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.V3Tests
{
    [Collection("DefaultSettings")]
    public class OpenApiCallbackTests
    {
        private const string SampleFolderPath = "V3Tests/Samples/OpenApiCallback/";
        [Fact]
        public async Task ParseBasicCallbackShouldSucceed()
        {
            // Act
            var callback = await OpenApiModelFactory.LoadAsync<OpenApiCallback>(Path.Combine(SampleFolderPath, "basicCallback.yaml"), OpenApiSpecVersion.OpenApi3_0, new(), SettingsFixture.ReaderSettings);

            // Assert
            Assert.Equivalent(
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
                }, callback);
        }

        [Fact]
        public async Task ParseCallbackWithReferenceShouldSucceed()
        {
            using var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "callbackWithReference.yaml"));

            // Act
            var result = await OpenApiModelFactory.LoadAsync(stream, OpenApiConstants.Yaml, SettingsFixture.ReaderSettings);

            // Assert
            var path = result.Document.Paths.First().Value;
            var subscribeOperation = path.Operations[OperationType.Post];

            var callback = subscribeOperation.Callbacks["simpleHook"];

            Assert.Equivalent(
                new OpenApiDiagnostic() { SpecificationVersion = OpenApiSpecVersion.OpenApi3_0 }, result.Diagnostic);

            Assert.Equivalent(
                new OpenApiCallbackReference("simpleHook", result.Document)
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
                }, callback);
        }

        [Fact]
        public async Task ParseMultipleCallbacksWithReferenceShouldSucceed()
        {
            // Act
            var result = await OpenApiModelFactory.LoadAsync(Path.Combine(SampleFolderPath, "multipleCallbacksWithReference.yaml"), SettingsFixture.ReaderSettings);

            // Assert
            var path = result.Document.Paths.First().Value;
            var subscribeOperation = path.Operations[OperationType.Post];

            Assert.Equivalent(
                new OpenApiDiagnostic() { SpecificationVersion = OpenApiSpecVersion.OpenApi3_0 }, result.Diagnostic);

            var callback1 = subscribeOperation.Callbacks["simpleHook"];

            Assert.Equivalent(
                new OpenApiCallbackReference("simpleHook", result.Document)
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
                }, callback1);

            var callback2 = subscribeOperation.Callbacks["callback2"];
            Assert.Equivalent(
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
                }, callback2);

            var callback3 = subscribeOperation.Callbacks["callback3"];
            Assert.Equivalent(
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
                }, callback3);
        }
    }
}
