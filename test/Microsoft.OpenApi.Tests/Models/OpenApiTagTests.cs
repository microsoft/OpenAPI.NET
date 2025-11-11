// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

namespace Microsoft.OpenApi.Tests.Models
{
    [Collection("DefaultSettings")]
    public class OpenApiTagTests
    {
        public static readonly OpenApiTag BasicTag = new();

        public static readonly OpenApiTag AdvancedTag = new()
        {
            Name = "pet",
            Description = "Pets operations",
            ExternalDocs = OpenApiExternalDocsTests.AdvanceExDocs,
            Extensions = new Dictionary<string, IOpenApiExtension>()
            {
                {"x-tag-extension", null}
            }
        };

        public static readonly OpenApiTag TagWithV32Properties = new()
        {
            Name = "store",
            Description = "Store operations",
            Summary = "Operations related to the pet store",
            Parent = new OpenApiTagReference("pet"),
            Kind = "operational",
            ExternalDocs = OpenApiExternalDocsTests.AdvanceExDocs
        };

        public static IOpenApiTag ReferencedTag = new OpenApiTagReference("pet");

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task SerializeBasicTagAsV3JsonWithoutReferenceWorksAsync(bool produceTerseOutput)
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new() { Terse = produceTerseOutput });

            // Act
            BasicTag.SerializeAsV3(writer);
            await writer.FlushAsync();

            // Assert
            await Verifier.Verify(outputStringWriter).UseParameters(produceTerseOutput);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task SerializeBasicTagAsV2JsonWithoutReferenceWorksAsync(bool produceTerseOutput)
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new() { Terse = produceTerseOutput });

            // Act
            BasicTag.SerializeAsV2(writer);
            await writer.FlushAsync();

            // Assert
            await Verifier.Verify(outputStringWriter).UseParameters(produceTerseOutput);
        }

        [Fact]
        public void SerializeBasicTagAsV3YamlWithoutReferenceWorks()
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiYamlWriter(outputStringWriter);
            var expected = "{ }";

            // Act
            BasicTag.SerializeAsV3(writer);
            var actual = outputStringWriter.GetStringBuilder().ToString();

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task SerializeBasicTagAsV2YamlWithoutReferenceWorks()
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiYamlWriter(outputStringWriter);
            var expected = "{ }";

            // Act
            BasicTag.SerializeAsV2(writer);
            await writer.FlushAsync();
            var actual = outputStringWriter.GetStringBuilder().ToString();

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task SerializeAdvancedTagAsV3YamlWithoutReferenceWorks()
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiYamlWriter(outputStringWriter);
            var expected =
                """
                name: pet
                description: Pets operations
                externalDocs:
                  description: Find more info here
                  url: https://example.com
                x-tag-extension: null
                """;

            // Act
            AdvancedTag.SerializeAsV3(writer);
            await writer.FlushAsync();
            var actual = outputStringWriter.GetStringBuilder().ToString();

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task SerializeAdvancedTagAsV2YamlWithoutReferenceWorks()
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiYamlWriter(outputStringWriter);
            var expected =
                """
                name: pet
                description: Pets operations
                externalDocs:
                  description: Find more info here
                  url: https://example.com
                x-tag-extension: null
                """;

            // Act
            AdvancedTag.SerializeAsV2(writer);
            await writer.FlushAsync();
            var actual = outputStringWriter.GetStringBuilder().ToString();

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task SerializeAdvancedTagAsV3JsonWorksAsync(bool produceTerseOutput)
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new() { Terse = produceTerseOutput });

            // Act
            AdvancedTag.SerializeAsV3(writer);
            await writer.FlushAsync();

            // Assert
            await Verifier.Verify(outputStringWriter).UseParameters(produceTerseOutput);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task SerializeAdvancedTagAsV2JsonWorksAsync(bool produceTerseOutput)
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new() { Terse = produceTerseOutput });

            // Act
            AdvancedTag.SerializeAsV2(writer);
            await writer.FlushAsync();

            // Assert
            await Verifier.Verify(outputStringWriter).UseParameters(produceTerseOutput);
        }

        [Fact]
        public async Task SerializeAdvancedTagAsV3YamlWorks()
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiYamlWriter(outputStringWriter);

            var expected = @"name: pet
