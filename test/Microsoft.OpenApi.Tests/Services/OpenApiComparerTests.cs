// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.OpenApi.Interfaces;
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

            // Ensure that all references are corrected to mirror what happens in the MapNode.
            new OpenApiWalker(new SelfReferenceFixer()).Walk(source);
            new OpenApiWalker(new SelfReferenceFixer()).Walk(target);

            var differences = OpenApiComparer.Compare(source, target).ToList();

            var actualPaths = differences.Select(x => x.Pointer).ToList();
            var expectedPaths = expectedDifferences.Select(x => x.Pointer).ToList();

            differences.Count().Should().Be(expectedDifferences.Count);

            differences.Should().BeEquivalentTo(expectedDifferences);
        }

        /// <summary>
        /// MapNode adds a self-reference for all IOpenApiReferenceable instances.
        /// This visitor mimics that behavior for C# created objects.
        /// </summary>
        internal class SelfReferenceFixer : OpenApiVisitorBase
        {
            public override void Visit(OpenApiComponents components)
            {
                UpdateNullReferences(components.Schemas, ReferenceType.Schema);
                UpdateNullReferences(components.Responses, ReferenceType.Response);
                UpdateNullReferences(components.Parameters, ReferenceType.Parameter);
                UpdateNullReferences(components.Examples, ReferenceType.Example);
                UpdateNullReferences(components.RequestBodies, ReferenceType.RequestBody);
                UpdateNullReferences(components.Headers, ReferenceType.Header);
                UpdateNullReferences(components.SecuritySchemes, ReferenceType.SecurityScheme);
                UpdateNullReferences(components.Links, ReferenceType.Link);
                UpdateNullReferences(components.Callbacks, ReferenceType.Callback);
            }

            private void UpdateNullReferences<T>(IDictionary<string, T> mapping, ReferenceType referenceType)
                where T : IOpenApiReferenceable
            {
                foreach (var kvp in mapping)
                {
                    var element = kvp.Value;

                    if (!element.UnresolvedReference && (element.Reference == null))
                    {
                        element.Reference = new OpenApiReference()
                        {
                            Id = kvp.Key,
                            Type = referenceType,
                        };
                    }
                }
            }
        }
    }
}
