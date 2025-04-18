// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Models.Interfaces;
using Microsoft.OpenApi.Models.References;
using Microsoft.OpenApi.Writers;
using VerifyXunit;
using Xunit;

namespace Microsoft.OpenApi.Tests.Models
{
    [Collection("DefaultSettings")]
    public class OpenApiParameterTests
    {
        private static OpenApiParameter BasicParameter => new()
        {
            Name = "name1",
            In = ParameterLocation.Path
        };

        private static OpenApiParameterReference OpenApiParameterReference => new("example1");
        private static OpenApiParameter ReferencedParameter => new()
        {
            Name = "name1",
            In = ParameterLocation.Path
        };

        private static OpenApiParameter AdvancedPathParameterWithSchema => new()
        {
            Name = "name1",
            In = ParameterLocation.Path,
            Description = "description1",
            Required = true,
            Deprecated = false,
            Style = ParameterStyle.Simple,
            Explode = true,
            Schema = new OpenApiSchema()
            {
                Title = "title2",
                Description = "description2",
                OneOf =
                [
                    new OpenApiSchema() { Type = JsonSchemaType.Number, Format = "double" },
                    new OpenApiSchema() { Type = JsonSchemaType.String }
                ]
            },
            Examples = new Dictionary<string, IOpenApiExample>
            {
                ["test"] = new OpenApiExample()
                {
                    Summary = "summary3",
                    Description = "description3"
                }
            }
        };

        private static OpenApiParameter ParameterWithFormStyleAndExplodeFalse => new()
        {
            Name = "name1",
            In = ParameterLocation.Query,
            Description = "description1",
            Style = ParameterStyle.Form,
            Explode = false,
            Schema = new OpenApiSchema()
            {
                Type = JsonSchemaType.Array,
                Items = new OpenApiSchema()
                {
                    Enum = ["value1", "value2"]
                }
            }
        };

        private static OpenApiParameter ParameterWithFormStyleAndExplodeTrue => new()
        {
            Name = "name1",
            In = ParameterLocation.Query,
            Description = "description1",
            Style = ParameterStyle.Form,
            Explode = true,
            Schema = new OpenApiSchema()
            {
                Type = JsonSchemaType.Array,
                Items = new OpenApiSchema()
                {
                    Enum = ["value1", "value2"]
                }
            }
        };

        private static OpenApiParameter QueryParameterWithMissingStyle => new OpenApiParameter
        {
            Name = "id",
            In = ParameterLocation.Query,
            Schema = new OpenApiSchema()
            {
                Type = JsonSchemaType.Object,
                AdditionalProperties = new OpenApiSchema
                {
                    Type = JsonSchemaType.Integer
                }
            }
        };

        private static OpenApiParameter AdvancedHeaderParameterWithSchemaTypeObject => new()
        {
            Name = "name1",
            In = ParameterLocation.Header,
            Description = "description1",
            Required = true,
            Deprecated = false,

            Style = ParameterStyle.Simple,
            Explode = true,
            Schema = new OpenApiSchema()
            {
                Type = JsonSchemaType.Object
            },
            Examples = new Dictionary<string, IOpenApiExample>
            {
                ["test"] = new OpenApiExample()
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
            Assert.Equal(expectedExplode, parameter.Explode);
        }

        [Theory]
        [InlineData(ParameterLocation.Path, ParameterStyle.Simple)]
        [InlineData(ParameterLocation.Query, ParameterStyle.Form)]
        [InlineData(ParameterLocation.Header, ParameterStyle.Simple)]
        [InlineData(ParameterLocation.Cookie, ParameterStyle.Form)]
        [InlineData(null, ParameterStyle.Simple)]
        public async Task WhenStyleAndInIsNullTheDefaultValueOfStyleShouldBeSimple(ParameterLocation? inValue, ParameterStyle expectedStyle)
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
            await writer.FlushAsync();

            Assert.Equal(expectedStyle, parameter.Style);
        }

        [Fact]
        public async Task SerializeQueryParameterWithMissingStyleSucceeds()
        {
            // Arrange
            var expected = @"name: id
in: query
schema:
  type: object
  additionalProperties:
    type: integer";

            // Act
            var actual = await QueryParameterWithMissingStyle.SerializeAsYamlAsync(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            Assert.Equal(expected.MakeLineBreaksEnvironmentNeutral(), actual.MakeLineBreaksEnvironmentNeutral());
        }

        [Fact]
        public async Task SerializeBasicParameterAsV3JsonWorks()
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
            var actual = await BasicParameter.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task SerializeAdvancedParameterAsV3JsonWorks()
        {
            // Arrange
            var expected =
                """
                {
                  "name": "name1",
                  "in": "path",
                  "description": "description1",
                  "required": true,
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
            var actual = await AdvancedPathParameterWithSchema.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task SerializeAdvancedParameterAsV2JsonWorks()
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
            var actual = await AdvancedPathParameterWithSchema.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi2_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            Assert.Equal(expected, actual);
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
            await writer.FlushAsync();

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
            ReferencedParameter.SerializeAsV3(writer);
            await writer.FlushAsync();

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
            await writer.FlushAsync();

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
            ReferencedParameter.SerializeAsV2(writer);
            await writer.FlushAsync();

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
            await writer.FlushAsync();

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
            ParameterWithFormStyleAndExplodeFalse.SerializeAsV3(writer);
            await writer.FlushAsync();

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
            ParameterWithFormStyleAndExplodeTrue.SerializeAsV3(writer);
            await writer.FlushAsync();

            // Assert
            await Verifier.Verify(outputStringWriter).UseParameters(produceTerseOutput);
        }
    }
}
