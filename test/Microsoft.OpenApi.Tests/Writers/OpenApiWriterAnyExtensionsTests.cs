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
using Microsoft.OpenApi.Writers;
using VerifyXunit;
using Xunit;

namespace Microsoft.OpenApi.Tests.Writers
{
    [Collection("DefaultSettings")]
    [UsesVerify]
    public class OpenApiWriterAnyExtensionsTests
    {
        static bool[] shouldProduceTerseOutputValues = new[] { true, false };

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void WriteOpenApiNullAsJsonWorks(bool produceTerseOutput)
        {
            // Arrange
            var json = WriteAsJson(null, produceTerseOutput);

            // Assert
            json.Should().Be("null");
        }

        public static IEnumerable<object[]> IntInputs
        {
            get
            {
                return
                    from input in new int[] {
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
        public void WriteOpenApiIntegerAsJsonWorks(int input, bool produceTerseOutput)
        {
            // Arrange
            var intValue = input;

            var json = WriteAsJson(intValue, produceTerseOutput);

            // Assert
            json.Should().Be(input.ToString());
        }

        public static IEnumerable<object[]> LongInputs
        {
            get
            {
                return
                    from input in new long[] {
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
        public void WriteOpenApiLongAsJsonWorks(long input, bool produceTerseOutput)
        {
            // Arrange
            var longValue = input;

            var json = WriteAsJson(longValue, produceTerseOutput);

            // Assert
            json.Should().Be(input.ToString());
        }

        public static IEnumerable<object[]> FloatInputs
        {
            get
            {
                return
                    from input in new float[] {
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
        public void WriteOpenApiFloatAsJsonWorks(float input, bool produceTerseOutput)
        {
            // Arrange
            var floatValue = input;

            var json = WriteAsJson(floatValue, produceTerseOutput);

            // Assert
            json.Should().Be(input.ToString());
        }

        public static IEnumerable<object[]> DoubleInputs
        {
            get
            {
                return
                    from input in new double[] {
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
        public void WriteOpenApiDoubleAsJsonWorks(double input, bool produceTerseOutput)
        {
            // Arrange
            var doubleValue = input;

            var json = WriteAsJson(doubleValue, produceTerseOutput);

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
        public void WriteOpenApiDateTimeAsJsonWorks(string inputString, bool produceTerseOutput)
        {
            // Arrange
            var input = DateTimeOffset.Parse(inputString, CultureInfo.InvariantCulture);
            var dateTimeValue = input;

            var json = WriteAsJson(dateTimeValue, produceTerseOutput);
            var expectedJson = "\"" + input.ToString("o") + "\"";

            // Assert
            json.Should().Be(expectedJson);
        }

        public static IEnumerable<object[]> BooleanInputs
        {
            get
            {
                return
                    from input in new [] { true, false }
                    from shouldBeTerse in shouldProduceTerseOutputValues
                    select new object[] { input, shouldBeTerse };
            }
        }

        [Theory]
        [MemberData(nameof(BooleanInputs))]
        public void WriteOpenApiBooleanAsJsonWorks(bool input, bool produceTerseOutput)
        {
            // Arrange
            var boolValue = input;

            var json = WriteAsJson(boolValue, produceTerseOutput);

            // Assert
            json.Should().Be(input.ToString().ToLower());
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task WriteOpenApiObjectAsJsonWorks(bool produceTerseOutput)
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

            var actualJson = WriteAsJson(openApiObject, produceTerseOutput);

            // Assert
            await Verifier.Verify(actualJson).UseParameters(produceTerseOutput);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task WriteOpenApiArrayAsJsonWorks(bool produceTerseOutput)
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

            var actualJson = WriteAsJson(array, produceTerseOutput);

            // Assert
            await Verifier.Verify(actualJson).UseParameters(produceTerseOutput);
        }

        private static string WriteAsJson(JsonNode any, bool produceTerseOutput = false)
        {
            // Arrange (continued)
            var stream = new MemoryStream();
            IOpenApiWriter writer = new OpenApiJsonWriter(
                new StreamWriter(stream),
                new OpenApiJsonWriterSettings { Terse = produceTerseOutput });

            writer.WriteAny(any);
            writer.Flush();
            stream.Position = 0;

            // Act
            var value = new StreamReader(stream).ReadToEnd();
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
