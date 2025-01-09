// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Models.References;
using Xunit;

namespace Microsoft.OpenApi.Tests.Models
{
    [Collection("DefaultSettings")]
    public class OpenApiComponentsTests
    {
        public static OpenApiComponents AdvancedComponents = new()
        {
            Schemas = new Dictionary<string, OpenApiSchema>
            {
                ["schema1"] = new()
                {
                    Properties = new Dictionary<string, OpenApiSchema>
                    {
                        ["property2"] = new()
                        {
                            Type = JsonSchemaType.Integer
                        },
                        ["property3"] = new()
                        {
                            Type = JsonSchemaType.String,
                            MaxLength = 15
                        }
                    }
                }
            },
            SecuritySchemes = new Dictionary<string, OpenApiSecurityScheme>
            {
                ["securityScheme1"] = new()
                {
                    Description = "description1",
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new()
                    {
                        Implicit = new()
                        {
                            Scopes = new Dictionary<string, string>
                            {
                                ["operation1:object1"] = "operation 1 on object 1",
                                ["operation2:object2"] = "operation 2 on object 2"
                            },
                            AuthorizationUrl = new("https://example.com/api/oauth")
                        }
                    }
                },
                ["securityScheme2"] = new()
                {
                    Description = "description1",
                    Type = SecuritySchemeType.OpenIdConnect,
                    Scheme = OpenApiConstants.Bearer,
                    OpenIdConnectUrl = new("https://example.com/openIdConnect")
                }
            }
        };

