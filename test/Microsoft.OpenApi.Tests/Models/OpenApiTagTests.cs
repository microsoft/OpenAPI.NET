// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Collections.Generic;
using System.IO;
using FluentAssertions;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;
using Xunit;

namespace Microsoft.OpenApi.Tests.Models
{
    [Collection("DefaultSettings")]
    public class OpenApiTagTests
    {
        public static OpenApiTag BasicTag = new OpenApiTag();

        public static OpenApiTag AdvancedTag = new OpenApiTag
        {
            Name = "pet",
            Description = "Pets operations",
            ExternalDocs = OpenApiExternalDocsTests.AdvanceExDocs,
            Extensions = new Dictionary<string, IOpenApiExtension>
            {
                {"x-tag-extension", new OpenApiNull()}
            }
        };

        public static OpenApiTag ReferencedTag = new OpenApiTag
        {
            Name = "pet",
            Description = "Pets operations",
            ExternalDocs = OpenApiExternalDocsTests.AdvanceExDocs,
            Extensions = new Dictionary<string, IOpenApiExtension>
            {
                {"x-tag-extension", new OpenApiNull()}
            },
            Reference = new OpenApiReference
            {
                Type = ReferenceType.Tag,
                Id = "pet"
            }
        };

        [Fact]
        public void SerializeBasicTagAsV3JsonWithoutReferenceWorks()
        {
            // Arrange
            var outputStringWriter = new StringWriter();
            var writer = new OpenApiJsonWriter(outputStringWriter);
            var expected = "{ }";

            // Act
            BasicTag.SerializeAsV3WithoutReference(writer);
            writer.Flush();
            var actual = outputStringWriter.GetStringBuilder().ToString();

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeBasicTagAsV2JsonWithoutReferenceWorks()
        {
            // Arrange
            var outputStringWriter = new StringWriter();
            var writer = new OpenApiJsonWriter(outputStringWriter);
            var expected = "{ }";

            // Act
            BasicTag.SerializeAsV2WithoutReference(writer);
            writer.Flush();
            var actual = outputStringWriter.GetStringBuilder().ToString();

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeBasicTagAsV3YamlWithoutReferenceWorks()
        {
            // Arrange
            var outputStringWriter = new StringWriter();
            var writer = new OpenApiYamlWriter(outputStringWriter);
            var expected = "{ }";

            // Act
            BasicTag.SerializeAsV3WithoutReference(writer);
            var actual = outputStringWriter.GetStringBuilder().ToString();

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeBasicTagAsV2YamlWithoutReferenceWorks()
        {
            // Arrange
            var outputStringWriter = new StringWriter();
            var writer = new OpenApiYamlWriter(outputStringWriter);
            var expected = "{ }";

            // Act
            BasicTag.SerializeAsV2WithoutReference(writer);
            writer.Flush();
            var actual = outputStringWriter.GetStringBuilder().ToString();

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeAdvancedTagAsV3JsonWithoutReferenceWorks()
        {
            // Arrange
            var outputStringWriter = new StringWriter();
            var writer = new OpenApiJsonWriter(outputStringWriter);
            var expected =
                @"{
  ""name"": ""pet"",
  ""description"": ""Pets operations"",
  ""externalDocs"": {
    ""description"": ""Find more info here"",
    ""url"": ""https://example.com""
  },
  ""x-tag-extension"": null
}";

            // Act
            AdvancedTag.SerializeAsV3WithoutReference(writer);
            writer.Flush();
            var actual = outputStringWriter.GetStringBuilder().ToString();

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeAdvancedTagAsV2JsonWithoutReferenceWorks()
        {
            // Arrange
            var outputStringWriter = new StringWriter();
            var writer = new OpenApiJsonWriter(outputStringWriter);
            var expected =
                @"{
  ""name"": ""pet"",
  ""description"": ""Pets operations"",
  ""externalDocs"": {
    ""description"": ""Find more info here"",
    ""url"": ""https://example.com""
  },
  ""x-tag-extension"": null
}";

            // Act
            AdvancedTag.SerializeAsV2WithoutReference(writer);
            writer.Flush();
            var actual = outputStringWriter.GetStringBuilder().ToString();

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeAdvancedTagAsV3YamlWithoutReferenceWorks()
        {
            // Arrange
            var outputStringWriter = new StringWriter();
            var writer = new OpenApiYamlWriter(outputStringWriter);
            var expected =
                @"name: pet
description: Pets operations
externalDocs:
  description: Find more info here
  url: https://example.com
x-tag-extension: ";

            // Act
            AdvancedTag.SerializeAsV3WithoutReference(writer);
            writer.Flush();
            var actual = outputStringWriter.GetStringBuilder().ToString();

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeAdvancedTagAsV2YamlWithoutReferenceWorks()
        {
            // Arrange
            var outputStringWriter = new StringWriter();
            var writer = new OpenApiYamlWriter(outputStringWriter);
            var expected =
                @"name: pet
description: Pets operations
externalDocs:
  description: Find more info here
  url: https://example.com
x-tag-extension: ";

            // Act
            AdvancedTag.SerializeAsV2WithoutReference(writer);
            writer.Flush();
            var actual = outputStringWriter.GetStringBuilder().ToString();

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeAdvancedTagAsV3JsonWorks()
        {
            // Arrange
            var outputStringWriter = new StringWriter();
            var writer = new OpenApiJsonWriter(outputStringWriter);

            var expected = @"""pet""";

            // Act
            AdvancedTag.SerializeAsV3(writer);
            writer.Flush();
            var actual = outputStringWriter.GetStringBuilder().ToString();

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeAdvancedTagAsV2JsonWorks()
        {
            // Arrange
            var outputStringWriter = new StringWriter();
            var writer = new OpenApiJsonWriter(outputStringWriter);

            var expected = @"""pet""";

            // Act
            AdvancedTag.SerializeAsV2(writer);
            writer.Flush();
            var actual = outputStringWriter.GetStringBuilder().ToString();

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeAdvancedTagAsV3YamlWorks()
        {
            // Arrange
            var outputStringWriter = new StringWriter();
            var writer = new OpenApiYamlWriter(outputStringWriter);

            var expected = @" pet";

            // Act
            AdvancedTag.SerializeAsV3(writer);
            writer.Flush();
            var actual = outputStringWriter.GetStringBuilder().ToString();

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeAdvancedTagAsV2YamlWorks()
        {
            // Arrange
            var outputStringWriter = new StringWriter();
            var writer = new OpenApiYamlWriter(outputStringWriter);

            var expected = @" pet";

            // Act
            AdvancedTag.SerializeAsV2(writer);
            writer.Flush();
            var actual = outputStringWriter.GetStringBuilder().ToString();

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeReferencedTagAsV3JsonWorks()
        {
            // Arrange
            var outputStringWriter = new StringWriter();
            var writer = new OpenApiJsonWriter(outputStringWriter);

            var expected = @"""pet""";

            // Act
            ReferencedTag.SerializeAsV3(writer);
            writer.Flush();
            var actual = outputStringWriter.GetStringBuilder().ToString();

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeReferencedTagAsV2JsonWorks()
        {
            // Arrange
            var outputStringWriter = new StringWriter();
            var writer = new OpenApiJsonWriter(outputStringWriter);

            var expected = @"""pet""";

            // Act
            ReferencedTag.SerializeAsV2(writer);
            writer.Flush();
            var actual = outputStringWriter.GetStringBuilder().ToString();

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeReferencedTagAsV3YamlWorks()
        {
            // Arrange
            var outputStringWriter = new StringWriter();
            var writer = new OpenApiYamlWriter(outputStringWriter);

            var expected = @" pet";

            // Act
            ReferencedTag.SerializeAsV3(writer);
            writer.Flush();
            var actual = outputStringWriter.GetStringBuilder().ToString();

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeReferencedTagAsV2YamlWorks()
        {
            // Arrange
            var outputStringWriter = new StringWriter();
            var writer = new OpenApiYamlWriter(outputStringWriter);

            var expected = @" pet";

            // Act
            ReferencedTag.SerializeAsV2(writer);
            writer.Flush();
            var actual = outputStringWriter.GetStringBuilder().ToString();

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }
    }
}