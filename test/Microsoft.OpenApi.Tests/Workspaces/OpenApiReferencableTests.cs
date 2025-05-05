// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using Microsoft.OpenApi.Exceptions;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Models.Interfaces;
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
            Examples = new OrderedDictionary<string, IOpenApiExample>
            {
                { "example1", new OpenApiExample() }
            }
        };
        private static readonly OpenApiParameter _parameterFragment = new()
        {
            Schema = new OpenApiSchema(),
            Examples = new OrderedDictionary<string, IOpenApiExample>
            {
                { "example1", new OpenApiExample() }
            }
        };
        private static readonly OpenApiRequestBody _requestBodyFragment = new();
        private static readonly OpenApiResponse _responseFragment = new()
        {
            Headers = new OrderedDictionary<string, IOpenApiHeader>
            {
                { "header1", new OpenApiHeader() }
            },
            Links = new OrderedDictionary<string, IOpenApiLink>
            {
                { "link1", new OpenApiLink() }
            }
        };
        private static readonly OpenApiSecurityScheme _securitySchemeFragment = new OpenApiSecurityScheme();
        public static IEnumerable<object[]> ResolveReferenceCanResolveValidJsonPointersTestData =>
        [
            [_callbackFragment, "/", _callbackFragment],
            [_exampleFragment, "/", _exampleFragment],
            [_linkFragment, "/", _linkFragment],
            [_headerFragment, "/", _headerFragment],
            [_headerFragment, "/examples/example1", _headerFragment.Examples["example1"]],
            [_parameterFragment, "/", _parameterFragment],
            [_parameterFragment, "/examples/example1", _parameterFragment.Examples["example1"]],
            [_requestBodyFragment, "/", _requestBodyFragment],
            [_responseFragment, "/", _responseFragment],
            [_responseFragment, "/headers/header1", _responseFragment.Headers["header1"]],
            [_responseFragment, "/links/link1", _responseFragment.Links["link1"]],
            [_securitySchemeFragment, "/", _securitySchemeFragment],
        ];

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
        [
            [_callbackFragment, "/a"],
            [_headerFragment, "/a"],
            [_headerFragment, "/examples"],
            [_headerFragment, "/examples/"],
            [_headerFragment, "/examples/a"],
            [_parameterFragment, "/a"],
            [_parameterFragment, "/examples"],
            [_parameterFragment, "/examples/"],
            [_parameterFragment, "/examples/a"],
            [_responseFragment, "/a"],
            [_responseFragment, "/headers"],
            [_responseFragment, "/headers/"],
            [_responseFragment, "/headers/a"],
            [_responseFragment, "/content"],
            [_responseFragment, "/content/"],
            [_responseFragment, "/content/a"]
        ];

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
