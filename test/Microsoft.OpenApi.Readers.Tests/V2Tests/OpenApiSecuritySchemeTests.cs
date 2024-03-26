// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.IO;
using System.Linq;
using FluentAssertions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Reader;
using Microsoft.OpenApi.Reader.ParseNodes;
using Microsoft.OpenApi.Reader.V2;
using SharpYaml.Serialization;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.V2Tests
{
    [Collection("DefaultSettings")]
    public class OpenApiSecuritySchemeTests
    {
        private const string SampleFolderPath = "V2Tests/Samples/OpenApiSecurityScheme/";

        [Fact]
        public void ParseHttpSecuritySchemeShouldSucceed()
        {
            // Arrange
            using var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "basicSecurityScheme.yaml"));
            var document = LoadYamlDocument(stream);

            var diagnostic = new OpenApiDiagnostic();
            var context = new ParsingContext(diagnostic);

            var asJsonNode = document.RootNode.ToJsonNode();
            var node = new MapNode(context, asJsonNode);

            // Act
            var securityScheme = OpenApiV2Deserializer.LoadSecurityScheme(node);

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
            // Arrange
            using var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "apiKeySecurityScheme.yaml"));
            var document = LoadYamlDocument(stream);
            var diagnostic = new OpenApiDiagnostic();
            var context = new ParsingContext(diagnostic);

            var asJsonNode = document.RootNode.ToJsonNode();

            var node = new MapNode(context, asJsonNode);

            // Act
            var securityScheme = OpenApiV2Deserializer.LoadSecurityScheme(node);

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
        public void ParseOAuth2ImplicitSecuritySchemeShouldSucceed()
        {
            using var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "oauth2ImplicitSecurityScheme.yaml"));
            var document = LoadYamlDocument(stream);
            var diagnostic = new OpenApiDiagnostic();
            var context = new ParsingContext(diagnostic);

                var asJsonNode = document.RootNode.ToJsonNode();

                var node = new MapNode(context, asJsonNode);

                // Act
                var securityScheme = OpenApiV2Deserializer.LoadSecurityScheme(node);

            // Assert
            securityScheme.Should().BeEquivalentTo(
                new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new()
                    {
                        Implicit = new()
                        {
                            AuthorizationUrl = new("http://swagger.io/api/oauth/dialog"),
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
        public void ParseOAuth2PasswordSecuritySchemeShouldSucceed()
        {
            // Arrange
            using var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "oauth2PasswordSecurityScheme.yaml"));
            var document = LoadYamlDocument(stream);
            var diagnostic = new OpenApiDiagnostic();
            var context = new ParsingContext(diagnostic);

            var asJsonNode = document.RootNode.ToJsonNode();
            var node = new MapNode(context, asJsonNode);

            // Act
            var securityScheme = OpenApiV2Deserializer.LoadSecurityScheme(node);

            // Assert
            securityScheme.Should().BeEquivalentTo(
                new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        Password = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri("http://swagger.io/api/oauth/dialog"),
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
        public void ParseOAuth2ApplicationSecuritySchemeShouldSucceed()
        {
            // Arrange
            using var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "oauth2ApplicationSecurityScheme.yaml"));
            var document = LoadYamlDocument(stream);
            var diagnostic = new OpenApiDiagnostic();
            var context = new ParsingContext(diagnostic);

            var asJsonNode = document.RootNode.ToJsonNode();
            var node = new MapNode(context, asJsonNode);

            // Act
            var securityScheme = OpenApiV2Deserializer.LoadSecurityScheme(node);

            // Assert
            securityScheme.Should().BeEquivalentTo(
                new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        ClientCredentials = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri("http://swagger.io/api/oauth/dialog"),
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
        public void ParseOAuth2AccessCodeSecuritySchemeShouldSucceed()
        {
            // Arrange
            using var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "oauth2AccessCodeSecurityScheme.yaml"));
            var document = LoadYamlDocument(stream);

            var diagnostic = new OpenApiDiagnostic();
            var context = new ParsingContext(diagnostic);

            var asJsonNode = document.RootNode.ToJsonNode();
            var node = new MapNode(context, asJsonNode);

            // Act
            var securityScheme = OpenApiV2Deserializer.LoadSecurityScheme(node);

            // Assert
            securityScheme.Should().BeEquivalentTo(
                new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        AuthorizationCode = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri("http://swagger.io/api/oauth/dialog"),
                            Scopes =
                            {
                                    ["write:pets"] = "modify pets in your account",
                                    ["read:pets"] = "read your pets"
                            }
                        }
                    }
                });
        }

        static YamlDocument LoadYamlDocument(Stream input)
        {
            using var reader = new StreamReader(input);
            var yamlStream = new YamlStream();
            yamlStream.Load(reader);
            return yamlStream.Documents.First();
        }
    }
}
