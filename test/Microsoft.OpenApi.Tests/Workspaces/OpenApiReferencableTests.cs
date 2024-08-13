// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using Microsoft.OpenApi.Exceptions;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Properties;
using Xunit;

namespace Microsoft.OpenApi.Tests.Workspaces
{
    public class OpenApiReferencableTests
    {
        private static readonly OpenApiCallback _callbackFragment = new();
        private static readonly OpenApiExample _exampleFragment = new();
        private static readonly OpenApiLink _linkFragment = new();
        private static readonly OpenApiHeader _headerFragment = new()
        {
            Schema = new OpenApiSchema(),
            Examples = new Dictionary<string, OpenApiExample>
            {
                { "example1", new OpenApiExample() }
            }
        };
        private static readonly OpenApiParameter _parameterFragment = new()
        {
            Schema = new OpenApiSchema(),
            Examples = new Dictionary<string, OpenApiExample>
            {
                { "example1", new OpenApiExample() }
            }
        };
        private static readonly OpenApiRequestBody _requestBodyFragment = new();
        private static readonly OpenApiResponse _responseFragment = new()
        {
            Headers = new Dictionary<string, OpenApiHeader>
            {
                { "header1", new OpenApiHeader() }
            },
            Links = new Dictionary<string, OpenApiLink>
            {
                { "link1", new OpenApiLink() }
            }
        };
        private static readonly OpenApiSchema _schemaFragment = new OpenApiSchema();
        private static readonly OpenApiSecurityScheme _securitySchemeFragment = new OpenApiSecurityScheme();
        private static readonly OpenApiTag _tagFragment = new OpenApiTag();

        public static IEnumerable<object[]> ResolveReferenceCanResolveValidJsonPointersTestData =>
        new List<object[]>
        {
            new object[] { _callbackFragment, "/", _callbackFragment },
            new object[] { _exampleFragment, "/", _exampleFragment },
            new object[] { _linkFragment, "/", _linkFragment },
            new object[] { _headerFragment, "/", _headerFragment },
            new object[] { _headerFragment, "/examples/example1", _headerFragment.Examples["example1"] },
            new object[] { _parameterFragment, "/", _parameterFragment },
            new object[] { _parameterFragment, "/examples/example1", _parameterFragment.Examples["example1"] },
            new object[] { _requestBodyFragment, "/", _requestBodyFragment },
            new object[] { _responseFragment, "/", _responseFragment },
            new object[] { _responseFragment, "/headers/header1", _responseFragment.Headers["header1"] },
            new object[] { _responseFragment, "/links/link1", _responseFragment.Links["link1"] },
            new object[] { _securitySchemeFragment, "/", _securitySchemeFragment},
            new object[] { _tagFragment, "/", _tagFragment}
        };

        [Theory]
        [MemberData(nameof(ResolveReferenceCanResolveValidJsonPointersTestData))]
        public void ResolveReferenceCanResolveValidJsonPointers(
            IOpenApiReferenceable element,
            string jsonPointer,
            IOpenApiElement expectedResolvedElement)
        {
            // Act
            var actualResolvedElement = element.ResolveReference(new(jsonPointer));

            // Assert
            Assert.Same(expectedResolvedElement, actualResolvedElement);
        }

        public static IEnumerable<object[]> ResolveReferenceShouldThrowOnInvalidReferenceIdTestData =>
        new List<object[]>
        {
            new object[] { _callbackFragment, "/a" },
            new object[] { _headerFragment, "/a" },
            new object[] { _headerFragment, "/examples" },
            new object[] { _headerFragment, "/examples/" },
            new object[] { _headerFragment, "/examples/a" },
            new object[] { _parameterFragment, "/a" },
            new object[] { _parameterFragment, "/examples" },
            new object[] { _parameterFragment, "/examples/" },
            new object[] { _parameterFragment, "/examples/a" },
            new object[] { _responseFragment, "/a" },
            new object[] { _responseFragment, "/headers" },
            new object[] { _responseFragment, "/headers/" },
            new object[] { _responseFragment, "/headers/a" },
            new object[] { _responseFragment, "/content" },
            new object[] { _responseFragment, "/content/" },
            new object[] { _responseFragment, "/content/a" }
        };

        [Theory]
        [MemberData(nameof(ResolveReferenceShouldThrowOnInvalidReferenceIdTestData))]
        public void ResolveReferenceShouldThrowOnInvalidReferenceId(IOpenApiReferenceable element, string jsonPointer)
        {
            // Act
            Action resolveReference = () => element.ResolveReference(new(jsonPointer));

            // Assert
            var exception = Assert.Throws<OpenApiException>(resolveReference);
            Assert.Equal(string.Format(SRResource.InvalidReferenceId, jsonPointer), exception.Message);
        }
    }
}
