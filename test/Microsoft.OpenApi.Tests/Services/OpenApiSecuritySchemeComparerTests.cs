// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
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
    public class OpenApiSecuritySchemeComparerTests
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
                            Name = "Test",
                            Flows = new OpenApiOAuthFlows
                            {
                                Implicit = new OpenApiOAuthFlow
                                {
                                    AuthorizationUrl = new Uri("http://localhost/1")
                                },
                                AuthorizationCode = new OpenApiOAuthFlow
                                {
                                    AuthorizationUrl = new Uri("http://localhost/2")
                                }
                            }
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
                            Description = "Test",
                            Name = "Test",
                            Flows = new OpenApiOAuthFlows
                            {
                                Implicit = new OpenApiOAuthFlow
                                {
                                    AuthorizationUrl = new Uri("http://localhost/3")
                                },
                                ClientCredentials = new OpenApiOAuthFlow
                                {
                                    AuthorizationUrl = new Uri("http://localhost/2")
                                }
                            }
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

        public OpenApiSecuritySchemeComparerTests(ITestOutputHelper output)
        {
            _output = output;
        }

        public static IEnumerable<object[]> GetTestCasesForOpenApiSecuritySchemeComparerShouldSucceed()
        {
            yield return new object[]
            {
                "Updated Type, Description, Name, In, BearerFormat, OpenIdConnectUrl",
                new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.ApiKey,
                    Description = "Test Description",
                    Name = "Test Name",
                    In = ParameterLocation.Path,
                    OpenIdConnectUrl = new Uri("http://localhost:1"),
                    BearerFormat = "Test Format"
                },
                new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Description = "Test Description Updated",
                    Name = "Test Name Updated",
                    Scheme = "basic"
                },
                new List<OpenApiDifference>
                {
                    new OpenApiDifference
                    {
                        Pointer = "#/description",
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Update,
                        OpenApiComparedElementType = typeof(string),
                        SourceValue = "Test Description",
                        TargetValue = "Test Description Updated"
                    },
                    new OpenApiDifference
                    {
                        Pointer = "#/type",
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Update,
                        OpenApiComparedElementType = typeof(SecuritySchemeType),
                        SourceValue = SecuritySchemeType.ApiKey,
                        TargetValue = SecuritySchemeType.Http
                    },
                    new OpenApiDifference
                    {
                        Pointer = "#/name",
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Update,
                        OpenApiComparedElementType = typeof(string),
                        SourceValue = "Test Name",
                        TargetValue = "Test Name Updated"
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
                        Pointer = "#/scheme",
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Update,
                        OpenApiComparedElementType = typeof(string),
                        SourceValue = null,
                        TargetValue = "basic"
                    },
                    new OpenApiDifference
                    {
                        Pointer = "#/bearerFormat",
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Update,
                        OpenApiComparedElementType = typeof(string),
                        SourceValue = "Test Format",
                        TargetValue = null
                    },
                    new OpenApiDifference
                    {
                        Pointer = "#/openIdConnectUrl",
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Update,
                        OpenApiComparedElementType = typeof(Uri),
                        SourceValue = new Uri("http://localhost:1"),
                        TargetValue = null
                    }
                }
            };

            yield return new object[]
            {
                "Difference in reference id",
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Id = "scheme1",
                        Type = ReferenceType.SecurityScheme
                    }
                },
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Id = "scheme2",
                        Type = ReferenceType.SecurityScheme
                    }
                },
                new List<OpenApiDifference>
                {
                    new OpenApiDifference
                    {
                        Pointer = "#/$ref",
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Update,
                        OpenApiComparedElementType = typeof(OpenApiReference),
                        SourceValue = new OpenApiReference
                        {
                            Id = "scheme1",
                            Type = ReferenceType.SecurityScheme
                        },
                        TargetValue = new OpenApiReference
                        {
                            Id = "scheme2",
                            Type = ReferenceType.SecurityScheme
                        }
                    }
                }
            };

            yield return new object[]
            {
                "New, Removed and Updated OAuthFlows",
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Id = "scheme1",
                        Type = ReferenceType.SecurityScheme
                    }
                },
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Id = "scheme1",
                        Type = ReferenceType.SecurityScheme
                    }
                },
                new List<OpenApiDifference>
                {
                    new OpenApiDifference
                    {
                        Pointer = "#/flows/implicit/authorizationUrl",
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Update,
                        OpenApiComparedElementType = typeof(Uri),
                        SourceValue = new Uri("http://localhost/1"),
                        TargetValue = new Uri("http://localhost/3")
                    },
                    new OpenApiDifference
                    {
                        Pointer = "#/flows/clientCredentials",
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Update,
                        OpenApiComparedElementType = typeof(OpenApiOAuthFlow),
                        SourceValue = null,
                        TargetValue = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri("http://localhost/2")
                        }
                    },
                    new OpenApiDifference
                    {
                        Pointer = "#/flows/authorizationCode",
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Update,
                        OpenApiComparedElementType = typeof(OpenApiOAuthFlow),
                        SourceValue = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri("http://localhost/2")
                        },
                        TargetValue = null
                    }
                }
            };
        }

        [Theory]
        [MemberData(nameof(GetTestCasesForOpenApiSecuritySchemeComparerShouldSucceed))]
        public void OpenApiSecuritySchemeComparerShouldSucceed(
            string testCaseName,
            OpenApiSecurityScheme source,
            OpenApiSecurityScheme target,
            List<OpenApiDifference> expectedDifferences)
        {
            _output.WriteLine(testCaseName);

            var comparisonContext = new ComparisonContext(new OpenApiComparerFactory(), _sourceDocument,
                _targetDocument);
            var comparer = new OpenApiSecuritySchemeComparer();
            comparer.Compare(source, target, comparisonContext);

            var differences = comparisonContext.OpenApiDifferences.ToList();

            differences.Count().Should().Be(expectedDifferences.Count);

            differences.Should().BeEquivalentTo(expectedDifferences);
        }
    }
}
