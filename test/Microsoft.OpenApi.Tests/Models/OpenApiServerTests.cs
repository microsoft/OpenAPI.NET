// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Microsoft.OpenApi.Tests.Models
{
    [Collection("DefaultSettings")]
    public class OpenApiServerTests
    {
        public static OpenApiServer BasicServer = new()
        {
            Description = "description1",
            Url = "https://example.com/server1"
        };

        public static OpenApiServer AdvancedServer = new()
        {
            Description = "description1",
            Url = "https://{username}.example.com:{port}/{basePath}",
            Variables = new Dictionary<string, OpenApiServerVariable>
            {
                ["username"] = new()
                {
                    Default = "unknown",
                    Description = "variableDescription1",
                },
                ["port"] = new()
                {
                    Default = "8443",
                    Description = "variableDescription2",
                    Enum = new()
                    {
                        "443",
                        "8443"
                    }
                },
                ["basePath"] = new()
                {
                    Default = "v1"
                },
            }
        };

        public static OpenApiServer ServerWithName = new()
        {
            Name = "dev-server",
            Description = "Development server",
            Url = "https://dev.example.com"
        };

        [Fact]
        public async Task SerializeBasicServerAsV3JsonWorks()
        {
            // Arrange
            var expected =
                """
                {
                  "url": "https://example.com/server1",
                  "description": "description1"
                }
                """;

            // Act
            var actual = await BasicServer.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task SerializeAdvancedServerAsV3JsonWorks()
        {
            // Arrange
            var expected =
                """
                {
                  "url": "https://{username}.example.com:{port}/{basePath}",
                  "description": "description1",
                  "variables": {
                    "username": {
                      "default": "unknown",
                      "description": "variableDescription1"
                    },
                    "port": {
                      "default": "8443",
                      "description": "variableDescription2",
                      "enum": [
                        "443",
                        "8443"
                      ]
                    },
                    "basePath": {
                      "default": "v1"
                    }
                  }
                }
                """;

            // Act
            var actual = await AdvancedServer.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task SerializeServerWithNameAsV32JsonWorks()
        {
            // Arrange
            var expected =
                """
                {
                  "url": "https://dev.example.com",
                  "name": "dev-server",
                  "description": "Development server"
                }
                """;

            // Act
            var actual = await ServerWithName.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_2);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task SerializeServerWithNameAsV31JsonWorks()
        {
            // Arrange
            var expected =
                """
                {
                  "url": "https://dev.example.com",
                  "description": "Development server",
                  "x-oai-name": "dev-server"
                }
                """;

            // Act
            var actual = await ServerWithName.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_1);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task SerializeServerWithNameAsV3JsonWorks()
        {
            // Arrange
            var expected =
                """
                {
                  "url": "https://dev.example.com",
                  "description": "Development server",
                  "x-oai-name": "dev-server"
                }
                """;

            // Act
            var actual = await ServerWithName.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CopyConstructorHandlesNameField()
        {
            // Arrange
            var original = new OpenApiServer
            {
                Name = "test-server",
                Description = "Test description",
                Url = "https://test.example.com"
            };

            // Act
            var copy = new OpenApiServer(original);

            // Assert
            Assert.Equal(original.Name, copy.Name);
            Assert.Equal(original.Description, copy.Description);
            Assert.Equal(original.Url, copy.Url);
        }
    }
}
