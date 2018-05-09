// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;
using System.IO;
using FluentAssertions;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.OpenApi.Tests.Models
{
    [Collection("DefaultSettings")]
    public class OpenApiSchemaTests
    {
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
            ExternalDocs = new OpenApiExternalDocs
            {
                Url = new Uri("http://example.com/externalDocs")
            }
        };

        public static OpenApiSchema AdvancedSchemaObject = new OpenApiSchema
        {
            Title = "title1",
            Properties = new Dictionary<string, OpenApiSchema>
            {
                ["property1"] = new OpenApiSchema
                {
                    Properties = new Dictionary<string, OpenApiSchema>
                    {
                        ["property2"] = new OpenApiSchema
                        {
                            Type = "integer"
                        },
                        ["property3"] = new OpenApiSchema
                        {
                            Type = "string",
                            MaxLength = 15
                        }
                    },
                },
                ["property4"] = new OpenApiSchema
                {
                    Properties = new Dictionary<string, OpenApiSchema>
                    {
                        ["property5"] = new OpenApiSchema
                        {
                            Properties = new Dictionary<string, OpenApiSchema>
                            {
                                ["property6"] = new OpenApiSchema
                                {
                                    Type = "boolean"
                                }
                            }
                        },
                        ["property7"] = new OpenApiSchema
                        {
                            Type = "string",
                            MinLength = 2
                        }
                    },
                },
            },
            Nullable = true,
            ExternalDocs = new OpenApiExternalDocs
            {
                Url = new Uri("http://example.com/externalDocs")
            }
        };

        public static OpenApiSchema AdvancedSchemaWithAllOf = new OpenApiSchema
        {
            Title = "title1",
            AllOf = new List<OpenApiSchema>
            {
                new OpenApiSchema
                {
                    Title = "title2",
                    Properties = new Dictionary<string, OpenApiSchema>
                    {
                        ["property1"] = new OpenApiSchema
                        {
                            Type = "integer"
                        },
                        ["property2"] = new OpenApiSchema
                        {
                            Type = "string",
                            MaxLength = 15
                        }
                    },
                },
                new OpenApiSchema
                {
                    Title = "title3",
                    Properties = new Dictionary<string, OpenApiSchema>
                    {
                        ["property3"] = new OpenApiSchema
                        {
                            Properties = new Dictionary<string, OpenApiSchema>
                            {
                                ["property4"] = new OpenApiSchema
                                {
                                    Type = "boolean"
                                }
                            }
                        },
                        ["property5"] = new OpenApiSchema
                        {
                            Type = "string",
                            MinLength = 2
                        }
                    },
                    Nullable = true
                },
            },
            Nullable = true,
            ExternalDocs = new OpenApiExternalDocs
            {
                Url = new Uri("http://example.com/externalDocs")
            }
        };

        public static OpenApiSchema ReferencedSchema = new OpenApiSchema
        {
            Title = "title1",
            MultipleOf = 3,
            Maximum = 42,
            ExclusiveMinimum = true,
            Minimum = 10,
            Default = new OpenApiInteger(15),
            Type = "integer",

            Nullable = true,
            ExternalDocs = new OpenApiExternalDocs
            {
                Url = new Uri("http://example.com/externalDocs")
            },

            Reference = new OpenApiReference
            {
                Type = ReferenceType.Schema,
                Id = "schemaObject1"
            }
        };

        public static OpenApiSchema AdvancedSchemaWithRequiredPropertiesObject = new OpenApiSchema
        {
            Title = "title1",
            Required = new HashSet<string>(){ "property1" },
            Properties = new Dictionary<string, OpenApiSchema>
            {
                ["property1"] = new OpenApiSchema
                {
                    Required = new HashSet<string>() { "property3" },
                    Properties = new Dictionary<string, OpenApiSchema>
                    {
                        ["property2"] = new OpenApiSchema
                        {
                            Type = "integer"
                        },
                        ["property3"] = new OpenApiSchema
                        {
                            Type = "string",
                            MaxLength = 15,
                            ReadOnly = true
                        }
                    },
                    ReadOnly = true,
                },
                ["property4"] = new OpenApiSchema
                {
                    Properties = new Dictionary<string, OpenApiSchema>
                    {
                        ["property5"] = new OpenApiSchema
                        {
                            Properties = new Dictionary<string, OpenApiSchema>
                            {
                                ["property6"] = new OpenApiSchema
                                {
                                    Type = "boolean"
                                }
                            }
                        },
                        ["property7"] = new OpenApiSchema
                        {
                            Type = "string",
                            MinLength = 2
                        }
                    },
                    ReadOnly = true,
                },
            },
            Nullable = true,
            ExternalDocs = new OpenApiExternalDocs
            {
                Url = new Uri("http://example.com/externalDocs")
            }
        };

        private readonly ITestOutputHelper _output;

        public OpenApiSchemaTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void SerializeBasicSchemaAsV3JsonWorks()
        {
            // Arrange
            var expected = @"{ }";

            // Act
            var actual = BasicSchema.SerializeAsJson(OpenApiSpecVersion.OpenApi3_0);

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
            var actual = AdvancedSchemaNumber.SerializeAsJson(OpenApiSpecVersion.OpenApi3_0);

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
            var actual = AdvancedSchemaObject.SerializeAsJson(OpenApiSpecVersion.OpenApi3_0);

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
            var actual = AdvancedSchemaWithAllOf.SerializeAsJson(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeReferencedSchemaAsV3WithoutReferenceJsonWorks()
        {
            // Arrange
            var outputStringWriter = new StringWriter();
            var writer = new OpenApiJsonWriter(outputStringWriter);

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
            ReferencedSchema.SerializeAsV3WithoutReference(writer);
            writer.Flush();
            var actual = outputStringWriter.GetStringBuilder().ToString();

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeReferencedSchemaAsV3JsonWorks()
        {
            // Arrange
            var outputStringWriter = new StringWriter();
            var writer = new OpenApiJsonWriter(outputStringWriter);

            var expected = @"{
  ""$ref"": ""#/components/schemas/schemaObject1""
}";

            // Act
            ReferencedSchema.SerializeAsV3(writer);
            writer.Flush();
            var actual = outputStringWriter.GetStringBuilder().ToString();

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeSchemaWRequiredPropertiesAsV2JsonWorks()
        {
            // Arrange
            var outputStringWriter = new StringWriter();
            var writer = new OpenApiJsonWriter(outputStringWriter);
            var expected = @"{
  ""title"": ""title1"",
  ""required"": [
    ""property1""
  ],
  ""properties"": {
    ""property1"": {
      ""required"": [
        ""property3""
      ],
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
      },
      ""readOnly"": true
    }
  },
  ""externalDocs"": {
    ""url"": ""http://example.com/externalDocs""
  }
}";

            // Act
            AdvancedSchemaWithRequiredPropertiesObject.SerializeAsV2(writer);
            writer.Flush();
            var actual = outputStringWriter.GetStringBuilder().ToString();

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }
    }
}