// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Collections;
using System.Collections.Generic;
using System.IO;
using FluentAssertions;
using Microsoft.OpenApi.Writers;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.OpenApi.Tests.Writers
{
    [Collection("DefaultSettings")]
    public class OpenApiYamlWriterTests
    {
        private readonly ITestOutputHelper _output;

        public OpenApiYamlWriterTests(ITestOutputHelper output)
        {
            _output = output;
        }

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
                @"- string1
- string2
- string3
- string4
- string5
- string6
- string7
- string8"
            };

            yield return new object[]
            {
                new[] {"string1", "string1", "string1", "string1"},
                @"- string1
- string1
- string1
- string1"
            };
        }

        [Theory]
        [MemberData(nameof(WriteStringListAsYamlShouldMatchExpectedTestCases))]
        public void WriteStringListAsYamlShouldMatchExpected(string[] stringValues, string expectedYaml)
        {
            // Arrange
            var outputString = new StringWriter();
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
                @"property1: value1
property2: value2
property3: value3
property4: value4"
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
                @"property1: value1
property2: value1
property3: value1
property4: value1"
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
                @"property1: { }
property2: [ ]
property3:
  - { }
property4: value4"
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
                @"property1: '10.0'
property2: '10'
property3: '-5'
property4: 10.0
property5: 10
property6: -5
property7: true
property8: 'true'
property9: 
property10: 'null'
property11: ''"
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
                @"property1:
  innerProperty1: innerValue1
property2: value2
property3:
  innerProperty3: innerValue3
property4: value4"
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
                @"property1: { }
property2: [ ]
property3:
  - { }
  - string1
  - innerProperty1: [ ]
    innerProperty2: string2
    innerProperty3:
      - 
        - string3
property4: value4"
            };
        }

        private void WriteValueRecursive(OpenApiYamlWriter writer, object value)
        {
            if (value == null || value.GetType().IsPrimitive || value is decimal || value is string)
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
            else if (typeof(IEnumerable).IsAssignableFrom(value.GetType()))
            {
                writer.WriteStartArray();
                foreach (var elementValue in (IEnumerable)value)
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
            var outputString = new StringWriter();
            var writer = new OpenApiYamlWriter(outputString);

            // Act
            WriteValueRecursive(writer, inputMap);
            var actualYaml = outputString.ToString();

            // Assert
            actualYaml = actualYaml.MakeLineBreaksEnvironmentNeutral();
            expectedYaml = expectedYaml.MakeLineBreaksEnvironmentNeutral();
            actualYaml.Should().Be(expectedYaml);
        }
    }
}