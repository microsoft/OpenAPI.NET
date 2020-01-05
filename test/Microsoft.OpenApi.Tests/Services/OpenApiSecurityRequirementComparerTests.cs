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
    public class OpenApiSecurityRequirementComparerTests
    {
        private readonly ITestOutputHelper _output;

        private readonly OpenApiDocument _sourceDocument = new OpenApiDocument
        {
            Components = new OpenApiComponents
            {
                SecuritySchemes = new Dictionary<string, OpenApiSecurityScheme>
                {
                    {
                        "scheme1", new OpenApiSecurityScheme
                        {
                            Description = "Test",
                            Name = "Test"
                        }
                    },
                    {
                        "scheme2", new OpenApiSecurityScheme
                        {
                            Description = "Test",
                            Name = "Test"
                        }
                    },
                    {
                        "scheme3", new OpenApiSecurityScheme
                        {
                            Description = "Test",
                            Name = "Test"
                        }
                    }
                }
            }
        };

        private readonly OpenApiDocument _targetDocument = new OpenApiDocument
        {
            Components = new OpenApiComponents
            {
                SecuritySchemes = new Dictionary<string, OpenApiSecurityScheme>
                {
                    {
                        "scheme1", new OpenApiSecurityScheme
                        {
                            Description = "Test Updated",
                            Name = "Test"
                        }
                    },
                    {
                        "scheme2", new OpenApiSecurityScheme
                        {
                            Description = "Test",
                            Name = "Test"
                        }
                    },
                    {
                        "scheme4", new OpenApiSecurityScheme
                        {
                            Description = "Test",
                            Name = "Test"
                        }
                    }
                }
            }
        };

        public OpenApiSecurityRequirementComparerTests(ITestOutputHelper output)
        {
            _output = output;
        }

        public static IEnumerable<object[]> GetTestCasesForOpenApiSecurityRequirementComparerShouldSucceed()
        {
            yield return new object[]
            {
                "New Removed And updated schemes",
                new OpenApiSecurityRequirement
                {
                    [
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference {Type = ReferenceType.SecurityScheme, Id = "scheme1"}
                        }
                    ] = new List<string>
                    {
                        "scope1",
                        "scope2",
                        "scope3"
                    },
                    [
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference {Type = ReferenceType.SecurityScheme, Id = "scheme2"}
                        }
                    ] = new List<string>
                    {
                        "scope4",
                        "scope5"
                    },
                    [
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference {Type = ReferenceType.SecurityScheme, Id = "scheme3"}
                        }
                    ] = new List<string>()
                },
                new OpenApiSecurityRequirement
                {
                    [
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference {Type = ReferenceType.SecurityScheme, Id = "scheme1"}
                        }
                    ] = new List<string>
                    {
                        "scope1",
                        "scope2",
                        "scope3"
                    },
                    [
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference {Type = ReferenceType.SecurityScheme, Id = "scheme2"}
                        }
                    ] = new List<string>
                    {
                        "scope4",
                        "scope5"
                    },
                    [
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference {Type = ReferenceType.SecurityScheme, Id = "scheme4"}
                        }
                    ] = new List<string>()
                },
                new List<OpenApiDifference>
                {
                    new OpenApiDifference
                    {
                        Pointer = "#/scheme4",
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Add,
                        OpenApiComparedElementType = typeof(IList<string>),
                        SourceValue = null,
                        TargetValue = new List<string>()
                    },
                    new OpenApiDifference
                    {
                        Pointer = "#/scheme1/description",
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Update,
                        OpenApiComparedElementType = typeof(string),
                        SourceValue = "Test",
                        TargetValue = "Test Updated"
                    },
                    new OpenApiDifference
                    {
                        Pointer = "#/scheme3",
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Remove,
                        OpenApiComparedElementType = typeof(IList<string>),
                        SourceValue = new List<string>(),
                        TargetValue = null
                    }
                }
            };

            yield return new object[]
            {
                "Source and target are null",
                null,
                null,
                new List<OpenApiDifference>()
            };

            yield return new object[]
            {
                "Source is null",
                null,
                new OpenApiSecurityRequirement
                {
                    [
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference {Type = ReferenceType.SecurityScheme, Id = "scheme1"}
                        }
                    ] = new List<string>()
                },
                new List<OpenApiDifference>
                {
                    new OpenApiDifference
                    {
                        Pointer = "#/",
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Update,
                        OpenApiComparedElementType = typeof(OpenApiSecurityRequirement),
                        SourceValue = null,
                        TargetValue = new OpenApiSecurityRequirement
                        {
                            [
                                new OpenApiSecurityScheme
                                {
                                    Reference = new OpenApiReference
                                    {
                                        Type = ReferenceType.SecurityScheme,
                                        Id = "scheme1"
                                    }
                                }
                            ] = new List<string>()
                        }
                    }
                }
            };

            yield return new object[]
            {
                "Target is null",
                new OpenApiSecurityRequirement
                {
                    [
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference {Type = ReferenceType.SecurityScheme, Id = "scheme1"}
                        }
                    ] = new List<string>()
                },
                null,
                new List<OpenApiDifference>
                {
                    new OpenApiDifference
                    {
                        Pointer = "#/",
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Update,
                        OpenApiComparedElementType = typeof(OpenApiSecurityRequirement),
                        SourceValue = new OpenApiSecurityRequirement
                        {
                            [
                                new OpenApiSecurityScheme
                                {
                                    Reference = new OpenApiReference
                                    {
                                        Type = ReferenceType.SecurityScheme,
                                        Id = "scheme1"
                                    }
                                }
                            ] = new List<string>()
                        },
                        TargetValue = null
                    }
                }
            };
        }

        [Theory]
        [MemberData(nameof(GetTestCasesForOpenApiSecurityRequirementComparerShouldSucceed))]
        public void OpenApiSecurityRequirementComparerShouldSucceed(
            string testCaseName,
            OpenApiSecurityRequirement source,
            OpenApiSecurityRequirement target,
            List<OpenApiDifference> expectedDifferences)


        {
            _output.WriteLine(testCaseName);

            var comparisonContext = new ComparisonContext(new OpenApiComparerFactory(), _sourceDocument,
                _targetDocument);
            var comparer = new OpenApiSecurityRequirementComparer();
            comparer.Compare(source, target, comparisonContext);

            var differences = comparisonContext.OpenApiDifferences.ToList();

            differences.Count().Should().Be(expectedDifferences.Count);

            differences.Should().BeEquivalentTo(expectedDifferences);
        }
    }
}