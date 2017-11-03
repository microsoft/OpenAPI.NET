// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using FluentAssertions;
using Microsoft.OpenApi.Models;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.OpenApi.Tests.Models
{
    public class OpenApiParameterTests
    {
        public static OpenApiParameter BasicParameter = new OpenApiParameter
        {
            Name = "name1",
            In = ParameterLocation.path
        };

        public static OpenApiParameter AdvancedPathParameterWithSchema = new OpenApiParameter
        {
            Name = "name1",
            In = ParameterLocation.path,
            Description = "description1",
            Required = true,
            Deprecated = false,

            Style = "simple",
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
            var actual = BasicParameter.SerializeAsJson();

            // Assert
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
            var actual = AdvancedPathParameterWithSchema.SerializeAsJson();

            // Assert
            actual.Should().Be(expected);
        }
    }
}