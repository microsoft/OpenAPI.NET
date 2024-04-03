// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.IO;
using System.Text.Json.Nodes;
using FluentAssertions;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Reader;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.V3Tests
{
    [Collection("DefaultSettings")]
    public class OpenApiInfoTests
    {
        private const string SampleFolderPath = "V3Tests/Samples/OpenApiInfo/";

        public OpenApiInfoTests()
        {
            OpenApiReaderRegistry.RegisterReader("yaml", new OpenApiYamlReader());
        }

        [Fact]
        public void ParseAdvancedInfoShouldSucceed()
        {
            // Act
            var openApiInfo = OpenApiModelFactory.Load<OpenApiInfo>(Path.Combine(SampleFolderPath, "advancedInfo.yaml"), OpenApiSpecVersion.OpenApi3_0, out var diagnostic);

            // Assert
            openApiInfo.Should().BeEquivalentTo(
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
            // Act
            var openApiInfo = OpenApiModelFactory.Load<OpenApiInfo>(Path.Combine(SampleFolderPath, "basicInfo.yaml"), OpenApiSpecVersion.OpenApi3_0, out _);

            // Assert
            openApiInfo.Should().BeEquivalentTo(
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

        [Fact]
        public void ParseMinimalInfoShouldSucceed()
        {
            using var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "minimalInfo.yaml"));

            // Act
            var openApiInfo = OpenApiModelFactory.Load<OpenApiInfo>(stream, OpenApiSpecVersion.OpenApi3_0, "yaml", out _);

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