description: Pets operations
externalDocs:
  description: Find more info here
  url: https://example.com
x-tag-extension: null";

            // Act
            AdvancedTag.SerializeAsV3(writer);
            await writer.FlushAsync();
            var actual = outputStringWriter.GetStringBuilder().ToString();

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task SerializeAdvancedTagAsV2YamlWorks()
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiYamlWriter(outputStringWriter);

            var expected = @"name: pet
description: Pets operations
externalDocs:
  description: Find more info here
  url: https://example.com
x-tag-extension: null";

            // Act
            AdvancedTag.SerializeAsV2(writer);
            await writer.FlushAsync();
            var actual = outputStringWriter.GetStringBuilder().ToString();

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task SerializeReferencedTagAsV3JsonWorksAsync(bool produceTerseOutput)
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new() { Terse = produceTerseOutput });

            // Act
            ReferencedTag.SerializeAsV3(writer);
            await writer.FlushAsync();

            // Assert
            await Verifier.Verify(outputStringWriter).UseParameters(produceTerseOutput);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task SerializeReferencedTagAsV2JsonWorksAsync(bool produceTerseOutput)
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new() { Terse = produceTerseOutput });

            // Act
            ReferencedTag.SerializeAsV2(writer);
            await writer.FlushAsync();

            // Assert
            await Verifier.Verify(outputStringWriter).UseParameters(produceTerseOutput);
        }

        [Fact]
        public async Task SerializeReferencedTagAsV3YamlWorks()
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiYamlWriter(outputStringWriter);

            var expected = @" pet";

            // Act
            ReferencedTag.SerializeAsV3(writer);
            await writer.FlushAsync();
            var actual = outputStringWriter.GetStringBuilder().ToString();

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task SerializeReferencedTagAsV2YamlWorks()
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiYamlWriter(outputStringWriter);

            var expected = @" pet";

            // Act
            ReferencedTag.SerializeAsV2(writer);
            await writer.FlushAsync();
            var actual = outputStringWriter.GetStringBuilder().ToString();

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            Assert.Equal(expected, actual);
        }

        // Tests for V3.2 properties in V3.1 serialization (using extension format)
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task SerializeTagWithV32PropertiesAsV31JsonWorksAsync(bool produceTerseOutput)
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new() { Terse = produceTerseOutput });

            // Act
            TagWithV32Properties.SerializeAsV31(writer);
            await writer.FlushAsync();

            // Assert
            await Verifier.Verify(outputStringWriter).UseParameters(produceTerseOutput);
        }

        [Fact]
        public async Task SerializeTagWithV32PropertiesAsV31YamlWorks()
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiYamlWriter(outputStringWriter);
            var expected = @"name: store
description: Store operations
externalDocs:
  description: Find more info here
  url: https://example.com
x-oas-summary: Operations related to the pet store
x-oas-parent: pet
x-oas-kind: operational";

            // Act
            TagWithV32Properties.SerializeAsV31(writer);
            await writer.FlushAsync();
            var actual = outputStringWriter.GetStringBuilder().ToString();

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            Assert.Equal(expected, actual);
        }

        // Tests for V3.2 properties in V3.2 serialization (using native format)
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task SerializeTagWithV32PropertiesAsV32JsonWorksAsync(bool produceTerseOutput)
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiJsonWriter(outputStringWriter, new() { Terse = produceTerseOutput });

            // Act
            TagWithV32Properties.SerializeAsV32(writer);
            await writer.FlushAsync();

            // Assert
            await Verifier.Verify(outputStringWriter).UseParameters(produceTerseOutput);
        }

        [Fact]
        public async Task SerializeTagWithV32PropertiesAsV32YamlWorks()
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiYamlWriter(outputStringWriter);
            var expected = @"name: store
description: Store operations
externalDocs:
  description: Find more info here
  url: https://example.com
