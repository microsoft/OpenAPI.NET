// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.IO;
using FluentAssertions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Reader;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.V3Tests
{
    [Collection("DefaultSettings")]
    public class OpenApiSecuritySchemeTests
    {
        private const string SampleFolderPath = "V3Tests/Samples/OpenApiSecurityScheme/";
        public OpenApiSecuritySchemeTests()
        {
            OpenApiReaderRegistry.RegisterReader("yaml", new OpenApiYamlReader());
        }

        [Fact]
        public void ParseHttpSecuritySchemeShouldSucceed()
        {
            // Act
            var securityScheme = OpenApiModelFactory.Load<OpenApiSecurityScheme>(Path.Combine(SampleFolderPath, "httpSecurityScheme.yaml"), OpenApiSpecVersion.OpenApi3_0, out _);

            // Assert
            securityScheme.Should().BeEquivalentTo(
                new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = OpenApiConstants.Basic
                });
        }

        [Fact]
        public void ParseApiKeySecuritySchemeShouldSucceed()
        {
            // Act
            var securityScheme = OpenApiModelFactory.Load<OpenApiSecurityScheme>(Path.Combine(SampleFolderPath, "apiKeySecurityScheme.yaml"), OpenApiSpecVersion.OpenApi3_0, out _);

            // Assert
            securityScheme.Should().BeEquivalentTo(
                new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.ApiKey,
                    Name = "api_key",
                    In = ParameterLocation.Header
                });
        }

        [Fact]
        public void ParseBearerSecuritySchemeShouldSucceed()
        {
            // Act
            var securityScheme = OpenApiModelFactory.Load<OpenApiSecurityScheme>(Path.Combine(SampleFolderPath, "bearerSecurityScheme.yaml"), OpenApiSpecVersion.OpenApi3_0, out _);

            // Assert
            securityScheme.Should().BeEquivalentTo(
                new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = OpenApiConstants.Bearer,
                    BearerFormat = OpenApiConstants.Jwt
                });
        }

        [Fact]
        public void ParseOAuth2SecuritySchemeShouldSucceed()
        {
            // Act
            var securityScheme = OpenApiModelFactory.Load<OpenApiSecurityScheme>(Path.Combine(SampleFolderPath, "oauth2SecurityScheme.yaml"), OpenApiSpecVersion.OpenApi3_0, out _);

            // Assert
            securityScheme.Should().BeEquivalentTo(
                new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        Implicit = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri("https://example.com/api/oauth/dialog"),
                            Scopes =
                            {
                                    ["write:pets"] = "modify pets in your account",
                                    ["read:pets"] = "read your pets"
                            }
                        }
                    }
                });
        }

        [Fact]
        public void ParseOpenIdConnectSecuritySchemeShouldSucceed()
        {
            // Act
            var securityScheme = OpenApiModelFactory.Load<OpenApiSecurityScheme>(Path.Combine(SampleFolderPath, "openIdConnectSecurityScheme.yaml"), OpenApiSpecVersion.OpenApi3_0, out _);

            // Assert
            securityScheme.Should().BeEquivalentTo(
                new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OpenIdConnect,
                    Description = "Sample Description",
                    OpenIdConnectUrl = new Uri("http://www.example.com")
                });
        }
    }
}
