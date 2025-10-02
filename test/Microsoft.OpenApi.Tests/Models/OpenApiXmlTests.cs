// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Xunit;

namespace Microsoft.OpenApi.Tests.Models
{
    [Collection("DefaultSettings")]
    public class OpenApiXmlTests
    {
#pragma warning disable CS0618 // Type or member is obsolete
        public static OpenApiXml AdvancedXml = new()
        {
            Name = "animal",
            Namespace = new("http://swagger.io/schema/sample"),
            Prefix = "sample",
            Wrapped = true,
            Attribute = true,
            Extensions = new Dictionary<string, IOpenApiExtension>()
            {
                {"x-xml-extension", new JsonNodeExtension(7)}
            }
        };
#pragma warning restore CS0618 // Type or member is obsolete

        public static OpenApiXml BasicXml = new();

        public static OpenApiXml XmlWithNodeType = new()
        {
            Name = "pet",
            Namespace = new("http://example.com/schema"),
            Prefix = "ex",
            NodeType = OpenApiXmlNodeType.Element,
            Extensions = new Dictionary<string, IOpenApiExtension>()
            {
                {"x-custom", new JsonNodeExtension("test")}
            }
        };

        public static OpenApiXml XmlWithAttributeNodeType = new()
        {
            Name = "id",
            NodeType = OpenApiXmlNodeType.Attribute
        };

        [Theory]
        [InlineData(OpenApiSpecVersion.OpenApi3_0, OpenApiConstants.Json)]
        [InlineData(OpenApiSpecVersion.OpenApi2_0, OpenApiConstants.Json)]
        [InlineData(OpenApiSpecVersion.OpenApi3_0, OpenApiConstants.Yaml)]
        [InlineData(OpenApiSpecVersion.OpenApi2_0, OpenApiConstants.Yaml)]
        public async Task SerializeBasicXmlWorks(
            OpenApiSpecVersion version,
            string format)
        {
            // Act
            var actual = await BasicXml.SerializeAsync(version, format);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            Assert.Equal("{ }", actual);
        }

        [Theory]
        [InlineData(OpenApiSpecVersion.OpenApi3_0)]
        [InlineData(OpenApiSpecVersion.OpenApi2_0)]
        public async Task SerializeAdvancedXmlAsJsonWorks(OpenApiSpecVersion version)
        {
            // Arrange
            var expected =
                """
                {
                  "name": "animal",
                  "namespace": "http://swagger.io/schema/sample",
                  "prefix": "sample",
                  "attribute": true,
                  "x-xml-extension": 7
                }
                """;

            // Act
            var actual = await AdvancedXml.SerializeAsJsonAsync(version);

            // Assert
            Assert.True(JsonNode.DeepEquals(JsonNode.Parse(actual), JsonNode.Parse(expected)));
        }

        [Theory]
        [InlineData(OpenApiSpecVersion.OpenApi3_0)]
        [InlineData(OpenApiSpecVersion.OpenApi2_0)]
        public async Task SerializeAdvancedXmlAsYamlWorks(OpenApiSpecVersion version)
        {
            // Arrange
            var expected =
                """
                name: animal
                namespace: http://swagger.io/schema/sample
                prefix: sample
                attribute: true
                x-xml-extension: 7
                """;

            // Act
            var actual = await AdvancedXml.SerializeAsYamlAsync(version);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task SerializeXmlWithNodeTypeAsJsonV32Works()
        {
            // Arrange
            var expected =
                """
                {
                  "name": "pet",
                  "namespace": "http://example.com/schema",
                  "prefix": "ex",
                  "nodeType": "element",
                  "x-custom": "test"
                }
                """;

            // Act
            var actual = await XmlWithNodeType.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_2);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task SerializeXmlWithNodeTypeAsYamlV32Works()
        {
            // Arrange
            var expected =
                """
                name: pet
                namespace: http://example.com/schema
                prefix: ex
                nodeType: element
                x-custom: test
                """;

            // Act
            var actual = await XmlWithNodeType.SerializeAsYamlAsync(OpenApiSpecVersion.OpenApi3_2);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task SerializeXmlWithAttributeNodeTypeAsJsonV32Works()
        {
            // Arrange
            var expected =
                """
                {
                  "name": "id",
                  "nodeType": "attribute"
                }
                """;

            // Act
            var actual = await XmlWithAttributeNodeType.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_2);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task SerializeXmlWithNodeTypeAsJsonV31DoesNotSerializeNodeType()
        {
            // Arrange - In v3.1, nodeType should not be serialized, instead attribute/wrapped should be
            var expected =
                """
                {
                  "name": "pet",
                  "namespace": "http://example.com/schema",
                  "prefix": "ex",
                  "wrapped": true,
                  "x-custom": "test"
                }
                """;

            // Act
            var actual = await XmlWithNodeType.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_1);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task SerializeXmlWithAttributeNodeTypeAsJsonV31DoesNotSerializeNodeType()
        {
            // Arrange - In v3.1, nodeType should not be serialized, instead attribute/wrapped should be
            var expected =
                """
                {
                  "name": "id",
                  "attribute": true
                }
                """;

            // Act
            var actual = await XmlWithAttributeNodeType.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_1);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            Assert.Equal(expected, actual);
        }
    }
}
