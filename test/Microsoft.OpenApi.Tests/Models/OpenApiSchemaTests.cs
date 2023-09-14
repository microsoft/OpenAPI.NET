// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.OpenApi.Tests.Models
{
    [Collection("DefaultSettings")]
    [UsesVerify]
    public class OpenApiSchemaTests
    {
        public static OpenApiSchema BasicSchema = new OpenApiSchema();

        public static OpenApiSchema AdvancedSchemaNumber = new OpenApiSchema
        {
            Title = "title1",
            MultipleOf = 3,
            Maximum = 42,
            ExclusiveMinimum = true,
            Minimum = 10,
            Default = new OpenApiInteger(15),
            Type = "integer",

            Nullable = true,
            ExternalDocs = new OpenApiExternalDocs
            {
                Url = new Uri("http://example.com/externalDocs")
            }
        };

        public static OpenApiSchema AdvancedSchemaObject = new OpenApiSchema
        {
            Title = "title1",
            Properties = new Dictionary<string, OpenApiSchema>
            {
                ["property1"] = new OpenApiSchema
                {
                    Properties = new Dictionary<string, OpenApiSchema>
                    {
                        ["property2"] = new OpenApiSchema
                        {
                            Type = "integer"
                        },
                        ["property3"] = new OpenApiSchema
                        {
                            Type = "string",
                            MaxLength = 15
                        }
                    },
                },
                ["property4"] = new OpenApiSchema
                {
                    Properties = new Dictionary<string, OpenApiSchema>
                    {
                        ["property5"] = new OpenApiSchema
                        {
                            Properties = new Dictionary<string, OpenApiSchema>
                            {
                                ["property6"] = new OpenApiSchema
                                {
                                    Type = "boolean"
                                }
                            }
                        },
                        ["property7"] = new OpenApiSchema
                        {
                            Type = "string",
                            MinLength = 2
                        }
                    },
                },
            },
            Nullable = true,
            ExternalDocs = new OpenApiExternalDocs
            {
                Url = new Uri("http://example.com/externalDocs")
            }
        };

        public static OpenApiSchema AdvancedSchemaWithAllOf = new OpenApiSchema
        {
            Title = "title1",
            AllOf = new List<OpenApiSchema>
            {
                new OpenApiSchema
                {
                    Title = "title2",
                    Properties = new Dictionary<string, OpenApiSchema>
                    {
                        ["property1"] = new OpenApiSchema
                        {
                            Type = "integer"
                        },
                        ["property2"] = new OpenApiSchema
                        {
                            Type = "string",
                            MaxLength = 15
                        }
                    },
                },
                new OpenApiSchema
                {
                    Title = "title3",
                    Properties = new Dictionary<string, OpenApiSchema>
                    {
                        ["property3"] = new OpenApiSchema
                        {
                            Properties = new Dictionary<string, OpenApiSchema>
                            {
                                ["property4"] = new OpenApiSchema
                                {
                                    Type = "boolean"
                                }
                            }
                        },
                        ["property5"] = new OpenApiSchema
                        {
                            Type = "string",
                            MinLength = 2
                        }
                    },
                    Nullable = true
                },
            },
            Nullable = true,
            ExternalDocs = new OpenApiExternalDocs
            {
                Url = new Uri("http://example.com/externalDocs")
            }
        };

        public static OpenApiSchema ReferencedSchema = new OpenApiSchema
        {
            Title = "title1",
            MultipleOf = 3,
            Maximum = 42,
            ExclusiveMinimum = true,
            Minimum = 10,
            Default = new OpenApiInteger(15),
            Type = "integer",

            Nullable = true,
            ExternalDocs = new OpenApiExternalDocs
            {
                Url = new Uri("http://example.com/externalDocs")
            },

            Reference = new OpenApiReference
            {
                Type = ReferenceType.Schema,
                Id = "schemaObject1"
            }
        };

        public static OpenApiSchema AdvancedSchemaWithRequiredPropertiesObject = new OpenApiSchema
        {
            Title = "title1",
            Required = new HashSet<string>() { "property1" },
            Properties = new Dictionary<string, OpenApiSchema>
            {
                ["property1"] = new OpenApiSchema
                {
                    Required = new HashSet<string>() { "property3" },
                    Properties = new Dictionary<string, OpenApiSchema>
                    {
                        ["property2"] = new OpenApiSchema
                        {
                            Type = "integer"
                        },
                        ["property3"] = new OpenApiSchema
                        {
                            Type = "string",
                            MaxLength = 15,
                            ReadOnly = true
                        }
                    },
                    ReadOnly = true,
                },
                ["property4"] = new OpenApiSchema
                {
                    Properties = new Dictionary<string, OpenApiSchema>
                    {
                        ["property5"] = new OpenApiSchema
                        {
                            Properties = new Dictionary<string, OpenApiSchema>
                            {
                                ["property6"] = new OpenApiSchema
                                {
                                    Type = "boolean"
                                }
                            }
                        },
                        ["property7"] = new OpenApiSchema
                        {
                            Type = "string",
                            MinLength = 2
                        }
                    },
                    ReadOnly = true,
                },
            },
            Nullable = true,
            ExternalDocs = new OpenApiExternalDocs
            {
                Url = new Uri("http://example.com/externalDocs")
            }
        };

        private readonly ITestOutputHelper _output;

        public OpenApiSchemaTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void SerializeBasicSchemaAsV3JsonWorks()
        {
            // Arrange
            var expected = @"{ }";

            // Act
            var actual = BasicSchema.SerializeAsJson(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeAdvancedSchemaNumberAsV3JsonWorks()
        {
            // Arrange
            var expected = @"{
  ""title"": ""title1"",
  ""multipleOf"": 3,
  ""maximum"": 42,
  ""minimum"": 10,
  ""exclusiveMinimum"": true,
  ""type"": ""integer"",
  ""default"": 15,
  ""nullable"": true,
  ""externalDocs"": {
    ""url"": ""http://example.com/externalDocs""
  }
}";

            // Act
            var actual = AdvancedSchemaNumber.SerializeAsJson(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeAdvancedSchemaObjectAsV3JsonWorks()
        {
            // Arrange
            var expected = @"{
  ""title"": ""title1"",
  ""properties"": {
    ""property1"": {
      ""properties"": {
        ""property2"": {
          ""type"": ""integer""
        },
        ""property3"": {
          ""maxLength"": 15,
          ""type"": ""string""
        }
      }
    },
    ""property4"": {
      ""properties"": {
        ""property5"": {
          ""properties"": {
            ""property6"": {
              ""type"": ""boolean""
            }
          }
        },
        ""property7"": {
          ""minLength"": 2,
          ""type"": ""string""
        }
      }
    }
  },
  ""nullable"": true,
  ""externalDocs"": {
    ""url"": ""http://example.com/externalDocs""
  }
}";

            // Act
            var actual = AdvancedSchemaObject.SerializeAsJson(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeAdvancedSchemaWithAllOfAsV3JsonWorks()
        {
            // Arrange
            var expected = @"{
  ""title"": ""title1"",
  ""allOf"": [
    {
      ""title"": ""title2"",
      ""properties"": {
        ""property1"": {
          ""type"": ""integer""
        },
        ""property2"": {
          ""maxLength"": 15,
          ""type"": ""string""
        }
      }
    },
    {
      ""title"": ""title3"",
      ""properties"": {
        ""property3"": {
          ""properties"": {
            ""property4"": {
              ""type"": ""boolean""
            }
          }
        },
        ""property5"": {
          ""minLength"": 2,
          ""type"": ""string""
        }
      },
      ""nullable"": true
    }
  ],
  ""nullable"": true,
  ""externalDocs"": {
    ""url"": ""http://example.com/externalDocs""
  }
}";

            // Act
            var actual = AdvancedSchemaWithAllOf.SerializeAsJson(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task SerializeReferencedSchemaAsV3WithoutReferenceJsonWorksAsync(bool produceTerseOutput)
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new OpenApiJsonWriterSettings { Terse = produceTerseOutput });


            // Act
            ReferencedSchema.SerializeAsV3WithoutReference(writer);
            writer.Flush();

            // Assert
            await Verifier.Verify(outputStringWriter).UseParameters(produceTerseOutput);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task SerializeReferencedSchemaAsV3JsonWorksAsync(bool produceTerseOutput)
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new OpenApiJsonWriterSettings { Terse = produceTerseOutput });

            // Act
            ReferencedSchema.SerializeAsV3(writer);
            writer.Flush();

            // Assert
            await Verifier.Verify(outputStringWriter).UseParameters(produceTerseOutput);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task SerializeSchemaWRequiredPropertiesAsV2JsonWorksAsync(bool produceTerseOutput)
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new OpenApiJsonWriterSettings { Terse = produceTerseOutput });

            // Act
            AdvancedSchemaWithRequiredPropertiesObject.SerializeAsV2(writer);
            writer.Flush();

            // Assert
            await Verifier.Verify(outputStringWriter).UseParameters(produceTerseOutput);
        }

        [Fact]
        public void SerializeAsV2ShouldSetFormatPropertyInParentSchemaIfPresentInChildrenSchema()
        {
            // Arrange
            var schema = new OpenApiSchema()
            {
                OneOf = new List<OpenApiSchema>
                {
                    new OpenApiSchema
                    { 
                        Type = "number",
                        Format = "decimal"
                    },
                    new OpenApiSchema { Type = "string" },
                }
            };

            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var openApiJsonWriter = new OpenApiJsonWriter(outputStringWriter, new OpenApiJsonWriterSettings { Terse = false });

            // Act
            // Serialize as V2
            schema.SerializeAsV2(openApiJsonWriter);
            openApiJsonWriter.Flush();

            var v2Schema = outputStringWriter.GetStringBuilder().ToString().MakeLineBreaksEnvironmentNeutral();

            var expectedV2Schema = @"{
  ""format"": ""decimal"",
  ""allOf"": [
    {
      ""format"": ""decimal"",
      ""type"": ""number""
    }
  ]
}".MakeLineBreaksEnvironmentNeutral();

            // Assert
            Assert.Equal(expectedV2Schema, v2Schema);
        }

        [Fact]
        public void OpenApiSchemaCopyConstructorSucceeds()
        {
            var baseSchema = new OpenApiSchema()
            {
                Type = "string",
                Format = "date"
            };

            var actualSchema = new OpenApiSchema(baseSchema)
            {
                Nullable = true
            };

            Assert.Equal("string", actualSchema.Type);
            Assert.Equal("date", actualSchema.Format);
            Assert.True(actualSchema.Nullable);
        }

        [Fact]
        public void CloningSchemaExtensionsWorks()
        {
            // Arrange
            var schema = new OpenApiSchema
            {
                Extensions =
                {
                    { "x-myextension", new OpenApiInteger(42) }
                }
            };

            // Act && Assert
            var schemaCopy = new OpenApiSchema(schema);
            Assert.Single(schemaCopy.Extensions);

            // Act && Assert
            schemaCopy.Extensions = new Dictionary<string, IOpenApiExtension>
            {
                { "x-myextension" , new OpenApiInteger(40) }
            };
            Assert.NotEqual(schema.Extensions, schemaCopy.Extensions);
        }
    }
}
