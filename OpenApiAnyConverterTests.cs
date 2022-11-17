// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Globalization;
using System.IO;
using System.Linq;
using FluentAssertions;
using Microsoft.OpenApi.Any;
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
aTimeSpan: 01:00:00
aDate: 2017-01-02
                ";
            var yamlStream = new YamlStream();
            yamlStream.Load(new StringReader(input));
            var yamlNode = yamlStream.Documents.First().RootNode;

            var diagnostic = new OpenApiDiagnostic();
            var context = new ParsingContext(diagnostic);

            var node = new MapNode(context, (YamlMappingNode)yamlNode);

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
                    ["aTimeSpan"] = new OpenApiSchema()
                    {
                        Type = "string",
                        Format = "date-span"
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
                new OpenApiObject
                {
                    ["aString"] = new OpenApiString("fooBar"),
                    ["aInteger"] = new OpenApiInteger(10),
                    ["aDouble"] = new OpenApiDouble(2.34),
                    ["aDateTime"] = new OpenApiDateTime(DateTimeOffset.Parse("2017-01-01", CultureInfo.InvariantCulture)),
                    ["aTimeSpan"] = new OpenApiTimeSpan(TimeSpan.FromHours(1)),
                    ["aDate"] = new OpenApiDate(DateTimeOffset.Parse("2017-01-02", CultureInfo.InvariantCulture).Date),
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

            var node = new MapNode(context, (YamlMappingNode)yamlNode);

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
                new OpenApiObject
                {
                    ["aString"] = new OpenApiString("fooBar"),
                    ["aInteger"] = new OpenApiInteger(10),
                    ["aArray"] = new OpenApiArray()
                    {
                        new OpenApiLong(1),
                        new OpenApiLong(2),
                        new OpenApiLong(3),
                    },
                    ["aNestedArray"] = new OpenApiArray()
                    {
                        new OpenApiObject()
                        {
                            ["aFloat"] = new OpenApiFloat(1),
                            ["aPassword"] = new OpenApiPassword("1234"),
                            ["aArray"] = new OpenApiArray()
                            {
                                new OpenApiString("abc"),
                                new OpenApiString("def")
                            },
                            ["aDictionary"] = new OpenApiObject()
                            {
                                ["arbitraryProperty"] = new OpenApiLong(1),
                                ["arbitraryProperty2"] = new OpenApiLong(2),
                            }
                        },
                        new OpenApiObject()
                        {
                            ["aFloat"] = new OpenApiFloat((float)1.6),
                            ["aArray"] = new OpenApiArray()
                            {
                                new OpenApiString("123"),
                            },
                            ["aDictionary"] = new OpenApiObject()
                            {
                                ["arbitraryProperty"] = new OpenApiLong(1),
                                ["arbitraryProperty3"] = new OpenApiLong(20),
                            }
                        }
                    },
                    ["aObject"] = new OpenApiObject()
                    {
                        ["aDate"] = new OpenApiDate(DateTimeOffset.Parse("2017-02-03", CultureInfo.InvariantCulture).Date)
                    },
                    ["aDouble"] = new OpenApiDouble(2.34),
                    ["aDateTime"] = new OpenApiDateTime(DateTimeOffset.Parse("2017-01-01", CultureInfo.InvariantCulture))
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

            var node = new MapNode(context, (YamlMappingNode)yamlNode);

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
                new OpenApiObject
                {
                    ["aString"] = new OpenApiString("fooBar"),
                    ["aInteger"] = new OpenApiInteger(10),
                    ["aArray"] = new OpenApiArray()
                    {
                            new OpenApiInteger(1),
                            new OpenApiInteger(2),
                            new OpenApiInteger(3),
                    },
                    ["aNestedArray"] = new OpenApiArray()
                    {
                            new OpenApiObject()
                            {
                                ["aFloat"] = new OpenApiInteger(1),
                                ["aPassword"] = new OpenApiInteger(1234),
                                ["aArray"] = new OpenApiArray()
                                {
                                    new OpenApiString("abc"),
                                    new OpenApiString("def")
                                },
                                ["aDictionary"] = new OpenApiObject()
                                {
                                    ["arbitraryProperty"] = new OpenApiInteger(1),
                                    ["arbitraryProperty2"] = new OpenApiInteger(2),
                                }
                            },
                            new OpenApiObject()
                            {
                                ["aFloat"] = new OpenApiDouble(1.6),
                                ["aArray"] = new OpenApiArray()
                                {
                                    new OpenApiString("123"),
                                },
                                ["aDictionary"] = new OpenApiObject()
                                {
                                    ["arbitraryProperty"] = new OpenApiInteger(1),
                                    ["arbitraryProperty3"] = new OpenApiInteger(20),
                                }
                            }
                    },
                    ["aObject"] = new OpenApiObject()
                    {
                        ["aDate"] = new OpenApiString("2017-02-03")
                    },
                    ["aDouble"] = new OpenApiDouble(2.34),
                    ["aDateTime"] = new OpenApiDateTime(DateTimeOffset.Parse("2017-01-01", CultureInfo.InvariantCulture))
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

            var node = new MapNode(context, (YamlMappingNode)yamlNode);

            var anyMap = node.CreateAny();

            anyMap = OpenApiAnyConverter.GetSpecificOpenApiAny(anyMap);

            diagnostic.Errors.Should().BeEmpty();

            anyMap.Should().BeEquivalentTo(
                new OpenApiObject
                {
                    ["aString"] = new OpenApiString("fooBar"),
                    ["aInteger"] = new OpenApiInteger(10),
                    ["aArray"] = new OpenApiArray()
                    {
                            new OpenApiInteger(1),
                            new OpenApiInteger(2),
                            new OpenApiInteger(3),
                    },
                    ["aNestedArray"] = new OpenApiArray()
                    {
                            new OpenApiObject()
                            {
                                ["aFloat"] = new OpenApiInteger(1),
                                ["aPassword"] = new OpenApiInteger(1234),
                                ["aArray"] = new OpenApiArray()
                                {
                                    new OpenApiString("abc"),
                                    new OpenApiString("def")
                                },
                                ["aDictionary"] = new OpenApiObject()
                                {
                                    ["arbitraryProperty"] = new OpenApiInteger(1),
                                    ["arbitraryProperty2"] = new OpenApiInteger(2),
                                }
                            },
                            new OpenApiObject()
                            {
                                ["aFloat"] = new OpenApiDouble(1.6),
                                ["aArray"] = new OpenApiArray()
                                {
                                    new OpenApiInteger(123),
                                },
                                ["aDictionary"] = new OpenApiObject()
                                {
                                    ["arbitraryProperty"] = new OpenApiInteger(1),
                                    ["arbitraryProperty3"] = new OpenApiInteger(20),
                                }
                            }
                    },
                    ["aObject"] = new OpenApiObject()
                    {
                        ["aDate"] = new OpenApiDateTime(DateTimeOffset.Parse("2017-02-03", CultureInfo.InvariantCulture))
                    },
                    ["aDouble"] = new OpenApiDouble(2.34),
                    ["aDateTime"] = new OpenApiDateTime(DateTimeOffset.Parse("2017-01-01", CultureInfo.InvariantCulture))
                });
        }
    }
}
