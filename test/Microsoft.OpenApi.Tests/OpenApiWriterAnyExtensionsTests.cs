// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.IO;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Writers;
using Xunit;

namespace Microsoft.OpenApi.Tests
{
    public class OpenApiWriterAnyExtensionsTests
    {
        [Fact]
        public void WriteOpenApiNullAsJsonWorks()
        {
            // Arrange
            OpenApiNull nullValue = new OpenApiNull();

            // Act
            string json = WriteAsJson(nullValue);

            // Assert
            Assert.Equal("null", json);
        }

        [Fact]
        public void WriteOpenApiIntergerAsJsonWorks()
        {
            // Arrange
            OpenApiInteger intValue = new OpenApiInteger(42);

            // Act
            string json = WriteAsJson(intValue);

            // Assert
            Assert.Equal("42", json);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void WriteOpenApiBooleanAsJsonWorks(bool input)
        {
            // Arrange
            OpenApiBoolean boolValue = new OpenApiBoolean(input);

            // Act
            string json = WriteAsJson(boolValue);

            // Assert
            Assert.Equal(input.ToString().ToLower(), json);
        }

        private static OpenApiObject openApiObject = new OpenApiObject
        {
            { "stringProp", new OpenApiString("stringValue1") },
            { "objProp", new OpenApiObject() },
            {
                "arrayProp",
                new OpenApiArray
                {
                    new OpenApiBoolean(false)
                }
            }
        };

        [Fact]
        public void WriteOpenApiObjectAsJsonWorks()
        {
            // Arrange
            string expect = @"
{
  ""stringProp"": ""stringValue1"",
  ""objProp"": { },
  ""arrayProp"": [
    false
  ]
}";

            // Act
            string json = WriteAsJson(openApiObject);

            // Assert
            Assert.Equal(expect, json);
        }

        [Fact]
        public void WriteOpenApiArrayAsJsonWorks()
        {
            // Arrange
            string expect = @"
[
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

            OpenApiArray array = new OpenApiArray
            {
                new OpenApiBoolean(false),
                openApiObject,
                new OpenApiString("stringValue2")
            };

            // Act
            string json = WriteAsJson(array);

            // Assert
            Assert.Equal(expect, json);
        }

        private static string WriteAsJson(IOpenApiAny any)
        {
            MemoryStream stream = new MemoryStream();
            IOpenApiWriter writer = new OpenApiJsonWriter(new StreamWriter(stream));
            writer.WriteAny(any);
            writer.Flush();
            stream.Position = 0;
            string value = new StreamReader(stream).ReadToEnd();

            if (any.AnyKind == AnyTypeKind.Primitive || any.AnyKind == AnyTypeKind.Null)
            {
                return value;
            }
            else
            {
                // add "\r\n" at the head because the expect string has a newline at starting.
                return "\r\n" + value.Replace("\n", "\r\n");
            }
        }
    }
}
