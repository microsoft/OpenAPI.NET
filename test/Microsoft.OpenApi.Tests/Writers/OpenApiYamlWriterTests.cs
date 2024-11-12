// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using FluentAssertions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;
using Xunit;

namespace Microsoft.OpenApi.Tests.Writers
{
    [Collection("DefaultSettings")]
    public class OpenApiYamlWriterTests
    {
        public static IEnumerable<object[]> WriteStringListAsYamlShouldMatchExpectedTestCases()
        {
            yield return new object[]
            {
                new[]
                {
                    "string1",
                    "string2",
                    "string3",
                    "string4",
                    "string5",
                    "string6",
                    "string7",
                    "string8"
                },
                """
                - string1
                - string2
                - string3
                - string4
                - string5
                - string6
                - string7
                - string8
                """
            };

            yield return new object[]
            {
                new[] {"string1", "string1", "string1", "string1"},
                """
                - string1
                - string1
                - string1
                - string1
                """
            };
        }

        [Theory]
        [MemberData(nameof(WriteStringListAsYamlShouldMatchExpectedTestCases))]
        public void WriteStringListAsYamlShouldMatchExpected(string[] stringValues, string expectedYaml)
        {
            // Arrange
            var outputString = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiYamlWriter(outputString);

            // Act
            writer.WriteStartArray();
            foreach (var stringValue in stringValues)
            {
                writer.WriteValue(stringValue);
            }

            writer.WriteEndArray();
            writer.Flush();

            var actualYaml = outputString.GetStringBuilder()
                .ToString()
                .MakeLineBreaksEnvironmentNeutral();

            expectedYaml = expectedYaml.MakeLineBreaksEnvironmentNeutral();

            // Assert
            actualYaml.Should().Be(expectedYaml);
        }

        public static IEnumerable<object[]> WriteMapAsYamlShouldMatchExpectedTestCasesSimple()
        {
            // Simple map
            yield return new object[]
            {
                new Dictionary<string, object>
                {
                    ["property1"] = "value1",
                    ["property2"] = "value2",
                    ["property3"] = "value3",
                    ["property4"] = "value4"
                },
                """
                property1: value1
                property2: value2
                property3: value3
                property4: value4
                """
            };

            // Simple map with duplicate value
            yield return new object[]
            {
                new Dictionary<string, object>
                {
                    ["property1"] = "value1",
                    ["property2"] = "value1",
                    ["property3"] = "value1",
                    ["property4"] = "value1"
                },
                """
                property1: value1
                property2: value1
                property3: value1
                property4: value1
                """
            };
        }

        public static IEnumerable<object[]> WriteMapAsYamlShouldMatchExpectedTestCasesComplex()
        {
            // Empty map and empty list
            yield return new object[]
            {
                new Dictionary<string, object>
                {
                    ["property1"] = new Dictionary<string, object>(),
                    ["property2"] = new List<string>(),
                    ["property3"] = new List<object>
                    {
                        new Dictionary<string, object>(),
                    },
                    ["property4"] = "value4"
                },
                """
                property1: { }
                property2: [ ]
                property3:
                  - { }
                property4: value4
                """
            };

            // Number, boolean, and null handling
            yield return new object[]
            {
                new Dictionary<string, object>
                {
                    ["property1"] = "10.0",
                    ["property2"] = "10",
                    ["property3"] = "-5",
                    ["property4"] = 10.0M,
                    ["property5"] = 10,
                    ["property6"] = -5,
                    ["property7"] = true,
                    ["property8"] = "true",
                    ["property9"] = null,
                    ["property10"] = "null",
                    ["property11"] = "",
                },
                """
                property1: '10.0'
                property2: '10'
                property3: '-5'
                property4: 10.0
                property5: 10
                property6: -5
                property7: true
                property8: 'true'
                property9:
                property10: 'null'
                property11: ''
                """
            };

            // DateTime
            yield return new object[]
            {
                new Dictionary<string, object>
                {
                    ["property1"] = new DateTime(1970, 01, 01),
                    ["property2"] = new DateTimeOffset(new DateTime(1970, 01, 01), TimeSpan.FromHours(3)),
                    ["property3"] = new DateTime(2018, 04, 03),
                },
                """
                property1: '1970-01-01T00:00:00.0000000'
                property2: '1970-01-01T00:00:00.0000000+03:00'
                property3: '2018-04-03T00:00:00.0000000'
                """
            };

            // Nested map
            yield return new object[]
            {
                new Dictionary<string, object>
                {
                    ["property1"] = new Dictionary<string, object>
                    {
                        ["innerProperty1"] = "innerValue1"
                    },
                    ["property2"] = "value2",
                    ["property3"] = new Dictionary<string, object>
                    {
                        ["innerProperty3"] = "innerValue3"
                    },
                    ["property4"] = "value4"
                },
                """
                property1:
                  innerProperty1: innerValue1
                property2: value2
                property3:
                  innerProperty3: innerValue3
                property4: value4
                """
            };

            // Nested map and list
            yield return new object[]
            {
                new Dictionary<string, object>
                {
                    ["property1"] = new Dictionary<string, object>(),
                    ["property2"] = new List<string>(),
                    ["property3"] = new List<object>
                    {
                        new Dictionary<string, object>(),
                        "string1",
                        new Dictionary<string, object>
                        {
                            ["innerProperty1"] = new List<object>(),
                            ["innerProperty2"] = "string2",
                            ["innerProperty3"] = new List<object>
                            {
                                new List<string>
                                {
                                    "string3"
                                }
                            }
                        }
                    },
                    ["property4"] = "value4"
                },
                """
                property1: { }
                property2: [ ]
                property3:
                  - { }
                  - string1
                  - innerProperty1: [ ]
                    innerProperty2: string2
                    innerProperty3:
                      -
                        - string3
                property4: value4
                """
            };
        }

