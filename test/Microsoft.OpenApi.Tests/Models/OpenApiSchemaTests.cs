// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Xunit;
using FluentAssertions;
using Microsoft.OpenApi.Extensions;

namespace Microsoft.OpenApi.Tests.Models
{
    public class OpenApiSchemaTests
    {
        public static OpenApiSchema BasicV31Schema = new()
        {
            Title = "title1",
            MultipleOf = 3,
            Maximum = 42,
            ExclusiveMinimum = true,
            Minimum = 10,
            Default = new OpenApiInteger(15),
            Type = "integer",

            Nullable = true,
            ExternalDocs = new()
            {
                Url = new("http://example.com/externalDocs")
            },
            Annotations = new Dictionary<string, object> { { "key1", "value1" }, { "key2", 2 } }
        };

        public static readonly OpenApiSchema AdvancedSchemaObject = new()
        {
            Title = "title1",
            Properties = new Dictionary<string, OpenApiSchema>
            {
                ["fruits"] = new OpenApiSchema
                {
                    Type = "array",
                    Items = new OpenApiSchema
                    {
                        Type = "string"
                    }
                },
                ["vegetables"] = new OpenApiSchema
                {
                    Type = "array"
                }
            },
            Definitions = new Dictionary<string, OpenApiSchema>
            {
                ["veggie"] = new OpenApiSchema
                {
                    Type = "object",
                    Required = new HashSet<string>{ "veggieName", "veggieLike" },
                    Properties = new Dictionary<string, OpenApiSchema>
                    {
                        ["veggieName"] = new OpenApiSchema
                        {
                            Type = "string",
                            Description = "The name of the vegetable."
                        },
                        ["veggieLike"] = new OpenApiSchema
                        {
                            Type = "boolean",
                            Description = "Do I like this vegetable?"
                        }
                    }
                }
            }
        };

        [Fact]
        public void SerializeBasicV31SchemaWorks()
        {
            // Arrange
            var expected = @"{
  ""$id"": ""https://example.com/arrays.schema.json"",
  ""$schema"": ""https://json-schema.org/draft/2020-12/schema"",
  ""$defs"": {
    ""veggie"": {
      ""required"": [
        ""veggieName"",
        ""veggieLike""
      ],
      ""type"": ""object"",
      ""properties"": {
        ""veggieName"": {
          ""type"": ""string"",
          ""description"": ""The name of the vegetable.""
        },
        ""veggieLike"": {
          ""type"": ""boolean"",
          ""description"": ""Do I like this vegetable?""
        }
      }
    }
  },
  ""type"": ""object"",
  ""properties"": {
    ""fruits"": {
      ""type"": ""array"",
      ""items"": {
        ""type"": ""string""
      }
    },
    ""vegetables"": {
      ""type"": ""array""
    }
  },
  ""description"": ""A representation of a person, company, organization, or place""
}";

            // Act
            var actual = BasicV31Schema.SerializeAsJson(OpenApiSpecVersion.OpenApi3_1);

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
            var writer = new OpenApiJsonWriter(outputStringWriter, new() { Terse = produceTerseOutput });

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
            var writer = new OpenApiJsonWriter(outputStringWriter, new() { Terse = produceTerseOutput });

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
            var writer = new OpenApiJsonWriter(outputStringWriter, new() { Terse = produceTerseOutput });

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
            var schema = new OpenApiSchema
            {
                OneOf = new List<OpenApiSchema>
                {
                    new()
                    {
                        Type = "number",
                        Format = "decimal"
                    },
                    new() { Type = "string" },
                }
            };

            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var openApiJsonWriter = new OpenApiJsonWriter(outputStringWriter, new() { Terse = false });

            // Act
            // Serialize as V2
            schema.SerializeAsV2(openApiJsonWriter);
            openApiJsonWriter.Flush();

            var v2Schema = outputStringWriter.GetStringBuilder().ToString().MakeLineBreaksEnvironmentNeutral();

            var expectedV2Schema =
                """
                {
                  "format": "decimal",
                  "allOf": [
                    {
                      "format": "decimal",
                      "type": "number"
                    }
                  ]
                }
                """.MakeLineBreaksEnvironmentNeutral();

            // Assert
            Assert.Equal(expectedV2Schema, v2Schema);
        }

        [Fact]
        public void OpenApiSchemaCopyConstructorSucceeds()
        {
            var baseSchema = new OpenApiSchema
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
        public void OpenApiSchemaCopyConstructorWithAnnotationsSucceeds()
        {
            var baseSchema = new OpenApiSchema
            {
                Annotations = new Dictionary<string, object>
                {
                    ["key1"] = "value1",
                    ["key2"] = 2
                }
            };

            var actualSchema = new OpenApiSchema(baseSchema);

            Assert.Equal(baseSchema.Annotations["key1"], actualSchema.Annotations["key1"]);

            baseSchema.Annotations["key1"] = "value2";

            Assert.NotEqual(baseSchema.Annotations["key1"], actualSchema.Annotations["key1"]);
        }

        public static TheoryData<IOpenApiAny> SchemaExamples()
        {
            return new()
            {
                new OpenApiArray() { new OpenApiString("example") },
                new OpenApiBinary([0, 1, 2]),
                new OpenApiBoolean(true),
                new OpenApiByte(42),
                new OpenApiDate(new(2024, 07, 19, 12, 34, 56)),
                new OpenApiDateTime(new(2024, 07, 19, 12, 34, 56, new(01, 00, 00))),
                new OpenApiDouble(42.37),
                new OpenApiFloat(42.37f),
                new OpenApiInteger(42),
                new OpenApiLong(42),
                new OpenApiNull(),
                new OpenApiObject() { ["prop"] = new OpenApiString("example") },
                new OpenApiPassword("secret"),
                new OpenApiString("example"),
            };
        }

        [Theory]
        [MemberData(nameof(SchemaExamples))]
        public void CloningSchemaExamplesWorks(IOpenApiAny example)
        {
            // Arrange
            var schema = new OpenApiSchema
            {
                Example = example
            };

            // Act && Assert
            var schemaCopy = new OpenApiSchema(schema);
            Assert.NotNull(schemaCopy.Example);

            // Act && Assert
            Assert.Equivalent(schema.Example, schemaCopy.Example);
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

        [Fact]
        public void OpenApiWalkerVisitsOpenApiSchemaNot()
        {
            var outerSchema = new OpenApiSchema()
            {
                Title = "Outer Schema",
                Not = new OpenApiSchema()
                {
                    Title = "Inner Schema",
                    Type = "string",
                }
            };        

            var document = new OpenApiDocument()
            {
                Paths = new OpenApiPaths()
                {
                    ["/foo"] = new OpenApiPathItem()
                    {
                        Parameters = new[]
                        {
                            new OpenApiParameter()
                            {
                                Name = "foo",
                                In = ParameterLocation.Query,
                                Schema = outerSchema,
                            }
                        }
                    }
                }
            };

            // Act
            var visitor = new SchemaVisitor();
            var walker = new OpenApiWalker(visitor);
            walker.Walk(document);

            // Assert
            visitor.Titles.Count.Should().Be(2);
        }
    }

    internal class SchemaVisitor : OpenApiVisitorBase
    {
        public List<string> Titles = new();

        public override void Visit(OpenApiSchema schema)
        {
            Titles.Add(schema.Title);
        }
    }
}
