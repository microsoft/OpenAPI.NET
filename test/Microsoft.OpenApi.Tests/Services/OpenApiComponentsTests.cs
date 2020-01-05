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
    public class OpenApiComponentsTests
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

        public OpenApiComponentsTests(ITestOutputHelper output)
        {
            _output = output;
        }

        public static IEnumerable<object[]> GetTestCasesForOpenApiComponentsComparerShouldSucceed()
        {
            // Differences in schema and request body
            yield return new object[]
            {
                "Differences in schema and request body",
                new OpenApiComponents
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
                        }
                    }
                },
                new OpenApiComponents
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
                        }
                    }
                },
                new List<OpenApiDifference>
                {
                    new OpenApiDifference
                    {
                        Pointer = "#/requestBodies/requestBody1/content/application~1json/schema/properties/property5",
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Add,
                        OpenApiComparedElementType = typeof(OpenApiSchema),
                        SourceValue = null,
                        TargetValue = new OpenApiSchema
                        {
                            Type = "string",
                            MaxLength = 15
                        }
                    },
                    new OpenApiDifference
                    {
                        Pointer = "#/requestBodies/requestBody1/content/application~1json/schema/properties/property7",
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Remove,
                        OpenApiComparedElementType = typeof(OpenApiSchema),
                        SourceValue = new OpenApiSchema
                        {
                            Type = "string",
                            MaxLength = 15
                        },
                        TargetValue = null
                    },
                    new OpenApiDifference
                    {
                        Pointer =
                            "#/requestBodies/requestBody1/content/application~1json/schema/properties/property6/properties/property6/properties/property5",
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Add,
                        OpenApiComparedElementType = typeof(OpenApiSchema),
                        SourceValue = null,
                        TargetValue = new OpenApiSchema
                        {
                            Type = "string",
                            MaxLength = 15
                        }
                    },
                    new OpenApiDifference
                    {
                        Pointer =
                            "#/requestBodies/requestBody1/content/application~1json/schema/properties/property6/properties/property6/properties/property7",
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Remove,
                        OpenApiComparedElementType = typeof(OpenApiSchema),
                        SourceValue = new OpenApiSchema
                        {
                            Type = "string",
                            MaxLength = 15
                        },
                        TargetValue = null
                    },
                    new OpenApiDifference
                    {
                        Pointer = "#/schemas/schemaObject1/properties/property5",
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Add,
                        OpenApiComparedElementType = typeof(OpenApiSchema),
                        SourceValue = null,
                        TargetValue = new OpenApiSchema
                        {
                            Type = "string",
                            MaxLength = 15
                        }
                    },
                    new OpenApiDifference
                    {
                        Pointer = "#/schemas/schemaObject1/properties/property7",
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Remove,
                        OpenApiComparedElementType = typeof(OpenApiSchema),
                        SourceValue = new OpenApiSchema
                        {
                            Type = "string",
                            MaxLength = 15
                        },
                        TargetValue = null
                    },
                    new OpenApiDifference
                    {
                        Pointer =
                            "#/schemas/schemaObject1/properties/property6/properties/property6/properties/property5",
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Add,
                        OpenApiComparedElementType = typeof(OpenApiSchema),
                        SourceValue = null,
                        TargetValue = new OpenApiSchema
                        {
                            Type = "string",
                            MaxLength = 15
                        }
                    },
                    new OpenApiDifference
                    {
                        Pointer =
                            "#/schemas/schemaObject1/properties/property6/properties/property6/properties/property7",
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Remove,
                        OpenApiComparedElementType = typeof(OpenApiSchema),
                        SourceValue = new OpenApiSchema
                        {
                            Type = "string",
                            MaxLength = 15
                        },
                        TargetValue = null
                    },
                    new OpenApiDifference
                    {
                        Pointer = "#/schemas/schemaObject2/properties/property6/properties/property5",
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Add,
                        OpenApiComparedElementType = typeof(OpenApiSchema),
                        SourceValue = null,
                        TargetValue = new OpenApiSchema
                        {
                            Type = "string",
                            MaxLength = 15
                        }
                    },
                    new OpenApiDifference
                    {
                        Pointer = "#/schemas/schemaObject2/properties/property6/properties/property7",
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Remove,
                        OpenApiComparedElementType = typeof(OpenApiSchema),
                        SourceValue = new OpenApiSchema
                        {
                            Type = "string",
                            MaxLength = 15
                        },
                        TargetValue = null
                    },
                    new OpenApiDifference
                    {
                        Pointer =
                            "#/schemas/schemaObject2/properties/property6/properties/property6/properties/property6/properties/property5",
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Add,
                        OpenApiComparedElementType = typeof(OpenApiSchema),
                        SourceValue = null,
                        TargetValue = new OpenApiSchema
                        {
                            Type = "string",
                            MaxLength = 15
                        }
                    },
                    new OpenApiDifference
                    {
                        Pointer =
                            "#/schemas/schemaObject2/properties/property6/properties/property6/properties/property6/properties/property7",
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Remove,
                        OpenApiComparedElementType = typeof(OpenApiSchema),
                        SourceValue = new OpenApiSchema
                        {
                            Type = "string",
                            MaxLength = 15
                        },
                        TargetValue = null
                    }
                }
            };

            // New schema and request body
            yield return new object[]
            {
                "New schema and request body",
                new OpenApiComponents
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
                                        Type = "string"
                                    }
                                }
                            }
                        }
                    }
                },
                new OpenApiComponents
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
                                        Type = "string"
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
                        Pointer = "#/requestBodies/requestBody2",
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Add,
                        OpenApiComparedElementType = typeof(OpenApiRequestBody),
                        SourceValue = null,
                        TargetValue = new OpenApiRequestBody
                        {
                            Description = "description",
                            Required = true,
                            Content =
                            {
                                ["application/json"] = new OpenApiMediaType
                                {
                                    Schema = new OpenApiSchema
                                    {
                                        Type = "string"
                                    }
                                }
                            }
                        }
                    },
                    new OpenApiDifference
                    {
                        Pointer =
                            "#/schemas/schemaObject2",
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Add,
                        OpenApiComparedElementType = typeof(OpenApiSchema),
                        SourceValue = null,
                        TargetValue = new OpenApiSchema
                        {
                            Properties = new Dictionary<string, OpenApiSchema>
                            {
                                ["property2"] = new OpenApiSchema
                                {
                                    Type = "integer"
                                }
                            }
                        }
                    }
                }
            };

            // New, removed and updated examples
            yield return new object[]
            {
                "New, removed and updated examples",
                new OpenApiComponents
                {
                    Examples = new Dictionary<string, OpenApiExample>
                    {
                        ["example1"] = new OpenApiExample
                        {
                            Value = new OpenApiObject
                            {
                                ["versions"] = new OpenApiArray
                                {
                                    new OpenApiObject
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
                        },
                        ["example3"] = new OpenApiExample
                        {
                            Value = new OpenApiObject
                            {
                                ["versions"] = new OpenApiObject
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
                    }
                },
                new OpenApiComponents
                {
                    Examples = new Dictionary<string, OpenApiExample>
                    {
                        ["example2"] = new OpenApiExample
                        {
                            Value = new OpenApiObject
                            {
                                ["versions"] = new OpenApiArray
                                {
                                    new OpenApiObject
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
                        },
                        ["example3"] = new OpenApiExample
                        {
                            Value = new OpenApiObject
                            {
                                ["versions"] = new OpenApiArray
                                {
                                    new OpenApiObject
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
                        }
                    }
                },
                new List<OpenApiDifference>
                {
                    new OpenApiDifference
                    {
                        Pointer = "#/examples/example2",
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Add,
                        OpenApiComparedElementType = typeof(OpenApiExample),
                        SourceValue = null,
                        TargetValue = new OpenApiExample
                        {
                            Value = new OpenApiObject
                            {
                                ["versions"] = new OpenApiArray
                                {
                                    new OpenApiObject
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
                        }
                    },
                    new OpenApiDifference
                    {
                        Pointer = "#/examples/example1",
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Remove,
                        OpenApiComparedElementType = typeof(OpenApiExample),
                        SourceValue = new OpenApiExample
                        {
                            Value = new OpenApiObject
                            {
                                ["versions"] = new OpenApiArray
                                {
                                    new OpenApiObject
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
                        },
                        TargetValue = null
                    },
                    new OpenApiDifference
                    {
                        Pointer = "#/examples/example3/value",
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Update,
                        OpenApiComparedElementType = typeof(IOpenApiAny),
                        SourceValue = new OpenApiObject
                        {
                            ["versions"] = new OpenApiObject
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
                        TargetValue = new OpenApiObject
                        {
                            ["versions"] = new OpenApiArray
                            {
                                new OpenApiObject
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
                    }
                }
            };
        }

        [Theory]
        [MemberData(nameof(GetTestCasesForOpenApiComponentsComparerShouldSucceed))]
        public void OpenApiComponentsComparerShouldSucceed(
            string testCaseName,
            OpenApiComponents source,
            OpenApiComponents target,
            List<OpenApiDifference> expectedDifferences)
        {
            _output.WriteLine(testCaseName);

            var comparisonContext = new ComparisonContext(new OpenApiComparerFactory(), _sourceDocument,
                _targetDocument);
            var comparer = new OpenApiComponentsComparer();
            comparer.Compare(source, target, comparisonContext);

            var differences = comparisonContext.OpenApiDifferences.ToList();

            differences.Count().ShouldBeEquivalentTo(expectedDifferences.Count);

            differences.ShouldBeEquivalentTo(expectedDifferences);
        }
    }
}
