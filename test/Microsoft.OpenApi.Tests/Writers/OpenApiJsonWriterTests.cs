// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using FluentAssertions;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;
using Newtonsoft.Json;
using Xunit;

namespace Microsoft.OpenApi.Tests.Writers
{
    [Collection("DefaultSettings")]
    public class OpenApiJsonWriterTests
    {
        static bool[] shouldProduceTerseOutputValues = new[] { true, false };

        public static IEnumerable<object[]> WriteStringListAsJsonShouldMatchExpectedTestCases()
        {
            return
                from input in new[]
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
                    new[] {"string1", "string1", "string1", "string1"}
                }
                from shouldBeTerse in shouldProduceTerseOutputValues
                select new object[] { input, shouldBeTerse };
        }

        [Theory]
        [MemberData(nameof(WriteStringListAsJsonShouldMatchExpectedTestCases))]
        public void WriteStringListAsJsonShouldMatchExpected(string[] stringValues, bool produceTerseOutput)
        {
            // Arrange
            var outputString = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputString, new() { Terse = produceTerseOutput });

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
            parsedObject.Should().BeEquivalentTo(expectedObject);
        }

        public static IEnumerable<object[]> WriteMapAsJsonShouldMatchExpectedTestCasesSimple()
        {
            return
                from input in new IDictionary<string, object>[] {
                    // Simple map
                    new Dictionary<string, object>
                    {
                        ["property1"] = "value1",
                        ["property2"] = "value2",
                        ["property3"] = "value3",
                        ["property4"] = "value4"
                    },

                    // Simple map with duplicate values
                    new Dictionary<string, object>
                    {
                        ["property1"] = "value1",
                        ["property2"] = "value1",
                        ["property3"] = "value1",
                        ["property4"] = "value1"
                    },
                }
                from shouldBeTerse in shouldProduceTerseOutputValues
                select new object[] { input, shouldBeTerse };
        }

        public static IEnumerable<object[]> WriteMapAsJsonShouldMatchExpectedTestCasesComplex()
        {
            return
                from input in new IDictionary<string, object>[] {
                    // Empty map and empty list
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

                    // Number, boolean, and null handling
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

                    // DateTime
                    new Dictionary<string, object>
                    {
                        ["property1"] = new DateTime(1970, 01, 01),
                        ["property2"] = new DateTimeOffset(new(1970, 01, 01)),
                        ["property3"] = new DateTime(2018, 04, 03),
                    },

                    // Nested map
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

                    // Nested map and list
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
                }
                from shouldBeTerse in shouldProduceTerseOutputValues
                select new object[] { input, shouldBeTerse };
        }

        private void WriteValueRecursive(OpenApiJsonWriter writer, object value)
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
        [MemberData(nameof(WriteMapAsJsonShouldMatchExpectedTestCasesSimple))]
        [MemberData(nameof(WriteMapAsJsonShouldMatchExpectedTestCasesComplex))]
        public void WriteMapAsJsonShouldMatchExpected(IDictionary<string, object> inputMap, bool produceTerseOutput)
        {
            // Arrange
            var outputString = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputString, new() { Terse = produceTerseOutput });

            // Act
            WriteValueRecursive(writer, inputMap);

            var parsedObject = JsonConvert.DeserializeObject(outputString.GetStringBuilder().ToString());
            var expectedObject = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(inputMap));

            // Assert
            parsedObject.Should().BeEquivalentTo(expectedObject);
        }

        public static IEnumerable<object[]> WriteDateTimeAsJsonTestCases()
        {
            return
                from input in new[] {
                    new(2018, 1, 1, 10, 20, 30, TimeSpan.Zero),
                    new(2018, 1, 1, 10, 20, 30, 100, TimeSpan.FromHours(14)),
                    DateTimeOffset.UtcNow + TimeSpan.FromDays(4),
                    DateTime.UtcNow + TimeSpan.FromDays(4),
                }
                from shouldBeTerse in shouldProduceTerseOutputValues
                select new object[] { input, shouldBeTerse };
        }

        [Theory]
        [MemberData(nameof(WriteDateTimeAsJsonTestCases))]
        public void WriteDateTimeAsJsonShouldMatchExpected(DateTimeOffset dateTimeOffset, bool produceTerseOutput)
        {
            // Arrange
            var outputString = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputString, new() { Terse = produceTerseOutput });

            // Act
            writer.WriteValue(dateTimeOffset);

            var writtenString = outputString.GetStringBuilder().ToString();
            var expectedString = JsonConvert.SerializeObject(dateTimeOffset, new JsonSerializerSettings
            {
                DateFormatString = "yyyy-MM-ddTHH:mm:ss.fffffffK",
            });

            // Assert
            writtenString.Should().Be(expectedString);
        }

        [Fact]
        public void OpenApiJsonWriterOutputsValidJsonValueWhenSchemaHasNanOrInfinityValues()
        {
            // Arrange
            var schema = new OpenApiSchema
            {
                Enum = new List<IOpenApiAny> {
                        new OpenApiDouble(double.NaN),
                        new OpenApiDouble(double.PositiveInfinity),
                        new OpenApiDouble(double.NegativeInfinity) 
                }
            };

            // Act
            var schemaBuilder = new StringBuilder();
            var jsonWriter = new OpenApiJsonWriter(new StringWriter(schemaBuilder));
            schema.SerializeAsV3(jsonWriter);
            var jsonString = schemaBuilder.ToString();

            // Assert
            var exception = Record.Exception(() => System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(jsonString));
            Assert.Null(exception);
        }
    }
}
