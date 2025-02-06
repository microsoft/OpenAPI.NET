// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Reader.ParseNodes;
using Microsoft.OpenApi.Reader.V2;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.V2Tests
{
    [Collection("DefaultSettings")]
    public class OpenApiPathItemTests
    {
        private const string SampleFolderPath = "V2Tests/Samples/OpenApiPathItem/";

        private static readonly OpenApiPathItem _basicPathItemWithFormData = new()
        {
            Parameters =
            [
                new OpenApiParameter()
                {
                    Name = "id",
                    In = ParameterLocation.Path,
                    Description = "ID of pet to use",
                    Required = true,
                    Schema = new OpenApiSchema()
                    {
                        Type = JsonSchemaType.Array,
                        Items = new OpenApiSchema()
                        {
                            Type = JsonSchemaType.String
                        }
                    },
                    Style = ParameterStyle.Simple
                }
            ],
            Operations =
            {
                [OperationType.Put] = new()
                {
                    Summary = "Puts a pet in the store with form data",
                    Description = "",
                    OperationId = "putPetWithForm",
                    Parameters =
                    [
                        new OpenApiParameter()
                        {
                            Name = "petId",
                            In = ParameterLocation.Path,
                            Description = "ID of pet that needs to be updated",
                            Required = true,
                            Schema = new OpenApiSchema()
                            {
                                Type = JsonSchemaType.String
                            }
                        }
                    ],
                    RequestBody = new OpenApiRequestBody()
                    {
                        Content =
                        {
                            ["application/x-www-form-urlencoded"] = new()
                            {
                                Schema = new OpenApiSchema()
                                {
                                    Type = JsonSchemaType.Object,
                                    Properties =
                                    {
                                        ["name"] = new OpenApiSchema()
                                        {
                                            Description = "Updated name of the pet",
                                            Type = JsonSchemaType.String
                                        },
                                        ["status"] = new OpenApiSchema()
                                        {
                                            Description = "Updated status of the pet",
                                            Type = JsonSchemaType.String
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
                               Schema = new OpenApiSchema()
                                {
                                    Type = JsonSchemaType.Object,
                                    Properties =
                                    {
                                        ["name"] = new OpenApiSchema()
                                        {
                                            Description = "Updated name of the pet",
                                            Type = JsonSchemaType.String
                                        },
                                        ["status"] = new OpenApiSchema()
                                        {
                                            Description = "Updated status of the pet",
                                            Type = JsonSchemaType.String
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
                        ["200"] = new OpenApiResponse()
                        {
                            Description = "Pet updated.",
                            Content = new Dictionary<string, OpenApiMediaType>
                                {
                                    ["application/json"] = new(),
                                    ["application/xml"] = new()
                                }
                        },
                        ["405"] = new OpenApiResponse()
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
                    Parameters =
                    [
                        new OpenApiParameter()
                        {
                            Name = "petId",
                            In = ParameterLocation.Path,
                            Description = "ID of pet that needs to be updated",
                            Required = true,
                            Schema = new OpenApiSchema()
                            {
                                Type = JsonSchemaType.String
                            }
                        },
                        new OpenApiParameter()
                        {
                            Name = "petName",
                            In = ParameterLocation.Path,
                            Description = "Name of pet that needs to be updated",
                            Required = true,
                            Schema = new OpenApiSchema()
                            {
                                Type = JsonSchemaType.String
                            }
                        }
                    ],
                    RequestBody = new OpenApiRequestBody()
                    {
                        Content =
                        {
                            ["application/x-www-form-urlencoded"] = new()
                            {
                                Schema = new OpenApiSchema()
                                {
                                    Type = JsonSchemaType.Object,
                                    Properties =
                                    {
                                        ["name"] = new OpenApiSchema()
                                        {
                                            Description = "Updated name of the pet",
                                            Type = JsonSchemaType.String
                                        },
                                        ["status"] = new OpenApiSchema()
                                        {
                                            Description = "Updated status of the pet",
                                            Type = JsonSchemaType.String
                                        },
                                        ["skill"] = new OpenApiSchema()
                                        {
                                            Description = "Updated skill of the pet",
                                            Type = JsonSchemaType.String
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
                                Schema = new OpenApiSchema()
                                {
                                    Type = JsonSchemaType.Object,
                                    Properties =
                                    {
                                        ["name"] = new OpenApiSchema()
                                        {
                                            Description = "Updated name of the pet",
                                            Type = JsonSchemaType.String
                                        },
                                        ["status"] = new OpenApiSchema()
                                        {
                                            Description = "Updated status of the pet",
                                            Type = JsonSchemaType.String
                                        },
                                        ["skill"] = new OpenApiSchema()
                                        {
                                            Description = "Updated skill of the pet",
                                            Type = JsonSchemaType.String
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
                        ["200"] = new OpenApiResponse()
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
            using var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "basicPathItemWithFormData.yaml"));
            var node = TestHelper.CreateYamlMapNode(stream);

            // Act
            var pathItem = OpenApiV2Deserializer.LoadPathItem(node, new());

            // Assert
            Assert.Equivalent(_basicPathItemWithFormData, pathItem);
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
            var pathItem = OpenApiV2Deserializer.LoadPathItem(node, new());

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
            var pathItem = OpenApiV2Deserializer.LoadPathItem(node, new());

            // Assert
            // FormData parameters at in the path level are pushed into Operation request bodies.
            Assert.True(pathItem.Operations[OperationType.Put].RequestBody != null);
            Assert.True(pathItem.Operations[OperationType.Post].RequestBody != null);
            Assert.Equal(2, pathItem.Operations.Count(o => o.Value.RequestBody != null));
        }
    }
}
