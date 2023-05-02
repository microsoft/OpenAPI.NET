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
            anyMap.Should().BeEquivalentTo(
                new JsonObject
                {
                    ["aString"] = "fooBar",
                    ["aInteger"] = 10,
                    ["aDouble"] = 2.34,
                    ["aDateTime"] = DateTimeOffset.Parse("2017-01-01", CultureInfo.InvariantCulture),
                    ["aDate"] = DateTimeOffset.Parse("2017-01-02", CultureInfo.InvariantCulture).Date
                });
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
                new JsonObject
                {
                    ["aString"] = "fooBar",
                    ["aInteger"] = 10,
                    ["aArray"] = new JsonArray()
                    {
                        1,2, 3
                    },
                    ["aNestedArray"] = new JsonArray()
                    {
                        new JsonObject()
                        {
                            ["aFloat"] = 1.0,
                            ["aPassword"] = "1234",
                            ["aArray"] = new JsonArray()
                            {
                                "abc",
                                "def"
                            },
                            ["aDictionary"] = new JsonObject()
                            {
                                ["arbitraryProperty"] = 1,
                                ["arbitraryProperty2"] = 2,
                            }
                        },
                        new JsonObject()
                        {
                            ["aFloat"] = (float)1.6,
                            ["aArray"] = new JsonArray()
                            {
                                "123",
                            },
                            ["aDictionary"] = new JsonObject()
                            {
                                ["arbitraryProperty"] = 1,
                                ["arbitraryProperty3"] = 20,
                            }
                        }
                    },
                    ["aObject"] = new JsonObject()
                    {
                        ["aDate"] = DateTimeOffset.Parse("2017-02-03", CultureInfo.InvariantCulture).Date
                    },
                    ["aDouble"] = 2.34,
                    ["aDateTime"] = DateTimeOffset.Parse("2017-01-01", CultureInfo.InvariantCulture)
                });
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
                new JsonObject
                {
                    ["aString"] = "fooBar",
                    ["aInteger"] = 10,
                    ["aArray"] = new JsonArray()
                    {
                            1, 2, 3
                    },
                    ["aNestedArray"] = new JsonArray()
                    {
                            new JsonObject()
                            {
                                ["aFloat"] = 1,
                                ["aPassword"] = 1234,
                                ["aArray"] = new JsonArray()
                                {
                                    "abc",
                                    "def"
                                },
                                ["aDictionary"] = new JsonObject()
                                {
                                    ["arbitraryProperty"] = 1,
                                    ["arbitraryProperty2"] = 2,
                                }
                            },
                            new JsonObject()
                            {
                                ["aFloat"] = 1.6,
                                ["aArray"] = new JsonArray()
                                {
                                    "123",
                                },
                                ["aDictionary"] = new JsonObject()
                                {
                                    ["arbitraryProperty"] = 1,
                                    ["arbitraryProperty3"] = 20,
                                }
                            }
                    },
                    ["aObject"] = new JsonObject()
                    {
                        ["aDate"] = "2017-02-03"
                    },
                    ["aDouble"] = 2.34,
                    ["aDateTime"] = DateTimeOffset.Parse("2017-01-01", CultureInfo.InvariantCulture)
                });
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
                new JsonObject()
                {
                    ["aString"] = "fooBar",
                    ["aInteger"] = 10,
                    ["aArray"] = new JsonArray()
                    {
                        1, 2, 3
                    },
                    ["aNestedArray"] = new JsonArray()
                    {
                            new JsonObject()
                            {
                                ["aFloat"] = 1,
                                ["aPassword"] = 1234,
                                ["aArray"] = new JsonArray()
                                {
                                    "abc",
                                    "def"
                                },
                                ["aDictionary"] = new JsonObject()
                                {
                                    ["arbitraryProperty"] = 1,
                                    ["arbitraryProperty2"] = 2,
                                }
                            },
                            new JsonObject()
                            {
                                ["aFloat"] = 1.6,
                                ["aArray"] = new JsonArray()
                                {
                                    123,
                                },
                                ["aDictionary"] = new JsonObject()
                                {
                                    ["arbitraryProperty"] = 1,
                                    ["arbitraryProperty3"] = 20,
                                }
                            }
                    },
                    ["aObject"] = new JsonObject()
                    {
                        ["aDate"] = DateTimeOffset.Parse("2017-02-03", CultureInfo.InvariantCulture)
                    },
                    ["aDouble"] = 2.34,
                    ["aDateTime"] = DateTimeOffset.Parse("2017-01-01", CultureInfo.InvariantCulture)
                });
        }
    }
}
