// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using Microsoft.OpenApi.Models;
using Xunit;
using FluentAssertions;
using Microsoft.OpenApi.Extensions;

namespace Microsoft.OpenApi.Tests.Models
{
    public class OpenApiSchemaTests
    {
        public static OpenApiSchema BasicV31Schema = new()
        {
            Id = "https://example.com/arrays.schema.json",
            Schema = "https://json-schema.org/draft/2020-12/schema",
            Description = "A representation of a person, company, organization, or place",
            Type = "object",
            Properties = new Dictionary<string, OpenApiSchema>
            {
                ["fruits"] = new OpenApiSchema
                {
                    Type = "array",
                    Items = new OpenApiSchema
                    {
                        Type = "string"
                    }
                },
                ["vegetables"] = new OpenApiSchema
                {
                    Type = "array"
                }
            },
            Definitions = new Dictionary<string, OpenApiSchema>
            {
                ["veggie"] = new OpenApiSchema
                {
                    Type = "object",
                    Required = new HashSet<string>{ "veggieName", "veggieLike" },
                    Properties = new Dictionary<string, OpenApiSchema>
                    {
                        ["veggieName"] = new OpenApiSchema
                        {
                            Type = "string",
                            Description = "The name of the vegetable."
                        },
                        ["veggieLike"] = new OpenApiSchema
                        {
                            Type = "boolean",
                            Description = "Do I like this vegetable?"
                        }
                    }
                }
            }
        };

        [Fact]
        public void SerializeBasicV31SchemaWorks()
        {
            // Arrange
            var expected = @"{
  ""$id"": ""https://example.com/arrays.schema.json"",
  ""$schema"": ""https://json-schema.org/draft/2020-12/schema"",
  ""$defs"": {
    ""veggie"": {
      ""required"": [
        ""veggieName"",
        ""veggieLike""
      ],
      ""type"": ""object"",
      ""properties"": {
        ""veggieName"": {
          ""type"": ""string"",
          ""description"": ""The name of the vegetable.""
        },
        ""veggieLike"": {
          ""type"": ""boolean"",
          ""description"": ""Do I like this vegetable?""
        }
      }
    }
  },
  ""type"": ""object"",
  ""properties"": {
    ""fruits"": {
      ""type"": ""array"",
      ""items"": {
        ""type"": ""string""
      }
    },
    ""vegetables"": {
      ""type"": ""array""
    }
  },
  ""description"": ""A representation of a person, company, organization, or place""
}";

            // Act
            var actual = BasicV31Schema.SerializeAsJson(OpenApiSpecVersion.OpenApi3_1);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }
    }
}
