// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

namespace Microsoft.OpenApi.Tests.Models
{
    [Collection("DefaultSettings")]
    public class OpenApiSecuritySchemeTests
    {
        private static OpenApiSecurityScheme ApiKeySecurityScheme => new()
        {
            Description = "description1",
            Name = "parameterName",
            Type = SecuritySchemeType.ApiKey,
            In = ParameterLocation.Query,
        };

        private static OpenApiSecurityScheme HttpBasicSecurityScheme => new()
        {
            Description = "description1",
            Type = SecuritySchemeType.Http,
            Scheme = OpenApiConstants.Basic
        };

        private static OpenApiSecurityScheme HttpBearerSecurityScheme => new()
        {
            Description = "description1",
            Type = SecuritySchemeType.Http,
            Scheme = OpenApiConstants.Bearer,
            BearerFormat = OpenApiConstants.Jwt
        };

        private static OpenApiSecurityScheme OAuth2SingleFlowSecurityScheme => new()
        {
            Description = "description1",
            Type = SecuritySchemeType.OAuth2,
            Flows = new()
            {
                Implicit = new()
                {
                    Scopes = new Dictionary<string, string>
                    {
                        ["operation1:object1"] = "operation 1 on object 1",
                        ["operation2:object2"] = "operation 2 on object 2"
                    },
                    AuthorizationUrl = new("https://example.com/api/oauth")
                }
            }
        };

        private static OpenApiSecurityScheme OAuth2MultipleFlowSecurityScheme => new()
        {
            Description = "description1",
            Type = SecuritySchemeType.OAuth2,
            Flows = new()
            {
                Implicit = new()
                {
                    Scopes = new Dictionary<string, string>
                    {
                        ["operation1:object1"] = "operation 1 on object 1",
                        ["operation2:object2"] = "operation 2 on object 2"
                    },
                    AuthorizationUrl = new("https://example.com/api/oauth")
                },
                ClientCredentials = new()
                {
                    Scopes = new Dictionary<string, string>
                    {
                        ["operation1:object1"] = "operation 1 on object 1",
                        ["operation2:object2"] = "operation 2 on object 2"
                    },
                    TokenUrl = new("https://example.com/api/token"),
                    RefreshUrl = new("https://example.com/api/refresh"),
                },
                AuthorizationCode = new()
                {
                    Scopes = new Dictionary<string, string>
                    {
                        ["operation1:object1"] = "operation 1 on object 1",
                        ["operation2:object2"] = "operation 2 on object 2"
                    },
                    TokenUrl = new("https://example.com/api/token"),
                    AuthorizationUrl = new("https://example.com/api/oauth"),
                }
            }
        };

        private static OpenApiSecurityScheme OpenIdConnectSecurityScheme => new()
        {
            Description = "description1",
            Type = SecuritySchemeType.OpenIdConnect,
            Scheme = OpenApiConstants.Bearer,
            OpenIdConnectUrl = new("https://example.com/openIdConnect")
        };

        private static OpenApiSecuritySchemeReference OpenApiSecuritySchemeReference => new("sampleSecurityScheme");
        private static OpenApiSecurityScheme ReferencedSecurityScheme => new()
        {
            Description = "description1",
            Type = SecuritySchemeType.OpenIdConnect,
            Scheme = OpenApiConstants.Bearer,
            OpenIdConnectUrl = new("https://example.com/openIdConnect")
        };

        private static OpenApiSecurityScheme DeprecatedApiKeySecurityScheme => new()
        {
            Description = "description1",
            Name = "parameterName",
            Type = SecuritySchemeType.ApiKey,
            In = ParameterLocation.Query,
            Deprecated = true
        };

