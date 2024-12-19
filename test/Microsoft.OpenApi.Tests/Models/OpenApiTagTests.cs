// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Models.References;
using Microsoft.OpenApi.Writers;
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
            Extensions = new Dictionary<string, IOpenApiExtension>
            {
                {"x-tag-extension", null}
            }
        };

        public static OpenApiTag ReferencedTag = new OpenApiTagReference("pet", null);

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
            writer.Flush();

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
            writer.Flush();

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
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeBasicTagAsV2YamlWithoutReferenceWorks()
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiYamlWriter(outputStringWriter);
            var expected = "{ }";

            // Act
            BasicTag.SerializeAsV2(writer);
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
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiYamlWriter(outputStringWriter);
            var expected =
                """
                name: pet
                description: Pets operations
                externalDocs:
                  description: Find more info here
                  url: https://example.com
                x-tag-extension:
                """;

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
        public void SerializeAdvancedTagAsV2YamlWithoutReferenceWorks()
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
                x-tag-extension:
                """;

            // Act
            AdvancedTag.SerializeAsV2(writer);
            writer.Flush();
            var actual = outputStringWriter.GetStringBuilder().ToString();

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
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
            writer.Flush();

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
            writer.Flush();

            // Assert
            await Verifier.Verify(outputStringWriter).UseParameters(produceTerseOutput);
        }

        [Fact]
        public void SerializeAdvancedTagAsV3YamlWorks()
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiYamlWriter(outputStringWriter);

            var expected = @"name: pet
description: Pets operations
externalDocs:
  description: Find more info here
  url: https://example.com
x-tag-extension:";

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
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
            var writer = new OpenApiYamlWriter(outputStringWriter);

            var expected = @"name: pet
description: Pets operations
externalDocs:
  description: Find more info here
  url: https://example.com
x-tag-extension:";

            // Act
            AdvancedTag.SerializeAsV2(writer);
            writer.Flush();
            var actual = outputStringWriter.GetStringBuilder().ToString();

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
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
            writer.Flush();

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
            writer.Flush();

            // Assert
            await Verifier.Verify(outputStringWriter).UseParameters(produceTerseOutput);
        }

        [Fact]
        public void SerializeReferencedTagAsV3YamlWorks()
        {
            // Arrange
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
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
            var outputStringWriter = new StringWriter(CultureInfo.InvariantCulture);
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
