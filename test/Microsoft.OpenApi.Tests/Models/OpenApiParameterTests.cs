﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Collections.Generic;
using System.IO;
using FluentAssertions;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.OpenApi.Tests.Models
{
    [Collection("DefaultSettings")]
    public class OpenApiParameterTests
    {
        public static OpenApiParameter BasicParameter = new OpenApiParameter
        {
            Name = "name1",
            In = ParameterLocation.Path
        };

        public static OpenApiParameter ReferencedParameter = new OpenApiParameter
        {
            Name = "name1",
            In = ParameterLocation.Path,
            Reference = new OpenApiReference
            {
                Type = ReferenceType.Parameter,
                Id = "example1"
            }
        };

        public static OpenApiParameter AdvancedPathParameterWithSchema = new OpenApiParameter
        {
            Name = "name1",
            In = ParameterLocation.Path,
            Description = "description1",
            Required = true,
            Deprecated = false,

            Style = ParameterStyle.Simple,
            Explode = true,
            Schema = new OpenApiSchema
            {
                Title = "title2",
                Description = "description2"
            },
            Examples = new List<OpenApiExample>
            {
                new OpenApiExample
                {
                    Summary = "summary3",
                    Description = "description3"
                }
            }
        };

        private readonly ITestOutputHelper _output;

        public OpenApiParameterTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void SerializeBasicParameterAsV3JsonWorks()
        {
            // Arrange
            var expected = @"{
  ""name"": ""name1"",
  ""in"": ""path""
}";

            // Act
            var actual = BasicParameter.SerializeAsJson(OpenApiSpecVersion.OpenApi3_0_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeAdvancedParameterAsV3JsonWorks()
        {
            // Arrange
            var expected = @"{
  ""name"": ""name1"",
  ""in"": ""path"",
  ""description"": ""description1"",
  ""required"": true,
  ""style"": ""simple"",
  ""explode"": true,
  ""schema"": {
    ""title"": ""title2"",
    ""description"": ""description2""
  },
  ""examples"": [
    {
      ""summary"": ""summary3"",
      ""description"": ""description3""
    }
  ]
}";

            // Act
            var actual = AdvancedPathParameterWithSchema.SerializeAsJson(OpenApiSpecVersion.OpenApi3_0_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeReferencedParameterAsV3JsonWorks()
        {
            // Arrange
            var outputStringWriter = new StringWriter();
            var writer = new OpenApiJsonWriter(outputStringWriter);
            var expected =
                @"{
  ""$ref"": ""#/components/parameters/example1""
}";

            // Act
            ReferencedParameter.SerializeAsV3(writer);
            writer.Flush();
            var actual = outputStringWriter.GetStringBuilder().ToString();

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeReferencedParameterAsV3JsonWithoutReferenceWorks()
        {
            // Arrange
            var outputStringWriter = new StringWriter();
            var writer = new OpenApiJsonWriter(outputStringWriter);
            var expected =
                @"{
  ""name"": ""name1"",
  ""in"": ""path""
}";

            // Act
            ReferencedParameter.SerializeAsV3WithoutReference(writer);
            writer.Flush();
            var actual = outputStringWriter.GetStringBuilder().ToString();

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeReferencedParameterAsV2JsonWorks()
        {
            // Arrange
            var outputStringWriter = new StringWriter();
            var writer = new OpenApiJsonWriter(outputStringWriter);
            var expected =
                @"{
  ""$ref"": ""#/parameters/example1""
}";

            // Act
            ReferencedParameter.SerializeAsV2(writer);
            writer.Flush();
            var actual = outputStringWriter.GetStringBuilder().ToString();

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeReferencedParameterAsV2JsonWithoutReferenceWorks()
        {
            // Arrange
            var outputStringWriter = new StringWriter();
            var writer = new OpenApiJsonWriter(outputStringWriter);
            var expected =
                @"{
  ""name"": ""name1"",
  ""in"": ""Path""
}";

            // Act
            ReferencedParameter.SerializeAsV2WithoutReference(writer);
            writer.Flush();
            var actual = outputStringWriter.GetStringBuilder().ToString();

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }
    }
}