﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Services;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.OpenApi.Tests.Services
{
    [Collection("DefaultSettings")]
    public class OpenApiResponsesComparerTests
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
                Responses = new Dictionary<string, OpenApiResponse>
                {
                    ["responseObject1"] = new OpenApiResponse
                    {
                        Description = "description",
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
                    ["responseObject2"] = new OpenApiResponse
                    {
                        Description = "description",
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
                Responses = new Dictionary<string, OpenApiResponse>
                {
                    ["responseObject1"] = new OpenApiResponse
                    {
                        Description = "description",
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
                    ["responseObject2"] = new OpenApiResponse
                    {
                        Description = "description",
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

        public OpenApiResponsesComparerTests(ITestOutputHelper output)
        {
            _output = output;
        }

        public static IEnumerable<object[]> GetTestCasesForOpenApiResponsesComparerShouldSucceed()
        {
            // Differences in description
            yield return new object[]
            {
                "Differences in description",
                new OpenApiResponses
                {
                    {
                        "200",
                        new OpenApiResponse
                        {
                            Description = "A complex object array response",
                            Content =
                            {
                                ["text/plain"] = new OpenApiMediaType
                                {
                                    Schema = new OpenApiSchema
                                    {
                                        Type = "string"
                                    }
                                }
                            }
                        }
                    }
                },
                new OpenApiResponses
                {
                    {
                        "200",
                        new OpenApiResponse
                        {
                            Description = "An updated complex object array response",
                            Content =
                            {
                                ["text/plain"] = new OpenApiMediaType
                                {
                                    Schema = new OpenApiSchema
                                    {
                                        Type = "string"
                                    }
                                }
                            }
                        }
                    }
                },
                new List<OpenApiDifference>
                {
                    new OpenApiDifference
                    {
                        Pointer = "#/200/description",
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Update,
                        OpenApiComparedElementType = typeof(string)
                    }
                }
            };

            // New response code
            yield return new object[]
            {
                "New response code",
                new OpenApiResponses
                {
                    {
                        "200",
                        new OpenApiResponse
                        {
                            Description = "A complex object array response",
                            Content =
                            {
                                ["text/plain"] = new OpenApiMediaType
                                {
                                    Schema = new OpenApiSchema
                                    {
                                        Type = "array",
                                        Items = new OpenApiSchema
                                        {
                                            Reference = new OpenApiReference
                                            {
                                                Type = ReferenceType.Schema,
                                                Id = "schemaObject1"
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                },
                new OpenApiResponses
                {
                    {
                        "400",
                        new OpenApiResponse
                        {
                            Description = "An updated complex object array response",
                            Content =
                            {
                                ["text/plain"] = new OpenApiMediaType
                                {
                                    Schema = new OpenApiSchema
                                    {
                                        Type = "array",
                                        Items = new OpenApiSchema
                                        {
                                            Reference = new OpenApiReference
                                            {
                                                Type = ReferenceType.Schema,
                                                Id = "schemaObject1"
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                },
                new List<OpenApiDifference>
                {
                    new OpenApiDifference
                    {
                        Pointer = "#/400",
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Add,
                        OpenApiComparedElementType = typeof(KeyValuePair<string, OpenApiResponse>)
                    },
                    new OpenApiDifference
                    {
                        Pointer = "#/200",
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Remove,
                        OpenApiComparedElementType = typeof(KeyValuePair<string, OpenApiResponse>)
                    }
                }
            };

            // Differences in Content
            yield return new object[]
            {
                "Differences in Content",
                new OpenApiResponses
                {
                    {
                        "200",
                        new OpenApiResponse
                        {
                            Description = "A complex object array response",
                            Content =
                            {
                                ["text/plain"] = new OpenApiMediaType
                                {
                                    Schema = new OpenApiSchema
                                    {
                                        Type = "array",
                                        Items = new OpenApiSchema
                                        {
                                            Reference = new OpenApiReference
                                            {
                                                Type = ReferenceType.Schema,
                                                Id = "schemaObject1"
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                },
                new OpenApiResponses
                {
                    {
                        "200",
                        new OpenApiResponse
                        {
                            Description = "A complex object array response",
                            Content =
                            {
                                ["application/json"] = new OpenApiMediaType
                                {
                                    Schema = new OpenApiSchema
                                    {
                                        Type = "array",
                                        Items = new OpenApiSchema
                                        {
                                            Reference = new OpenApiReference
                                            {
                                                Type = ReferenceType.Schema,
                                                Id = "schemaObject1"
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                },
                new List<OpenApiDifference>
                {
                    new OpenApiDifference
                    {
                        Pointer = "#/200/content/application~1json",
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Add,
                        OpenApiComparedElementType = typeof(KeyValuePair<string, OpenApiMediaType>)
                    },
                    new OpenApiDifference
                    {
                        Pointer = "#/200/content/text~1plain",
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Remove,
                        OpenApiComparedElementType = typeof(KeyValuePair<string, OpenApiMediaType>)
                    }
                }
            };

            // Null source
            yield return new object[]
            {
                "Null source",
                null,
                new OpenApiResponses
                {
                    {
                        "200",
                        new OpenApiResponse
                        {
                            Description = "An updated complex object array response",
                            Content =
                            {
                                ["application/json"] = new OpenApiMediaType
                                {
                                    Schema = new OpenApiSchema
                                    {
                                        Type = "array",
                                        Items = new OpenApiSchema
                                        {
                                            Reference = new OpenApiReference
                                            {
                                                Type = ReferenceType.Schema,
                                                Id = "schemaObject1"
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                },
                new List<OpenApiDifference>
                {
                    new OpenApiDifference
                    {
                        Pointer = "#/",
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Update,
                        OpenApiComparedElementType = typeof(IDictionary<string, OpenApiResponse>)
                    }
                }
            };

            // Null target
            yield return new object[]
            {
                "Null target",
                new OpenApiResponses
                {
                    {
                        "200",
                        new OpenApiResponse
                        {
                            Description = "An updated complex object array response",
                            Content =
                            {
                                ["application/json"] = new OpenApiMediaType
                                {
                                    Schema = new OpenApiSchema
                                    {
                                        Type = "array",
                                        Items = new OpenApiSchema
                                        {
                                            Reference = new OpenApiReference
                                            {
                                                Type = ReferenceType.Schema,
                                                Id = "schemaObject1"
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                },
                null,
                new List<OpenApiDifference>
                {
                    new OpenApiDifference
                    {
                        Pointer = "#/",
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Update,
                        OpenApiComparedElementType = typeof(IDictionary<string, OpenApiResponse>)
                    }
                }
            };

            // Differences in reference id
            yield return new object[]
            {
                "Differences in reference id",
                new OpenApiResponses
                {
                    {
                        "200",
                        new OpenApiResponse
                        {
                            Description = "A complex object array response",
                            Reference = new OpenApiReference
                            {
                                Id = "responseObject1",
                                Type = ReferenceType.Response
                            }
                        }
                    }
                },
                new OpenApiResponses
                {
                    {
                        "200",
                        new OpenApiResponse
                        {
                            Description = "A complex object array response",
                            Reference = new OpenApiReference
                            {
                                Id = "responseObject2",
                                Type = ReferenceType.Response
                            }
                        }
                    }
                },
                new List<OpenApiDifference>
                {
                    new OpenApiDifference
                    {
                        Pointer = "#/200/$ref",
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Update,
                        OpenApiComparedElementType = typeof(OpenApiReference)
                    }
                }
            };

            // Differences in schema
            yield return new object[]
            {
                "Differences in schema",
                new OpenApiResponses
                {
                    {
                        "200",
                        new OpenApiResponse
                        {
                            Description = "A complex object array response",
                            Reference = new OpenApiReference
                            {
                                Id = "responseObject1",
                                Type = ReferenceType.Response
                            }
                        }
                    }
                },
                new OpenApiResponses
                {
                    {
                        "200",
                        new OpenApiResponse
                        {
                            Description = "A complex object array response",
                            Reference = new OpenApiReference
                            {
                                Id = "responseObject1",
                                Type = ReferenceType.Response
                            }
                        }
                    }
                },
                new List<OpenApiDifference>
                {
                    new OpenApiDifference
                    {
                        Pointer = "#/200/content/application~1json/properties/property5",
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Add,
                        OpenApiComparedElementType = typeof(KeyValuePair<string, OpenApiSchema>)
                    },
                    new OpenApiDifference
                    {
                        Pointer = "#/200/content/application~1json/properties/property7",
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Remove,
                        OpenApiComparedElementType = typeof(KeyValuePair<string, OpenApiSchema>)
                    },
                    new OpenApiDifference
                    {
                        Pointer =
                            "#/200/content/application~1json/properties/property6/properties/property6/properties/property5",
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Add,
                        OpenApiComparedElementType = typeof(KeyValuePair<string, OpenApiSchema>)
                    },
                    new OpenApiDifference
                    {
                        Pointer =
                            "#/200/content/application~1json/properties/property6/properties/property6/properties/property7",
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Remove,
                        OpenApiComparedElementType = typeof(KeyValuePair<string, OpenApiSchema>)
                    }
                }
            };
        }

        [Theory]
        [MemberData(nameof(GetTestCasesForOpenApiResponsesComparerShouldSucceed))]
        public void OpenApiResponsesComparerShouldSucceed(
            string testCaseName,
            OpenApiResponses source,
            OpenApiResponses target,
            List<OpenApiDifference> expectedDifferences)
        {
            _output.WriteLine(testCaseName);

            var comparisonContext = new ComparisonContext(new OpenApiComparerFactory(), _sourceDocument,
                _targetDocument);
            var comparer = new OpenApiDictionaryComparer<OpenApiResponse>();
            comparer.Compare(source, target, comparisonContext);

            var differences = comparisonContext.OpenApiDifferences.ToList();

            differences.Count().ShouldBeEquivalentTo(expectedDifferences.Count);

            for (var i = 0; i < differences.Count(); i++)
            {
                differences[i].Pointer.ShouldBeEquivalentTo(expectedDifferences[i].Pointer);
                differences[i].OpenApiComparedElementType
                    .ShouldBeEquivalentTo(expectedDifferences[i].OpenApiComparedElementType);
                differences[i].OpenApiDifferenceOperation
                    .ShouldBeEquivalentTo(expectedDifferences[i].OpenApiDifferenceOperation);
            }
        }
    }
}