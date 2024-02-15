// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.IO;
using FluentAssertions;
using Json.Schema;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Reader;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.V3Tests
{
    [Collection("DefaultSettings")]
    public class OpenApiParameterTests
    {
        private const string SampleFolderPath = "V3Tests/Samples/OpenApiParameter/";

        public OpenApiParameterTests() 
        {
            OpenApiReaderRegistry.RegisterReader("yaml", new OpenApiYamlReader());
        }

        [Fact]
        public void ParsePathParameterShouldSucceed()
        {
            // Arrange
            using var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "pathParameter.yaml"));

            // Act
            var parameter = OpenApiModelFactory.Load<OpenApiParameter>(stream, OpenApiSpecVersion.OpenApi3_0, "yaml", out _);

            // Assert
            parameter.Should().BeEquivalentTo(
                new OpenApiParameter
                {
                    In = ParameterLocation.Path,
                    Name = "username",
                    Description = "username to fetch",
                    Required = true,
                    Schema = new JsonSchemaBuilder().Type(SchemaValueType.String)
                });
        }

        [Fact]
        public void ParseQueryParameterShouldSucceed()
        {
            // Act
            var parameter = OpenApiModelFactory.Load<OpenApiParameter>(Path.Combine(SampleFolderPath, "queryParameter.yaml"), OpenApiSpecVersion.OpenApi3_0, out _);

            // Assert
            parameter.Should().BeEquivalentTo(
                new OpenApiParameter
                {
                    In = ParameterLocation.Query,
                    Name = "id",
                    Description = "ID of the object to fetch",
                    Required = false,
                    Schema = new JsonSchemaBuilder().Type(SchemaValueType.Array).Items(new JsonSchemaBuilder().Type(SchemaValueType.String)),
                    Style = ParameterStyle.Form,
                    Explode = true
                });
        }

        [Fact]
        public void ParseQueryParameterWithObjectTypeShouldSucceed()
        {
            // Act
            var parameter = OpenApiModelFactory.Load<OpenApiParameter>(Path.Combine(SampleFolderPath, "queryParameterWithObjectType.yaml"), OpenApiSpecVersion.OpenApi3_0, out _);

            // Assert
            parameter.Should().BeEquivalentTo(
                new OpenApiParameter
                {
                    In = ParameterLocation.Query,
                    Name = "freeForm",
                    Schema = new JsonSchemaBuilder()
                    .Type(SchemaValueType.Object)
                    .AdditionalProperties(new JsonSchemaBuilder().Type(SchemaValueType.Integer)),
                    Style = ParameterStyle.Form
                });
        }

        [Fact]
        public void ParseQueryParameterWithObjectTypeAndContentShouldSucceed()
        {
            // Arrange
            using var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "queryParameterWithObjectTypeAndContent.yaml"));

            // Act
            var parameter = OpenApiModelFactory.Load<OpenApiParameter>(stream, OpenApiSpecVersion.OpenApi3_0, "yaml", out _);

            // Assert
            parameter.Should().BeEquivalentTo(
                new OpenApiParameter
                {
                    In = ParameterLocation.Query,
                    Name = "coordinates",
                    Content =
                    {
                        ["application/json"] = new()
                        {
                            Schema = new JsonSchemaBuilder()
                                .Type(SchemaValueType.Object)
                                .Required("lat", "long")
                                .Properties(
                                    ("lat", new JsonSchemaBuilder()
                                        .Type(SchemaValueType.Number)
                                    ),
                                    ("long", new JsonSchemaBuilder()
                                        .Type(SchemaValueType.Number)
                                    )
                                )
                        }
                    }
                });
        }

        [Fact]
        public void ParseHeaderParameterShouldSucceed()
        {
            // Act
            var parameter = OpenApiModelFactory.Load<OpenApiParameter>(Path.Combine(SampleFolderPath, "headerParameter.yaml"), OpenApiSpecVersion.OpenApi3_0, out _);

            // Assert
            parameter.Should().BeEquivalentTo(
                new OpenApiParameter
                {
                    In = ParameterLocation.Header,
                    Name = "token",
                    Description = "token to be passed as a header",
                    Required = true,
                    Style = ParameterStyle.Simple,

                    Schema = new JsonSchemaBuilder()
                        .Type(SchemaValueType.Array)
                        .Items(new JsonSchemaBuilder()
                            .Type(SchemaValueType.Integer)
                            .Format("int64"))
                });
        }

        [Fact]
        public void ParseParameterWithNullLocationShouldSucceed()
        {
            // Act
            var parameter = OpenApiModelFactory.Load<OpenApiParameter>(Path.Combine(SampleFolderPath, "parameterWithNullLocation.yaml"), OpenApiSpecVersion.OpenApi3_0, out _);

            // Assert
            parameter.Should().BeEquivalentTo(
                new OpenApiParameter
                {
                    In = null,
                    Name = "username",
                    Description = "username to fetch",
                    Required = true,
                    Schema = new JsonSchemaBuilder()
                                .Type(SchemaValueType.String)
                });
        }

        [Fact]
        public void ParseParameterWithNoLocationShouldSucceed()
        {
            // Arrange
            using var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "parameterWithNoLocation.yaml"));

            // Act
            var parameter = OpenApiModelFactory.Load<OpenApiParameter>(stream, OpenApiSpecVersion.OpenApi3_0, "yaml", out _);

            // Assert
            parameter.Should().BeEquivalentTo(
                new OpenApiParameter
                {
                    In = null,
                    Name = "username",
                    Description = "username to fetch",
                    Required = true,
                    Schema = new JsonSchemaBuilder()
                                .Type(SchemaValueType.String)
                });
        }

        [Fact]
        public void ParseParameterWithUnknownLocationShouldSucceed()
        {
            // Arrange
            using var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "parameterWithUnknownLocation.yaml"));

            // Act
            var parameter = OpenApiModelFactory.Load<OpenApiParameter>(stream, OpenApiSpecVersion.OpenApi3_0, "yaml", out _);

            // Assert
            parameter.Should().BeEquivalentTo(
                new OpenApiParameter
                {
                    In = null,
                    Name = "username",
                    Description = "username to fetch",
                    Required = true,
                    Schema = new JsonSchemaBuilder()
                                .Type(SchemaValueType.String)
                });
        }

        [Fact]
        public void ParseParameterWithExampleShouldSucceed()
        {
            // Act
            var parameter = OpenApiModelFactory.Load<OpenApiParameter>(Path.Combine(SampleFolderPath, "parameterWithExample.yaml"), OpenApiSpecVersion.OpenApi3_0, out _);

            // Assert
            parameter.Should().BeEquivalentTo(
                new OpenApiParameter
                {
                    In = null,
                    Name = "username",
                    Description = "username to fetch",
                    Required = true,
                    Example = new OpenApiAny((float)5.0),
                    Schema = new JsonSchemaBuilder()
                        .Type(SchemaValueType.Number)
                        .Format("float")
                }, options => options.IgnoringCyclicReferences().Excluding(p => p.Example.Node.Parent));
        }

        [Fact]
        public void ParseParameterWithExamplesShouldSucceed()
        {
            // Act
            var parameter = OpenApiModelFactory.Load<OpenApiParameter>(Path.Combine(SampleFolderPath, "parameterWithExamples.yaml"), OpenApiSpecVersion.OpenApi3_0, out _);

            // Assert
            parameter.Should().BeEquivalentTo(
                new OpenApiParameter
                {
                    In = null,
                    Name = "username",
                    Description = "username to fetch",
                    Required = true,
                    Examples =
                    {
                        ["example1"] = new()
                        {
                            Value = new OpenApiAny(5.0)
                        },
                        ["example2"] = new()
                        {
                            Value = new OpenApiAny((float)7.5)
                        }
                    },
                    Schema = new JsonSchemaBuilder()
                                .Type(SchemaValueType.Number)
                                .Format("float")
                }, options => options.IgnoringCyclicReferences()
                .Excluding(p => p.Examples["example1"].Value.Node.Parent)
                .Excluding(p => p.Examples["example2"].Value.Node.Parent));
        }
    }
}
