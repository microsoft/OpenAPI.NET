// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Models.References;
using Microsoft.OpenApi.Writers;
using VerifyXunit;
using Xunit;

namespace Microsoft.OpenApi.Tests.Models
{
    [Collection("DefaultSettings")]
    public class OpenApiSecuritySchemeTests
    {
        public static OpenApiSecurityScheme ApiKeySecurityScheme = new()
        {
            Description = "description1",
            Name = "parameterName",
            Type = SecuritySchemeType.ApiKey,
            In = ParameterLocation.Query,
        };

        public static OpenApiSecurityScheme HttpBasicSecurityScheme = new()
        {
            Description = "description1",
            Type = SecuritySchemeType.Http,
            Scheme = OpenApiConstants.Basic
        };

        public static OpenApiSecurityScheme HttpBearerSecurityScheme = new()
        {
            Description = "description1",
            Type = SecuritySchemeType.Http,
            Scheme = OpenApiConstants.Bearer,
            BearerFormat = OpenApiConstants.Jwt
        };

        public static OpenApiSecurityScheme OAuth2SingleFlowSecurityScheme = new()
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

        public static OpenApiSecurityScheme OAuth2MultipleFlowSecurityScheme = new()
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

        public static OpenApiSecurityScheme OpenIdConnectSecurityScheme = new()
        {
            Description = "description1",
            Type = SecuritySchemeType.OpenIdConnect,
            Scheme = OpenApiConstants.Bearer,
            OpenIdConnectUrl = new("https://example.com/openIdConnect")
        };

        public static OpenApiSecuritySchemeReference OpenApiSecuritySchemeReference = new(target: ReferencedSecurityScheme, referenceId: "sampleSecurityScheme");
        public static OpenApiSecurityScheme ReferencedSecurityScheme = new()
        {
            Description = "description1",
            Type = SecuritySchemeType.OpenIdConnect,
            Scheme = OpenApiConstants.Bearer,
            OpenIdConnectUrl = new("https://example.com/openIdConnect")
        };

        [Fact]
        public void SerializeApiKeySecuritySchemeAsV3JsonWorks()
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
            var actual = ApiKeySecurityScheme.SerializeAsJson(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeApiKeySecuritySchemeAsV3YamlWorks()
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
            var actual = ApiKeySecurityScheme.SerializeAsYaml(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeHttpBasicSecuritySchemeAsV3JsonWorks()
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
            var actual = HttpBasicSecurityScheme.SerializeAsJson(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeHttpBearerSecuritySchemeAsV3JsonWorks()
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
            var actual = HttpBearerSecurityScheme.SerializeAsJson(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeOAuthSingleFlowSecuritySchemeAsV3JsonWorks()
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
            var actual = OAuth2SingleFlowSecurityScheme.SerializeAsJson(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeOAuthMultipleFlowSecuritySchemeAsV3JsonWorks()
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
            var actual = OAuth2MultipleFlowSecurityScheme.SerializeAsJson(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeOpenIdConnectSecuritySchemeAsV3JsonWorks()
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
            var actual = OpenIdConnectSecurityScheme.SerializeAsJson(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
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
            writer.Flush();

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
            writer.Flush();

            // Assert
            await Verifier.Verify(outputStringWriter).UseParameters(produceTerseOutput);
        }
    }
}
