// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;
using FluentAssertions;
using Json.Schema;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.OpenApi.Tests.Models
{
    [Collection("DefaultSettings")]
    public class OpenApiComponentsTests
    {
        public static OpenApiComponents AdvancedComponents = new OpenApiComponents
        {
            Schemas = new Dictionary<string, JsonSchema>
            {
                ["schema1"] = new JsonSchemaBuilder()
                .Properties(
                    ("property2", new JsonSchemaBuilder().Type(SchemaValueType.Integer).Build()),
                    ("property3", new JsonSchemaBuilder().Type(SchemaValueType.String).MaxLength(15).Build()))
                .Build()

            },
            SecuritySchemes = new Dictionary<string, OpenApiSecurityScheme>
            {
                ["securityScheme1"] = new OpenApiSecurityScheme
                {
                    Description = "description1",
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        Implicit = new OpenApiOAuthFlow
                        {
                            Scopes = new Dictionary<string, string>
                            {
                                ["operation1:object1"] = "operation 1 on object 1",
                                ["operation2:object2"] = "operation 2 on object 2"
                            },
                            AuthorizationUrl = new Uri("https://example.com/api/oauth")
                        }
                    }
                },
                ["securityScheme2"] = new OpenApiSecurityScheme
                {
                    Description = "description1",
                    Type = SecuritySchemeType.OpenIdConnect,
                    Scheme = OpenApiConstants.Bearer,
                    OpenIdConnectUrl = new Uri("https://example.com/openIdConnect")
                }
            }
        };

        public static OpenApiComponents AdvancedComponentsWithReference = new OpenApiComponents
        {
            Schemas = new Dictionary<string, JsonSchema>
            {
                ["schema1"] = new JsonSchemaBuilder()
                .Properties(
                    ("property2", new JsonSchemaBuilder().Type(SchemaValueType.Integer)),
                    ("property3", new JsonSchemaBuilder().Ref("#/components/schemas/schema2"))),
                ["schema2"] = new JsonSchemaBuilder()
                .Properties(
                    ("property2", new JsonSchemaBuilder().Type(SchemaValueType.Integer)))
            },
            SecuritySchemes = new Dictionary<string, OpenApiSecurityScheme>
            {
                ["securityScheme1"] = new OpenApiSecurityScheme
                {
                    Description = "description1",
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        Implicit = new OpenApiOAuthFlow
                        {
                            Scopes = new Dictionary<string, string>
                            {
                                ["operation1:object1"] = "operation 1 on object 1",
                                ["operation2:object2"] = "operation 2 on object 2"
                            },
                            AuthorizationUrl = new Uri("https://example.com/api/oauth")
                        }
                    },
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "securityScheme1"
                    }
                },
                ["securityScheme2"] = new OpenApiSecurityScheme
                {
                    Description = "description1",
                    Type = SecuritySchemeType.OpenIdConnect,
                    Scheme = OpenApiConstants.Bearer,
                    OpenIdConnectUrl = new Uri("https://example.com/openIdConnect"),
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "securityScheme2"
                    }
                }
            }
        };

        public static OpenApiComponents BasicComponents = new OpenApiComponents();

        public static OpenApiComponents BrokenComponents = new OpenApiComponents
        {
            Schemas = new Dictionary<string, JsonSchema>
            {
                ["schema1"] = new JsonSchemaBuilder().Type(SchemaValueType.String),
                ["schema4"] = new JsonSchemaBuilder()
                .Type(SchemaValueType.String)
                .AllOf(new JsonSchemaBuilder().Type(SchemaValueType.String).Build())
                .Build()
            }
        };

        public static OpenApiComponents TopLevelReferencingComponents = new OpenApiComponents()
        {
            Schemas =
            {
                ["schema1"] = new JsonSchemaBuilder()
                    .Ref("schema2").Build(),
                ["schema2"] = new JsonSchemaBuilder()
                    .Type(SchemaValueType.Object)
                    .Properties(("property1", new JsonSchemaBuilder().Type(SchemaValueType.String)))
                    .Build()
            }
        };

        public static OpenApiComponents TopLevelSelfReferencingComponentsWithOtherProperties = new OpenApiComponents()
        {
            Schemas =
            {
                ["schema1"] = new JsonSchemaBuilder()
                .Type(SchemaValueType.Object)
                .Properties(
                    ("property1", new JsonSchemaBuilder().Type(SchemaValueType.String).Ref("schema1")))
                .Build(),

                ["schema2"] = new JsonSchemaBuilder()
                .Type(SchemaValueType.Object)
                .Properties(
                    ("property1", new JsonSchemaBuilder().Type(SchemaValueType.String)))
                .Build()
            }
        };

        public static OpenApiComponents TopLevelSelfReferencingComponents = new OpenApiComponents()
        {
            Schemas =
            {
                ["schema1"] = new JsonSchemaBuilder()
                    .Ref("schema1").Build()
            }
        };

        public static OpenApiComponents ComponentsWithPathItem = new OpenApiComponents
        {
            Schemas = new Dictionary<string, JsonSchema>
            {
                ["schema1"] = new JsonSchemaBuilder()
                .Properties(
                    ("property2", new JsonSchemaBuilder().Type(SchemaValueType.Integer).Build()),
                    ("property3", new JsonSchemaBuilder().Ref("#/components/schemas/schema2").Build()))
                .Build(),

                ["schema2"] = new JsonSchemaBuilder()
                .Properties(
                    ("property2", new JsonSchemaBuilder().Type(SchemaValueType.Integer)))
                .Build()
            },
            PathItems = new Dictionary<string, OpenApiPathItem>
            {
                ["/pets"] = new OpenApiPathItem
                {
                    Operations = new Dictionary<OperationType, OpenApiOperation>
                    {
                        [OperationType.Post] = new OpenApiOperation
                        {
                            RequestBody = new OpenApiRequestBody
                            {
                                Description = "Information about a new pet in the system",
                                Content = new Dictionary<string, OpenApiMediaType>
                                {
                                    ["application/json"] = new OpenApiMediaType
                                    {
                                        Schema = new JsonSchemaBuilder().Ref("#/components/schemas/schema1")
                                    }
                                }
                            },
                            Responses = new OpenApiResponses
                            {
                                ["200"] = new OpenApiResponse
                                {
                                    Description = "Return a 200 status to indicate that the data was received successfully"
                                }
                            }
                        }
                    }
                }

            }
        };

        private readonly ITestOutputHelper _output;

        public OpenApiComponentsTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void SerializeBasicComponentsAsJsonWorks()
        {
            // Arrange
            var expected = @"{ }";

            // Act
            var actual = BasicComponents.SerializeAsJson(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeBasicComponentsAsYamlWorks()
        {
            // Arrange
            var expected = @"{ }";

            // Act
            var actual = BasicComponents.SerializeAsYaml(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeAdvancedComponentsAsJsonV3Works()
        {
            // Arrange
            var expected = @"{
  ""schemas"": {
    ""schema1"": {
      ""properties"": {
        ""property2"": {
          ""type"": ""integer""
        },
        ""property3"": {
          ""type"": ""string"",
          ""maxLength"": 15
        }
      }
    }
  },
  ""securitySchemes"": {
    ""securityScheme1"": {
      ""type"": ""oauth2"",
      ""description"": ""description1"",
      ""flows"": {
        ""implicit"": {
          ""authorizationUrl"": ""https://example.com/api/oauth"",
          ""scopes"": {
            ""operation1:object1"": ""operation 1 on object 1"",
            ""operation2:object2"": ""operation 2 on object 2""
          }
        }
      }
    },
    ""securityScheme2"": {
      ""type"": ""openIdConnect"",
      ""description"": ""description1"",
      ""openIdConnectUrl"": ""https://example.com/openIdConnect""
    }
  }
}";

            // Act
            var actual = AdvancedComponents.SerializeAsJson(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeAdvancedComponentsWithReferenceAsJsonV3Works()
        {
            // Arrange
            var expected = @"{
  ""schemas"": {
    ""schema1"": {
      ""properties"": {
        ""property2"": {
          ""type"": ""integer""
        },
        ""property3"": {
          ""$ref"": ""#/components/schemas/schema2""
        }
      }
    },
    ""schema2"": {
      ""properties"": {
        ""property2"": {
          ""type"": ""integer""
        }
      }
    }
  },
  ""securitySchemes"": {
    ""securityScheme1"": {
      ""type"": ""oauth2"",
      ""description"": ""description1"",
      ""flows"": {
        ""implicit"": {
          ""authorizationUrl"": ""https://example.com/api/oauth"",
          ""scopes"": {
            ""operation1:object1"": ""operation 1 on object 1"",
            ""operation2:object2"": ""operation 2 on object 2""
          }
        }
      }
    },
    ""securityScheme2"": {
      ""type"": ""openIdConnect"",
      ""description"": ""description1"",
      ""openIdConnectUrl"": ""https://example.com/openIdConnect""
    }
  }
}";

            // Act
            var actual = AdvancedComponentsWithReference.SerializeAsJson(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeAdvancedComponentsAsYamlV3Works()
        {
            // Arrange
            var expected = @"schemas:
  schema1:
    properties:
      property2:
        type: integer
      property3:
        type: string
        maxLength: 15
securitySchemes:
  securityScheme1:
    type: oauth2
    description: description1
    flows:
      implicit:
        authorizationUrl: https://example.com/api/oauth
        scopes:
          operation1:object1: operation 1 on object 1
          operation2:object2: operation 2 on object 2
  securityScheme2:
    type: openIdConnect
    description: description1
    openIdConnectUrl: https://example.com/openIdConnect";

            // Act
            var actual = AdvancedComponents.SerializeAsYaml(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeAdvancedComponentsWithReferenceAsYamlV3Works()
        {
            // Arrange
            var expected = @"schemas:
  schema1:
    properties:
      property2:
        type: integer
      property3:
        $ref: '#/components/schemas/schema2'
  schema2:
    properties:
      property2:
        type: integer
securitySchemes:
  securityScheme1:
    type: oauth2
    description: description1
    flows:
      implicit:
        authorizationUrl: https://example.com/api/oauth
        scopes:
          operation1:object1: operation 1 on object 1
          operation2:object2: operation 2 on object 2
  securityScheme2:
    type: openIdConnect
    description: description1
    openIdConnectUrl: https://example.com/openIdConnect";

            // Act
            var actual = AdvancedComponentsWithReference.SerializeAsYaml(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeBrokenComponentsAsJsonV3Works()
        {
            // Arrange
            var expected = @"{
  ""schemas"": {
    ""schema1"": {
      ""type"": ""string""
    },
    ""schema4"": {
      ""type"": ""string"",
      ""allOf"": [
        {
          ""type"": ""string""
        }
      ]
    }
  }
}";

            // Act
            var actual = BrokenComponents.SerializeAsJson(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeBrokenComponentsAsYamlV3Works()
        {
            // Arrange
            var expected = @"schemas:
  schema1:
    type: string
  schema4:
    type: string
    allOf:
    - type: string";

            // Act
            var actual = BrokenComponents.SerializeAsYaml(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeTopLevelReferencingComponentsAsYamlV3Works()
        {
            // Arrange
            var expected = @"schemas:
  schema1:
    $ref: schema2
  schema2:
    type: object
    properties:
      property1:
        type: string";

            // Act
            var actual = TopLevelReferencingComponents.SerializeAsYaml(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeTopLevelSelfReferencingWithOtherPropertiesComponentsAsYamlV3Works()
        {
            // Arrange
            var expected = @"schemas:
  schema1:
    type: object
    properties:
      property1:
        type: string
        $ref: schema1
  schema2:
    type: object
    properties:
      property1:
        type: string";

            // Act
            var actual = TopLevelSelfReferencingComponentsWithOtherProperties.SerializeAsYaml(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeComponentsWithPathItemsAsJsonWorks()
        {
            // Arrange
            var expected = @"{
  ""pathItems"": {
    ""/pets"": {
      ""post"": {
        ""requestBody"": {
          ""description"": ""Information about a new pet in the system"",
          ""content"": {
            ""application/json"": {
              ""schema"": {
                ""$ref"": ""#/components/schemas/schema1""
              }
            }
          }
        },
        ""responses"": {
          ""200"": {
            ""description"": ""Return a 200 status to indicate that the data was received successfully""
          }
        }
      }
    }
  },
  ""schemas"": {
    ""schema1"": {
      ""properties"": {
        ""property2"": {
          ""type"": ""integer""
        },
        ""property3"": {
          ""$ref"": ""#/components/schemas/schema2""
        }
      }
    },
    ""schema2"": {
      ""properties"": {
        ""property2"": {
          ""type"": ""integer""
        }
      }
    }
  }
}";
            // Act
            var actual = ComponentsWithPathItem.SerializeAsJson(OpenApiSpecVersion.OpenApi3_1);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeComponentsWithPathItemsAsYamlWorks()
        {
            // Arrange
            var expected = @"pathItems:
  /pets:
    post:
      requestBody:
        description: Information about a new pet in the system
        content:
          application/json:
            schema:
              $ref: '#/components/schemas/schema1'
      responses:
        '200':
          description: Return a 200 status to indicate that the data was received successfully
schemas:
  schema1:
    properties:
      property2:
        type: integer
      property3:
        $ref: '#/components/schemas/schema2'
  schema2:
    properties:
      property2:
        type: integer";

            // Act
            var actual = ComponentsWithPathItem.SerializeAsYaml(OpenApiSpecVersion.OpenApi3_1);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }
    }
}
