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
    public class OpenApiParametersComparerTests
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

        public OpenApiParametersComparerTests(ITestOutputHelper output)
        {
            _output = output;
        }

        public static IEnumerable<object[]> GetTestCasesForOpenApiParametersComparerShouldSucceed()
        {
            // Source and Target are null
            yield return new object[]
            {
                "Source and Target are null",
                null,
                null,
                new List<OpenApiDifference>()
            };

            // Source and Target are empty
            yield return new object[]
            {
                "Source and Target are null",
                new List<OpenApiParameter>(),
                new List<OpenApiParameter>(),
                new List<OpenApiDifference>()
            };

            // Source is null
            yield return new object[]
            {
                "Source is null",
                null,
                new List<OpenApiParameter>
                {
                    new OpenApiParameter
                    {
                        Name = "pathParam1",
                        In = ParameterLocation.Path
                    }
                },
                new List<OpenApiDifference>
                {
                    new OpenApiDifference
                    {
                        Pointer = "#/",
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Update,
                        OpenApiComparedElementType = typeof(IList<OpenApiParameter>),
                        SourceValue = null,
                        TargetValue = new List<OpenApiParameter>
                        {
                            new OpenApiParameter
                            {
                                Name = "pathParam1",
                                In = ParameterLocation.Path
                            }
                        }
                    }
                }
            };

            // Target is null
            yield return new object[]
            {
                "Target is null",
                new List<OpenApiParameter>
                {
                    new OpenApiParameter
                    {
                        Name = "pathParam1",
                        In = ParameterLocation.Path
                    }
                },
                null,
                new List<OpenApiDifference>
                {
                    new OpenApiDifference
                    {
                        Pointer = "#/",
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Update,
                        OpenApiComparedElementType = typeof(IList<OpenApiParameter>),
                        TargetValue = null,
                        SourceValue = new List<OpenApiParameter>
                        {
                            new OpenApiParameter
                            {
                                Name = "pathParam1",
                                In = ParameterLocation.Path
                            }
                        }
                    }
                }
            };

            // New, Removed and Updated Parameters
            yield return new object[]
            {
                "New, Removed and Updated Parameters",
                new List<OpenApiParameter>
                {
                    new OpenApiParameter
                    {
                        Name = "pathParam1",
                        In = ParameterLocation.Path
                    },
                    new OpenApiParameter
                    {
                        Name = "pathParam2",
                        In = ParameterLocation.Path
                    },
                    new OpenApiParameter
                    {
                        Name = "pathParam3",
                        In = ParameterLocation.Path,
                        Description = "Sample path parameter description"
                    },
                    new OpenApiParameter
                    {
                        Name = "queryParam1",
                        In = ParameterLocation.Query
                    },
                    new OpenApiParameter
                    {
                        Name = "queryParam2",
                        In = ParameterLocation.Query
                    }
                },
                new List<OpenApiParameter>
                {
                    new OpenApiParameter
                    {
                        Name = "queryParam1",
                        In = ParameterLocation.Query
                    },
                    new OpenApiParameter
                    {
                        Name = "pathParam1",
                        In = ParameterLocation.Path
                    },
                    new OpenApiParameter
                    {
                        Name = "queryParam3",
                        In = ParameterLocation.Query
                    },
                    new OpenApiParameter
                    {
                        Name = "pathParam3",
                        In = ParameterLocation.Path,
                        Description = "Updated Sample path parameter description"
                    }
                },
                new List<OpenApiDifference>
                {
                    new OpenApiDifference
                    {
                        Pointer = "#/4",
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Remove,
                        OpenApiComparedElementType = typeof(OpenApiParameter),
                        TargetValue = null,
                        SourceValue = new OpenApiParameter
                        {
                            Name = "queryParam2",
                            In = ParameterLocation.Query
                        }
                    },
                    new OpenApiDifference
                    {
                        Pointer = "#/1",
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Remove,
                        OpenApiComparedElementType = typeof(OpenApiParameter),
                        TargetValue = null,
                        SourceValue = new OpenApiParameter
                        {
                            Name = "pathParam2",
                            In = ParameterLocation.Path
                        }
                    },
                    new OpenApiDifference
                    {
                        Pointer = "#/2",
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Add,
                        OpenApiComparedElementType = typeof(OpenApiParameter),
                        SourceValue = null,
                        TargetValue = new OpenApiParameter
                        {
                            Name = "queryParam3",
                            In = ParameterLocation.Query
                        }
                    },
                    new OpenApiDifference
                    {
                        Pointer = "#/3/description",
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Update,
                        OpenApiComparedElementType = typeof(string),
                        SourceValue = "Sample path parameter description",
                        TargetValue = "Updated Sample path parameter description"
                    }
                }
            };
        }

        [Theory]
        [MemberData(nameof(GetTestCasesForOpenApiParametersComparerShouldSucceed))]
        public void OpenApiParametersComparerShouldSucceed(
            string testCaseName,
            IList<OpenApiParameter> source,
            IList<OpenApiParameter> target,
            List<OpenApiDifference> expectedDifferences)
        {
            _output.WriteLine(testCaseName);

            var comparisonContext = new ComparisonContext(new OpenApiComparerFactory(), _sourceDocument,
                _targetDocument);
            var comparer = new OpenApiParametersComparer();
            comparer.Compare(source, target, comparisonContext);

            var differences = comparisonContext.OpenApiDifferences.ToList();
            differences.Count().ShouldBeEquivalentTo(expectedDifferences.Count);

            differences.ShouldBeEquivalentTo(expectedDifferences);
        }
    }
}