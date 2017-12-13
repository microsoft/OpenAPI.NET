// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Collections;
using System.Collections.Generic;
using System.IO;
using FluentAssertions;
using Microsoft.OpenApi.Writers;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.OpenApi.Tests.Writers
{
    [Collection("DefaultSettings")]
    public class OpenApiJsonWriterTests
    {
        private readonly ITestOutputHelper _output;

        public OpenApiJsonWriterTests(ITestOutputHelper output)
        {
            _output = output;
        }

        public static IEnumerable<object[]> WriteStringListAsJsonShouldMatchExpectedTestCases()
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
                }
            };

            yield return new object[]
            {
                new[] {"string1", "string1", "string1", "string1"}
            };
        }

        [Theory]
        [MemberData(nameof(WriteStringListAsJsonShouldMatchExpectedTestCases))]
        public void WriteStringListAsJsonShouldMatchExpected(string[] stringValues)
        {
            // Arrange
            var outputString = new StringWriter();
            var writer = new OpenApiJsonWriter(outputString);

            // Act
            writer.WriteStartArray();
            foreach (var stringValue in stringValues)
            {
                writer.WriteValue(stringValue);
            }

            writer.WriteEndArray();
            writer.Flush();

            var parsedObject = JsonConvert.DeserializeObject(outputString.GetStringBuilder().ToString());
            var expectedObject =
                JsonConvert.DeserializeObject(JsonConvert.SerializeObject(new List<string>(stringValues)));

            // Assert
            parsedObject.ShouldBeEquivalentTo(expectedObject);
        }

        public static IEnumerable<object[]> WriteMapAsJsonShouldMatchExpectedTestCasesSimple()
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
                }
            };

            // Simple map with duplicate values
            yield return new object[]
            {
                new Dictionary<string, object>
                {
                    ["property1"] = "value1",
                    ["property2"] = "value1",
                    ["property3"] = "value1",
                    ["property4"] = "value1"
                }
            };
        }

        public static IEnumerable<object[]> WriteMapAsJsonShouldMatchExpectedTestCasesComplex()
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
                }
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
                }
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
                }
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
                }
            };
        }

        private void WriteValueRecursive(OpenApiJsonWriter writer, object value)
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
        [MemberData(nameof(WriteMapAsJsonShouldMatchExpectedTestCasesSimple))]
        [MemberData(nameof(WriteMapAsJsonShouldMatchExpectedTestCasesComplex))]
        public void WriteMapAsJsonShouldMatchExpected(IDictionary<string, object> inputMap)
        {
            // Arrange
            var outputString = new StringWriter();
            var writer = new OpenApiJsonWriter(outputString);

            // Act
            WriteValueRecursive(writer, inputMap);

            var parsedObject = JsonConvert.DeserializeObject(outputString.GetStringBuilder().ToString());
            var expectedObject = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(inputMap));

            // Assert
            parsedObject.ShouldBeEquivalentTo(expectedObject);
        }
    }
}