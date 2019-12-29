// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Services;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.OpenApi.Tests.Services
{
    [Collection("DefaultSettings")]
    public class OpenApiExampleComparerTests
    {
        private readonly ITestOutputHelper _output;

        private readonly OpenApiDocument _sourceDocument = new OpenApiDocument
        {
            Components = new OpenApiComponents
            {
                Schemas = new Dictionary<string, OpenApiSchema>
                {
                    ["schemaObject1"] = new OpenApiSchema
                    {
                        Properties = new Dictionary<string, OpenApiSchema>
                        {
                            ["property2"] = new OpenApiSchema
                            {
                                Type = "integer"
                            },
                            ["property7"] = new OpenApiSchema
                            {
                                Type = "string",
                                MaxLength = 15
                            },
                            ["property6"] = new OpenApiSchema
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.Schema,
                                    Id = "schemaObject2"
                                }
                            }
                        }
                    },
                    ["schemaObject2"] = new OpenApiSchema
                    {
                        Properties = new Dictionary<string, OpenApiSchema>
                        {
                            ["property2"] = new OpenApiSchema
                            {
                                Type = "integer"
                            },
                            ["property5"] = new OpenApiSchema
                            {
                                Type = "string",
                                MaxLength = 15
                            },
                            ["property6"] = new OpenApiSchema
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.Schema,
                                    Id = "schemaObject1"
                                }
                            }
                        }
                    }
                },
                RequestBodies = new Dictionary<string, OpenApiRequestBody>
                {
                    ["requestBody1"] = new OpenApiRequestBody
                    {
                        Description = "description",
                        Required = true,
                        Content =
                        {
                            ["application/json"] = new OpenApiMediaType
                            {
                                Schema = new OpenApiSchema
                                {
                                    Reference = new OpenApiReference
                                    {
                                        Id = "schemaObject1",
                                        Type = ReferenceType.Schema
                                    }
                                }
                            }
                        }
                    },
                    ["requestBody2"] = new OpenApiRequestBody
                    {
                        Description = "description",
                        Required = true,
                        Content =
                        {
                            ["application/json"] = new OpenApiMediaType
                            {
                                Schema = new OpenApiSchema
                                {
                                    Reference = new OpenApiReference
                                    {
                                        Id = "schemaObject1",
                                        Type = ReferenceType.Schema
                                    }
                                }
                            }
                        }
                    }
                }
            }
        };

        private readonly OpenApiDocument _targetDocument = new OpenApiDocument
        {
            Components = new OpenApiComponents
            {
                Schemas = new Dictionary<string, OpenApiSchema>
                {
                    ["schemaObject1"] = new OpenApiSchema
                    {
                        Properties = new Dictionary<string, OpenApiSchema>
                        {
                            ["property2"] = new OpenApiSchema
                            {
                                Type = "integer"
                            },
                            ["property5"] = new OpenApiSchema
                            {
                                Type = "string",
                                MaxLength = 15
                            },
                            ["property6"] = new OpenApiSchema
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.Schema,
                                    Id = "schemaObject2"
                                }
                            }
                        }
                    },
                    ["schemaObject2"] = new OpenApiSchema
                    {
                        Properties = new Dictionary<string, OpenApiSchema>
                        {
                            ["property2"] = new OpenApiSchema
                            {
                                Type = "integer"
                            },
                            ["property5"] = new OpenApiSchema
                            {
                                Type = "string",
                                MaxLength = 15
                            },
                            ["property6"] = new OpenApiSchema
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.Schema,
                                    Id = "schemaObject1"
                                }
                            }
                        }
                    }
                },
                RequestBodies = new Dictionary<string, OpenApiRequestBody>
                {
                    ["requestBody1"] = new OpenApiRequestBody
                    {
                        Description = "description",
                        Required = true,
                        Content =
                        {
                            ["application/json"] = new OpenApiMediaType
                            {
                                Schema = new OpenApiSchema
                                {
                                    Reference = new OpenApiReference
                                    {
                                        Id = "schemaObject1",
                                        Type = ReferenceType.Schema
                                    }
                                }
                            }
                        }
                    },
                    ["requestBody2"] = new OpenApiRequestBody
                    {
                        Description = "description",
                        Required = true,
                        Content =
                        {
                            ["application/json"] = new OpenApiMediaType
                            {
                                Schema = new OpenApiSchema
                                {
                                    Reference = new OpenApiReference
                                    {
                                        Id = "schemaObject1",
                                        Type = ReferenceType.Schema
                                    }
                                }
                            }
                        }
                    }
                }
            }
        };

        public OpenApiExampleComparerTests(ITestOutputHelper output)
        {
            _output = output;
        }

        public static IEnumerable<object[]> GetTestCasesForOpenApiExampleComparerShouldSucceed()
        {
            yield return new object[]
            {
                "Differences in description, summary and external value",
                new OpenApiExample
                {
                    Description = "Test description",
                    Summary = "Test summary",
                    ExternalValue = "http://localhost/1"
                },
                new OpenApiExample
                {
                    Description = "Test description updated",
                    Summary = "Test summary updated",
                    ExternalValue = "http://localhost/2"
                },
                new List<OpenApiDifference>
                {
                    new OpenApiDifference
                    {
                        Pointer = "#/description",
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Update,
                        OpenApiComparedElementType = typeof(string),
                        SourceValue = "Test description",
                        TargetValue = "Test description updated"
                    },
                    new OpenApiDifference
                    {
                        Pointer = "#/summary",
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Update,
                        OpenApiComparedElementType = typeof(string),
                        SourceValue = "Test summary",
                        TargetValue = "Test summary updated"
                    },
                    new OpenApiDifference
                    {
                        Pointer = "#/externalValue",
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Update,
                        OpenApiComparedElementType = typeof(string),
                        SourceValue = "http://localhost/1",
                        TargetValue = "http://localhost/2"
                    }
                }
            };

            yield return new object[]
            {
                "Null source",
                null,
                new OpenApiExample
                {
                    Description = "Test description",
                    Summary = "Test summary",
                    ExternalValue = "http://localhost/1"
                },
                new List<OpenApiDifference>
                {
                    new OpenApiDifference
                    {
                        Pointer = "#/",
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Update,
                        OpenApiComparedElementType = typeof(OpenApiExample),
                        SourceValue = null,
                        TargetValue = new OpenApiExample
                        {
                            Description = "Test description",
                            Summary = "Test summary",
                            ExternalValue = "http://localhost/1"
                        }
                    }
                }
            };

            yield return new object[]
            {
                "Null target",
                new OpenApiExample
                {
                    Description = "Test description",
                    Summary = "Test summary",
                    ExternalValue = "http://localhost/1"
                },
                null,
                new List<OpenApiDifference>
                {
                    new OpenApiDifference
                    {
                        Pointer = "#/",
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Update,
                        OpenApiComparedElementType = typeof(OpenApiExample),
                        TargetValue = null,
                        SourceValue = new OpenApiExample
                        {
                            Description = "Test description",
                            Summary = "Test summary",
                            ExternalValue = "http://localhost/1"
                        }
                    }
                }
            };

            yield return new object[]
            {
                "Difference in value",
                new OpenApiExample
                {
                    Description = "Test description",
                    Summary = "Test summary",
                    Value = new OpenApiObject
                    {
                        ["status"] = new OpenApiString("Status1"),
                        ["id"] = new OpenApiString("v1"),
                        ["links"] = new OpenApiArray
                        {
                            new OpenApiObject
                            {
                                ["href"] = new OpenApiString("http://example.com/1"),
                                ["rel"] = new OpenApiString("sampleRel1")
                            }
                        }
                    }
                },
                new OpenApiExample
                {
                    Description = "Test description",
                    Summary = "Test summary",
                    Value = new OpenApiObject
                    {
                        ["status"] = new OpenApiString("Status1"),
                        ["id"] = new OpenApiString("v1"),
                        ["links"] = new OpenApiArray
                        {
                            new OpenApiObject
                            {
                                ["href"] = new OpenApiString("http://example.com/1"),
                                ["relUpdated"] = new OpenApiString("sampleRel1")
                            }
                        }
                    }
                },
                new List<OpenApiDifference>
                {
                    new OpenApiDifference
                    {
                        Pointer = "#/value",
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Update,
                        OpenApiComparedElementType = typeof(IOpenApiAny),
                        TargetValue = new OpenApiObject
                        {
                            ["status"] = new OpenApiString("Status1"),
                            ["id"] = new OpenApiString("v1"),
                            ["links"] = new OpenApiArray
                            {
                                new OpenApiObject
                                {
                                    ["href"] = new OpenApiString("http://example.com/1"),
                                    ["relUpdated"] = new OpenApiString("sampleRel1")
                                }
                            }
                        },
                        SourceValue = new OpenApiObject
                        {
                            ["status"] = new OpenApiString("Status1"),
                            ["id"] = new OpenApiString("v1"),
                            ["links"] = new OpenApiArray
                            {
                                new OpenApiObject
                                {
                                    ["href"] = new OpenApiString("http://example.com/1"),
                                    ["rel"] = new OpenApiString("sampleRel1")
                                }
                            }
                        }
                    }
                }
            };

            yield return new object[]
            {
                "No differences",
                new OpenApiExample
                {
                    Description = "Test description",
                    Summary = "Test summary",
                    Value = new OpenApiObject
                    {
                        ["status"] = new OpenApiString("Status1"),
                        ["id"] = new OpenApiString("v1"),
                        ["links"] = new OpenApiArray
                        {
                            new OpenApiObject
                            {
                                ["href"] = new OpenApiString("http://example.com/1"),
                                ["rel"] = new OpenApiString("sampleRel1")
                            }
                        }
                    }
                },
                new OpenApiExample
                {
                    Description = "Test description",
                    Summary = "Test summary",
                    Value = new OpenApiObject
                    {
                        ["status"] = new OpenApiString("Status1"),
                        ["id"] = new OpenApiString("v1"),
                        ["links"] = new OpenApiArray
                        {
                            new OpenApiObject
                            {
                                ["href"] = new OpenApiString("http://example.com/1"),
                                ["rel"] = new OpenApiString("sampleRel1")
                            }
                        }
                    }
                },
                new List<OpenApiDifference>()
            };
        }

        [Theory]
        [MemberData(nameof(GetTestCasesForOpenApiExampleComparerShouldSucceed))]
        public void OpenApiExampleComparerShouldSucceed(
            string testCaseName,
            OpenApiExample source,
            OpenApiExample target,
            List<OpenApiDifference> expectedDifferences)
        {
            _output.WriteLine(testCaseName);

            var comparisonContext = new ComparisonContext(new OpenApiComparerFactory(), _sourceDocument,
                _targetDocument);
            var comparer = new OpenApiExampleComparer();
            comparer.Compare(source, target, comparisonContext);

            var differences = comparisonContext.OpenApiDifferences.ToList();
            differences.Count().ShouldBeEquivalentTo(expectedDifferences.Count);

            differences.ShouldBeEquivalentTo(expectedDifferences);
        }
    }
}