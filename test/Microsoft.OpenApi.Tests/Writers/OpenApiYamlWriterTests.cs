// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using System.IO;
using Microsoft.OpenApi.Writers;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.OpenApi.Tests.Writers
{
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
                @"
- string1
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
                @"
- string1
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
            Assert.Equal(expectedYaml, actualYaml);
        }

        public static IEnumerable<object[]> WriteMapAsYamlShouldMatchExpectedTestCasesSimple()
        {
            yield return new object[]
            {
                new Dictionary<string, object>
                {
                    ["property1"] = "value1",
                    ["property2"] = "value2",
                    ["property3"] = "value3",
                    ["property4"] = "value4"
                },
                @"
property1: value1
property2: value2
property3: value3
property4: value4"
            };

            yield return new object[]
            {
                new Dictionary<string, object>
                {
                    ["property1"] = "value1",
                    ["property2"] = "value1",
                    ["property3"] = "value1",
                    ["property4"] = "value1"
                },
                @"
property1: value1
property2: value1
property3: value1
property4: value1"
            };
        }

        public static IEnumerable<object[]> WriteMapAsYamlShouldMatchExpectedTestCasesComplex()
        {
//            // Disable because our YamlWriter doesn't handle empty object properly
//            yield return new object[]
//            {
//                new Dictionary<string, object>
//                {
//                    ["property1"] = new Dictionary<string, object>(),
//                    ["property2"] = "value2",
//                    ["property3"] = "value3",
//                    ["property4"] = "value4"
//                },
//                @"property1: {
//property2: value1
//property3: value1
//property4: value1"
//            };

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
                @"
property1:
  innerProperty1: innerValue1
property2: value2
property3:
  innerProperty3: innerValue3
property4: value4"
            };

            yield return new object[]
            {
                new Dictionary<string, object>
                {
                    ["property1"] = "value1",
                    ["property2"] = "value2",
                    ["property3"] = new Dictionary<string, object>
                    {
                        ["innerProperty1"] = "innerValue1"
                    },
                    ["property4"] = new List<object> {"listValue1", "listValue2"}
                },
                @"
property1: value1
property2: value2
property3:
  innerProperty1: innerValue1
property4:
  - listValue1
  - listValue2"
            };
        }

        [Theory]
        [MemberData(nameof(WriteMapAsYamlShouldMatchExpectedTestCasesSimple))]
        [MemberData(nameof(WriteMapAsYamlShouldMatchExpectedTestCasesComplex))]
        public void WriteMapAsYamlShouldMatchExpected(IDictionary<string, object> inputMap, string expectedYaml)
        {
            var outputString = new StringWriter();
            var writer = new OpenApiYamlWriter(outputString);

            writer.WriteStartObject();

            foreach (var keyValue in inputMap)
            {
                if (keyValue.Value.GetType().IsPrimitive || keyValue.Value.GetType() == typeof(string))
                {
                    writer.WritePropertyName(keyValue.Key);
                    writer.WriteValue(keyValue.Value);
                }
                else if (keyValue.Value.GetType().IsGenericType &&
                    (typeof(IDictionary<,>).IsAssignableFrom(keyValue.Value.GetType().GetGenericTypeDefinition()) ||
                        typeof(Dictionary<,>).IsAssignableFrom(keyValue.Value.GetType().GetGenericTypeDefinition())))
                {
                    writer.WritePropertyName(keyValue.Key);
                    writer.WriteStartObject();
                    foreach (var elementValue in (dynamic)(keyValue.Value))
                    {
                        writer.WritePropertyName(elementValue.Key);
                        writer.WriteValue(elementValue.Value);
                    }

                    writer.WriteEndObject();
                }
                else if (typeof(IEnumerable).IsAssignableFrom(keyValue.Value.GetType()))
                {
                    writer.WritePropertyName(keyValue.Key);
                    writer.WriteStartArray();
                    foreach (var elementValue in (IEnumerable)keyValue.Value)
                    {
                        writer.WriteValue(elementValue);
                    }

                    writer.WriteEndArray();
                }
            }

            writer.WriteEndObject();

            var actualYaml = outputString.GetStringBuilder().Insert(0, "\r\n")
                .ToString()
                .MakeLineBreaksEnvironmentNeutral();

            expectedYaml = expectedYaml.MakeLineBreaksEnvironmentNeutral();

            // Assert
            Assert.Equal(expectedYaml, actualYaml);
        }
    }
}