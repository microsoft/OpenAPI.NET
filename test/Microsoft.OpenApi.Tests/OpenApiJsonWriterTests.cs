// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using System.IO;
using Microsoft.OpenApi.Writers;
using Newtonsoft.Json.Linq;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.OpenApi.Tests
{
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

            var parsedJToken = JToken.Parse(outputString.GetStringBuilder().ToString());
            var expectedJToken = JToken.FromObject(stringValues);

            // Assert
            Assert.Equal(expectedJToken, parsedJToken);
        }

        public static IEnumerable<object[]> WriteMapAsJsonShouldMatchExpectedTestCasesSimple()
        {
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
            yield return new object[]
            {
                new Dictionary<string, object>
                {
                    ["property1"] = new Dictionary<string, object>(),
                    ["property2"] = "value2",
                    ["property3"] = "value3",
                    ["property4"] = "value4"
                }
            };

            yield return new object[]
            {
                new Dictionary<string, object>
                {
                    ["property1"] = new Dictionary<string, object>(),
                    ["property2"] = "value2",
                    ["property3"] = new Dictionary<string, object>
                    {
                        ["innerProperty1"] = "innerValue1"
                    },
                    ["property4"] = "value4"
                }
            };

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
                    ["property4"] = new List<object> {1, "listValue1"}
                }
            };
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
            writer.WriteStartObject();

            foreach (var keyValue in inputMap)
            {
                if (keyValue.Value.GetType().IsPrimitive || keyValue.Value.GetType() == typeof(string))
                {
                    writer.WritePropertyName(keyValue.Key);
                    writer.WriteValue(keyValue.Value);
                }
                else if (keyValue.Value.GetType() == typeof(Dictionary<string, object>))
                {
                    writer.WritePropertyName(keyValue.Key);
                    writer.WriteStartObject();

                    foreach (var elementValue in (Dictionary<string, object>)(keyValue.Value))
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

            var parsedJToken = JToken.Parse(outputString.GetStringBuilder().ToString());
            var expectedJToken = JToken.FromObject(inputMap);

            // Assert
            Assert.Equal(expectedJToken, parsedJToken);
        }
    }
}