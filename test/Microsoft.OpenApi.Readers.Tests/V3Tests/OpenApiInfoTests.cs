// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.IO;
using System.Linq;
using FluentAssertions;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers.ParseNodes;
using Microsoft.OpenApi.Readers.V3;
using SharpYaml.Serialization;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.V3Tests
{
    [Collection("DefaultSettings")]
    public class OpenApiInfoTests
    {
        private const string SampleFolderPath = "V3Tests/Samples/OpenApiInfo/";

        [Fact]
        public void ParseAdvancedInfoShouldSucceed()
        {
            using (var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "advancedInfo.yaml")))
            {
                var yamlStream = new YamlStream();
                yamlStream.Load(new StreamReader(stream));
                var yamlNode = yamlStream.Documents.First().RootNode;

                var context = new ParsingContext();
                var diagnostic = new OpenApiDiagnostic();

                var node = new MapNode(context, diagnostic, (YamlMappingNode)yamlNode);

                // Act
                var openApiInfo = OpenApiV3Deserializer.LoadInfo(node);

                // Assert
                openApiInfo.ShouldBeEquivalentTo(
                    new OpenApiInfo
                    {
                        Title = "Advanced Info",
                        Description = "Sample Description",
                        Version = "1.0.0",
                        TermsOfService = new Uri("http://example.org/termsOfService"),
                        Contact = new OpenApiContact
                        {
                            Email = "example@example.com",
                            Extensions =
                            {
                                ["x-twitter"] = new OpenApiString("@exampleTwitterHandler")
                            },
                            Name = "John Doe",
                            Url = new Uri("http://www.example.com/url1")
                        },
                        License = new OpenApiLicense
                        {
                            Extensions = {["x-disclaimer"] = new OpenApiString("Sample Extension String Disclaimer")},
                            Name = "licenseName",
                            Url = new Uri("http://www.example.com/url2")
                        },
                        Extensions =
                        {
                            ["x-something"] = new OpenApiString("Sample Extension String Something"),
                            ["x-contact"] = new OpenApiObject
                            {
                                ["name"] = new OpenApiString("John Doe"),
                                ["url"] = new OpenApiString("http://www.example.com/url3"),
                                ["email"] = new OpenApiString("example@example.com")
                            },
                            ["x-list"] = new OpenApiArray
                            {
                                new OpenApiInteger(1),
                                new OpenApiInteger(2)
                            }
                        }
                    });
            }
        }

        [Fact]
        public void ParseBasicInfoShouldSucceed()
        {
            using (var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "basicInfo.yaml")))
            {
                var yamlStream = new YamlStream();
                yamlStream.Load(new StreamReader(stream));
                var yamlNode = yamlStream.Documents.First().RootNode;

                var context = new ParsingContext();
                var diagnostic = new OpenApiDiagnostic();

                var node = new MapNode(context, diagnostic, (YamlMappingNode)yamlNode);

                // Act
                var openApiInfo = OpenApiV3Deserializer.LoadInfo(node);

                // Assert
                openApiInfo.ShouldBeEquivalentTo(
                    new OpenApiInfo
                    {
                        Title = "Basic Info",
                        Description = "Sample Description",
                        Version = "1.0.1",
                        TermsOfService = new Uri("http://swagger.io/terms/"),
                        Contact = new OpenApiContact
                        {
                            Email = "support@swagger.io",
                            Name = "API Support",
                            Url = new Uri("http://www.swagger.io/support")
                        },
                        License = new OpenApiLicense
                        {
                            Name = "Apache 2.0",
                            Url = new Uri("http://www.apache.org/licenses/LICENSE-2.0.html")
                        }
                    });
            }
        }

        [Fact]
        public void ParseMinimalInfoShouldSucceed()
        {
            using (var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "minimalInfo.yaml")))
            {
                var yamlStream = new YamlStream();
                yamlStream.Load(new StreamReader(stream));
                var yamlNode = yamlStream.Documents.First().RootNode;

                var context = new ParsingContext();
                var diagnostic = new OpenApiDiagnostic();

                var node = new MapNode(context, diagnostic, (YamlMappingNode)yamlNode);

                // Act
                var openApiInfo = OpenApiV3Deserializer.LoadInfo(node);

                // Assert
                openApiInfo.ShouldBeEquivalentTo(
                    new OpenApiInfo
                    {
                        Title = "Minimal Info",
                        Version = "1.0.1"
                    });
            }
        }
    }
}