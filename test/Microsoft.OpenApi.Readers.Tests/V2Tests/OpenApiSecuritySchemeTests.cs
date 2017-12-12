﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.IO;
using System.Linq;
using FluentAssertions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers.ParseNodes;
using Microsoft.OpenApi.Readers.V2;
using SharpYaml.Serialization;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.V2Tests
{
    [Collection("DefaultSettings")]
    public class OpenApiSecuritySchemeTests
    {
        private const string SampleFolderPath = "V2Tests/Samples/OpenApiSecurityScheme";

        [Fact]
        public void ParseHttpSecuritySchemeShouldSucceed()
        {
            using (var stream = File.OpenRead(Path.Combine(SampleFolderPath, "basicSecurityScheme.yaml")))
            {
                var yamlStream = new YamlStream();
                yamlStream.Load(new StreamReader(stream));
                var yamlNode = yamlStream.Documents.First().RootNode;

                var context = new ParsingContext();
                var diagnostic = new OpenApiDiagnostic();

                var node = new MapNode(context, diagnostic, (YamlMappingNode)yamlNode);

                // Act
                var securityScheme = OpenApiV2Deserializer.LoadSecurityScheme(node);

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
            using (var stream = File.OpenRead(Path.Combine(SampleFolderPath, "apiKeySecurityScheme.yaml")))
            {
                var yamlStream = new YamlStream();
                yamlStream.Load(new StreamReader(stream));
                var yamlNode = yamlStream.Documents.First().RootNode;

                var context = new ParsingContext();
                var diagnostic = new OpenApiDiagnostic();

                var node = new MapNode(context, diagnostic, (YamlMappingNode)yamlNode);

                // Act
                var securityScheme = OpenApiV2Deserializer.LoadSecurityScheme(node);

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
        public void ParseOAuth2ImplicitSecuritySchemeShouldSucceed()
        {
            using (var stream = File.OpenRead(Path.Combine(SampleFolderPath, "oAuth2ImplicitSecurityScheme.yaml")))
            {
                var yamlStream = new YamlStream();
                yamlStream.Load(new StreamReader(stream));
                var yamlNode = yamlStream.Documents.First().RootNode;

                var context = new ParsingContext();
                var diagnostic = new OpenApiDiagnostic();

                var node = new MapNode(context, diagnostic, (YamlMappingNode)yamlNode);

                // Act
                var securityScheme = OpenApiV2Deserializer.LoadSecurityScheme(node);

                // Assert
                securityScheme.ShouldBeEquivalentTo(
                    new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.OAuth2,
                        Flows = new OpenApiOAuthFlows
                        {
                            Implicit = new OpenApiOAuthFlow
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
        }

        [Fact]
        public void ParseOAuth2PasswordSecuritySchemeShouldSucceed()
        {
            using (var stream = File.OpenRead(Path.Combine(SampleFolderPath, "oAuth2PasswordSecurityScheme.yaml")))
            {
                var yamlStream = new YamlStream();
                yamlStream.Load(new StreamReader(stream));
                var yamlNode = yamlStream.Documents.First().RootNode;

                var context = new ParsingContext();
                var diagnostic = new OpenApiDiagnostic();

                var node = new MapNode(context, diagnostic, (YamlMappingNode)yamlNode);

                // Act
                var securityScheme = OpenApiV2Deserializer.LoadSecurityScheme(node);

                // Assert
                securityScheme.ShouldBeEquivalentTo(
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
        }

        [Fact]
        public void ParseOAuth2ApplicationSecuritySchemeShouldSucceed()
        {
            using (var stream = File.OpenRead(Path.Combine(SampleFolderPath, "oAuth2ApplicationSecurityScheme.yaml")))
            {
                var yamlStream = new YamlStream();
                yamlStream.Load(new StreamReader(stream));
                var yamlNode = yamlStream.Documents.First().RootNode;

                var context = new ParsingContext();
                var diagnostic = new OpenApiDiagnostic();

                var node = new MapNode(context, diagnostic, (YamlMappingNode)yamlNode);

                // Act
                var securityScheme = OpenApiV2Deserializer.LoadSecurityScheme(node);

                // Assert
                securityScheme.ShouldBeEquivalentTo(
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
        }

        [Fact]
        public void ParseOAuth2AccessCodeSecuritySchemeShouldSucceed()
        {
            using (var stream = File.OpenRead(Path.Combine(SampleFolderPath, "oAuth2AccessCodeSecurityScheme.yaml")))
            {
                var yamlStream = new YamlStream();
                yamlStream.Load(new StreamReader(stream));
                var yamlNode = yamlStream.Documents.First().RootNode;

                var context = new ParsingContext();
                var diagnostic = new OpenApiDiagnostic();

                var node = new MapNode(context, diagnostic, (YamlMappingNode)yamlNode);

                // Act
                var securityScheme = OpenApiV2Deserializer.LoadSecurityScheme(node);

                // Assert
                securityScheme.ShouldBeEquivalentTo(
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
        }
    }
}