        [Fact]
        public async Task SerializeApiKeySecuritySchemeAsV3JsonWorks()
        {
            // Arrange
            var expected =
                """
                {
                  "type": "apiKey",
                  "description": "description1",
                  "name": "parameterName",
                  "in": "query"
                }
                """;

            // Act
            var actual = await ApiKeySecurityScheme.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task SerializeApiKeySecuritySchemeAsV3YamlWorks()
        {
            // Arrange
            var expected =
                """
                type: apiKey
                description: description1
                name: parameterName
                in: query
                """;

            // Act
            var actual = await ApiKeySecurityScheme.SerializeAsYamlAsync(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task SerializeHttpBasicSecuritySchemeAsV3JsonWorks()
        {
            // Arrange
            var expected =
                """
                {
                  "type": "http",
                  "description": "description1",
                  "scheme": "basic"
                }
                """;

            // Act
            var actual = await HttpBasicSecurityScheme.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task SerializeHttpBearerSecuritySchemeAsV3JsonWorks()
        {
            // Arrange
            var expected =
                """
                {
                  "type": "http",
                  "description": "description1",
                  "scheme": "bearer",
                  "bearerFormat": "JWT"
                }
                """;

            // Act
            var actual = await HttpBearerSecurityScheme.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task SerializeOAuthSingleFlowSecuritySchemeAsV3JsonWorks()
        {
            // Arrange
            var expected =
                """
                {
                  "type": "oauth2",
                  "description": "description1",
                  "flows": {
                    "implicit": {
                      "authorizationUrl": "https://example.com/api/oauth",
                      "scopes": {
                        "operation1:object1": "operation 1 on object 1",
                        "operation2:object2": "operation 2 on object 2"
                      }
                    }
                  }
                }
                """;

            // Act
            var actual = await OAuth2SingleFlowSecurityScheme.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task SerializeOAuthMultipleFlowSecuritySchemeAsV3JsonWorks()
        {
            // Arrange
            var expected =
                """
                {
                  "type": "oauth2",
                  "description": "description1",
                  "flows": {
                    "implicit": {
                      "authorizationUrl": "https://example.com/api/oauth",
                      "scopes": {
                        "operation1:object1": "operation 1 on object 1",
                        "operation2:object2": "operation 2 on object 2"
                      }
                    },
                    "clientCredentials": {
                      "tokenUrl": "https://example.com/api/token",
                      "refreshUrl": "https://example.com/api/refresh",
                      "scopes": {
                        "operation1:object1": "operation 1 on object 1",
                        "operation2:object2": "operation 2 on object 2"
                      }
                    },
                    "authorizationCode": {
                      "authorizationUrl": "https://example.com/api/oauth",
                      "tokenUrl": "https://example.com/api/token",
                      "scopes": {
                        "operation1:object1": "operation 1 on object 1",
                        "operation2:object2": "operation 2 on object 2"
                      }
                    }
                  }
                }
                """;

            // Act
            var actual = await OAuth2MultipleFlowSecurityScheme.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task SerializeOpenIdConnectSecuritySchemeAsV3JsonWorks()
        {
            // Arrange
            var expected =
                """
                {
                  "type": "openIdConnect",
                  "description": "description1",
                  "openIdConnectUrl": "https://example.com/openIdConnect"
                }
                """;

            // Act
            var actual = await OpenIdConnectSecurityScheme.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task SerializeReferencedSecuritySchemeAsV3JsonWorksAsync(bool produceTerseOutput)
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new() { Terse = produceTerseOutput });

            // Act
            OpenApiSecuritySchemeReference.SerializeAsV3(writer);
            await writer.FlushAsync();

            // Assert
            await Verifier.Verify(outputStringWriter).UseParameters(produceTerseOutput);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task SerializeReferencedSecuritySchemeAsV3JsonWithoutReferenceWorksAsync(bool produceTerseOutput)
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new() { Terse = produceTerseOutput });

            // Act
            ReferencedSecurityScheme.SerializeAsV3(writer);
            await writer.FlushAsync();

            // Assert
            await Verifier.Verify(outputStringWriter).UseParameters(produceTerseOutput);
        }

        [Fact]
        public async Task SerializeDeprecatedSecuritySchemeAsV32JsonWorks()
        {
            // Arrange
            var expected =
                """
                {
                  "type": "apiKey",
                  "description": "description1",
                  "name": "parameterName",
                  "in": "query",
                  "deprecated": true
                }
                """;

            // Act
            var actual = await DeprecatedApiKeySecurityScheme.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_2);

            // Assert
            Assert.True(JsonNode.DeepEquals(JsonNode.Parse(expected), JsonNode.Parse(actual)));
        }

        [Fact]
        public async Task SerializeDeprecatedSecuritySchemeAsV31JsonWorks()
        {
            // Arrange
            var expected =
                """
                {
                  "type": "apiKey",
                  "description": "description1",
                  "name": "parameterName",
                  "in": "query",
                  "x-oai-deprecated": true
                }
                """;

            // Act
            var actual = await DeprecatedApiKeySecurityScheme.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_1);

            // Assert
            Assert.True(JsonNode.DeepEquals(JsonNode.Parse(expected), JsonNode.Parse(actual)));
        }

        [Fact]
        public async Task SerializeDeprecatedSecuritySchemeAsV3JsonWorks()
        {
            // Arrange
            var expected =
                """
                {
                  "type": "apiKey",
                  "description": "description1",
                  "name": "parameterName",
                  "in": "query",
                  "x-oai-deprecated": true
                }
                """;

            // Act
            var actual = await DeprecatedApiKeySecurityScheme.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            Assert.True(JsonNode.DeepEquals(JsonNode.Parse(expected), JsonNode.Parse(actual)));
        }
    }
}
