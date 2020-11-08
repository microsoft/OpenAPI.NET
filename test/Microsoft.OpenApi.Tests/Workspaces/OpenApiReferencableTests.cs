// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Collections.Generic;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Xunit;

namespace Microsoft.OpenApi.Tests.Workspaces
{

    public class OpenApiReferencableTests
    {
        public static IEnumerable<object[]> ReferencableElementResolvesEmptyJsonPointerToItselfTestData =>
        new List<object[]>
        {
            new object[] { new OpenApiCallback() },
            new object[] { new OpenApiExample() },
            new object[] { new OpenApiHeader() },
            new object[] { new OpenApiLink() },
            new object[] { new OpenApiParameter() },
            new object[] { new OpenApiRequestBody() },
            new object[] { new OpenApiResponse() },
            new object[] { new OpenApiSchema() },
            new object[] { new OpenApiSecurityScheme() },
            new object[] { new OpenApiTag() }

        };

        [Theory]
        [MemberData(nameof(ReferencableElementResolvesEmptyJsonPointerToItselfTestData))]
        public void ReferencableElementResolvesEmptyJsonPointerToItself(IOpenApiReferenceable referencableElement)
        {
            // Arrange - above

            // Act
            var resolvedReference = referencableElement.ResolveReference(string.Empty);

            // Assert
            Assert.Same(referencableElement, resolvedReference);
        }

        [Fact]
        public void ParameterElementCanResolveReferenceToSchemaProperty()
        {
            // Arrange
            var parameterElement = new OpenApiParameter
            {
                Schema = new OpenApiSchema()
            };

            // Act
            var resolvedReference = parameterElement.ResolveReference("schema");

            // Assert
            Assert.Same(parameterElement.Schema, resolvedReference);
        }

        [Fact]
        public void ParameterElementCanResolveReferenceToExampleTmpDbgImproveMyName()
        {
            // Arrange
            var parameterElement = new OpenApiParameter
            {
                Examples = new Dictionary<string, OpenApiExample>()
            {
                { "example1", new OpenApiExample() }
            },
            };

            // Act
            var resolvedReference = parameterElement.ResolveReference("examples/example1");

            // Assert
            Assert.Same(parameterElement.Examples["example1"], resolvedReference);
        }
    }
}