        private void WriteValueRecursive(OpenApiYamlWriter writer, object value)
        {
            if (value == null
                || value.GetType().IsPrimitive
                || value is decimal
                || value is string
                || value is DateTimeOffset
                || value is DateTime)
            {
                writer.WriteValue(value);
            }
            else if (value.GetType().IsGenericType &&
                (typeof(IDictionary<,>).IsAssignableFrom(value.GetType().GetGenericTypeDefinition()) ||
                    typeof(Dictionary<,>).IsAssignableFrom(value.GetType().GetGenericTypeDefinition())))
            {
                writer.WriteStartObject();
                foreach (var elementValue in (dynamic)(value))
                {
                    writer.WritePropertyName(elementValue.Key);
                    WriteValueRecursive(writer, elementValue.Value);
                }

                writer.WriteEndObject();
            }
            else if (value is IEnumerable enumerable)
            {
                writer.WriteStartArray();
                foreach (var elementValue in enumerable)
                {
                    WriteValueRecursive(writer, elementValue);
                }

                writer.WriteEndArray();
            }
        }

        [Theory]
        [MemberData(nameof(WriteMapAsYamlShouldMatchExpectedTestCasesSimple))]
        [MemberData(nameof(WriteMapAsYamlShouldMatchExpectedTestCasesComplex))]
        public void WriteMapAsYamlShouldMatchExpected(IDictionary<string, object> inputMap, string expectedYaml)
        {
            // Arrange
            var outputString = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiYamlWriter(outputString);

            // Act
            WriteValueRecursive(writer, inputMap);
            var actualYaml = outputString.ToString();

            // Assert
            actualYaml = actualYaml.MakeLineBreaksEnvironmentNeutral();
            expectedYaml = expectedYaml.MakeLineBreaksEnvironmentNeutral();
            actualYaml.Should().Be(expectedYaml);
        }

        public static IEnumerable<object[]> WriteDateTimeAsJsonTestCases()
        {
            yield return new object[]
            {
                new DateTimeOffset(2018, 1, 1, 10, 20, 30, TimeSpan.Zero)
            };

            yield return new object[]
            {
                new DateTimeOffset(2018, 1, 1, 10, 20, 30, 100, TimeSpan.FromHours(14))
            };

            yield return new object[]
            {
                DateTimeOffset.UtcNow + TimeSpan.FromDays(4)
            };

            yield return new object[]
            {
                DateTime.UtcNow + TimeSpan.FromDays(4)
            };
        }

        [Theory]
        [MemberData(nameof(WriteDateTimeAsJsonTestCases))]
        public void WriteDateTimeAsJsonShouldMatchExpected(DateTimeOffset dateTimeOffset)
        {
            // Arrange
            var outputString = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiYamlWriter(outputString);

            // Act
            writer.WriteValue(dateTimeOffset);

            var writtenString = outputString.GetStringBuilder().ToString();
            var expectedString = " '" + dateTimeOffset.ToString("o") + "'";

            // Assert
            writtenString.Should().Be(expectedString);
        }

        [Fact]

        public void WriteInlineSchema()
        {
            // Arrange
            var doc = CreateDocWithSimpleSchemaToInline();

            var expected =
                """
                openapi: 3.0.4
                info:
                  title: Demo
                  version: 1.0.0
                paths:
                  /:
                    get:
                      responses:
                        '200':
                          description: OK
                          content:
                            application/json:
                              schema:
                                type: object
                components: { }
                """;

            var outputString = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiYamlWriter(outputString, new() { InlineLocalReferences = true } );

            // Act
            doc.SerializeAsV3(writer);
            var mediaType = doc.Paths["/"].Operations[OperationType.Get].Responses["200"].Content["application/json"];
            var actual = outputString.GetStringBuilder().ToString();

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().BeEquivalentTo(expected);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void WriteInlineSchemaV2()
        {
            var doc = CreateDocWithSimpleSchemaToInline();

            var expected =
                """
                swagger: '2.0'
                info:
                  title: Demo
                  version: 1.0.0
                paths:
                  /:
                    get:
                      produces:
                        - application/json
                      responses:
                        '200':
                          description: OK
                          schema:
                            type: object
                """;

            var outputString = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiYamlWriter(outputString, new() { InlineLocalReferences = true });

            // Act
            doc.SerializeAsV2(writer);
            var actual = outputString.GetStringBuilder().ToString();

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            Assert.Equal(expected, actual);
        }

        private static OpenApiDocument CreateDocWithSimpleSchemaToInline()
        {
            // Arrange
            var thingSchema = new OpenApiSchema
            {
                Type = JsonSchemaType.Object,
                UnresolvedReference = false,
                Reference = new()
                {
                    Id = "thing",
                    Type = ReferenceType.Schema
                }
            };

            var doc = new OpenApiDocument()
            {
                Info = new()
                {
                    Title = "Demo",
                    Version = "1.0.0"
                },
                Paths = new()
                {
                    ["/"] = new()
                    {
                        Operations = {
                            [OperationType.Get] = new()
                            {
                                Responses = {
                                    ["200"] = new()
                                    {
                                        Description = "OK",
                                        Content = {
                                             ["application/json"] = new()
                                             {
                                                     Schema = thingSchema
                                             }
                                        }
                                    }
                                }
                            }
                        }
                    }
                },
                Components = new()
                {
                    Schemas = {
                        ["thing"] = thingSchema}
                }
            };

            return doc;
        }
    }
}
