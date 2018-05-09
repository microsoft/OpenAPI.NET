// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.IO;
using FluentAssertions;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Writers;
using Xunit;

namespace Microsoft.OpenApi.Tests.Writers
{
    [Collection("DefaultSettings")]
    public class OpenApiWriterAnyExtensionsTests
    {
        [Fact]
        public void WriteOpenApiNullAsJsonWorks()
        {
            // Arrange
            var nullValue = new OpenApiNull();

            var json = WriteAsJson(nullValue);

            // Assert
            json.Should().Be("null");
        }

        [Theory]
        [InlineData(int.MinValue)]
        [InlineData(42)]
        [InlineData(int.MaxValue)]
        public void WriteOpenApiIntegerAsJsonWorks(int input)
        {
            // Arrange
            var intValue = new OpenApiInteger(input);

            var json = WriteAsJson(intValue);

            // Assert
            json.Should().Be(input.ToString());
        }

        [Theory]
        [InlineData(long.MinValue)]
        [InlineData(42)]
        [InlineData(long.MaxValue)]
        public void WriteOpenApiLongAsJsonWorks(long input)
        {
            // Arrange
            var longValue = new OpenApiLong(input);

            var json = WriteAsJson(longValue);

            // Assert
            json.Should().Be(input.ToString());
        }

        [Theory]
        [InlineData(float.MinValue)]
        [InlineData(42.42)]
        [InlineData(float.MaxValue)]
        public void WriteOpenApiFloatAsJsonWorks(float input)
        {
            // Arrange
            var floatValue = new OpenApiFloat(input);

            var json = WriteAsJson(floatValue);

            // Assert
            json.Should().Be(input.ToString());
        }

        [Theory]
        [InlineData(double.MinValue)]
        [InlineData(42.42)]
        [InlineData(double.MaxValue)]
        public void WriteOpenApiDoubleAsJsonWorks(double input)
        {
            // Arrange
            var doubleValue = new OpenApiDouble(input);

            var json = WriteAsJson(doubleValue);

            // Assert
            json.Should().Be(input.ToString());
        }

        [Theory]
        [InlineData("2017-1-2")]
        [InlineData("1999-01-02T12:10:22")]
        [InlineData("1999-01-03")]
        [InlineData("10:30:12")]
        public void WriteOpenApiDateTimeAsJsonWorks(string inputString)
        {
            // Arrange
            var input = DateTimeOffset.Parse(inputString);
            var dateTimeValue = new OpenApiDateTime(input);

            var json = WriteAsJson(dateTimeValue);

            // Assert
            json.Should().Be(input.ToString("o"));
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void WriteOpenApiBooleanAsJsonWorks(bool input)
        {
            // Arrange
            var boolValue = new OpenApiBoolean(input);

            var json = WriteAsJson(boolValue);

            // Assert
            json.Should().Be(input.ToString().ToLower());
        }

        [Fact]
        public void WriteOpenApiObjectAsJsonWorks()
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

            var actualJson = WriteAsJson(openApiObject);

            // Assert

            var expectedJson = @"{
  ""stringProp"": ""stringValue1"",
  ""objProp"": { },
  ""arrayProp"": [
    false
  ]
}";
            expectedJson = expectedJson.MakeLineBreaksEnvironmentNeutral();

            actualJson.Should().Be(expectedJson);
        }

        [Fact]
        public void WriteOpenApiArrayAsJsonWorks()
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

            var actualJson = WriteAsJson(array);

            // Assert

            var expectedJson = @"[
  false,
  {
    ""stringProp"": ""stringValue1"",
    ""objProp"": { },
    ""arrayProp"": [
      false
    ]
  },
  ""stringValue2""
]";

            expectedJson = expectedJson.MakeLineBreaksEnvironmentNeutral();

            actualJson.Should().Be(expectedJson);
        }

        private static string WriteAsJson(IOpenApiAny any)
        {
            // Arrange (continued)
            var stream = new MemoryStream();
            IOpenApiWriter writer = new OpenApiJsonWriter(new StreamWriter(stream));

            writer.WriteAny(any);
            writer.Flush();
            stream.Position = 0;

            // Act
            var value = new StreamReader(stream).ReadToEnd();

            if (any.AnyType == AnyType.Primitive || any.AnyType == AnyType.Null)
            {
                return value;
            }

            return value.MakeLineBreaksEnvironmentNeutral();
        }
    }
}