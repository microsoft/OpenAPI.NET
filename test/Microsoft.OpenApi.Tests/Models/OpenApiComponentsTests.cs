// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;
using FluentAssertions;
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
            Schemas = new Dictionary<string, OpenApiSchema>
            {
                ["schema1"] = new OpenApiSchema
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
                    Scheme = "openIdConnectUrl",
                    OpenIdConnectUrl = new Uri("https://example.com/openIdConnect")
                }
            }
        };

        public static OpenApiComponents AdvancedComponentsWithReference = new OpenApiComponents
        {
            Schemas = new Dictionary<string, OpenApiSchema>
            {
                ["schema1"] = new OpenApiSchema
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
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.Schema,
                        Id = "schema1"
                    }
                },
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
                    Scheme = "openIdConnectUrl",
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
            Schemas = new Dictionary<string, OpenApiSchema>
            {
                ["schema1"] = new OpenApiSchema
                {
                    Type = "string"
                },
                ["schema2"] = null,
                ["schema3"] = null,
                ["schema4"] = new OpenApiSchema
                {
                    Type = "string",
                    AllOf = new List<OpenApiSchema>
                    {
                        null,
                        null,
                        new OpenApiSchema
                        {
                            Type = "string"
                        },
                        null,
                        null
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
            var actual = BasicComponents.SerializeAsJson(OpenApiSpecVersion.OpenApi3_0_0);

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
            var actual = BasicComponents.SerializeAsYaml(OpenApiSpecVersion.OpenApi3_0_0);

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
          ""maxLength"": 15,
          ""type"": ""string""
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
            var actual = AdvancedComponents.SerializeAsJson(OpenApiSpecVersion.OpenApi3_0_0);

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
          ""maxLength"": 15,
          ""type"": ""string""
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
            var actual = AdvancedComponentsWithReference.SerializeAsJson(OpenApiSpecVersion.OpenApi3_0_0);

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
    openIdConnectUrl: https://example.com/openIdConnect";

            // Act
            var actual = AdvancedComponents.SerializeAsYaml(OpenApiSpecVersion.OpenApi3_0_0);

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
    openIdConnectUrl: https://example.com/openIdConnect";

            // Act
            var actual = AdvancedComponentsWithReference.SerializeAsYaml(OpenApiSpecVersion.OpenApi3_0_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeBrokenComponentsAsJsonWorks()
        {
            // Arrange
            var expected = @"{
  ""schemas"": {
    ""schema1"": {
      ""type"": ""string""
    },
    ""schema2"": null,
    ""schema3"": null,
    ""schema4"": {
      ""type"": ""string"",
      ""allOf"": [
        null,
        null,
        {
          ""type"": ""string""
        },
        null,
        null
      ]
    }
  }
}";

            // Act
            var actual = BrokenComponents.SerializeAsJson(OpenApiSpecVersion.OpenApi3_0_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeBrokenComponentsAsYamlWorks()
        {
            // Arrange
            var expected = @"schemas:
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
      - ";

            // Act
            var actual = BrokenComponents.SerializeAsYaml(OpenApiSpecVersion.OpenApi3_0_0);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }
    }
}