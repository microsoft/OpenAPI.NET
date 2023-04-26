// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json.Nodes;
using FluentAssertions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers.ParseNodes;
using SharpYaml.Serialization;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.ParseNodes
{
    [Collection("DefaultSettings")]
    public class OpenApiAnyConverterTests
    {
        [Fact]
        public void ParseObjectAsAnyShouldSucceed()
        {
            var input = @"
aString: fooBar
aInteger: 10
aDouble: 2.34
aDateTime: 2017-01-01
aDate: 2017-01-02
                ";
            var yamlStream = new YamlStream();
            yamlStream.Load(new StringReader(input));
            var yamlNode = yamlStream.Documents.First().RootNode;

            var diagnostic = new OpenApiDiagnostic();
            var context = new ParsingContext(diagnostic);

            var asJsonNode = yamlNode.ToJsonNode();
            var node = new MapNode(context, asJsonNode);
            
            var anyMap = node.CreateAny();

            var schema = new OpenApiSchema()
            {
                Type = "object",
                Properties =
                {
                    ["aString"] = new OpenApiSchema()
                    {
                        Type = "string"
                    },
                    ["aInteger"] = new OpenApiSchema()
                    {
                        Type = "integer",
                        Format = "int32"
                    },
                    ["aDouble"] = new OpenApiSchema()
                    {
                        Type = "number",
                        Format = "double"
                    },
                    ["aDateTime"] = new OpenApiSchema()
                    {
                        Type = "string",
                        Format = "date-time"
                    },
                    ["aDate"] = new OpenApiSchema()
                    {
                        Type = "string",
                        Format = "date"
                    }
                }
            };

            anyMap = OpenApiAnyConverter.GetSpecificOpenApiAny(anyMap, schema);

            diagnostic.Errors.Should().BeEmpty();
            anyMap.Should().BeEquivalentTo(@"{
  ""aString"": {
    ""type"": ""string"",
    ""value"": ""fooBar""
  },
  ""aInteger"": {
    ""type"": ""integer"",
    ""value"": 10
  },
  ""aDouble"": {
    ""type"": ""number"",
    ""format"": ""double"",
    ""value"": 2.34
  },
  ""aDateTime"": {
    ""type"": ""string"",
    ""format"": ""date-time"",
    ""value"": ""2017-01-01T00:00:00+00:00""
  },
  ""aDate"": {
    ""type"": ""string"",
    ""format"": ""date"",
    ""value"": ""2017-01-02""
  }
}");
        }


        [Fact]
        public void ParseNestedObjectAsAnyShouldSucceed()
        {
            var input = @"
    aString: fooBar
    aInteger: 10
    aArray:
      - 1
      - 2
      - 3
    aNestedArray:
      - aFloat: 1
        aPassword: 1234
        aArray: [abc, def]
        aDictionary:
          arbitraryProperty: 1
          arbitraryProperty2: 2
      - aFloat: 1.6
        aArray: [123]
        aDictionary:
          arbitraryProperty: 1
          arbitraryProperty3: 20
    aObject:
      aDate: 2017-02-03
    aDouble: 2.34
    aDateTime: 2017-01-01
                    ";
            var yamlStream = new YamlStream();
            yamlStream.Load(new StringReader(input));
            var yamlNode = yamlStream.Documents.First().RootNode;

            var diagnostic = new OpenApiDiagnostic();
            var context = new ParsingContext(diagnostic);

            var asJsonNode = yamlNode.ToJsonNode();
            var node = new MapNode(context, asJsonNode);
            
            var anyMap = node.CreateAny();

            var schema = new OpenApiSchema()
            {
                Type = "object",
                Properties =
                    {
                        ["aString"] = new OpenApiSchema()
                        {
                            Type = "string"
                        },
                        ["aInteger"] = new OpenApiSchema()
                        {
                            Type = "integer",
                            Format = "int32"
                        },
                        ["aArray"] = new OpenApiSchema()
                        {
                            Type = "array",
                            Items = new OpenApiSchema()
                            {
                                Type = "integer",
                                Format = "int64"
                            }
                        },
                        ["aNestedArray"] = new OpenApiSchema()
                        {
                            Type = "array",
                            Items = new OpenApiSchema()
                            {
                                Type = "object",
                                Properties =
                                {
                                    ["aFloat"] = new OpenApiSchema()
                                    {
                                        Type = "number",
                                        Format = "float"
                                    },
                                    ["aPassword"] = new OpenApiSchema()
                                    {
                                        Type = "string",
                                        Format = "password"
                                    },
                                    ["aArray"] = new OpenApiSchema()
                                    {
                                        Type = "array",
                                        Items = new OpenApiSchema()
                                        {
                                            Type = "string",
                                        }
                                    },
                                    ["aDictionary"] = new OpenApiSchema()
                                    {
                                        Type = "object",
                                        AdditionalProperties = new OpenApiSchema()
                                        {
                                            Type = "integer",
                                            Format = "int64"
                                        }
                                    }
                                }
                            }
                        },
                        ["aObject"] = new OpenApiSchema()
                        {
                            Type = "array",
                            Properties =
                            {
                                ["aDate"] = new OpenApiSchema()
                                {
                                    Type = "string",
                                    Format = "date"
                                }
                            }
                        },
                        ["aDouble"] = new OpenApiSchema()
                        {
                            Type = "number",
                            Format = "double"
                        },
                        ["aDateTime"] = new OpenApiSchema()
                        {
                            Type = "string",
                            Format = "date-time"
                        }
                    }
            };

            anyMap = OpenApiAnyConverter.GetSpecificOpenApiAny(anyMap, schema);

            diagnostic.Errors.Should().BeEmpty();

            anyMap.Should().BeEquivalentTo(
                @"{
  ""aString"": {
    ""value"": ""fooBar""
  },
  ""aInteger"": {
    ""value"": 10
  },
  ""aArray"": {
    ""items"": [
      {
        ""value"": 1
      },
      {
        ""value"": 2
      },
      {
        ""value"": 3
      }
    ]
  },
  ""aNestedArray"": [
    {
      ""aFloat"": {
        ""value"": 1
      },
      ""aPassword"": {
        ""value"": ""1234""
      },
      ""aArray"": {
        ""items"": [
          {
            ""value"": ""abc""
          },
          {
            ""value"": ""def""
          }
        ]
      },
      ""aDictionary"": {
        ""arbitraryProperty"": {
          ""value"": 1
        },
        ""arbitraryProperty2"": {
          ""value"": 2
        }
      }
    },
    {
      ""aFloat"": {
        ""value"": 1.6
      },
      ""aArray"": {
        ""items"": [
          {
            ""value"": ""123""
          }
        ]
      },
      ""aDictionary"": {
        ""arbitraryProperty"": {
          ""value"": 1
        },
        ""arbitraryProperty3"": {
          ""value"": 20
        }
      }
    }
  ],
  ""aObject"": {
    ""aDate"": {
      ""value"": ""2017-02-03T00:00:00Z""
    }
  },
  ""aDouble"": {
    ""value"": 2.34
  },
  ""aDateTime"": {
    ""value"": ""2017-01-01T00:00:00Z""
  }
}");
        }


        [Fact]
        public void ParseNestedObjectAsAnyWithPartialSchemaShouldSucceed()
        {
            var input = @"
        aString: fooBar
        aInteger: 10
        aArray:
          - 1
          - 2
          - 3
        aNestedArray:
          - aFloat: 1
            aPassword: 1234
            aArray: [abc, def]
            aDictionary:
              arbitraryProperty: 1
              arbitraryProperty2: 2
          - aFloat: 1.6
            aArray: [123]
            aDictionary:
              arbitraryProperty: 1
              arbitraryProperty3: 20
        aObject:
          aDate: 2017-02-03
        aDouble: 2.34
        aDateTime: 2017-01-01
                        ";
            var yamlStream = new YamlStream();
            yamlStream.Load(new StringReader(input));
            var yamlNode = yamlStream.Documents.First().RootNode;

            var diagnostic = new OpenApiDiagnostic();
            var context = new ParsingContext(diagnostic);

            var asJsonNode = yamlNode.ToJsonNode();
            var node = new MapNode(context, asJsonNode);
            
            var anyMap = node.CreateAny();

            var schema = new OpenApiSchema()
            {
                Type = "object",
                Properties =
                        {
                            ["aString"] = new OpenApiSchema()
                            {
                                Type = "string"
                            },
                            ["aArray"] = new OpenApiSchema()
                            {
                                Type = "array",
                                Items = new OpenApiSchema()
                                {
                                    Type = "integer"
                                }
                            },
                            ["aNestedArray"] = new OpenApiSchema()
                            {
                                Type = "array",
                                Items = new OpenApiSchema()
                                {
                                    Type = "object",
                                    Properties =
                                    {
                                        ["aFloat"] = new OpenApiSchema()
                                        {
                                        },
                                        ["aPassword"] = new OpenApiSchema()
                                        {
                                        },
                                        ["aArray"] = new OpenApiSchema()
                                        {
                                            Type = "array",
                                            Items = new OpenApiSchema()
                                            {
                                                Type = "string",
                                            }
                                        }
                                    }
                                }
                            },
                            ["aObject"] = new OpenApiSchema()
                            {
                                Type = "array",
                                Properties =
                                {
                                    ["aDate"] = new OpenApiSchema()
                                    {
                                        Type = "string"
                                    }
                                }
                            },
                            ["aDouble"] = new OpenApiSchema()
                            {
                            },
                            ["aDateTime"] = new OpenApiSchema()
                            {
                            }
                        }
            };

            anyMap = OpenApiAnyConverter.GetSpecificOpenApiAny(anyMap, schema);

            diagnostic.Errors.Should().BeEmpty();

            anyMap.Should().BeEquivalentTo(
                @"{
  ""aString"": {
    ""value"": ""fooBar""
  },
  ""aInteger"": {
    ""value"": 10
  },
  ""aArray"": {
    ""items"": [
      {
        ""value"": 1
      },
      {
        ""value"": 2
      },
      {
        ""value"": 3
      }
    ]
  },
  ""aNestedArray"": [
    {
      ""aFloat"": {
        ""value"": 1
      },
      ""aPassword"": {
        ""value"": 1234
      },
      ""aArray"": {
        ""items"": [
          {
            ""value"": ""abc""
          },
          {
            ""value"": ""def""
          }
        ]
      },
      ""aDictionary"": {
        ""arbitraryProperty"": {
          ""value"": 1
        },
        ""arbitraryProperty2"": {
          ""value"": 2
        }
      }
    },
    {
      ""aFloat"": {
        ""value"": 1.6
      },
      ""aArray"": {
        ""items"": [
          {
            ""value"": ""123""
          }
        ]
      },
      ""aDictionary"": {
        ""arbitraryProperty"": {
          ""value"": 1
        },
        ""arbitraryProperty3"": {
          ""value"": 20
        }
      }
    }
  ],
  ""aObject"": {
    ""aDate"": {
      ""value"": ""2017-02-03""
    }
  },
  ""aDouble"": {
    ""value"": 2.34
  },
  ""aDateTime"": {
    ""value"": ""2017-01-01T00:00:00Z""
  }
}");
        }

        [Fact]
        public void ParseNestedObjectAsAnyWithoutUsingSchemaShouldSucceed()
        {
            var input = @"
        aString: fooBar
        aInteger: 10
        aArray:
          - 1
          - 2
          - 3
        aNestedArray:
          - aFloat: 1
            aPassword: 1234
            aArray: [abc, def]
            aDictionary:
              arbitraryProperty: 1
              arbitraryProperty2: 2
          - aFloat: 1.6
            aArray: [123]
            aDictionary:
              arbitraryProperty: 1
              arbitraryProperty3: 20
        aObject:
          aDate: 2017-02-03
        aDouble: 2.34
        aDateTime: 2017-01-01
                        ";
            var yamlStream = new YamlStream();
            yamlStream.Load(new StringReader(input));
            var yamlNode = yamlStream.Documents.First().RootNode;

            var diagnostic = new OpenApiDiagnostic();
            var context = new ParsingContext(diagnostic);

            var asJsonNode = yamlNode.ToJsonNode();
            var node = new MapNode(context, asJsonNode);
            
            var anyMap = node.CreateAny();

            anyMap = OpenApiAnyConverter.GetSpecificOpenApiAny(anyMap);

            diagnostic.Errors.Should().BeEmpty();

            anyMap.Should().BeEquivalentTo(
                @"{
  ""aString"": ""fooBar"",
  ""aInteger"": 10,
  ""aArray"": [
    1,
    2,
    3
  ],
  ""aNestedArray"": [
    {
      ""aFloat"": 1,
      ""aPassword"": 1234,
      ""aArray"": [
        ""abc"",
        ""def""
      ],
      ""aDictionary"": {
        ""arbitraryProperty"": 1,
        ""arbitraryProperty2"": 2
      }
    },
    {
      ""aFloat"": 1.6,
      ""aArray"": [
        123
      ],
      ""aDictionary"": {
        ""arbitraryProperty"": 1,
        ""arbitraryProperty3"": 20
      }
    }
  ],
  ""aObject"": {
    ""aDate"": ""2017-02-03T00:00:00+00:00""
  },
  ""aDouble"": 2.34,
  ""aDateTime"": ""2017-01-01T00:00:00+00:00""
}");
        }
    }
}
