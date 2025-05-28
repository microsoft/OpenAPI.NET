using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

namespace Microsoft.OpenApi.Tests.Extensions
{
    public class CollectionExtensionsTests
    {
        public static readonly OpenApiDocument Document = new OpenApiDocument
        {
            Info = new OpenApiInfo { Title = "Test API", Version = "1.0" },
            Paths = new OpenApiPaths
            {
                ["pet"] = new OpenApiPathItem
                {
                    Operations = new ()
                    {
                        [HttpMethod.Get] = new OpenApiOperation
                        {
                            Responses = new OpenApiResponses
                            {
                                ["200"] = new OpenApiResponse
                                {
                                    Description = "Return a 200 status to indicate that the data was received successfully"
                                }
                            }
                        }
                    }
                },
                ["newPet"] = new OpenApiPathItem
                {
                    Operations = new()
                    {
                        [HttpMethod.Post] = new OpenApiOperation
                        {
                            RequestBody = new OpenApiRequestBody
                            {
                                Description = "Information about a new pet in the system",
                                Content = new Dictionary<string, OpenApiMediaType>()
                                {
                                    ["application/json"] = new OpenApiMediaType
                                    {
                                        Schema = new OpenApiSchemaReference("Pet", null)
                                    }
                                }
                            },
                            Responses = new OpenApiResponses
                            {
                                ["200"] = new OpenApiResponse
                                {
                                    Description = "Return a 200 status to indicate that the data was received successfully"
                                }
                            }
                        }
                    }
                }
            },
            Components = new OpenApiComponents
            {
                Schemas = new Dictionary<string, IOpenApiSchema>()
                {
                    ["pet"] = new OpenApiSchema()
                    {
                        Required = new HashSet<string>
                        {
                            "id", "name"
                        },
                        Properties = new Dictionary<string, IOpenApiSchema>()
                        {
                            ["name"] = new OpenApiSchema()
                            {
                                Type = JsonSchemaType.String
                            },
                            ["id"] = new OpenApiSchema()
                            {
                                Type = JsonSchemaType.Integer,
                                Format = "int64"
                            },
                            ["tag"] = new OpenApiSchema()
                            {
                                Type = JsonSchemaType.String
                            }
                        },
                    },
                    ["newPet"] = new OpenApiSchema()
                    {
                        Type = JsonSchemaType.Object,
                        Required = new HashSet<string>
                        {
                            "name"
                        },
                        Properties = new Dictionary<string, IOpenApiSchema>()
                        {
                            ["name"] = new OpenApiSchema()
                            {
                                Type = JsonSchemaType.String
                            },
                            ["id"] = new OpenApiSchema()
                            {
                                Type = JsonSchemaType.Integer,
                                Format = "int64"
                            }
                        }
                    },
                    ["errorModel"] = new OpenApiSchema()
                    {
                        Type = JsonSchemaType.Object,
                        Required = new HashSet<string>
                        {
                            "code",
                            "message"
                        },
                        Properties = new Dictionary<string, IOpenApiSchema>()
                        {
                            ["message"] = new OpenApiSchema()
                            {
                                Type = JsonSchemaType.String
                            },
                            ["code"] = new OpenApiSchema()
                            {
                                Type = JsonSchemaType.Integer,
                                Format = "int32"
                            }
                        }
                    }
                }
            }
        };

        [Theory]
        [MemberData(nameof(OpenApiSpecVersions))]
        public async Task SortOpenApiDocumentUsingCustomComparerSucceeds(OpenApiSpecVersion version)
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var settings = new OpenApiWriterSettings
            {
                Comparer = StringComparer.OrdinalIgnoreCase
            };
            var writer = new OpenApiYamlWriter(outputStringWriter, settings);

            // Act
            Document.SerializeAs(version, writer);
            await writer.FlushAsync();

            // Assert
            await Verifier.Verify(outputStringWriter).UseParameters(version);
        }

        [Fact]
        public async Task SortHashSetsWorks()
        {
            // Arrange
            var expected = @"required:
  - id
  - name
  - tag";
            var schema = new OpenApiSchema
            {
                Required = new HashSet<string> { "tag", "id", "name" },
            };

            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var settings = new OpenApiWriterSettings
            {
                Comparer = StringComparer.OrdinalIgnoreCase
            };
            var writer = new OpenApiYamlWriter(outputStringWriter, settings);

            // Act
            schema.SerializeAsV3(writer);
            await writer.FlushAsync();
            var sortedString = outputStringWriter.ToString();

            // Assert
            Assert.Equal(expected.MakeLineBreaksEnvironmentNeutral(), sortedString.MakeLineBreaksEnvironmentNeutral());
        }

        [Fact]
        public void SortTagsByNameUsingComparerWorks()
        {
            // Arrange
            var tags = new HashSet<OpenApiTag>
            {
                new() { Name = "three" },
                new() { Name = "two" },
                new() { Name = "one" }
            };

            // Act
            var sortedTags = tags.Sort(new OpenApiTagNameComparer());

            // Assert
            Assert.Equal(3, sortedTags.Count);
            Assert.True(sortedTags[0].Name == "one");
        }

        public static TheoryData<OpenApiSpecVersion> OpenApiSpecVersions()
        {
            var values = new TheoryData<OpenApiSpecVersion>();
            foreach (var value in Enum.GetValues<OpenApiSpecVersion>())
            {
                values.Add(value);
            }
            return values;
        }
    }

    public class OpenApiTagNameComparer : IComparer<OpenApiTag>
    {
        public int Compare(OpenApiTag tag1, OpenApiTag tag2)
        {
            return string.Compare(tag1.Name, tag2.Name, StringComparison.OrdinalIgnoreCase);
        }
    }
}
