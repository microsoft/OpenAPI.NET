// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.OpenApi.Tests.Models
{
    public class OpenApiSchemaTests
    {
        private readonly ITestOutputHelper _output;

        public OpenApiSchemaTests(ITestOutputHelper output)
        {
            _output = output;
        }

        public static OpenApiSchema BasicSchema = new OpenApiSchema();

        public static OpenApiSchema AdvancedSchemaNumber = new OpenApiSchema
        {
            Title = "title1",
            MultipleOf = 3,
            Maximum = 42,
            ExclusiveMinimum = true,
            Minimum = 10,
            Default = new OpenApiInteger(15),
            Type = "integer",
            
            Nullable = true,
            ExternalDocs = new OpenApiExternalDocs()
            {
                Url = new Uri("http://example.com/externalDocs")
            }
        };

        public static OpenApiSchema AdvancedSchemaObject = new OpenApiSchema
        {
            Title = "title1",
            Properties = new Dictionary<string, OpenApiSchema>()
            {
                ["property1"] = new OpenApiSchema()
                {
                    Properties = new Dictionary<string, OpenApiSchema>()
                    {
                        ["property2"] = new OpenApiSchema()
                        {
                            Type = "integer"
                        },
                        ["property3"] = new OpenApiSchema()
                        {
                            Type = "string",
                            MaxLength = 15
                        }
                    },
                },
                ["property4"] = new OpenApiSchema()
                {
                    Properties = new Dictionary<string, OpenApiSchema>()
                    {
                        ["property5"] = new OpenApiSchema()
                        {
                            Properties = new Dictionary<string, OpenApiSchema>()
                            {
                                ["property6"] = new OpenApiSchema()
                                {
                                    Type = "boolean"
                                }
                            }
                        },
                        ["property7"] = new OpenApiSchema()
                        {
                            Type = "string",
                            MinLength = 2
                        }
                    },
                },
            },
            Nullable = true,
            ExternalDocs = new OpenApiExternalDocs()
            {
                Url = new Uri("http://example.com/externalDocs")
            }
        };

        public static OpenApiSchema AdvancedSchemaWithAllOf = new OpenApiSchema
        {
            Title = "title1",
            AllOf = new List<OpenApiSchema>()
            {
                new OpenApiSchema()
                {
                    Title = "title2",
                    Properties = new Dictionary<string, OpenApiSchema>()
                    {
                        ["property1"] = new OpenApiSchema()
                        {
                            Type = "integer"
                        },
                        ["property2"] = new OpenApiSchema()
                        {
                            Type = "string",
                            MaxLength = 15
                        }
                    },
                },
                new OpenApiSchema()
                {
                    Title = "title3",
                    Properties = new Dictionary<string, OpenApiSchema>()
                    {
                        ["property3"] = new OpenApiSchema()
                        {
                            Properties = new Dictionary<string, OpenApiSchema>()
                            {
                                ["property4"] = new OpenApiSchema()
                                {
                                    Type = "boolean"
                                }
                            }
                        },
                        ["property5"] = new OpenApiSchema()
                        {
                            Type = "string",
                            MinLength = 2
                        }
                    },
                    Nullable = true
                },
            },
            Nullable = true,
            ExternalDocs = new OpenApiExternalDocs()
            {
                Url = new Uri("http://example.com/externalDocs")
            }
        };
        
        [Fact]
        public void SerializeBasicSchemaAsV3JsonWorks()
        {
            // Arrange
            var expected = @"{ }";

            // Act
            var actual = BasicSchema.SerializeAsJson();

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeAdvancedSchemaNumberAsV3JsonWorks()
        {
            // Arrange
            var expected = @"{
  ""title"": ""title1"",
  ""multipleOf"": 3,
  ""maximum"": 42,
  ""minimum"": 10,
  ""exclusiveMinimum"": true,
  ""type"": ""integer"",
  ""default"": 15,
  ""nullable"": true,
  ""externalDocs"": {
    ""url"": ""http://example.com/externalDocs""
  }
}";

            // Act
            var actual = AdvancedSchemaNumber.SerializeAsJson();

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeAdvancedSchemaObjectAsV3JsonWorks()
        {
            // Arrange
            var expected = @"{
  ""title"": ""title1"",
  ""properties"": {
    ""property1"": {
      ""properties"": {
        ""property2"": {
          ""type"": ""integer""
        },
        ""property3"": {
          ""maxLength"": 15,
          ""type"": ""string""
        }
      }
    },
    ""property4"": {
      ""properties"": {
        ""property5"": {
          ""properties"": {
            ""property6"": {
              ""type"": ""boolean""
            }
          }
        },
        ""property7"": {
          ""minLength"": 2,
          ""type"": ""string""
        }
      }
    }
  },
  ""nullable"": true,
  ""externalDocs"": {
    ""url"": ""http://example.com/externalDocs""
  }
}";

            // Act
            var actual = AdvancedSchemaObject.SerializeAsJson();

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeAdvancedSchemaWithAllOfAsV3JsonWorks()
        {
            // Arrange
            var expected = @"{
  ""title"": ""title1"",
  ""allOf"": [
    {
      ""title"": ""title2"",
      ""properties"": {
        ""property1"": {
          ""type"": ""integer""
        },
        ""property2"": {
          ""maxLength"": 15,
          ""type"": ""string""
        }
      }
    },
    {
      ""title"": ""title3"",
      ""properties"": {
        ""property3"": {
          ""properties"": {
            ""property4"": {
              ""type"": ""boolean""
            }
          }
        },
        ""property5"": {
          ""minLength"": 2,
          ""type"": ""string""
        }
      },
      ""nullable"": true
    }
  ],
  ""nullable"": true,
  ""externalDocs"": {
    ""url"": ""http://example.com/externalDocs""
  }
}";

            // Act
            var actual = AdvancedSchemaWithAllOf.SerializeAsJson();

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }
    }
}