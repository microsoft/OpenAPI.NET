// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using FluentAssertions;
using Json.Schema;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Models.References;
using Microsoft.OpenApi.Writers;
using VerifyXunit;
using Xunit;

namespace Microsoft.OpenApi.Tests.Models
{
    [Collection("DefaultSettings")]
    public class OpenApiParameterTests
    {
        public static OpenApiParameter BasicParameter = new()
        {
            Name = "name1",
            In = ParameterLocation.Path
        };

        public static OpenApiParameterReference OpenApiParameterReference = new(ReferencedParameter, "example1");
        public static OpenApiParameter ReferencedParameter = new()
        {
            Name = "name1",
            In = ParameterLocation.Path
        };

        public static OpenApiParameter AdvancedPathParameterWithSchema = new()
        {
            Name = "name1",
            In = ParameterLocation.Path,
            Description = "description1",
            Required = true,
            Deprecated = false,
            Style = ParameterStyle.Simple,
            Explode = true,
            Schema = new JsonSchemaBuilder()
                        .Title("title2")
                        .Description("description2")
                        .OneOf(new JsonSchemaBuilder().Type(SchemaValueType.Number).Format("double").Build(),
                            new JsonSchemaBuilder().Type(SchemaValueType.String).Build())
                        .Build(),

            Examples = new Dictionary<string, OpenApiExample>
            {
                ["test"] = new()
                {
                    Summary = "summary3",
                    Description = "description3"
                }
            }
        };

        public static OpenApiParameter ParameterWithFormStyleAndExplodeFalse = new()
        {
            Name = "name1",
            In = ParameterLocation.Query,
            Description = "description1",
            Style = ParameterStyle.Form,
            Explode = false,
            Schema = new JsonSchemaBuilder()
            .Type(SchemaValueType.Array)
            .Items(
                new JsonSchemaBuilder()
                .Enum(new List<JsonNode>
                    {
                        new OpenApiAny("value1").Node,
                        new OpenApiAny("value2").Node
                    })
                .Build())
            .Build()

        };

        public static OpenApiParameter ParameterWithFormStyleAndExplodeTrue = new()
        {
            Name = "name1",
            In = ParameterLocation.Query,
            Description = "description1",
            Style = ParameterStyle.Form,
            Explode = true,
            Schema = new JsonSchemaBuilder()
            .Type(SchemaValueType.Array)
            .Items(
                new JsonSchemaBuilder()
                .Enum(new List<JsonNode>
                    {
                        new OpenApiAny("value1").Node,
                        new OpenApiAny("value2").Node
                    })
                .Build())
            .Build()

        };

        public static OpenApiParameter QueryParameterWithMissingStyle = new OpenApiParameter
        {
            Name = "id",
            In = ParameterLocation.Query,
            Schema = new JsonSchemaBuilder()
            .Type(SchemaValueType.Object)
            .AdditionalProperties(
                new JsonSchemaBuilder()
                .Type(SchemaValueType.Integer).Build())
            .AdditionalProperties(true)
            .Build()
        };

        public static OpenApiParameter AdvancedHeaderParameterWithSchemaReference = new OpenApiParameter
        {
            Name = "name1",
            In = ParameterLocation.Header,
            Description = "description1",
            Required = true,
            Deprecated = false,

            Style = ParameterStyle.Simple,
            Explode = true,
            Schema = new JsonSchemaBuilder().Ref("schemaObject1").Build(),
            Examples = new Dictionary<string, OpenApiExample>
            {
                ["test"] = new()
                {
                    Summary = "summary3",
                    Description = "description3"
                }
            }
        };

        public static OpenApiParameter AdvancedHeaderParameterWithSchemaTypeObject = new()
        {
            Name = "name1",
            In = ParameterLocation.Header,
            Description = "description1",
            Required = true,
            Deprecated = false,

            Style = ParameterStyle.Simple,
            Explode = true,
            Schema = new JsonSchemaBuilder().Type(SchemaValueType.Object),
            Examples = new Dictionary<string, OpenApiExample>
            {
                ["test"] = new()
                {
                    Summary = "summary3",
                    Description = "description3"
                }
            }
        };

        [Theory]
        [InlineData(ParameterStyle.Form, true)]
        [InlineData(ParameterStyle.SpaceDelimited, false)]
        public void WhenStyleIsFormTheDefaultValueOfExplodeShouldBeTrueOtherwiseFalse(ParameterStyle? style, bool expectedExplode)
        {
            // Arrange
            var parameter = new OpenApiParameter
            {
                Name = "name1",
                In = ParameterLocation.Query,
                Style = style
            };

            // Act & Assert
            parameter.Explode.Should().Be(expectedExplode);
        }

        [Theory]
        [InlineData(ParameterLocation.Path, ParameterStyle.Simple)]
        [InlineData(ParameterLocation.Query, ParameterStyle.Form)]
        [InlineData(ParameterLocation.Header, ParameterStyle.Simple)]
        [InlineData(ParameterLocation.Cookie, ParameterStyle.Form)]
        [InlineData(null, ParameterStyle.Simple)]
        public void WhenStyleAndInIsNullTheDefaultValueOfStyleShouldBeSimple(ParameterLocation? inValue, ParameterStyle expectedStyle)
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new OpenApiJsonWriterSettings { Terse = false });
            var parameter = new OpenApiParameter
            {
                Name = "name1",
                In = inValue,
            };

