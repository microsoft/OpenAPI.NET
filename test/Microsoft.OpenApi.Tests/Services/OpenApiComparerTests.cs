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
    public class OpenApiComparerTests
    {
        public static OpenApiExample AdvancedExample = new OpenApiExample
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
                    },

                    new OpenApiObject
                    {
                        ["status"] = new OpenApiString("Status2"),
                        ["id"] = new OpenApiString("v2"),
                        ["links"] = new OpenApiArray
                        {
                            new OpenApiObject
                            {
                                ["href"] = new OpenApiString("http://example.com/2"),
                                ["rel"] = new OpenApiString("sampleRel2")
                            }
                        }
                    }
                }
            }
        };

        private readonly ITestOutputHelper _output;

        public OpenApiComparerTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Theory]
        [MemberData(
            nameof(OpenApiComparerTestCases.GetTestCasesForOpenApiComparerShouldSucceed),
            MemberType = typeof(OpenApiComparerTestCases))]
        public void OpenApiComparerShouldSucceed(
            string testCaseName,
            OpenApiDocument source,
            OpenApiDocument target,
            List<OpenApiDifference> expectedDifferences)
        {
            _output.WriteLine(testCaseName);

            new OpenApiExampleComparer().Compare(AdvancedExample, AdvancedExample,
                new ComparisonContext(new OpenApiComparerFactory(), new OpenApiDocument(), new OpenApiDocument()));
            var differences = OpenApiComparer.Compare(source, target).ToList();
            differences.Count().ShouldBeEquivalentTo(expectedDifferences.Count);

            differences.ShouldBeEquivalentTo(expectedDifferences);
        }
    }
}