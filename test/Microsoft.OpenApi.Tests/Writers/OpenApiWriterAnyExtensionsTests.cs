// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Writers;
using VerifyXunit;
using Xunit;

namespace Microsoft.OpenApi.Tests.Writers
{
    [Collection("DefaultSettings")]
    public class OpenApiWriterAnyExtensionsTests
    {
        static bool[] shouldProduceTerseOutputValues = new[] { true, false };

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task WriteOpenApiNullAsJsonWorksAsync(bool produceTerseOutput)
        {
            // Arrange
            var json = await WriteAsJsonAsync(null, produceTerseOutput);

            // Assert
            json.Should().Be("null");
        }

        public static IEnumerable<object[]> IntInputs
        {
            get
            {
                return
                    from input in new[] {
                        int.MinValue,
                        42,
                        int.MaxValue,
                     }
                    from shouldBeTerse in shouldProduceTerseOutputValues
                    select new object[] { input, shouldBeTerse };
            }
        }

        [Theory]
        [MemberData(nameof(IntInputs))]
        public async Task WriteOpenApiIntegerAsJsonWorksAsync(int input, bool produceTerseOutput)
        {
            // Arrange
            var intValue = input;

            var json = await WriteAsJsonAsync(intValue, produceTerseOutput);

            // Assert
            json.Should().Be(input.ToString());
        }

        public static IEnumerable<object[]> LongInputs
        {
            get
            {
                return
                    from input in new[] {
                        long.MinValue,
                        42,
                        long.MaxValue,
                     }
                    from shouldBeTerse in shouldProduceTerseOutputValues
                    select new object[] { input, shouldBeTerse };
            }
        }

        [Theory]
        [MemberData(nameof(LongInputs))]
        public async Task WriteOpenApiLongAsJsonWorksAsync(long input, bool produceTerseOutput)
        {
            // Arrange
            var longValue = input;

            var json = await WriteAsJsonAsync(longValue, produceTerseOutput);

            // Assert
            json.Should().Be(input.ToString());
        }

        public static IEnumerable<object[]> FloatInputs
        {
            get
            {
                return
                    from input in new[] {
                        float.MinValue,
                        42.42f,
                        float.MaxValue,
                     }
                    from shouldBeTerse in shouldProduceTerseOutputValues
                    select new object[] { input, shouldBeTerse };
            }
        }

        [Theory]
        [MemberData(nameof(FloatInputs))]
        public async Task WriteOpenApiFloatAsJsonWorksAsync(float input, bool produceTerseOutput)
        {
            // Arrange
            var floatValue = input;

            var json = await WriteAsJsonAsync(floatValue, produceTerseOutput);

            // Assert
            json.Should().Be(input.ToString());
        }

        public static IEnumerable<object[]> DoubleInputs
        {
            get
            {
                return
                    from input in new[] {
                        double.MinValue,
                        42.42d,
                        double.MaxValue,
                     }
                    from shouldBeTerse in shouldProduceTerseOutputValues
                    select new object[] { input, shouldBeTerse };
            }
        }

        [Theory]
        [MemberData(nameof(DoubleInputs))]
        public async Task WriteOpenApiDoubleAsJsonWorksAsync(double input, bool produceTerseOutput)
        {
            // Arrange
            var doubleValue = input;

            var json = await WriteAsJsonAsync(doubleValue, produceTerseOutput);

            // Assert
            json.Should().Be(input.ToString());
        }

        public static IEnumerable<object[]> StringifiedDateTimes
        {
            get
            {
                return
                    from input in new [] {
                        "2017-1-2",
                        "1999-01-02T12:10:22",
                        "1999-01-03",
                        "10:30:12"
                     }
                    from shouldBeTerse in shouldProduceTerseOutputValues
                    select new object[] { input, shouldBeTerse };
            }
        }

        [Theory]
        [MemberData(nameof(StringifiedDateTimes))]
        public async Task WriteOpenApiDateTimeAsJsonWorksAsync(string inputString, bool produceTerseOutput)
        {
            // Arrange
            var input = DateTimeOffset.Parse(inputString, CultureInfo.InvariantCulture);
            var dateTimeValue = input;

            var json = await WriteAsJsonAsync(dateTimeValue, produceTerseOutput);
            var expectedJson = "\"" + input.ToString("o") + "\"";

            // Assert
            json.Should().Be(expectedJson);
        }

        public static IEnumerable<object[]> BooleanInputs
        {
            get =>
                from input in new [] { true, false }
                from shouldBeTerse in shouldProduceTerseOutputValues
                select new object[] { input, shouldBeTerse };
        }

        [Theory]
        [MemberData(nameof(BooleanInputs))]
        public async Task WriteOpenApiBooleanAsJsonWorksAsync(bool input, bool produceTerseOutput)
        {
            // Arrange
            var boolValue = input;

            var json = await WriteAsJsonAsync(boolValue, produceTerseOutput);

            // Assert
            json.Should().Be(input.ToString().ToLower());
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task WriteOpenApiObjectAsJsonWorksAsync(bool produceTerseOutput)
        {
            // Arrange
            var openApiObject = new JsonObject
            {
                {"stringProp", "stringValue1"},
                {"objProp", new JsonObject()},
                {
                    "arrayProp",
                    new JsonArray
                    {
                        false
                    }
                }
            };

            var actualJson = WriteAsJsonAsync(openApiObject, produceTerseOutput);

            // Assert
            await Verifier.Verify(actualJson).UseParameters(produceTerseOutput);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task WriteOpenApiArrayAsJsonWorksAsync(bool produceTerseOutput)
        {
            // Arrange
            var openApiObject = new JsonObject
            {
                {"stringProp", "stringValue1"},
                {"objProp", new JsonObject()},
                {
                    "arrayProp",
                    new JsonArray
                    {
                        false
                    }
                }
            };

            var array = new JsonArray
            {
                false,
                openApiObject,
                "stringValue2"
            };

            var actualJson = WriteAsJsonAsync(array, produceTerseOutput);

            // Assert
            await Verifier.Verify(actualJson).UseParameters(produceTerseOutput);
        }

        private static async Task<string> WriteAsJsonAsync(JsonNode any, bool produceTerseOutput = false)
        {
            // Arrange (continued)
            using var stream = new MemoryStream();
            var writer = new OpenApiJsonWriter(
                new StreamWriter(stream),
                new() { Terse = produceTerseOutput });

            writer.WriteAny(any);
            writer.Flush();
            stream.Position = 0;

            // Act
            using var sr = new StreamReader(stream);
            var value = await sr.ReadToEndAsync();
            var element = JsonDocument.Parse(value).RootElement;
            return element.ValueKind switch
            {
                JsonValueKind.String => value,
                JsonValueKind.Number => value,
                JsonValueKind.Null => value,
                JsonValueKind.False => value,
                JsonValueKind.True => value,
                _ => value.MakeLineBreaksEnvironmentNeutral(),
            };
        }
    }
}
