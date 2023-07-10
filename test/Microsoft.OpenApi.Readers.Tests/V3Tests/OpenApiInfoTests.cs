﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.IO;
using System.Linq;
using System.Text.Json.Nodes;
using System.Xml.Linq;
using FluentAssertions;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Extensions;
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
            // Arrange
            using var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "advancedInfo.yaml"));
            var yamlStream = new YamlStream();
            yamlStream.Load(new StreamReader(stream));
            var yamlNode = yamlStream.Documents.First().RootNode;

            var diagnostic = new OpenApiDiagnostic();
            var context = new ParsingContext(diagnostic);

            var asJsonNode = yamlNode.ToJsonNode();
            var node = new MapNode(context, asJsonNode);

            // Act
            var openApiInfo = OpenApiV3Deserializer.LoadInfo(node);

            // Assert
            openApiInfo.Should().BeEquivalentTo(
                new OpenApiInfo
                {
                    Title = "Advanced Info",
                    Summary = "Sample Summary",
                    Description = "Sample Description",
                    Version = "1.0.0",
                    TermsOfService = new Uri("http://example.org/termsOfService"),
                    Contact = new OpenApiContact
                    {
                        Email = "example@example.com",
                        Extensions =
                        {
                                ["x-twitter"] = new OpenApiAny("@exampleTwitterHandler")
                        },
                        Name = "John Doe",
                        Url = new Uri("http://www.example.com/url1")
                    },
                    License = new OpenApiLicense
                    {
                        Extensions = { ["x-disclaimer"] = new OpenApiAny("Sample Extension String Disclaimer") },
                        Name = "licenseName",
                        Url = new Uri("http://www.example.com/url2")
                    },
                    Extensions =
                    {
                            ["x-something"] = new OpenApiAny("Sample Extension String Something"),
                            ["x-contact"] = new OpenApiAny(new JsonObject()
                            {
                                ["name"] = "John Doe",
                                ["url"] = "http://www.example.com/url3",
                                ["email"] = "example@example.com"
                            }),
                            ["x-list"] = new OpenApiAny (new JsonArray { "1", "2" })
                    }
                }, options => options.IgnoringCyclicReferences()
                .Excluding(i => ((OpenApiAny)i.Contact.Extensions["x-twitter"]).Node.Parent)
                .Excluding(i => ((OpenApiAny)i.License.Extensions["x-disclaimer"]).Node.Parent)
                .Excluding(i => ((OpenApiAny)i.Extensions["x-something"]).Node.Parent)
                .Excluding(i => ((OpenApiAny)i.Extensions["x-contact"]).Node["name"].Parent)
                .Excluding(i => ((OpenApiAny)i.Extensions["x-contact"]).Node["name"].Root)
                .Excluding(i => ((OpenApiAny)i.Extensions["x-contact"]).Node["url"].Parent)
                .Excluding(i => ((OpenApiAny)i.Extensions["x-contact"]).Node["url"].Root)
                .Excluding(i => ((OpenApiAny)i.Extensions["x-contact"]).Node["email"].Parent)
                .Excluding(i => ((OpenApiAny)i.Extensions["x-contact"]).Node["email"].Root)
                .Excluding(i => ((OpenApiAny)i.Extensions["x-list"]).Node[0].Parent)
                .Excluding(i => ((OpenApiAny)i.Extensions["x-list"]).Node[0].Root)
                .Excluding(i => ((OpenApiAny)i.Extensions["x-list"]).Node[1].Parent)
                .Excluding(i => ((OpenApiAny)i.Extensions["x-list"]).Node[1].Root));
        }

        [Fact]
        public void ParseBasicInfoShouldSucceed()
        {
            using (var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "basicInfo.yaml")))
            {
                var yamlStream = new YamlStream();
                yamlStream.Load(new StreamReader(stream));
                var yamlNode = yamlStream.Documents.First().RootNode;

                var diagnostic = new OpenApiDiagnostic();
                var context = new ParsingContext(diagnostic);

                var asJsonNode = yamlNode.ToJsonNode();
                var node = new MapNode(context, asJsonNode);
                
                // Act
                var openApiInfo = OpenApiV3Deserializer.LoadInfo(node);

                // Assert
                openApiInfo.Should().BeEquivalentTo(
                    new OpenApiInfo
                    {
                        Title = "Basic Info",
                        Summary = "Sample Summary",
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

                var diagnostic = new OpenApiDiagnostic();
                var context = new ParsingContext(diagnostic);

                var asJsonNode = yamlNode.ToJsonNode();
                var node = new MapNode(context, asJsonNode);
                
                // Act
                var openApiInfo = OpenApiV3Deserializer.LoadInfo(node);

                // Assert
                openApiInfo.Should().BeEquivalentTo(
                    new OpenApiInfo
                    {
                        Title = "Minimal Info",
                        Version = "1.0.1"
                    });
            }
        }
    }
}
