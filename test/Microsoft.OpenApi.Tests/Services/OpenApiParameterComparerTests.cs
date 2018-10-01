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
    public class OpenApiParameterComparerTests
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

        public OpenApiParameterComparerTests(ITestOutputHelper output)
        {
            _output = output;
        }

        public static IEnumerable<object[]> GetTestCasesForOpenApiParameterComparerShouldSucceed()
        {
            // Source and Target are null
            yield return new object[]
            {
                "Source and Target are null",
                null,
                null,
                new List<OpenApiDifference>()
            };

            // Source is null
            yield return new object[]
            {
                "Source is null",
                null,
                new OpenApiParameter
                {
                    Name = "pathParam",
                    In = ParameterLocation.Path
                },
                new List<OpenApiDifference>
                {
                    new OpenApiDifference
                    {
                        Pointer = "#/",
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Update,
                        OpenApiComparedElementType = typeof(OpenApiParameter),
                        SourceValue = null,
                        TargetValue = new OpenApiParameter
                        {
                            Name = "pathParam",
                            In = ParameterLocation.Path
                        }
                    }
                }
            };

            // Target is null
            yield return new object[]
            {
                "Target is null",
                new OpenApiParameter
                {
                    Name = "pathParam",
                    In = ParameterLocation.Path
                },
                null,
                new List<OpenApiDifference>
                {
                    new OpenApiDifference
                    {
                        Pointer = "#/",
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Update,
                        OpenApiComparedElementType = typeof(OpenApiParameter),
                        TargetValue = null,
                        SourceValue = new OpenApiParameter
                        {
                            Name = "pathParam",
                            In = ParameterLocation.Path
                        }
                    }
                }
            };

            // Differences in target and source
            yield return new object[]
            {
                "Differences in target and source",
                new OpenApiParameter
                {
                    Name = "pathParam",
                    Description = "Sample path parameter description",
                    In = ParameterLocation.Path,
                    Required = true,
                    AllowEmptyValue = true,
                    AllowReserved = true,
                    Style = ParameterStyle.Form,
                    Deprecated = false,
                    Explode = false,
                    Schema = new OpenApiSchema
                    {
                        Type = "string",
                        MaxLength = 15
                    },
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
                new OpenApiParameter
                {
                    Name = "pathParamUpdate",
                    Description = "Updated Sample path parameter description",
                    In = ParameterLocation.Query,
                    Required = false,
                    AllowEmptyValue = false,
                    AllowReserved = false,
                    Style = ParameterStyle.Label,
                    Deprecated = true,
                    Explode = true,
                    Schema = new OpenApiSchema
                    {
                        Type = "bool",
                        MaxLength = 15
                    },
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
                },
                new List<OpenApiDifference>
                {
                    new OpenApiDifference
                    {
                        Pointer = "#/content/text~1plain",
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Add,
                        OpenApiComparedElementType = typeof(KeyValuePair<string, OpenApiMediaType>),
                        TargetValue = new KeyValuePair<string, OpenApiMediaType>("text/plain", new OpenApiMediaType
                        {
                            Schema = new OpenApiSchema
                            {
                                Type = "string"
                            }
                        }),
                        SourceValue = null
                    },
                    new OpenApiDifference
                    {
                        Pointer = "#/content/application~1json",
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Remove,
                        OpenApiComparedElementType = typeof(KeyValuePair<string, OpenApiMediaType>),
                        SourceValue = new KeyValuePair<string, OpenApiMediaType>("application/json",
                            new OpenApiMediaType
                            {
                                Schema = new OpenApiSchema
                                {
                                    Type = "string"
                                }
                            }),
                        TargetValue = null
                    },
                    new OpenApiDifference
                    {
                        Pointer = "#/description",
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Update,
                        OpenApiComparedElementType = typeof(string),
                        SourceValue = "Sample path parameter description",
                        TargetValue = "Updated Sample path parameter description"
                    },
                    new OpenApiDifference
                    {
                        Pointer = "#/required",
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Update,
                        OpenApiComparedElementType = typeof(bool?),
                        TargetValue = false,
                        SourceValue = true
                    },
                    new OpenApiDifference
                    {
                        Pointer = "#/name",
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Update,
                        OpenApiComparedElementType = typeof(string),
                        SourceValue = "pathParam",
                        TargetValue = "pathParamUpdate"
                    },
                    new OpenApiDifference
                    {
                        Pointer = "#/deprecated",
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Update,
                        OpenApiComparedElementType = typeof(bool?),
                        TargetValue = true,
                        SourceValue = false
                    },
                    new OpenApiDifference
                    {
                        Pointer = "#/allowEmptyValue",
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Update,
                        OpenApiComparedElementType = typeof(bool?),
                        TargetValue = false,
                        SourceValue = true
                    },
                    new OpenApiDifference
                    {
                        Pointer = "#/explode",
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Update,
                        OpenApiComparedElementType = typeof(bool?),
                        TargetValue = true,
                        SourceValue = false
                    },
                    new OpenApiDifference
                    {
                        Pointer = "#/allowReserved",
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Update,
                        OpenApiComparedElementType = typeof(bool?),
                        TargetValue = false,
                        SourceValue = true
                    },
                    new OpenApiDifference
                    {
                        Pointer = "#/style",
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Update,
                        OpenApiComparedElementType = typeof(ParameterStyle),
                        SourceValue = ParameterStyle.Form,
                        TargetValue = ParameterStyle.Label
                    },
                    new OpenApiDifference
                    {
                        Pointer = "#/in",
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Update,
                        OpenApiComparedElementType = typeof(ParameterLocation),
                        SourceValue = ParameterLocation.Path,
                        TargetValue = ParameterLocation.Query
                    },

                    new OpenApiDifference
                    {
                        Pointer = "#/schema/type",
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Update,
                        OpenApiComparedElementType = typeof(string),
                        SourceValue = "string",
                        TargetValue = "bool"
                    }
                }
            };
        }

        [Theory]
        [MemberData(nameof(GetTestCasesForOpenApiParameterComparerShouldSucceed))]
        public void OpenApiParameterComparerShouldSucceed(
            string testCaseName,
            OpenApiParameter source,
            OpenApiParameter target,
            List<OpenApiDifference> expectedDifferences)
        {
            _output.WriteLine(testCaseName);

            var comparisonContext = new ComparisonContext(new OpenApiComparerFactory(), _sourceDocument,
                _targetDocument);
            var comparer = new OpenApiParameterComparer();
            comparer.Compare(source, target, comparisonContext);

            var differences = comparisonContext.OpenApiDifferences.ToList();
            differences.Count().ShouldBeEquivalentTo(expectedDifferences.Count);

            differences.ShouldBeEquivalentTo(expectedDifferences);
        }
    }
}