summary: Operations related to the pet store
parent: pet
kind: operational";

            // Act
            TagWithV32Properties.SerializeAsV32(writer);
            await writer.FlushAsync();
            var actual = outputStringWriter.GetStringBuilder().ToString();

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            Assert.Equal(expected, actual);
        }

        // Test for V3.0 serialization with V3.2 properties (should use extension format)
        [Fact]
        public async Task SerializeTagWithV32PropertiesAsV3YamlWorks()
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiYamlWriter(outputStringWriter);
            var expected = @"name: store
description: Store operations
externalDocs:
  description: Find more info here
  url: https://example.com
x-oas-summary: Operations related to the pet store
x-oas-parent: pet
x-oas-kind: operational";

            // Act
            TagWithV32Properties.SerializeAsV3(writer);
            await writer.FlushAsync();
            var actual = outputStringWriter.GetStringBuilder().ToString();

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            Assert.Equal(expected, actual);
        }

        // Tests for individual V3.2 properties
        [Fact]
        public void TagWithSummaryPropertyWorks()
        {
            // Arrange & Act
            var tag = new OpenApiTag
            {
                Name = "test",
                Summary = "Test summary"
            };

            // Assert
            Assert.Equal("test", tag.Name);
            Assert.Equal("Test summary", tag.Summary);
        }

        [Fact]
        public void TagWithParentPropertyWorks()
        {
            // Arrange & Act
            var parentTag = new OpenApiTagReference("parent-tag");
            var tag = new OpenApiTag
            {
                Name = "child",
                Parent = parentTag
            };

            // Assert
            Assert.Equal("child", tag.Name);
            Assert.NotNull(tag.Parent);
            Assert.Equal(parentTag, tag.Parent);
        }

        [Fact]
        public void TagWithKindPropertyWorks()
        {
            // Arrange & Act
            var tag = new OpenApiTag
            {
                Name = "test",
                Kind = "category"
            };

            // Assert
            Assert.Equal("test", tag.Name);
            Assert.Equal("category", tag.Kind);
        }

        [Fact]
        public void TagWithAllV32PropertiesWorks()
        {
            // Arrange & Act
            var parentTag = new OpenApiTagReference("parent-tag");
            var tag = new OpenApiTag
            {
                Name = "test",
                Description = "Test description",
                Summary = "Test summary",
                Parent = parentTag,
                Kind = "category"
            };

            // Assert
            Assert.Equal("test", tag.Name);
            Assert.Equal("Test description", tag.Description);
            Assert.Equal("Test summary", tag.Summary);
            Assert.Equal(parentTag, tag.Parent);
            Assert.Equal("category", tag.Kind);
        }

        [Fact]
        public void CreateShallowCopyIncludesV32Properties()
        {
            // Arrange
            var originalParent = new OpenApiTagReference("original-parent");
            var original = new OpenApiTag
            {
                Name = "original",
                Description = "Original description",
                Summary = "Original summary",
                Parent = originalParent,
                Kind = "original-kind"
            };

            // Act
            var copy = original.CreateShallowCopy();

            // Assert
            Assert.Equal(original.Name, copy.Name);
            Assert.Equal(original.Description, copy.Description);
            Assert.Equal(original.Summary, copy.Summary);
            Assert.Equal(original.Parent, copy.Parent);
            Assert.Equal(original.Kind, copy.Kind);
        }

        [Fact]
        public void TagWithNullV32PropertiesWorks()
        {
            // Arrange & Act
            var tag = new OpenApiTag
            {
                Name = "test",
                Summary = null,
                Parent = null,
                Kind = null
            };

            // Assert
            Assert.Equal("test", tag.Name);
            Assert.Null(tag.Summary);
            Assert.Null(tag.Parent);
            Assert.Null(tag.Kind);
        }

        [Theory]
        [InlineData("category")]
        [InlineData("operational")]
        [InlineData("system")]
        [InlineData("custom-kind")]
        public void TagKindPropertyAcceptsVariousValues(string kindValue)
        {
            // Arrange & Act
            var tag = new OpenApiTag
            {
                Name = "test",
                Kind = kindValue
            };

            // Assert
            Assert.Equal(kindValue, tag.Kind);
        }
    }
}
