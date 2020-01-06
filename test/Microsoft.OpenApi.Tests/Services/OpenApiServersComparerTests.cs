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
    public class OpenApiServersComparerTests
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

        public OpenApiServersComparerTests(ITestOutputHelper output)
        {
            _output = output;
        }

        public static IEnumerable<object[]> GetTestCasesForOpenApiServersComparerShouldSucceed()
        {
            // Differences in description
            yield return new object[]
            {
                "Differences in description",
                new List<OpenApiServer>
                {
                    new OpenApiServer
                    {
                        Description = "description1",
                        Url = "https://{username}.example.com:{port}/{basePath}",
                        Variables = new Dictionary<string, OpenApiServerVariable>
                        {
                            ["username"] = new OpenApiServerVariable
                            {
                                Default = "unknown",
                                Description = "variableDescription1"
                            },
                            ["port"] = new OpenApiServerVariable
                            {
                                Default = "8443",
                                Description = "variableDescription2",
                                Enum = new List<string>
                                {
                                    "443",
                                    "8443"
                                }
                            },
                            ["basePath"] = new OpenApiServerVariable
                            {
                                Default = "v1"
                            }
                        }
                    }
                },
                new List<OpenApiServer>
                {
                    new OpenApiServer
                    {
                        Description = "description2",
                        Url = "https://{username}.example.com:{port}/{basePath}",
                        Variables = new Dictionary<string, OpenApiServerVariable>
                        {
                            ["username"] = new OpenApiServerVariable
                            {
                                Default = "unknown",
                                Description = "variableDescription1"
                            },
                            ["port"] = new OpenApiServerVariable
                            {
                                Default = "8443",
                                Description = "variableDescription2",
                                Enum = new List<string>
                                {
                                    "443",
                                    "8443"
                                }
                            },
                            ["basePath"] = new OpenApiServerVariable
                            {
                                Default = "v1"
                            }
                        }
                    }
                },
                new List<OpenApiDifference>
                {
                    new OpenApiDifference
                    {
                        Pointer = "#/0/description",
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Update,
                        OpenApiComparedElementType = typeof(string),
                        SourceValue = "description1",
                        TargetValue = "description2"
                    }
                }
            };

            // New and Removed server
            yield return new object[]
            {
                "New and Removed server",
                new List<OpenApiServer>
                {
                    new OpenApiServer
                    {
                        Description = "description1",
                        Url = "https://{username}.example.com:{port}/{basePath}",
                        Variables = new Dictionary<string, OpenApiServerVariable>
                        {
                            ["username"] = new OpenApiServerVariable
                            {
                                Default = "unknown",
                                Description = "variableDescription1"
                            },
                            ["port"] = new OpenApiServerVariable
                            {
                                Default = "8443",
                                Description = "variableDescription2",
                                Enum = new List<string>
                                {
                                    "443",
                                    "8443"
                                }
                            },
                            ["basePath"] = new OpenApiServerVariable
                            {
                                Default = "v1"
                            }
                        }
                    }
                },
                new List<OpenApiServer>
                {
                    new OpenApiServer
                    {
                        Description = "description1",
                        Url = "https://{username}.example.com:{port}/test",
                        Variables = new Dictionary<string, OpenApiServerVariable>
                        {
                            ["username"] = new OpenApiServerVariable
                            {
                                Default = "unknown",
                                Description = "variableDescription1"
                            },
                            ["port"] = new OpenApiServerVariable
                            {
                                Default = "8443",
                                Description = "variableDescription2",
                                Enum = new List<string>
                                {
                                    "443",
                                    "8443"
                                }
                            },
                            ["basePath"] = new OpenApiServerVariable
                            {
                                Default = "v1"
                            }
                        }
                    },
                    new OpenApiServer
                    {
                        Description = "description3",
                        Url = "https://{username}.example.com:{port}/{basePath}/test",
                        Variables = new Dictionary<string, OpenApiServerVariable>
                        {
                            ["username"] = new OpenApiServerVariable
                            {
                                Default = "unknown",
                                Description = "variableDescription1"
                            },
                            ["port"] = new OpenApiServerVariable
                            {
                                Default = "8443",
                                Description = "variableDescription2",
                                Enum = new List<string>
                                {
                                    "443",
                                    "8443"
                                }
                            },
                            ["basePath"] = new OpenApiServerVariable
                            {
                                Default = "v1"
                            }
                        }
                    }
                },
                new List<OpenApiDifference>
                {
                    new OpenApiDifference
                    {
                        Pointer = "#/0",
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Add,
                        OpenApiComparedElementType = typeof(OpenApiServer),
                        SourceValue = null,
                        TargetValue = new OpenApiServer
                        {
                            Description = "description1",
                            Url = "https://{username}.example.com:{port}/test",
                            Variables = new Dictionary<string, OpenApiServerVariable>
                            {
                                ["username"] = new OpenApiServerVariable
                                {
                                    Default = "unknown",
                                    Description = "variableDescription1"
                                },
                                ["port"] = new OpenApiServerVariable
                                {
                                    Default = "8443",
                                    Description = "variableDescription2",
                                    Enum = new List<string>
                                    {
                                        "443",
                                        "8443"
                                    }
                                },
                                ["basePath"] = new OpenApiServerVariable
                                {
                                    Default = "v1"
                                }
                            }
                        }
                    },
                    new OpenApiDifference
                    {
                        Pointer = "#/1",
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Add,
                        OpenApiComparedElementType = typeof(OpenApiServer),
                        SourceValue = null,
                        TargetValue = new OpenApiServer
                        {
                            Description = "description3",
                            Url = "https://{username}.example.com:{port}/{basePath}/test",
                            Variables = new Dictionary<string, OpenApiServerVariable>
                            {
                                ["username"] = new OpenApiServerVariable
                                {
                                    Default = "unknown",
                                    Description = "variableDescription1"
                                },
                                ["port"] = new OpenApiServerVariable
                                {
                                    Default = "8443",
                                    Description = "variableDescription2",
                                    Enum = new List<string>
                                    {
                                        "443",
                                        "8443"
                                    }
                                },
                                ["basePath"] = new OpenApiServerVariable
                                {
                                    Default = "v1"
                                }
                            }
                        }
                    },
                    new OpenApiDifference
                    {
                        Pointer = "#/0",
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Remove,
                        OpenApiComparedElementType = typeof(OpenApiServer),
                        TargetValue = null,
                        SourceValue = new OpenApiServer
                        {
                            Description = "description1",
                            Url = "https://{username}.example.com:{port}/{basePath}",
                            Variables = new Dictionary<string, OpenApiServerVariable>
                            {
                                ["username"] = new OpenApiServerVariable
                                {
                                    Default = "unknown",
                                    Description = "variableDescription1"
                                },
                                ["port"] = new OpenApiServerVariable
                                {
                                    Default = "8443",
                                    Description = "variableDescription2",
                                    Enum = new List<string>
                                    {
                                        "443",
                                        "8443"
                                    }
                                },
                                ["basePath"] = new OpenApiServerVariable
                                {
                                    Default = "v1"
                                }
                            }
                        }
                    }
                }
            };
        }

        [Theory(Skip = "Need to fix")]
        [MemberData(nameof(GetTestCasesForOpenApiServersComparerShouldSucceed))]
        public void OpenApiServersComparerShouldSucceed(
            string testCaseName,
            IList<OpenApiServer> source,
            IList<OpenApiServer> target,
            List<OpenApiDifference> expectedDifferences)
        {
            _output.WriteLine(testCaseName);

            var comparisonContext = new ComparisonContext(new OpenApiComparerFactory(), _sourceDocument,
                _targetDocument);
            var comparer = new OpenApiServersComparer();
            comparer.Compare(source, target, comparisonContext);

            var differences = comparisonContext.OpenApiDifferences.ToList();
            differences.Count().Should().Be(expectedDifferences.Count);

            differences.Should().BeEquivalentTo(expectedDifferences);
        }
    }
}
