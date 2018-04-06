// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.IO;
using FluentAssertions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.OpenApi.Tests.Models
{
    [Collection("DefaultSettings")]
    public class OpenApiHeaderTests
    {
        public static OpenApiHeader AdvancedHeader = new OpenApiHeader
        {
            Description = "sampleHeader",
            Schema = new OpenApiSchema
            {
                Type = "integer",
                Format = "int32"
            }
        };

        public static OpenApiHeader ReferencedHeader = new OpenApiHeader
        {
            Reference = new OpenApiReference
            {
                Type = ReferenceType.Header,
                Id = "example1",
            },
            Description = "sampleHeader",
            Schema = new OpenApiSchema
            {
                Type = "integer",
                Format = "int32"
            }
        };

        private readonly ITestOutputHelper _output;

        public OpenApiHeaderTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void SerializeAdvancedHeaderAsV3JsonWorks()
        {
            // Arrange
            var outputStringWriter = new StringWriter();
            var writer = new OpenApiJsonWriter(outputStringWriter);
            var expected =
                @"{
  ""description"": ""sampleHeader"",
  ""schema"": {
    ""type"": ""integer"",
    ""format"": ""int32""
  }
}";

            // Act
            AdvancedHeader.SerializeAsV3(writer);
            writer.Flush();
            var actual = outputStringWriter.GetStringBuilder().ToString();

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeReferencedHeaderAsV3JsonWorks()
        {
            // Arrange
            var outputStringWriter = new StringWriter();
            var writer = new OpenApiJsonWriter(outputStringWriter);
            var expected =
                @"{
  ""$ref"": ""#/components/headers/example1""
}";

            // Act
            ReferencedHeader.SerializeAsV3(writer);
            writer.Flush();
            var actual = outputStringWriter.GetStringBuilder().ToString();

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeReferencedHeaderAsV3JsonWithoutReferenceWorks()
        {
            // Arrange
            var outputStringWriter = new StringWriter();
            var writer = new OpenApiJsonWriter(outputStringWriter);
            var expected =
                @"{
  ""description"": ""sampleHeader"",
  ""schema"": {
    ""type"": ""integer"",
    ""format"": ""int32""
  }
}";

            // Act
            ReferencedHeader.SerializeAsV3WithoutReference(writer);
            writer.Flush();
            var actual = outputStringWriter.GetStringBuilder().ToString();

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeAdvancedHeaderAsV2JsonWorks()
        {
            // Arrange
            var outputStringWriter = new StringWriter();
            var writer = new OpenApiJsonWriter(outputStringWriter);
            var expected =
                @"{
  ""description"": ""sampleHeader"",
  ""type"": ""integer"",
  ""format"": ""int32""
}";

            // Act
            AdvancedHeader.SerializeAsV2(writer);
            writer.Flush();
            var actual = outputStringWriter.GetStringBuilder().ToString();

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeReferencedHeaderAsV2JsonWorks()
        {
            // Arrange
            var outputStringWriter = new StringWriter();
            var writer = new OpenApiJsonWriter(outputStringWriter);
            var expected =
                @"{
  ""$ref"": ""#/headers/example1""
}";

            // Act
            ReferencedHeader.SerializeAsV2(writer);
            writer.Flush();
            var actual = outputStringWriter.GetStringBuilder().ToString();

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeReferencedHeaderAsV2JsonWithoutReferenceWorks()
        {
            // Arrange
            var outputStringWriter = new StringWriter();
            var writer = new OpenApiJsonWriter(outputStringWriter);
            var expected =
                @"{
  ""description"": ""sampleHeader"",
  ""type"": ""integer"",
  ""format"": ""int32""
}";

            // Act
            ReferencedHeader.SerializeAsV2WithoutReference(writer);
            writer.Flush();
            var actual = outputStringWriter.GetStringBuilder().ToString();

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }
    }
}