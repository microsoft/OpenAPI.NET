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
            using var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "basicCallback.yaml"));
            // Arrange
            var yamlStream = new YamlStream();
            yamlStream.Load(new StreamReader(stream));
            var yamlNode = yamlStream.Documents.First().RootNode;

            var diagnostic = new OpenApiDiagnostic();
            var context = new ParsingContext(diagnostic);

            var node = new MapNode(context, (YamlMappingNode)yamlNode);

            // Act
            var callback = OpenApiV3Deserializer.LoadCallback(node);

            // Assert
            diagnostic.Should().BeEquivalentTo(new OpenApiDiagnostic());

            callback.Should().BeEquivalentTo(
                new OpenApiCallback
                {
                    PathItems =
                    {
                        [RuntimeExpression.Build("$request.body#/url")]
                            = new()
                            {
                                Operations =
                                {
                                    [OperationType.Post] =
                                        new()
                                        {
                                            RequestBody = new()
                                            {
                                                Content =
                                                {
                                                    ["application/json"] = null
                                                }
                                            },
                                            Responses = new()
                                            {
                                                ["200"] = new()
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
            var openApiDoc = new OpenApiStreamReader().Read(stream, out var diagnostic);

            // Assert
            var path = openApiDoc.Paths.First().Value;
            var subscribeOperation = path.Operations[OperationType.Post];

            var callback = subscribeOperation.Callbacks["simpleHook"];

            diagnostic.Should().BeEquivalentTo(
                new OpenApiDiagnostic { SpecificationVersion = OpenApiSpecVersion.OpenApi3_0 });

            callback.Should().BeEquivalentTo(
                new OpenApiCallback
                {
                    PathItems =
                    {
                        [RuntimeExpression.Build("$request.body#/url")]= new()
                        {
                            Operations = {
                                [OperationType.Post] = new()
                                {
                                    RequestBody = new()
                                    {
                                        Content =
                                        {
                                            ["application/json"] = new()
                                            {
                                                Schema = new()
                                                {
                                                    Type = "object"
                                                }
                                            }
                                        }
                                    },
                                    Responses = {
                                        ["200"]= new()
                                        {
                                            Description = "Success"
                                        }
                                    }
                                }
                            }
                        }
                    },
                    Reference = new()
                    {
                        Type = ReferenceType.Callback,
                        Id = "simpleHook",
                        HostDocument = openApiDoc
                    }
                });
        }

        [Fact]
        public void ParseMultipleCallbacksWithReferenceShouldSucceed()
        {
            using var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "multipleCallbacksWithReference.yaml"));
            // Act
            var openApiDoc = new OpenApiStreamReader().Read(stream, out var diagnostic);

            // Assert
            var path = openApiDoc.Paths.First().Value;
            var subscribeOperation = path.Operations[OperationType.Post];

            diagnostic.Should().BeEquivalentTo(
                new OpenApiDiagnostic { SpecificationVersion = OpenApiSpecVersion.OpenApi3_0 });

            var callback1 = subscribeOperation.Callbacks["simpleHook"];

            callback1.Should().BeEquivalentTo(
                new OpenApiCallback
                {
                    PathItems =
                    {
                        [RuntimeExpression.Build("$request.body#/url")]= new()
                        {
                            Operations = {
                                [OperationType.Post] = new()
                                {
                                    RequestBody = new()
                                    {
                                        Content =
                                        {
                                            ["application/json"] = new()
                                            {
                                                Schema = new()
                                                {
                                                    Type = "object"
                                                }
                                            }
                                        }
                                    },
                                    Responses = {
                                        ["200"]= new()
                                        {
                                            Description = "Success"
                                        }
                                    }
                                }
                            }
                        }
                    },
                    Reference = new()
                    {
                        Type = ReferenceType.Callback,
                        Id = "simpleHook",
                        HostDocument = openApiDoc
                    }
                });

            var callback2 = subscribeOperation.Callbacks["callback2"];
            callback2.Should().BeEquivalentTo(
                new OpenApiCallback
                {
                    PathItems =
                    {
                        [RuntimeExpression.Build("/simplePath")]= new()
                        {
                            Operations = {
                                [OperationType.Post] = new()
                                {
                                    RequestBody = new()
                                    {
                                        Description = "Callback 2",
                                        Content =
                                        {
                                            ["application/json"] = new()
                                            {
                                                Schema = new()
                                                {
                                                    Type = "string"
                                                }
                                            }
                                        }
                                    },
                                    Responses = {
                                        ["400"]= new()
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
                        [RuntimeExpression.Build(@"http://example.com?transactionId={$request.body#/id}&email={$request.body#/email}")] = new()
                        {
                            Operations = {
                                [OperationType.Post] = new()
                                {
                                    RequestBody = new()
                                    {
                                        Content =
                                        {
                                            ["application/xml"] = new()
                                            {
                                                Schema = new()
                                                {
                                                    Type = "object"
                                                }
                                            }
                                        }
                                    },
                                    Responses = {
                                        ["200"]= new()
                                        {
                                            Description = "Success"
                                        },
                                        ["401"]= new()
                                        {
                                            Description = "Unauthorized"
                                        },
                                        ["404"]= new()
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
