// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;
using System.IO;
using FluentAssertions;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.OpenApi.Tests.Models
{
    [Collection("DefaultSettings")]
    public class OpenApiSecuritySchemeTests
    {
        public static OpenApiSecurityScheme ApiKeySecurityScheme = new OpenApiSecurityScheme
        {
            Description = "description1",
            Name = "parameterName",
            Type = SecuritySchemeType.ApiKey,
            In = ParameterLocation.Query,
        };

        public static OpenApiSecurityScheme HttpBasicSecurityScheme = new OpenApiSecurityScheme
        {
            Description = "description1",
            Type = SecuritySchemeType.Http,
            Scheme = "basic",
        };

        public static OpenApiSecurityScheme HttpBearerSecurityScheme = new OpenApiSecurityScheme
        {
            Description = "description1",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
        };

        public static OpenApiSecurityScheme OAuth2SingleFlowSecurityScheme = new OpenApiSecurityScheme
        {
            Description = "description1",
            Type = SecuritySchemeType.OAuth2,
            Flows = new OpenApiOAuthFlows
            {
                Implicit = new OpenApiOAuthFlow
                {
                    Scopes = new Dictionary<string, string>
                    {
                        ["operation1:object1"] = "operation 1 on object 1",
                        ["operation2:object2"] = "operation 2 on object 2"
                    },
                    AuthorizationUrl = new Uri("https://example.com/api/oauth")
                }
            }
        };

        public static OpenApiSecurityScheme OAuth2MultipleFlowSecurityScheme = new OpenApiSecurityScheme
        {
            Description = "description1",
            Type = SecuritySchemeType.OAuth2,
            Flows = new OpenApiOAuthFlows
            {
                Implicit = new OpenApiOAuthFlow
                {
                    Scopes = new Dictionary<string, string>
                    {
                        ["operation1:object1"] = "operation 1 on object 1",
                        ["operation2:object2"] = "operation 2 on object 2"
                    },
                    AuthorizationUrl = new Uri("https://example.com/api/oauth")
                },
                ClientCredentials = new OpenApiOAuthFlow
                {
                    Scopes = new Dictionary<string, string>
                    {
                        ["operation1:object1"] = "operation 1 on object 1",
                        ["operation2:object2"] = "operation 2 on object 2"
                    },
                    TokenUrl = new Uri("https://example.com/api/token"),
                    RefreshUrl = new Uri("https://example.com/api/refresh"),
                },
                AuthorizationCode = new OpenApiOAuthFlow
                {
                    Scopes = new Dictionary<string, string>
                    {
                        ["operation1:object1"] = "operation 1 on object 1",
                        ["operation2:object2"] = "operation 2 on object 2"
                    },
                    TokenUrl = new Uri("https://example.com/api/token"),
                    AuthorizationUrl = new Uri("https://example.com/api/oauth"),
                }
            }
        };

        public static OpenApiSecurityScheme OpenIdConnectSecurityScheme = new OpenApiSecurityScheme
        {
            Description = "description1",
            Type = SecuritySchemeType.OpenIdConnect,
            Scheme = "openIdConnectUrl",
            OpenIdConnectUrl = new Uri("https://example.com/openIdConnect")
        };

        public static OpenApiSecurityScheme ReferencedSecurityScheme = new OpenApiSecurityScheme
        {
            Description = "description1",
            Type = SecuritySchemeType.OpenIdConnect,
            Scheme = "openIdConnectUrl",
            OpenIdConnectUrl = new Uri("https://example.com/openIdConnect"),
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = "sampleSecurityScheme"
            }
        };

        private readonly ITestOutputHelper _output;

        public OpenApiSecuritySchemeTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void SerializeApiKeySecuritySchemeAsV3JsonWorks()
        {
            // Arrange
            var expected =
                @"{
  ""type"": ""apiKey"",
  ""description"": ""description1"",
  ""name"": ""parameterName"",
  ""in"": ""query""
}";

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
                @"type: apiKey
description: description1
name: parameterName
in: query";

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
                @"{
  ""type"": ""http"",
  ""description"": ""description1"",
  ""scheme"": ""basic""
}";

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
                @"{
  ""type"": ""http"",
  ""description"": ""description1"",
  ""scheme"": ""bearer"",
  ""bearerFormat"": ""JWT""
}";

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
                @"{
  ""type"": ""oauth2"",
  ""description"": ""description1"",
  ""flows"": {
    ""implicit"": {
      ""authorizationUrl"": ""https://example.com/api/oauth"",
      ""scopes"": {
        ""operation1:object1"": ""operation 1 on object 1"",
        ""operation2:object2"": ""operation 2 on object 2""
      }
    }
  }
}";

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
                @"{
  ""type"": ""oauth2"",
  ""description"": ""description1"",
  ""flows"": {
    ""implicit"": {
      ""authorizationUrl"": ""https://example.com/api/oauth"",
      ""scopes"": {
        ""operation1:object1"": ""operation 1 on object 1"",
        ""operation2:object2"": ""operation 2 on object 2""
      }
    },
    ""clientCredentials"": {
      ""tokenUrl"": ""https://example.com/api/token"",
      ""refreshUrl"": ""https://example.com/api/refresh"",
      ""scopes"": {
        ""operation1:object1"": ""operation 1 on object 1"",
        ""operation2:object2"": ""operation 2 on object 2""
      }
    },
    ""authorizationCode"": {
      ""authorizationUrl"": ""https://example.com/api/oauth"",
      ""tokenUrl"": ""https://example.com/api/token"",
      ""scopes"": {
        ""operation1:object1"": ""operation 1 on object 1"",
        ""operation2:object2"": ""operation 2 on object 2""
      }
    }
  }
}";

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
                @"{
  ""type"": ""openIdConnect"",
  ""description"": ""description1"",
  ""openIdConnectUrl"": ""https://example.com/openIdConnect""
}";

            // Act
            var actual = OpenIdConnectSecurityScheme.SerializeAsJson(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeReferencedSecuritySchemeAsV3JsonWorks()
        {
            // Arrange
            var outputStringWriter = new StringWriter();
            var writer = new OpenApiJsonWriter(outputStringWriter);
            var expected =
                @"{
  ""sampleSecurityScheme"": null
}";

            // Act
            // Add dummy start object, value, and end object to allow SerializeAsV3 to output security scheme 
            // as property name.
            writer.WriteStartObject();
            ReferencedSecurityScheme.SerializeAsV3(writer);
            writer.WriteNull();
            writer.WriteEndObject();
            writer.Flush();
            var actual = outputStringWriter.GetStringBuilder().ToString();

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeReferencedSecuritySchemeAsV3JsonWithoutReferenceWorks()
        {
            // Arrange
            var outputStringWriter = new StringWriter();
            var writer = new OpenApiJsonWriter(outputStringWriter);
            var expected =
                @"{
  ""type"": ""openIdConnect"",
  ""description"": ""description1"",
  ""openIdConnectUrl"": ""https://example.com/openIdConnect""
}";

            // Act
            ReferencedSecurityScheme.SerializeAsV3WithoutReference(writer);
            writer.Flush();
            var actual = outputStringWriter.GetStringBuilder().ToString();

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }
    }
}