            // Act & Assert
            parameter.SerializeAsV3(writer);
            writer.Flush();

            parameter.Style.Should().Be(expectedStyle);
        }

        [Fact]
        public void SerializeQueryParameterWithMissingStyleSucceeds()
        {
            // Arrange
            var expected = @"name: id
in: query
schema:
  type: object
  additionalProperties:
    type: integer";

            // Act
            var actual = QueryParameterWithMissingStyle.SerializeAsYaml(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual.MakeLineBreaksEnvironmentNeutral().Should().Be(expected.MakeLineBreaksEnvironmentNeutral());
        }

        [Fact]
        public void SerializeBasicParameterAsV3JsonWorks()
        {
            // Arrange
            var expected =
                """
                {
                  "name": "name1",
                  "in": "path"
                }
                """;

            // Act
            var actual = BasicParameter.SerializeAsJson(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeAdvancedParameterAsV3JsonWorks()
        {
            // Arrange
            var expected =
                """
                {
                  "name": "name1",
                  "in": "path",
                  "description": "description1",
                  "required": true,
                  "style": "simple",
                  "explode": true,
                  "schema": {
                    "title": "title2",
                    "oneOf": [
                      {
                        "type": "number",
                        "format": "double"
                      },
                      {
                        "type": "string"
                      }
                    ],
                    "description": "description2"
                  },
                  "examples": {
                    "test": {
                      "summary": "summary3",
                      "description": "description3"
                    }
                  }
                }
                """;

            // Act
            var actual = AdvancedPathParameterWithSchema.SerializeAsJson(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeAdvancedParameterAsV2JsonWorks()
        {
            // Arrange
            var expected =
                """
                {
                  "in": "path",
                  "name": "name1",
                  "description": "description1",
                  "required": true,
                  "format": "double",
                  "x-examples": {
                    "test": {
                      "summary": "summary3",
                      "description": "description3"
                    }
                  }
                }
                """;

            // Act
            var actual = AdvancedPathParameterWithSchema.SerializeAsJson(OpenApiSpecVersion.OpenApi2_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task SerializeReferencedParameterAsV3JsonWorksAsync(bool produceTerseOutput)
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new() { Terse = produceTerseOutput });

            // Act
            OpenApiParameterReference.SerializeAsV3(writer);
            writer.Flush();

            // Assert
            await Verifier.Verify(outputStringWriter).UseParameters(produceTerseOutput);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task SerializeReferencedParameterAsV3JsonWithoutReferenceWorksAsync(bool produceTerseOutput)
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new() { Terse = produceTerseOutput });

            // Act
            ReferencedParameter.SerializeAsV3WithoutReference(writer);
            writer.Flush();

            // Assert
            await Verifier.Verify(outputStringWriter).UseParameters(produceTerseOutput);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task SerializeReferencedParameterAsV2JsonWorksAsync(bool produceTerseOutput)
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new() { Terse = produceTerseOutput });

            // Act
            OpenApiParameterReference.SerializeAsV2(writer);
            writer.Flush();

            // Assert
            await Verifier.Verify(outputStringWriter).UseParameters(produceTerseOutput);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task SerializeReferencedParameterAsV2JsonWithoutReferenceWorksAsync(bool produceTerseOutput)
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new() { Terse = produceTerseOutput });

            // Act
            ReferencedParameter.SerializeAsV2WithoutReference(writer);
            writer.Flush();

            // Assert
            await Verifier.Verify(outputStringWriter).UseParameters(produceTerseOutput);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task SerializeParameterWithSchemaTypeObjectAsV2JsonWorksAsync(bool produceTerseOutput)
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new() { Terse = produceTerseOutput });

            // Act
            AdvancedHeaderParameterWithSchemaTypeObject.SerializeAsV2(writer);
            writer.Flush();

            // Assert
            await Verifier.Verify(outputStringWriter).UseParameters(produceTerseOutput);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task SerializeParameterWithFormStyleAndExplodeFalseWorksAsync(bool produceTerseOutput)
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new() { Terse = produceTerseOutput });

            // Act
            ParameterWithFormStyleAndExplodeFalse.SerializeAsV3WithoutReference(writer);
            writer.Flush();

            // Assert
            await Verifier.Verify(outputStringWriter).UseParameters(produceTerseOutput);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task SerializeParameterWithFormStyleAndExplodeTrueWorksAsync(bool produceTerseOutput)
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new() { Terse = produceTerseOutput });

            // Act
            ParameterWithFormStyleAndExplodeTrue.SerializeAsV3WithoutReference(writer);
            writer.Flush();

            // Assert
            await Verifier.Verify(outputStringWriter).UseParameters(produceTerseOutput);
        }
    }
}
