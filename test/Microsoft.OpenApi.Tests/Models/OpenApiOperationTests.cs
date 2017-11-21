// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.OpenApi.Tests.Models
{
    public class OpenApiOperationTests
    {
        private readonly ITestOutputHelper _output;

        public OpenApiOperationTests(ITestOutputHelper output)
        {
            _output = output;
        }

        public static OpenApiOperation BasicOperation = new OpenApiOperation()
        {
        };

        public static OpenApiOperation AdvancedOperation= new OpenApiOperation
        {
            Summary = "summary1",
            Description = "operationDescription",
            ExternalDocs = new OpenApiExternalDocs()
            {
                Description = "externalDocsDescription",
                Url = new Uri("http://external.com")
            },
            OperationId = "operationId1",
            Parameters = new List<OpenApiParameter>()
            {
                new OpenApiParameter()
                {
                    In = ParameterLocation.Path,
                    Name = "parameter1",
                },
                new OpenApiParameter()
                {
                    In = ParameterLocation.Header,
                    Name = "parameter2"
                }
            },
            RequestBody = new OpenApiRequestBody()
            {
                Description = "description2",
                Required = true,
                Content = new Dictionary<string, OpenApiMediaType>()
                {
                    ["application/json"] = new OpenApiMediaType()
                    {
                        Schema = new OpenApiSchema()
                        {
                            Type = "number",
                            Minimum = 5,
                            Maximum =  10
                        }
                    }
                }
            },
            Responses = new OpenApiResponses()
            {
                ["200"] = new OpenApiResponse()
                {
                    Reference = new OpenApiReference()
                    {
                        Id = "response1",
                        Type = ReferenceType.Response
                    }
                },
                ["400"] = new OpenApiResponse()
                {
                    Content = new Dictionary<string, OpenApiMediaType>()
                    {
                        ["application/json"] = new OpenApiMediaType()
                        {
                            Schema = new OpenApiSchema()
                            {
                                Type = "number",
                                Minimum = 5,
                                Maximum = 10
                            }
                        }
                    }
                }
            },
            Servers = new List<OpenApiServer>()
            {
                new OpenApiServer()
                {
                    Url = "http://server.com",
                    Description = "serverDescription"
                }
            }
        };

        public static OpenApiOperation AdvancedOperationWithTagsAndSecurity = new OpenApiOperation
        {
            Tags = new List<OpenApiTag>()
            {
                new OpenApiTag()
                {
                    Name = "tagName1",
                    Description = "tagDescription1",
                },
                new OpenApiTag()
                {
                    Reference = new OpenApiReference()
                    {
                        Id = "tagId1",
                        Type = ReferenceType.Tag
                    }
                }
            },
            Summary = "summary1",
            Description = "operationDescription",
            ExternalDocs = new OpenApiExternalDocs()
            {
                Description = "externalDocsDescription",
                Url = new Uri("http://external.com")
            },
            OperationId = "operationId1",
            Parameters = new List<OpenApiParameter>()
            {
                new OpenApiParameter()
                {
                    In = ParameterLocation.Path,
                    Name = "parameter1"
                },
                new OpenApiParameter()
                {
                    In = ParameterLocation.Header,
                    Name = "parameter2"
                }
            },
            RequestBody = new OpenApiRequestBody()
            {
                Description = "description2",
                Required = true,
                Content = new Dictionary<string, OpenApiMediaType>()
                {
                    ["application/json"] = new OpenApiMediaType()
                    {
                        Schema = new OpenApiSchema()
                        {
                            Type = "number",
                            Minimum = 5,
                            Maximum = 10
                        }
                    }
                }
            },
            Responses = new OpenApiResponses()
            {
                ["200"] = new OpenApiResponse()
                {
                    Reference = new OpenApiReference()
                    {
                        Id = "response1",
                        Type = ReferenceType.Response
                    }
                },
                ["400"] = new OpenApiResponse()
                {
                    Content = new Dictionary<string, OpenApiMediaType>()
                    {
                        ["application/json"] = new OpenApiMediaType()
                        {
                            Schema = new OpenApiSchema()
                            {
                                Type = "number",
                                Minimum = 5,
                                Maximum = 10
                            }
                        }
                    }
                }
            },
            Security = new List<OpenApiSecurityRequirement>()
            {
                new OpenApiSecurityRequirement()
                {
                    [new OpenApiSecurityScheme()
                    {
                        Reference = new OpenApiReference()
                        {
                            Id = "securitySchemeId1",
                            Type = ReferenceType.SecurityScheme
                        }
                    }] = new List<string>(),
                    [new OpenApiSecurityScheme()
                    {
                        Reference = new OpenApiReference()
                        {
                            Id = "securitySchemeId2",
                            Type = ReferenceType.SecurityScheme
                        }
                    }] = new List<string>()
                    {
                        "scopeName1",
                        "scopeName2"
                    }
                }
            },
            Servers = new List<OpenApiServer>()
            {
                new OpenApiServer()
                {
                    Url = "http://server.com",
                    Description = "serverDescription"
                }
            }
        };

        [Fact]
        public void SerializeBasicOperationAsV3JsonWorks()
        {
            // Arrange
            var expected = @"{
  ""responses"": { }
}";

            // Act
                var actual = BasicOperation.SerializeAsJson(OpenApiSpecVersion.OpenApi3_0_0);

            _output.WriteLine(actual);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeAdvancedOperationAsV3JsonWorks()
        {
            // Arrange
            var expected = @"{
  ""summary"": ""summary1"",
  ""description"": ""operationDescription"",
  ""externalDocs"": {
    ""description"": ""externalDocsDescription"",
    ""url"": ""http://external.com""
  },
  ""operationId"": ""operationId1"",
  ""parameters"": [
    {
      ""name"": ""parameter1"",
      ""in"": ""path""
    },
    {
      ""name"": ""parameter2"",
      ""in"": ""header""
    }
  ],
  ""requestBody"": {
    ""description"": ""description2"",
    ""content"": {
      ""application/json"": {
        ""schema"": {
          ""maximum"": 10,
          ""minimum"": 5,
          ""type"": ""number""
        }
      }
    },
    ""required"": true
  },
  ""responses"": {
    ""200"": {
      ""$ref"": ""#/components/responses/response1""
    },
    ""400"": {
      ""content"": {
        ""application/json"": {
          ""schema"": {
            ""maximum"": 10,
            ""minimum"": 5,
            ""type"": ""number""
          }
        }
      }
    }
  },
  ""servers"": [
    {
      ""url"": ""http://server.com"",
      ""description"": ""serverDescription""
    }
  ]
}";

            // Act
            var actual = AdvancedOperation.SerializeAsJson(OpenApiSpecVersion.OpenApi3_0_0);

            _output.WriteLine(actual);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }
        
        [Fact]
        public void SerializeAdvancedOperationWithTagAndSecurityAsV3JsonWorks()
        {
            // Arrange
            var expected = @"{
  ""tags"": [
    ""tagName1"",
    ""tagId1""
  ],
  ""summary"": ""summary1"",
  ""description"": ""operationDescription"",
  ""externalDocs"": {
    ""description"": ""externalDocsDescription"",
    ""url"": ""http://external.com""
  },
  ""operationId"": ""operationId1"",
  ""parameters"": [
    {
      ""name"": ""parameter1"",
      ""in"": ""path""
    },
    {
      ""name"": ""parameter2"",
      ""in"": ""header""
    }
  ],
  ""requestBody"": {
    ""description"": ""description2"",
    ""content"": {
      ""application/json"": {
        ""schema"": {
          ""maximum"": 10,
          ""minimum"": 5,
          ""type"": ""number""
        }
      }
    },
    ""required"": true
  },
  ""responses"": {
    ""200"": {
      ""$ref"": ""#/components/responses/response1""
    },
    ""400"": {
      ""content"": {
        ""application/json"": {
          ""schema"": {
            ""maximum"": 10,
            ""minimum"": 5,
            ""type"": ""number""
          }
        }
      }
    }
  },
  ""security"": [
    {
      ""securitySchemeId1"": [ ],
      ""securitySchemeId2"": [
        ""scopeName1"",
        ""scopeName2""
      ]
    }
  ],
  ""servers"": [
    {
      ""url"": ""http://server.com"",
      ""description"": ""serverDescription""
    }
  ]
}";

            // Act
            var actual = AdvancedOperationWithTagsAndSecurity.SerializeAsJson(OpenApiSpecVersion.OpenApi3_0_0);

            _output.WriteLine(actual);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeBasicOperationAsV2JsonWorks()
        {
            // Arrange
            var expected = @"{
  ""responses"": { }
}";

            // Act
            var actual = BasicOperation.SerializeAsJson(OpenApiSpecVersion.OpenApi2_0);

            _output.WriteLine(actual);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeAdvancedOperationAsV2JsonWorks()
        {
            // Arrange
            var expected = @"{
  ""summary"": ""summary1"",
  ""description"": ""operationDescription"",
  ""externalDocs"": {
    ""description"": ""externalDocsDescription"",
    ""url"": ""http://external.com""
  },
  ""operationId"": ""operationId1"",
  ""consumes"": [
    ""application/json""
  ],
  ""produces"": [
    ""application/json""
  ],
  ""parameters"": [
    {
      ""name"": ""parameter1"",
      ""in"": ""Path""
    },
    {
      ""name"": ""parameter2"",
      ""in"": ""Header""
    },
    {
      ""name"": ""body"",
      ""in"": ""body"",
      ""description"": ""description2"",
      ""schema"": {
        ""maximum"": 10,
        ""minimum"": 5,
        ""type"": ""number""
      }
    }
  ],
  ""responses"": {
    ""200"": {
      ""$ref"": ""#/responses/response1""
    },
    ""400"": {
      ""schema"": {
        ""maximum"": 10,
        ""minimum"": 5,
        ""type"": ""number""
      }
    }
  },
  ""schemes"": [
    ""http""
  ]
}";

            // Act
            var actual = AdvancedOperation.SerializeAsJson(OpenApiSpecVersion.OpenApi2_0);

            _output.WriteLine(actual);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }

        [Fact]
        public void SerializeAdvancedOperationWithTagAndSecurityAsV2JsonWorks()
        {
            // Arrange
            var expected = @"{
  ""tags"": [
    ""tagName1"",
    ""tagId1""
  ],
  ""summary"": ""summary1"",
  ""description"": ""operationDescription"",
  ""externalDocs"": {
    ""description"": ""externalDocsDescription"",
    ""url"": ""http://external.com""
  },
  ""operationId"": ""operationId1"",
  ""consumes"": [
    ""application/json""
  ],
  ""produces"": [
    ""application/json""
  ],
  ""parameters"": [
    {
      ""name"": ""parameter1"",
      ""in"": ""Path""
    },
    {
      ""name"": ""parameter2"",
      ""in"": ""Header""
    },
    {
      ""name"": ""body"",
      ""in"": ""body"",
      ""description"": ""description2"",
      ""schema"": {
        ""maximum"": 10,
        ""minimum"": 5,
        ""type"": ""number""
      }
    }
  ],
  ""responses"": {
    ""200"": {
      ""$ref"": ""#/responses/response1""
    },
    ""400"": {
      ""schema"": {
        ""maximum"": 10,
        ""minimum"": 5,
        ""type"": ""number""
      }
    }
  },
  ""schemes"": [
    ""http""
  ],
  ""security"": [
    {
      ""securitySchemeId1"": [ ],
      ""securitySchemeId2"": [
        ""scopeName1"",
        ""scopeName2""
      ]
    }
  ]
}";

            // Act
            var actual = AdvancedOperationWithTagsAndSecurity.SerializeAsJson(OpenApiSpecVersion.OpenApi2_0);

            _output.WriteLine(actual);

            // Assert
            actual = actual.MakeLineBreaksEnvironmentNeutral();
            expected = expected.MakeLineBreaksEnvironmentNeutral();
            actual.Should().Be(expected);
        }
    }
}