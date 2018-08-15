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
    public class OpenApiComparerTests
    {
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

            var differences = OpenApiComparer.Compare(source, target).ToList();
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