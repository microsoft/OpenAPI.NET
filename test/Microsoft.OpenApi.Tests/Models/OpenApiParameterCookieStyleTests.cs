// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Xunit;
using VerifyXunit;

namespace Microsoft.OpenApi.Tests.Models
{
    [Collection("DefaultSettings")]
    public class OpenApiParameterCookieStyleTests
    {
        private static OpenApiParameter CookieParameter => new()
        {
            Name = "sessionId",
            In = ParameterLocation.Cookie,
            Style = ParameterStyle.Cookie,
            Description = "Session identifier stored in cookie",
            Schema = new OpenApiSchema()
            {
                Type = JsonSchemaType.String
            }
        };

        private static OpenApiParameter CookieParameterWithDefault => new()
        {
            Name = "preferences",
            In = ParameterLocation.Cookie,
            Description = "User preferences stored in cookie",
            Schema = new OpenApiSchema()
            {
                Type = JsonSchemaType.String
            }
        };

        [Fact]
        public void CookieParameterStyleIsAvailable()
        {
            // Arrange & Act
            var parameter = CookieParameter;

            // Assert
            Assert.Equal(ParameterStyle.Cookie, parameter.Style);
            Assert.Equal(ParameterLocation.Cookie, parameter.In);
        }

        [Fact]
        public void CookieParameterHasCorrectDefaultStyle()
        {
            // Arrange & Act
            var parameter = CookieParameterWithDefault;

            // Assert
            Assert.Equal(ParameterStyle.Form, parameter.Style); // Default for cookie location should be Form
        }

        [Fact]
        public void CookieParameterStyleDisplayNameIsCookie()
        {
            // Arrange & Act
            var displayName = ParameterStyle.Cookie.GetDisplayName();

            // Assert
            Assert.Equal("cookie", displayName);
        }

        [Fact]
        public async Task SerializeCookieParameterAsV32JsonWorks()
        {
            // Arrange
            var expected =
                """
                {
                  "name": "sessionId",
                  "in": "cookie",
                  "description": "Session identifier stored in cookie",
                  "style": "cookie",
                  "schema": {
                    "type": "string"
                  }
                }
                """;

            // Act
            var actual = await CookieParameter.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_2);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task SerializeCookieParameterWithDefaultStyleAsV32JsonWorks()
        {
            // Arrange
            var expected =
                """
                {
                  "name": "preferences",
                  "in": "cookie",
                  "description": "User preferences stored in cookie",
                  "schema": {
                    "type": "string"
                  }
                }
                """;

            // Act
            var actual = await CookieParameterWithDefault.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_2);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(OpenApiSpecVersion.OpenApi2_0)]
        [InlineData(OpenApiSpecVersion.OpenApi3_0)]
        [InlineData(OpenApiSpecVersion.OpenApi3_1)]
        public void SerializeCookieParameterStyleThrowsForEarlierVersions(OpenApiSpecVersion version)
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter);

            // Act & Assert
            var exception = Assert.Throws<OpenApiException>(() =>
            {
                switch (version)
                {
                    case OpenApiSpecVersion.OpenApi2_0:
                        CookieParameter.SerializeAsV2(writer);
                        break;
                    case OpenApiSpecVersion.OpenApi3_0:
                        CookieParameter.SerializeAsV3(writer);
                        break;
                    case OpenApiSpecVersion.OpenApi3_1:
                        CookieParameter.SerializeAsV31(writer);
                        break;
                }
            });

            Assert.Contains("Parameter style 'cookie' is only supported in OpenAPI 3.2 and later versions", exception.Message);
            Assert.Contains($"Current version: {version}", exception.Message);
        }

        [Theory]
        [InlineData(OpenApiSpecVersion.OpenApi2_0)]
        [InlineData(OpenApiSpecVersion.OpenApi3_0)]
        [InlineData(OpenApiSpecVersion.OpenApi3_1)]
        public async Task SerializeCookieParameterWithDefaultStyleWorksForEarlierVersions(OpenApiSpecVersion version)
        {
            // Arrange & Act
            string actual = version switch
            {
                OpenApiSpecVersion.OpenApi2_0 => await CookieParameterWithDefault.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi2_0),
                OpenApiSpecVersion.OpenApi3_0 => await CookieParameterWithDefault.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_0),
                OpenApiSpecVersion.OpenApi3_1 => await CookieParameterWithDefault.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_1),
                _ => throw new ArgumentOutOfRangeException()
            };

            // Assert - Should not throw because default style (Form) is being used
            Assert.NotEmpty(actual);
            Assert.DoesNotContain("\"style\"", actual); // Style should not be emitted when it's the default
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task SerializeCookieParameterAsV32JsonWorksAsync(bool produceTerseOutput)
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new() { Terse = produceTerseOutput });

            // Act
            CookieParameter.SerializeAsV32(writer);
            await writer.FlushAsync();

            // Assert
            await Verifier.Verify(outputStringWriter).UseParameters(produceTerseOutput);
        }

        [Fact]
        public void CookieParameterStyleEnumValueExists()
        {
            // Arrange & Act
            var cookieStyleExists = Enum.IsDefined(typeof(ParameterStyle), ParameterStyle.Cookie);
            
            // Assert
            Assert.True(cookieStyleExists);
        }

        [Fact]
        public void CookieParameterStyleCanBeDeserialized()
        {
            // Arrange
            var cookieStyleString = "cookie";
            
            // Act
            var success = cookieStyleString.TryGetEnumFromDisplayName<ParameterStyle>(out var parameterStyle);
            
            // Assert
            Assert.True(success);
            Assert.Equal(ParameterStyle.Cookie, parameterStyle);
        }

        [Fact]
        public async Task SerializeCookieParameterAsYamlV32Works()
        {
            // Arrange
            var expected = """
                name: sessionId
                in: cookie
                description: Session identifier stored in cookie
                style: cookie
                schema:
                  type: string
                """;

            // Act
            var actual = await CookieParameter.SerializeAsYamlAsync(OpenApiSpecVersion.OpenApi3_2);

            // Assert
            Assert.Equal(expected.MakeLineBreaksEnvironmentNeutral(), actual.MakeLineBreaksEnvironmentNeutral());
        }

        [Theory]
        [InlineData(ParameterStyle.Form, true)]
        [InlineData(ParameterStyle.SpaceDelimited, false)]
        [InlineData(ParameterStyle.Cookie, true)]
        public void WhenStyleIsFormOrCookieTheDefaultValueOfExplodeShouldBeTrueOtherwiseFalse(ParameterStyle? style, bool expectedExplode)
        {
            // Arrange
            var parameter = new OpenApiParameter
            {
                Name = "name1",
                In = ParameterLocation.Query,
                Style = style
            };

            // Act & Assert
            Assert.Equal(expectedExplode, parameter.Explode);
        }
    }
}
