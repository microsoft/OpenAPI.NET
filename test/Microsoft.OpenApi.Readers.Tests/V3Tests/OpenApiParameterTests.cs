// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.IO;
using FluentAssertions;
using Microsoft.OpenApi.Reader;
using Xunit;
using Microsoft.OpenApi.Reader.V3;
using System.Threading.Tasks;
using System.Net.Http;

namespace Microsoft.OpenApi.Readers.Tests.V3Tests
{
    [Collection("DefaultSettings")]
    public class OpenApiParameterTests
    {
        private const string SampleFolderPath = "V3Tests/Samples/OpenApiParameter/";

        [Fact]
        public async Task ParsePathParameterShouldSucceed()
        {
            // Arrange
            using var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "pathParameter.yaml"));

            // Act
            var parameter = await OpenApiModelFactory.LoadAsync<OpenApiParameter>(stream, OpenApiSpecVersion.OpenApi3_0, new(), settings: SettingsFixture.ReaderSettings);

            // Assert
            Assert.Equivalent(
                new OpenApiParameter
                {
                    In = ParameterLocation.Path,
                    Name = "username",
                    Description = "username to fetch",
                    Required = true,
                    Schema = new OpenApiSchema()
                    {
                        Type = JsonSchemaType.String
                    }
                }, parameter);
        }

        [Fact]
        public async Task ParseQueryParameterShouldSucceed()
        {
            // Act
            var parameter = await OpenApiModelFactory.LoadAsync<OpenApiParameter>(Path.Combine(SampleFolderPath, "queryParameter.yaml"), OpenApiSpecVersion.OpenApi3_0, new(), settings: SettingsFixture.ReaderSettings);

            // Assert
            Assert.Equivalent(
                new OpenApiParameter
                {
                    In = ParameterLocation.Query,
                    Name = "id",
                    Description = "ID of the object to fetch",
                    Required = false,
                    Schema = new OpenApiSchema()
                    {
                        Type = JsonSchemaType.Array,
                        Items = new OpenApiSchema()
                        {
                            Type = JsonSchemaType.String
                        }
                    },
                    Style = ParameterStyle.Form,
                    Explode = true
                }, parameter);
        }

        [Fact]
        public async Task ParseQueryParameterWithObjectTypeShouldSucceed()
        {
            // Act
            var parameter = await OpenApiModelFactory.LoadAsync<OpenApiParameter>(Path.Combine(SampleFolderPath, "queryParameterWithObjectType.yaml"), OpenApiSpecVersion.OpenApi3_0, new(), settings: SettingsFixture.ReaderSettings);

            // Assert
            Assert.Equivalent(
                new OpenApiParameter
                {
                    In = ParameterLocation.Query,
                    Name = "freeForm",
                    Schema = new OpenApiSchema()
                    {
                        Type = JsonSchemaType.Object,
                        AdditionalProperties = new OpenApiSchema()
                        {
                            Type = JsonSchemaType.Integer
                        }
                    },
                    Style = ParameterStyle.Form
                }, parameter);
        }

        [Fact]
        public async Task ParseQueryParameterWithObjectTypeAndContentShouldSucceed()
        {
            // Arrange
            using var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "queryParameterWithObjectTypeAndContent.yaml"));

            // Act
            var parameter = await OpenApiModelFactory.LoadAsync<OpenApiParameter>(stream, OpenApiSpecVersion.OpenApi3_0, new(), settings: SettingsFixture.ReaderSettings);

            // Assert
            Assert.Equivalent(
                new OpenApiParameter
                {
                    In = ParameterLocation.Query,
                    Name = "coordinates",
                    Content = new()
                    {
                        ["application/json"] = new()
                        {
                           Schema = new OpenApiSchema()
                           {
                                Type = JsonSchemaType.Object,
                                Required = new HashSet<string>
                                {
                                    "lat",
                                    "long"
                                },
                                Properties = new()
                                {
                                    ["lat"] = new OpenApiSchema()
                                    {
                                        Type = JsonSchemaType.Number
                                    },
                                    ["long"] = new OpenApiSchema()
                                    {
                                        Type = JsonSchemaType.Number
                                    }
                                }
                           }
                        }
                    }
                }, parameter);
        }

        [Fact]
        public async Task ParseHeaderParameterShouldSucceed()
        {
            // Act
            var parameter = await OpenApiModelFactory.LoadAsync<OpenApiParameter>(Path.Combine(SampleFolderPath, "headerParameter.yaml"), OpenApiSpecVersion.OpenApi3_0, new(), settings: SettingsFixture.ReaderSettings);

            // Assert
            Assert.Equivalent(
                new OpenApiParameter
                {
                    In = ParameterLocation.Header,
                    Name = "token",
                    Description = "token to be passed as a header",
                    Required = true,
                    Style = ParameterStyle.Simple,

                    Schema = new OpenApiSchema()
                    {
                        Type = JsonSchemaType.Array,
                        Items = new OpenApiSchema()
                        {
                            Type = JsonSchemaType.Integer,
                            Format = "int64",
                        }
                    }
                }, parameter);
        }

        [Fact]
        public async Task ParseParameterWithNullLocationShouldSucceed()
        {
            // Act
            var parameter = await OpenApiModelFactory.LoadAsync<OpenApiParameter>(Path.Combine(SampleFolderPath, "parameterWithNullLocation.yaml"), OpenApiSpecVersion.OpenApi3_0, new(), settings: SettingsFixture.ReaderSettings);

            // Assert
            Assert.Equivalent(
                new OpenApiParameter
                {
                    In = null,
                    Name = "username",
                    Description = "username to fetch",
                    Required = true,
                    Schema = new OpenApiSchema()
                    {
                        Type = JsonSchemaType.String
                    }
                }, parameter);
        }

        [Fact]
        public async Task ParseParameterWithNoLocationShouldSucceed()
        {
            // Arrange
            using var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "parameterWithNoLocation.yaml"));

            // Act
            var parameter = await OpenApiModelFactory.LoadAsync<OpenApiParameter>(stream, OpenApiSpecVersion.OpenApi3_0, new(), settings: SettingsFixture.ReaderSettings);

            // Assert
            Assert.Equivalent(
                new OpenApiParameter
                {
                    In = null,
                    Name = "username",
                    Description = "username to fetch",
                    Required = true,
                    Schema = new OpenApiSchema()
                    {
                        Type = JsonSchemaType.String
                    }
                }, parameter);
        }

        [Fact]
        public async Task ParseParameterWithUnknownLocationShouldSucceed()
        {
            // Arrange
            using var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "parameterWithUnknownLocation.yaml"));

            // Act
            var parameter = await OpenApiModelFactory.LoadAsync<OpenApiParameter>(stream, OpenApiSpecVersion.OpenApi3_0, new(), settings: SettingsFixture.ReaderSettings);

            // Assert
            Assert.Equivalent(
                new OpenApiParameter
                {
                    In = null,
                    Name = "username",
                    Description = "username to fetch",
                    Required = true,
                    Schema = new OpenApiSchema()
                    {
                        Type = JsonSchemaType.String
                    }
                }, parameter);
        }

        [Fact]
        public async Task ParseParameterWithExampleShouldSucceed()
        {
            // Act
            var parameter = await OpenApiModelFactory.LoadAsync<OpenApiParameter>(Path.Combine(SampleFolderPath, "parameterWithExample.yaml"), OpenApiSpecVersion.OpenApi3_0, new(), settings: SettingsFixture.ReaderSettings);

            // Assert
            parameter.Should().BeEquivalentTo(
                new OpenApiParameter
                {
                    In = null,
                    Name = "username",
                    Description = "username to fetch",
                    Required = true,
                    Example = (float)5.0,
                    Schema = new OpenApiSchema()
                    {
                        Type = JsonSchemaType.Number,
                        Format = "float"
                    }
                }, options => options.IgnoringCyclicReferences().Excluding(p => p.Example.Parent));
        }

        [Fact]
        public async Task ParseParameterWithExamplesShouldSucceed()
        {
            // Act
            var parameter = await OpenApiModelFactory.LoadAsync<OpenApiParameter>(Path.Combine(SampleFolderPath, "parameterWithExamples.yaml"), OpenApiSpecVersion.OpenApi3_0, new(), settings: SettingsFixture.ReaderSettings);

            // Assert
            parameter.Should().BeEquivalentTo(
                new OpenApiParameter
                {
                    In = null,
                    Name = "username",
                    Description = "username to fetch",
                    Required = true,
                    Examples = new Dictionary<string, IOpenApiExample>
                    {
                        ["example1"] = new OpenApiExample()
                        {
                            Value = 5.0
                        },
                        ["example2"] = new OpenApiExample()
                        {
                            Value = (float) 7.5
                        }
                    },
                    Schema = new OpenApiSchema()
                    {
                        Type = JsonSchemaType.Number,
                        Format = "float"
                    }
                }, options => options.IgnoringCyclicReferences()
                .Excluding(p => p.Examples["example1"].Value.Parent)
                .Excluding(p => p.Examples["example2"].Value.Parent));
        }

        [Fact]
        public void ParseParameterWithReferenceWorks()
        {
            // Arrange
            var parameter = new OpenApiParameter
            {
                Name = "tags",
                In = ParameterLocation.Query,
                Description = "tags to filter by",
                Required = false,
                Schema = new OpenApiSchema()
                {
                    Type = JsonSchemaType.Array,
                    Items = new OpenApiSchema
                    {
                        Type = JsonSchemaType.String
                    }
                }
            };
            var document = new OpenApiDocument
            {
                Info = new OpenApiInfo
                {
                    Version = "1.0.0",
                    Title = "Swagger Petstore (Simple)"
                },
                Servers =
                [
                    new OpenApiServer
                    {
                        Url = "http://petstore.swagger.io/api"
                    }
                ],
                Paths = new OpenApiPaths
                {
                    ["/pets"] = new OpenApiPathItem
                    {
                        Operations = new()
                        {
                            [HttpMethod.Get] = new OpenApiOperation
                            {
                                Description = "Returns all pets from the system that the user has access to",
                                OperationId = "findPets",
                                Parameters =
                                [
                                    new OpenApiParameterReference("tagsParameter"),
                                ],
                            }
                        }
                    }
                },
                Components = new OpenApiComponents
                {
                    Parameters = new Dictionary<string, IOpenApiParameter>()
                    {
                        ["tagsParameter"] = parameter,
                    }
                }
            };

            document.Workspace.RegisterComponents(document);

            using var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "parameterWithRef.yaml"));
            var node = TestHelper.CreateYamlMapNode(stream);

            var expected = document.Components.Parameters["tagsParameter"];

            // Act
            var param = OpenApiV3Deserializer.LoadParameter(node, document);

            // Assert
            Assert.Equivalent(expected, param);
        }
    }
}
