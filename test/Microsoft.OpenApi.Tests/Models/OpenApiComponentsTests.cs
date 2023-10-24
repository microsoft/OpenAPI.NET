// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using FluentAssertions;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
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
                            Type = "integer"
                        },
                        ["property3"] = new()
                        {
                            Type = "string",
                            MaxLength = 15
                        }
                    },
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
                            Type = "integer"
                        },
                        ["property3"] = new()
                        {
                            Reference = new()
                            {
                                Type = ReferenceType.Schema,
                                Id = "schema2"
                            }
                        }
                    },
                    Reference = new()
                    {
                        Type = ReferenceType.Schema,
                        Id = "schema1"
                    }
                },
                ["schema2"] = new()
                {
                    Properties = new Dictionary<string, OpenApiSchema>
                    {
                        ["property2"] = new()
                        {
                            Type = "integer"
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
                    Type = "string"
                },
                ["schema2"] = null,
                ["schema3"] = null,
                ["schema4"] = new()
                {
                    Type = "string",
                    AllOf = new List<OpenApiSchema>
                    {
                        null,
                        null,
                        new()
                        {
                            Type = "string"
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
                ["schema1"] = new()
                {
                    Reference = new()
                    {
                        Type = ReferenceType.Schema,
                        Id = "schema2"
                    }
                },
                ["schema2"] = new()
                {
                    Type = "object",
                    Properties =
                    {
                        ["property1"] = new()
                        {
                            Type = "string"
                        }
                    }
                },
            }
        };

        public static OpenApiComponents TopLevelSelfReferencingComponentsWithOtherProperties = new()
        {
            Schemas =
            {
                ["schema1"] = new()
                {
                    Type = "object",
                    Properties =
                    {
                        ["property1"] = new()
                        {
                            Type = "string"
                        }
                    },
                    Reference = new()
                    {
                        Type = ReferenceType.Schema,
                        Id = "schema1"
                    }
                },
                ["schema2"] = new()
                {
                    Type = "object",
                    Properties =
                    {
                        ["property1"] = new()
                        {
                            Type = "string"
                        }
                    }
                },
            }
        };

        public static OpenApiComponents TopLevelSelfReferencingComponents = new()
        {
            Schemas =
            {
                ["schema1"] = new()
                {
                    Reference = new()
                    {
                        Type = ReferenceType.Schema,
                        Id = "schema1"
                    }
                }
            }
        };

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
            var expected =
                """
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
            var actual = TopLevelReferencingComponents.SerializeAsYaml(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeTopLevelSelfReferencingComponentsAsYamlV3Works()
        {
            // Arrange
            var expected =
                """
                schemas:
                  schema1: { }
                """;

            // Act
            var actual = TopLevelSelfReferencingComponents.SerializeAsYaml(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeTopLevelSelfReferencingWithOtherPropertiesComponentsAsYamlV3Works()
        {
            // Arrange
            var expected =
                """
                schemas:
                  schema1:
                    type: object
                    properties:
                      property1:
                        type: string
                  schema2:
                    type: object
                    properties:
                      property1:
                        type: string
                """;

            // Act
            var actual = TopLevelSelfReferencingComponentsWithOtherProperties.SerializeAsYaml(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }
    }
}
