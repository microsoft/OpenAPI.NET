// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.IO;
using System.Linq;
using FluentAssertions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers.ParseNodes;
using Microsoft.OpenApi.Readers.V3;
using SharpYaml.Serialization;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.V3Tests
{
    [Collection("DefaultSettings")]
    public class OpenApiSecuritySchemeTests
    {
        private const string SampleFolderPath = "V3Tests/Samples/OpenApiSecurityScheme/";

        [Fact]
        public void ParseHttpSecuritySchemeShouldSucceed()
        {
            using (var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "httpSecurityScheme.yaml")))
            {
                var yamlStream = new YamlStream();
                yamlStream.Load(new StreamReader(stream));
                var yamlNode = yamlStream.Documents.First().RootNode;

                var context = new ParsingContext();
                var diagnostic = new OpenApiDiagnostic();

                var node = new MapNode(context, diagnostic, (YamlMappingNode)yamlNode);

                // Act
                var securityScheme = OpenApiV3Deserializer.LoadSecurityScheme(node);

                // Assert
                securityScheme.ShouldBeEquivalentTo(
                    new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.Http,
                        Scheme = "basic"
                    });
            }
        }

        [Fact]
        public void ParseApiKeySecuritySchemeShouldSucceed()
        {
            using (var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "apiKeySecurityScheme.yaml")))
            {
                var yamlStream = new YamlStream();
                yamlStream.Load(new StreamReader(stream));
                var yamlNode = yamlStream.Documents.First().RootNode;

                var context = new ParsingContext();
                var diagnostic = new OpenApiDiagnostic();

                var node = new MapNode(context, diagnostic, (YamlMappingNode)yamlNode);

                // Act
                var securityScheme = OpenApiV3Deserializer.LoadSecurityScheme(node);

                // Assert
                securityScheme.ShouldBeEquivalentTo(
                    new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.ApiKey,
                        Name = "api_key",
                        In = ParameterLocation.Header
                    });
            }
        }

        [Fact]
        public void ParseBearerSecuritySchemeShouldSucceed()
        {
            using (var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "bearerSecurityScheme.yaml")))
            {
                var yamlStream = new YamlStream();
                yamlStream.Load(new StreamReader(stream));
                var yamlNode = yamlStream.Documents.First().RootNode;

                var context = new ParsingContext();
                var diagnostic = new OpenApiDiagnostic();

                var node = new MapNode(context, diagnostic, (YamlMappingNode)yamlNode);

                // Act
                var securityScheme = OpenApiV3Deserializer.LoadSecurityScheme(node);

                // Assert
                securityScheme.ShouldBeEquivalentTo(
                    new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.Http,
                        Scheme = "bearer",
                        BearerFormat = "JWT"
                    });
            }
        }

        [Fact]
        public void ParseOAuth2SecuritySchemeShouldSucceed()
        {
            using (var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "oauth2SecurityScheme.yaml")))
            {
                var yamlStream = new YamlStream();
                yamlStream.Load(new StreamReader(stream));
                var yamlNode = yamlStream.Documents.First().RootNode;

                var context = new ParsingContext();
                var diagnostic = new OpenApiDiagnostic();

                var node = new MapNode(context, diagnostic, (YamlMappingNode)yamlNode);

                // Act
                var securityScheme = OpenApiV3Deserializer.LoadSecurityScheme(node);

                // Assert
                securityScheme.ShouldBeEquivalentTo(
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
        }

        [Fact]
        public void ParseOpenIdConnectSecuritySchemeShouldSucceed()
        {
            using (var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "openIdConnectSecurityScheme.yaml")))
            {
                var yamlStream = new YamlStream();
                yamlStream.Load(new StreamReader(stream));
                var yamlNode = yamlStream.Documents.First().RootNode;

                var context = new ParsingContext();
                var diagnostic = new OpenApiDiagnostic();

                var node = new MapNode(context, diagnostic, (YamlMappingNode)yamlNode);

                // Act
                var securityScheme = OpenApiV3Deserializer.LoadSecurityScheme(node);

                // Assert
                securityScheme.ShouldBeEquivalentTo(
                    new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.OpenIdConnect,
                        Description = "Sample Description",
                        OpenIdConnectUrl = new Uri("http://www.example.com")
                    });
            }
        }
    }
}