        public static OpenApiComponents AdvancedComponentsWithReference = new()
        {
            Schemas = new Dictionary<string, OpenApiSchema>
            {
                ["schema1"] = new()
                {
                    Properties = new Dictionary<string, OpenApiSchema>
                    {
                        ["property2"] = new()
                        {
                            Type = JsonSchemaType.Integer
                        },
                        ["property3"] = new OpenApiSchemaReference("schema2", null)                        
                    }
                },
                ["schema2"] = new()
                {
                    Properties = new Dictionary<string, OpenApiSchema>
                    {
                        ["property2"] = new()
                        {
                            Type = JsonSchemaType.Integer
                        }
                    }
                },
            },
            SecuritySchemes = new Dictionary<string, OpenApiSecurityScheme>
            {
                ["securityScheme1"] = new()
                {
                    Description = "description1",
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new()
                    {
                        Implicit = new()
                        {
                            Scopes = new Dictionary<string, string>
                            {
                                ["operation1:object1"] = "operation 1 on object 1",
                                ["operation2:object2"] = "operation 2 on object 2"
                            },
                            AuthorizationUrl = new("https://example.com/api/oauth")
                        }
                    },
                    Reference = new()
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "securityScheme1"
                    }
                },
                ["securityScheme2"] = new()
                {
                    Description = "description1",
                    Type = SecuritySchemeType.OpenIdConnect,
                    Scheme = OpenApiConstants.Bearer,
                    OpenIdConnectUrl = new("https://example.com/openIdConnect"),
                    Reference = new()
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "securityScheme2"
                    }
                }
            }
        };

        public static OpenApiComponents BasicComponents = new();

        public static OpenApiComponents BrokenComponents = new()
        {
            Schemas = new Dictionary<string, OpenApiSchema>
            {
                ["schema1"] = new()
                {
                    Type = JsonSchemaType.String
                },
                ["schema2"] = null,
                ["schema3"] = null,
                ["schema4"] = new()
                {
                    Type = JsonSchemaType.String,
                    AllOf = new List<OpenApiSchema>
                    {
                        null,
                        null,
                        new()
                        {
                            Type = JsonSchemaType.String
                        },
                        null,
                        null
                    }
                }
            }
        };

        public static OpenApiComponents TopLevelReferencingComponents = new()
        {
            Schemas =
            {
                ["schema1"] = new OpenApiSchemaReference("schema2", null),
                ["schema2"] = new()
                {
                    Type = JsonSchemaType.Object,
                    Properties =
                    {
                        ["property1"] = new()
                        {
                            Type = JsonSchemaType.String
                        }
                    }
                }
            }
        };

        public static OpenApiComponents TopLevelSelfReferencingComponentsWithOtherProperties = new()
        {
            Schemas =
            {
                ["schema1"] = new()
                {
                    Type = JsonSchemaType.Object,
                    Properties =
                    {
                        ["property1"] = new()
                        {
                            Type = JsonSchemaType.String
                        }
                    }
                },
                ["schema2"] = new()
                {
                    Type = JsonSchemaType.Object,
                    Properties =
                    {
                        ["property1"] = new()
                        {
                            Type = JsonSchemaType.String
                        }
                    }
                },
            }
        };

        public static OpenApiComponents TopLevelSelfReferencingComponents = new()
        {
            Schemas =
            {
                ["schema1"] = new OpenApiSchemaReference("schema1", null)
            }
        };

        public static OpenApiComponents ComponentsWithPathItem = new OpenApiComponents
        {
            Schemas = new Dictionary<string, OpenApiSchema>()
            {
                ["schema1"] = new OpenApiSchema()
                {
                    Properties = new Dictionary<string, OpenApiSchema>()
                    {
                        ["property2"] = new OpenApiSchema()
                        {
                            Type = JsonSchemaType.Integer
                        },
                        ["property3"] = new OpenApiSchemaReference("schema2", null)
                    }
                },                

                ["schema2"] = new()
                {
                    Properties = new Dictionary<string, OpenApiSchema>()
                    {
                        ["property2"] = new OpenApiSchema()
                        {
                            Type = JsonSchemaType.Integer
                        }
                    }
                }
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
                                        Schema = new OpenApiSchemaReference("schema1", null)
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

        [Fact]
        public async Task SerializeBasicComponentsAsJsonWorks()
        {
            // Arrange
            var expected = @"{ }";

            // Act
            var actual = await BasicComponents.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public async Task SerializeBasicComponentsAsYamlWorks()
        {
            // Arrange
            var expected = @"{ }";

            // Act
            var actual = await BasicComponents.SerializeAsYamlAsync(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public async Task SerializeAdvancedComponentsAsJsonV3Works()
        {
            // Arrange
            var expected =
                """
                {
                  "schemas": {
                    "schema1": {
                      "properties": {
                        "property2": {
                          "type": "integer"
                        },
                        "property3": {
                          "maxLength": 15,
                          "type": "string"
                        }
                      }
                    }
                  },
                  "securitySchemes": {
                    "securityScheme1": {
                      "type": "oauth2",
                      "description": "description1",
                      "flows": {
                        "implicit": {
                          "authorizationUrl": "https://example.com/api/oauth",
                          "scopes": {
                            "operation1:object1": "operation 1 on object 1",
                            "operation2:object2": "operation 2 on object 2"
                          }
                        }
                      }
                    },
                    "securityScheme2": {
                      "type": "openIdConnect",
                      "description": "description1",
                      "openIdConnectUrl": "https://example.com/openIdConnect"
                    }
                  }
                }
                """;

            // Act
            var actual = await AdvancedComponents.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public async Task SerializeAdvancedComponentsWithReferenceAsJsonV3Works()
        {
            // Arrange
            var expected =
                """
                {
                  "schemas": {
                    "schema1": {
                      "properties": {
                        "property2": {
                          "type": "integer"
                        },
                        "property3": {
                          "$ref": "#/components/schemas/schema2"
                        }
                      }
                    },
                    "schema2": {
                      "properties": {
                        "property2": {
                          "type": "integer"
                        }
                      }
                    }
                  },
                  "securitySchemes": {
                    "securityScheme1": {
                      "type": "oauth2",
                      "description": "description1",
                      "flows": {
                        "implicit": {
                          "authorizationUrl": "https://example.com/api/oauth",
                          "scopes": {
                            "operation1:object1": "operation 1 on object 1",
                            "operation2:object2": "operation 2 on object 2"
                          }
                        }
                      }
                    },
                    "securityScheme2": {
                      "type": "openIdConnect",
                      "description": "description1",
                      "openIdConnectUrl": "https://example.com/openIdConnect"
                    }
                  }
                }
                """;

            // Act
            var actual = await AdvancedComponentsWithReference.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public async Task SerializeAdvancedComponentsAsYamlV3Works()
        {
            // Arrange
            var expected =
                """
                schemas:
                  schema1:
                    properties:
                      property2:
                        type: integer
                      property3:
                        maxLength: 15
                        type: string
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
                    openIdConnectUrl: https://example.com/openIdConnect
                """;

            // Act
            var actual = await AdvancedComponents.SerializeAsYamlAsync(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public async Task SerializeAdvancedComponentsWithReferenceAsYamlV3Works()
        {
            // Arrange
            var expected =
                """
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
                    openIdConnectUrl: https://example.com/openIdConnect
                """;

            // Act
            var actual = await AdvancedComponentsWithReference.SerializeAsYamlAsync(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public async Task SerializeBrokenComponentsAsJsonV3Works()
        {
            // Arrange
            var expected = """
                {
                  "schemas": {
                    "schema1": {
                      "type": "string"
                    },
                    "schema2": null,
                    "schema3": null,
                    "schema4": {
                      "type": "string",
                      "allOf": [
                        null,
                        null,
                        {
                          "type": "string"
                        },
                        null,
                        null
                      ]
                    }
                  }
                }
                """;

            // Act
            var actual = await BrokenComponents.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public async Task SerializeBrokenComponentsAsYamlV3Works()
        {
            // Arrange
            var expected =
                """
                schemas:
                  schema1:
                    type: string
                  schema2:
                  schema3:
                  schema4:
                    type: string
                    allOf:
                      -
                      -
                      - type: string
                      -
                      -
                """;

            // Act
            var actual = await BrokenComponents.SerializeAsYamlAsync(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public async Task SerializeTopLevelReferencingComponentsAsYamlV3Works()
        {
            // Arrange
            // Arrange
            var expected =
                """
                schemas:
                  schema1:
                    $ref: '#/components/schemas/schema2'
                  schema2:
                    type: object
                    properties:
                      property1:
                        type: string
                """;

            // Act
            var actual = await TopLevelReferencingComponents.SerializeAsYamlAsync(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public async Task SerializeTopLevelSelfReferencingWithOtherPropertiesComponentsAsYamlV3Works()
        {
            // Arrange
            var expected = @"schemas:
  schema1:
    type: object
    properties:
      property1:
        type: string
  schema2:
    type: object
    properties:
      property1:
        type: string";

            // Act
            var actual = await TopLevelSelfReferencingComponentsWithOtherProperties.SerializeAsYamlAsync(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public async Task SerializeComponentsWithPathItemsAsJsonWorks()
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
            var actual = await ComponentsWithPathItem.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_1);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public async Task SerializeComponentsWithPathItemsAsYamlWorks()
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
            var actual = await ComponentsWithPathItem.SerializeAsYamlAsync(OpenApiSpecVersion.OpenApi3_1);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }
    }
}
