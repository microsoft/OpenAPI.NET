// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FluentAssertions;
using Json.Schema;
using Microsoft.OpenApi.Extensions;
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

        private static readonly OpenApiPathItem _basicPathItemWithFormData = new OpenApiPathItem()
        {
            Parameters = new List<OpenApiParameter>
            {
                new OpenApiParameter()
                {
                    Name = "id",
                    In = ParameterLocation.Path,
                    Description = "ID of pet to use",
                    Required = true,
                    Schema31 = new JsonSchemaBuilder().Type(SchemaValueType.Array).Items(new JsonSchemaBuilder().Type(SchemaValueType.String)),
                    Style = ParameterStyle.Simple
                }
            },
            Operations =
            {
                [OperationType.Put] = new OpenApiOperation
                {
                    Summary = "Puts a pet in the store with form data",
                    Description = "",
                    OperationId = "putPetWithForm",
                    Parameters = new List<OpenApiParameter>
                    {
                        new OpenApiParameter
                        {
                            Name = "petId",
                            In = ParameterLocation.Path,
                            Description = "ID of pet that needs to be updated",
                            Required = true,
                            Schema31 = new JsonSchemaBuilder().Type(SchemaValueType.String)
                        }
                    },
                    RequestBody = new OpenApiRequestBody
                    {
                        Content =
                        {
                            ["application/x-www-form-urlencoded"] = new OpenApiMediaType
                            {
                                Schema31 = new JsonSchemaBuilder()
                                .Properties(
                                    ("name", new JsonSchemaBuilder().Description("Updated name of the pet").Type(SchemaValueType.String)),
                                    ("status", new JsonSchemaBuilder().Description("Updated status of the pet").Type(SchemaValueType.String)))
                                .Required("name")
                            },
                            ["multipart/form-data"] = new OpenApiMediaType
                            {
                                Schema31 = new JsonSchemaBuilder()
                                .Properties(
                                    ("name", new JsonSchemaBuilder().Description("Updated name of the pet").Type(SchemaValueType.String)),
                                    ("status", new JsonSchemaBuilder().Description("Updated status of the pet").Type(SchemaValueType.String)))
                                .Required("name")
                            }
                        }
                    },
                    Responses = new OpenApiResponses
                    {
                        ["200"] = new OpenApiResponse
                        {
                            Description = "Pet updated.",
                            Content = new Dictionary<string, OpenApiMediaType>
                                {
                                    ["application/json"] = new OpenApiMediaType(),
                                    ["application/xml"] = new OpenApiMediaType()
                                }
                        },
                        ["405"] = new OpenApiResponse
                        {
                            Description = "Invalid input",
                            Content = new Dictionary<string, OpenApiMediaType>
                                {
                                    ["application/json"] = new OpenApiMediaType(),
                                    ["application/xml"] = new OpenApiMediaType()
                                }
                        }
                    }
                },
                [OperationType.Post] = new OpenApiOperation
                {
                    Summary = "Posts a pet in the store with form data",
                    Description = "",
                    OperationId = "postPetWithForm",
                    Parameters = new List<OpenApiParameter>
                    {
                        new OpenApiParameter
                        {
                            Name = "petId",
                            In = ParameterLocation.Path,
                            Description = "ID of pet that needs to be updated",
                            Required = true,
                            Schema31 = new JsonSchemaBuilder().Type(SchemaValueType.String)
                        },
                        new OpenApiParameter
                        {
                            Name = "petName",
                            In = ParameterLocation.Path,
                            Description = "Name of pet that needs to be updated",
                            Required = true,
                            Schema31 = new JsonSchemaBuilder().Type(SchemaValueType.String)
                        }
                    },
                    RequestBody = new OpenApiRequestBody
                    {
                        Content =
                        {
                            ["application/x-www-form-urlencoded"] = new OpenApiMediaType
                            {
                                Schema31 = new JsonSchemaBuilder()
                                .Properties(
                                    ("name", new JsonSchemaBuilder().Description("Updated name of the pet").Type(SchemaValueType.String)),
                                    ("status", new JsonSchemaBuilder().Description("Updated status of the pet").Type(SchemaValueType.String)),
                                    ("skill", new JsonSchemaBuilder().Description("Updated skill of the pet").Type(SchemaValueType.String)))
                                .Required("name")
                            },
                            ["multipart/form-data"] = new OpenApiMediaType
                            {
                                Schema31 = new JsonSchemaBuilder()
                                .Properties(
                                    ("name", new JsonSchemaBuilder().Description("Updated name of the pet").Type(SchemaValueType.String)),
                                    ("status", new JsonSchemaBuilder().Description("Updated status of the pet").Type(SchemaValueType.String)),
                                    ("skill", new JsonSchemaBuilder().Description("Updated skill of the pet").Type(SchemaValueType.String)))
                                .Required("name")
                            }
                        }
                    },
                    Responses = new OpenApiResponses
                    {
                        ["200"] = new OpenApiResponse
                        {
                            Description = "Pet updated.",
                            Content = new Dictionary<string, OpenApiMediaType>
                                {
                                    ["application/json"] = new OpenApiMediaType(),
                                    ["application/xml"] = new OpenApiMediaType()
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
