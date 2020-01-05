// Copyright (c) Microsoft Corporation. All rights reserved.
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
    public class OpenApiRequestBodyComparerTests
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

        public OpenApiRequestBodyComparerTests(ITestOutputHelper output)
        {
            _output = output;
        }

        public static IEnumerable<object[]> GetTestCasesForOpenApiRequestBodyComparerShouldSucceed()
        {
            // Differences in description and Required
            yield return new object[]
            {
                "Differences in description and Required",
                new OpenApiRequestBody
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
                new OpenApiRequestBody
                {
                    Description = "udpated description",
                    Required = false,
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
                new List<OpenApiDifference>
                {
                    new OpenApiDifference
                    {
                        Pointer = "#/description",
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Update,
                        OpenApiComparedElementType = typeof(string),
                        TargetValue = "udpated description",
                        SourceValue = "description"
                    },
                    new OpenApiDifference
                    {
                        Pointer = "#/required",
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Update,
                        OpenApiComparedElementType = typeof(bool?),
                        TargetValue = false,
                        SourceValue = true
                    }
                }
            };

            // Differences in Content
            yield return new object[]
            {
                "Differences in Content",
                new OpenApiRequestBody
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
                new OpenApiRequestBody
                {
                    Description = "description",
                    Required = true,
                    Content =
                    {
                        ["application/xml"] = new OpenApiMediaType
                        {
                            Schema = new OpenApiSchema
                            {
                                Type = "string"
                            }
                        }
                    }
                },
                new List<OpenApiDifference>
                {
                    new OpenApiDifference
                    {
                        Pointer = "#/content/application~1xml",
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Add,
                        OpenApiComparedElementType = typeof(OpenApiMediaType),
                        SourceValue = null,
                        TargetValue = new OpenApiMediaType
                        {
                            Schema = new OpenApiSchema
                            {
                                Type = "string"
                            }
                        }
                    },
                    new OpenApiDifference
                    {
                        Pointer = "#/content/application~1json",
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Remove,
                        OpenApiComparedElementType = typeof(OpenApiMediaType),
                        TargetValue = null,
                        SourceValue = new OpenApiMediaType
                        {
                            Schema = new OpenApiSchema
                            {
                                Type = "string"
                            }
                        }
                    }
                }
            };

            // Null source
            yield return new object[]
            {
                "Null source",
                null,
                new OpenApiRequestBody
                {
                    Description = "udpated description",
                    Required = false,
                    Content =
                    {
                        ["application/xml"] = new OpenApiMediaType
                        {
                            Schema = new OpenApiSchema
                            {
                                Type = "string"
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
                        OpenApiComparedElementType = typeof(OpenApiRequestBody),
                        SourceValue = null,
                        TargetValue = new OpenApiRequestBody
                        {
                            Description = "udpated description",
                            Required = false,
                            Content =
                            {
                                ["application/xml"] = new OpenApiMediaType
                                {
                                    Schema = new OpenApiSchema
                                    {
                                        Type = "string"
                                    }
                                }
                            }
                        }
                    }
                }
            };

            // Null target
            yield return new object[]
            {
                "Null target",
                new OpenApiRequestBody
                {
                    Description = "udpated description",
                    Required = false,
                    Content =
                    {
                        ["application/xml"] = new OpenApiMediaType
                        {
                            Schema = new OpenApiSchema
                            {
                                Type = "string"
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
                        OpenApiComparedElementType = typeof(OpenApiRequestBody),
                        SourceValue = new OpenApiRequestBody
                        {
                            Description = "udpated description",
                            Required = false,
                            Content =
                            {
                                ["application/xml"] = new OpenApiMediaType
                                {
                                    Schema = new OpenApiSchema
                                    {
                                        Type = "string"
                                    }
                                }
                            }
                        },
                        TargetValue = null
                    }
                }
            };

            // Differences in reference id
            yield return new object[]
            {
                "Differences in reference id",
                new OpenApiRequestBody
                {
                    Reference = new OpenApiReference
                    {
                        Id = "Id",
                        Type = ReferenceType.RequestBody
                    },

                    Description = "description",
                    Required = true
                },
                new OpenApiRequestBody
                {
                    Reference = new OpenApiReference
                    {
                        Id = "NewId",
                        Type = ReferenceType.RequestBody
                    },

                    Description = "description",
                    Required = true
                },
                new List<OpenApiDifference>
                {
                    new OpenApiDifference
                    {
                        Pointer = "#/$ref",
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Update,
                        OpenApiComparedElementType = typeof(OpenApiReference),
                        TargetValue = new OpenApiReference
                        {
                            Id = "NewId",
                            Type = ReferenceType.RequestBody
                        },
                        SourceValue = new OpenApiReference
                        {
                            Id = "Id",
                            Type = ReferenceType.RequestBody
                        }
                    }
                }
            };

            // Differences in schema
            yield return new object[]
            {
                "Differences in schema",
                new OpenApiRequestBody
                {
                    Reference = new OpenApiReference
                    {
                        Id = "requestBody1",
                        Type = ReferenceType.RequestBody
                    },

                    Description = "description",
                    Required = true
                },
                new OpenApiRequestBody
                {
                    Reference = new OpenApiReference
                    {
                        Id = "requestBody1",
                        Type = ReferenceType.RequestBody
                    },

                    Description = "description",
                    Required = true
                },
                new List<OpenApiDifference>
                {
                    new OpenApiDifference
                    {
                        Pointer = "#/content/application~1json/schema/properties/property5",
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
                        Pointer = "#/content/application~1json/schema/properties/property7",
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
                            "#/content/application~1json/schema/properties/property6/properties/property6/properties/property5",
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
                            "#/content/application~1json/schema/properties/property6/properties/property6/properties/property7",
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
        }

        [Theory]
        [MemberData(nameof(GetTestCasesForOpenApiRequestBodyComparerShouldSucceed))]
        public void OpenApiRequestBodyComparerShouldSucceed(
            string testCaseName,
            OpenApiRequestBody source,
            OpenApiRequestBody target,
            List<OpenApiDifference> expectedDifferences)
        {
            _output.WriteLine(testCaseName);

            var comparisonContext = new ComparisonContext(new OpenApiComparerFactory(), _sourceDocument,
                _targetDocument);
            var comparer = new OpenApiRequestBodyComparer();
            comparer.Compare(source, target, comparisonContext);

            var differences = comparisonContext.OpenApiDifferences.ToList();

            differences.Count().ShouldBeEquivalentTo(expectedDifferences.Count);

            differences.ShouldBeEquivalentTo(expectedDifferences);
        }
    }
}
