// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Collections.Generic;
using System.IO;
using System.Text;
using FluentAssertions;
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
                    Schema = new OpenApiSchema()
                    {
                        Type = "array",
                        Items = new OpenApiSchema()
                        {
                            Type = "string"
                        }
                    },
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
                            Schema = new OpenApiSchema
                            {
                                Type = "string"
                            }
                        }
                    },
                    RequestBody = new OpenApiRequestBody
                    {
                        Content =
                        {
                            ["application/x-www-form-urlencoded"] = new OpenApiMediaType
                            {
                                Schema = new OpenApiSchema
                                {
                                    Properties =
                                    {
                                        ["name"] = new OpenApiSchema
                                        {
                                            Description = "Updated name of the pet",
                                            Type = "string"
                                        },
                                        ["status"] = new OpenApiSchema
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
                            ["multipart/form-data"] = new OpenApiMediaType
                            {
                                Schema = new OpenApiSchema
                                {
                                    Properties =
                                    {
                                        ["name"] = new OpenApiSchema
                                        {
                                            Description = "Updated name of the pet",
                                            Type = "string"
                                        },
                                        ["status"] = new OpenApiSchema
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
                            Schema = new OpenApiSchema
                            {
                                Type = "string"
                            }
                        },
                        new OpenApiParameter
                        {
                            Name = "petName",
                            In = ParameterLocation.Path,
                            Description = "Name of pet that needs to be updated",
                            Required = true,
                            Schema = new OpenApiSchema
                            {
                                Type = "string"
                            }
                        }
                    },
                    RequestBody = new OpenApiRequestBody
                    {
                        Content =
                        {
                            ["application/x-www-form-urlencoded"] = new OpenApiMediaType
                            {
                                Schema = new OpenApiSchema
                                {
                                    Properties =
                                    {
                                        ["name"] = new OpenApiSchema
                                        {
                                            Description = "Updated name of the pet",
                                            Type = "string"
                                        },
                                        ["status"] = new OpenApiSchema
                                        {
                                            Description = "Updated status of the pet",
                                            Type = "string"
                                        },
                                        ["skill"] = new OpenApiSchema
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
                            ["multipart/form-data"] = new OpenApiMediaType
                            {
                                Schema = new OpenApiSchema
                                {
                                    Properties =
                                    {
                                        ["name"] = new OpenApiSchema
                                        {
                                            Description = "Updated name of the pet",
                                            Type = "string"
                                        },
                                        ["status"] = new OpenApiSchema
                                        {
                                            Description = "Updated status of the pet",
                                            Type = "string"
                                        },
                                        ["skill"] = new OpenApiSchema
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
            var operation = OpenApiV2Deserializer.LoadPathItem(node);

            // Assert
            operation.ShouldBeEquivalentTo(_basicPathItemWithFormData);
        }
    }
}