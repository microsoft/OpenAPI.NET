// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.IO;
using Microsoft.OpenApi.Writers;
using Xunit;

namespace Microsoft.OpenApi.Tests
{
    public class WriterOpenApiAnyExtensionsTests
    {
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
        public void WriteOpenApiObjectWorksJson()
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
            string json = WriteToJson(openApiObject);

            // Assert
            Assert.Equal(expect, json);
        }

        [Fact]
        public void WriteOpenApiArrayWorksJson()
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
            string json = WriteToJson(array);

            // Assert
            Assert.Equal(expect, json);
        }

        private static string WriteToJson(IOpenApiAny any)
        {
            MemoryStream stream = new MemoryStream();
            IOpenApiWriter writer = new OpenApiJsonWriter(new StreamWriter(stream));
            writer.WriteAny(any);
            writer.Flush();
            stream.Position = 0;
            string value = new StreamReader(stream).ReadToEnd();
            return "\r\n" + value.Replace("\n", "\r\n");
        }
    }
}
