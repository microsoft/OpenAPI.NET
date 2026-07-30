// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.IO;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Microsoft.OpenApi.Reader;
using Microsoft.OpenApi.Reader.V3;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.V3Tests
{
    [Collection("DefaultSettings")]
    public class OpenApiSecuritySchemeTests
    {
        private const string SampleFolderPath = "V3Tests/Samples/OpenApiSecurityScheme/";
        [Fact]
        public async Task ParseHttpSecuritySchemeShouldSucceed()
        {
            // Act
            var securityScheme = await OpenApiModelFactory.LoadAsync<OpenApiSecurityScheme>(Path.Combine(SampleFolderPath, "httpSecurityScheme.yaml"), OpenApiSpecVersion.OpenApi3_0, new(), SettingsFixture.ReaderSettings);

            // Assert
            Assert.Equivalent(
                new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = OpenApiConstants.Basic
                }, securityScheme);
        }

        [Fact]
        public async Task ParseApiKeySecuritySchemeShouldSucceed()
        {
            // Act
            var securityScheme = await OpenApiModelFactory.LoadAsync<OpenApiSecurityScheme>(Path.Combine(SampleFolderPath, "apiKeySecurityScheme.yaml"), OpenApiSpecVersion.OpenApi3_0, new(), SettingsFixture.ReaderSettings);

            // Assert
            Assert.Equivalent(
                new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.ApiKey,
                    Name = "api_key",
                    In = ParameterLocation.Header
                }, securityScheme);
        }

        [Fact]
        public async Task ParseBearerSecuritySchemeShouldSucceed()
        {
            // Act
            var securityScheme = await OpenApiModelFactory.LoadAsync<OpenApiSecurityScheme>(Path.Combine(SampleFolderPath, "bearerSecurityScheme.yaml"), OpenApiSpecVersion.OpenApi3_0, new(), SettingsFixture.ReaderSettings);

            // Assert
            Assert.Equivalent(
                new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = OpenApiConstants.Bearer,
                    BearerFormat = OpenApiConstants.Jwt
                }, securityScheme);
        }

        [Fact]
        public async Task ParseOAuth2SecuritySchemeShouldSucceed()
        {
            // Act
            var securityScheme = await OpenApiModelFactory.LoadAsync<OpenApiSecurityScheme>(Path.Combine(SampleFolderPath, "oauth2SecurityScheme.yaml"), OpenApiSpecVersion.OpenApi3_0, new(), SettingsFixture.ReaderSettings);

            // Assert
            Assert.Equivalent(
                new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        Implicit = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri("https://example.com/api/oauth/dialog"),
                            Scopes = new System.Collections.Generic.Dictionary<string, string>
                            {
                                    ["write:pets"] = "modify pets in your account",
                                    ["read:pets"] = "read your pets"
                            }
                        }
                    }
                }, securityScheme);
        }

        [Fact]
        public async Task ParseOpenIdConnectSecuritySchemeShouldSucceed()
        {
            // Act
            var securityScheme = await OpenApiModelFactory.LoadAsync<OpenApiSecurityScheme>(Path.Combine(SampleFolderPath, "openIdConnectSecurityScheme.yaml"), OpenApiSpecVersion.OpenApi3_0, new(), SettingsFixture.ReaderSettings);

            // Assert
            Assert.Equivalent(
                new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OpenIdConnect,
                    Description = "Sample Description",
                    OpenIdConnectUrl = new Uri("http://www.example.com")
                }, securityScheme);
        }

        [Fact]
        public async Task ParseOAuth2SecuritySchemeWithDeviceAuthorizationUrlShouldSucceed()
        {
            // Act
            var securityScheme = await OpenApiModelFactory.LoadAsync<OpenApiSecurityScheme>(
                Path.Combine(SampleFolderPath, "oauth2SecuritySchemeWithDeviceUrl.yaml"),
                OpenApiSpecVersion.OpenApi3_0,
                new(),
                SettingsFixture.ReaderSettings);

            // Assert
            Assert.NotNull(securityScheme);
            Assert.Equal(SecuritySchemeType.OAuth2, securityScheme.Type);
            Assert.NotNull(securityScheme.Flows?.AuthorizationCode);
            Assert.Equal(new Uri("https://example.com/api/oauth/dialog"), securityScheme.Flows.AuthorizationCode.AuthorizationUrl);
            Assert.Equal(new Uri("https://example.com/api/oauth/token"), securityScheme.Flows.AuthorizationCode.TokenUrl);
            Assert.Equal(new Uri("https://example.com/api/oauth/device"), securityScheme.Flows.AuthorizationCode.DeviceAuthorizationUrl);
            Assert.NotNull(securityScheme.Flows.AuthorizationCode.Scopes);
            Assert.Equal(2, securityScheme.Flows.AuthorizationCode.Scopes.Count);
        }

        [Fact]
        public void ParseOAuth2SecuritySchemeWithMetadataUrlExtensionShouldSucceed()
        {
            var json =
                """
                {
                  "type": "oauth2",
                  "x-oai-oauth2-metadata-url": "https://idp.example.com/.well-known/oauth-authorization-server",
                  "flows": {
                    "clientCredentials": {
                      "tokenUrl": "https://idp.example.com/oauth/token",
                      "scopes": {
                        "scope:one": "Scope one"
                      }
                    }
                  }
                }
                """;

            var securityScheme = Assert.IsType<OpenApiSecurityScheme>(
                OpenApiV3Deserializer.LoadSecurityScheme(JsonNode.Parse(json)!, new(), new ParsingContext(new())));

            Assert.Equal(new Uri("https://idp.example.com/.well-known/oauth-authorization-server"), securityScheme.OAuth2MetadataUrl);
            Assert.True(securityScheme.Extensions is null || !securityScheme.Extensions.ContainsKey(OpenApiConstants.OAuth2MetadataUrlExtension));
        }
    }
}
