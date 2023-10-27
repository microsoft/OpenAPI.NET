// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers.ParseNodes;
using Microsoft.OpenApi.Readers.V2;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.V2Tests
{
    [Collection("DefaultSettings")]
    public class OpenApiPathItemTests
    {
        private const string SampleFolderPath = "V2Tests/Samples/OpenApiPathItem/";

        private static readonly OpenApiPathItem _basicPathItemWithFormData = new()
        {
            Parameters = new List<OpenApiParameter>
            {
                new()
                {
                    Name = "id",
                    In = ParameterLocation.Path,
                    Description = "ID of pet to use",
                    Required = true,
                    Schema = new()
                    {
                        Type = "array",
                        Items = new()
                        {
                            Type = "string"
                        }
                    },
                    Style = ParameterStyle.Simple
                }
            },
            Operations =
            {
                [OperationType.Put] = new()
                {
                    Summary = "Puts a pet in the store with form data",
                    Description = "",
                    OperationId = "putPetWithForm",
                    Parameters = new List<OpenApiParameter>
                    {
                        new()
                        {
                            Name = "petId",
                            In = ParameterLocation.Path,
                            Description = "ID of pet that needs to be updated",
                            Required = true,
                            Schema = new()
                            {
                                Type = "string"
                            }
                        }
                    },
                    RequestBody = new()
                    {
                        Content =
                        {
                            ["application/x-www-form-urlencoded"] = new()
                            {
                                Schema = new()
                                {
                                    Type = "object",
                                    Properties =
                                    {
                                        ["name"] = new()
                                        {
                                            Description = "Updated name of the pet",
                                            Type = "string"
                                        },
                                        ["status"] = new()
                                        {
                                            Description = "Updated status of the pet",
                                            Type = "string"
                                        }
                                    },
                                    Required = new HashSet<string>
                                    {
                                        "name"
                                    }
                                }
                            },
                            ["multipart/form-data"] = new()
                            {
                                Schema = new()
                                {
                                    Type = "object",
                                    Properties =
                                    {
                                        ["name"] = new()
                                        {
                                            Description = "Updated name of the pet",
                                            Type = "string"
                                        },
                                        ["status"] = new()
                                        {
                                            Description = "Updated status of the pet",
                                            Type = "string"
                                        }
                                    },
                                    Required = new HashSet<string>
                                    {
                                        "name"
                                    }
                                }
                            }
                        }
                    },
                    Responses = new()
                    {
                        ["200"] = new()
                        {
                            Description = "Pet updated.",
                            Content = new Dictionary<string, OpenApiMediaType>
                                {
                                    ["application/json"] = new(),
                                    ["application/xml"] = new()
                                }
                        },
                        ["405"] = new()
                        {
                            Description = "Invalid input",
                            Content = new Dictionary<string, OpenApiMediaType>
                                {
                                    ["application/json"] = new(),
                                    ["application/xml"] = new()
                                }
                        }
                    }
                },
                [OperationType.Post] = new()
                {
                    Summary = "Posts a pet in the store with form data",
                    Description = "",
                    OperationId = "postPetWithForm",
                    Parameters = new List<OpenApiParameter>
                    {
                        new()
                        {
                            Name = "petId",
                            In = ParameterLocation.Path,
                            Description = "ID of pet that needs to be updated",
                            Required = true,
                            Schema = new()
                            {
                                Type = "string"
                            }
                        },
                        new()
                        {
                            Name = "petName",
                            In = ParameterLocation.Path,
                            Description = "Name of pet that needs to be updated",
                            Required = true,
                            Schema = new()
                            {
                                Type = "string"
                            }
                        }
                    },
                    RequestBody = new()
                    {
                        Content =
                        {
                            ["application/x-www-form-urlencoded"] = new()
                            {
                                Schema = new()
                                {
                                    Type = "object",
                                    Properties =
                                    {
                                        ["name"] = new()
                                        {
                                            Description = "Updated name of the pet",
                                            Type = "string"
                                        },
                                        ["status"] = new()
                                        {
                                            Description = "Updated status of the pet",
                                            Type = "string"
                                        },
                                        ["skill"] = new()
                                        {
                                            Description = "Updated skill of the pet",
                                            Type = "string"
                                        }
                                    },
                                    Required = new HashSet<string>
                                    {
                                        "name"
                                    }
                                }
                            },
                            ["multipart/form-data"] = new()
                            {
                                Schema = new()
                                {
                                    Type = "object",
                                    Properties =
                                    {
                                        ["name"] = new()
                                        {
                                            Description = "Updated name of the pet",
                                            Type = "string"
                                        },
                                        ["status"] = new()
                                        {
                                            Description = "Updated status of the pet",
                                            Type = "string"
                                        },
                                        ["skill"] = new()
                                        {
                                            Description = "Updated skill of the pet",
                                            Type = "string"
                                        }
                                    },
                                    Required = new HashSet<string>
                                    {
                                        "name"
                                    }
                                }
                            }
                        }
                    },
                    Responses = new()
                    {
                        ["200"] = new()
                        {
                            Description = "Pet updated.",
                            Content = new Dictionary<string, OpenApiMediaType>
                                {
                                    ["application/json"] = new(),
                                    ["application/xml"] = new()
                                }
                        }
                    }
                }
            }
        };

        [Fact]
        public void ParseBasicPathItemWithFormDataShouldSucceed()
        {
            // Arrange
            MapNode node;
            using (var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "basicPathItemWithFormData.yaml")))
            {
                node = TestHelper.CreateYamlMapNode(stream);
            }

            // Act
            var pathItem = OpenApiV2Deserializer.LoadPathItem(node);

            // Assert
            pathItem.Should().BeEquivalentTo(_basicPathItemWithFormData);
        }

        [Fact]
        public void ParsePathItemWithFormDataPathParameterShouldSucceed()
        {
            // Arrange
            MapNode node;
            using (var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "pathItemWithFormDataPathParameter.yaml")))
            {
                node = TestHelper.CreateYamlMapNode(stream);
            }

            // Act
            var pathItem = OpenApiV2Deserializer.LoadPathItem(node);

            // Assert
            // FormData parameters at in the path level are pushed into Operation request bodies.
            Assert.True(pathItem.Operations[OperationType.Put].RequestBody != null);
            Assert.True(pathItem.Operations[OperationType.Post].RequestBody != null);
            Assert.Equal(2, pathItem.Operations.Count(o => o.Value.RequestBody != null));
        }
        [Fact]
        public void ParsePathItemBodyDataPathParameterShouldSucceed()
        {
            // Arrange
            MapNode node;
            using (var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "pathItemWithBodyPathParameter.yaml")))
            {
                node = TestHelper.CreateYamlMapNode(stream);
            }

            // Act
            var pathItem = OpenApiV2Deserializer.LoadPathItem(node);

            // Assert
            // FormData parameters at in the path level are pushed into Operation request bodies.
            Assert.True(pathItem.Operations[OperationType.Put].RequestBody != null);
            Assert.True(pathItem.Operations[OperationType.Post].RequestBody != null);
            Assert.Equal(2, pathItem.Operations.Count(o => o.Value.RequestBody != null));
        }
    }
}
