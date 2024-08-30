// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
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
            var nullValue = new OpenApiNull();

            var json = await WriteAsJsonAsync(nullValue, produceTerseOutput);

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
            var intValue = new OpenApiInteger(input);

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
            var longValue = new OpenApiLong(input);

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
            var floatValue = new OpenApiFloat(input);

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
            var doubleValue = new OpenApiDouble(input);

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
            var dateTimeValue = new OpenApiDateTime(input);

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
            var boolValue = new OpenApiBoolean(input);

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
            var openApiObject = new OpenApiObject
            {
                {"stringProp", new OpenApiString("stringValue1")},
                {"objProp", new OpenApiObject()},
                {
                    "arrayProp",
                    new OpenApiArray
                    {
                        new OpenApiBoolean(false)
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
            var openApiObject = new OpenApiObject
            {
                {"stringProp", new OpenApiString("stringValue1")},
                {"objProp", new OpenApiObject()},
                {
                    "arrayProp",
                    new OpenApiArray
                    {
                        new OpenApiBoolean(false)
                    }
                }
            };

            var array = new OpenApiArray
            {
                new OpenApiBoolean(false),
                openApiObject,
                new OpenApiString("stringValue2")
            };

            var actualJson = WriteAsJsonAsync(array, produceTerseOutput);

            // Assert
            await Verifier.Verify(actualJson).UseParameters(produceTerseOutput);
        }

        private static async Task<string> WriteAsJsonAsync(IOpenApiAny any, bool produceTerseOutput = false)
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

            if (any.AnyType is AnyType.Primitive or AnyType.Null)
            {
                return value;
            }

            return value.MakeLineBreaksEnvironmentNeutral();
        }
    }
}
