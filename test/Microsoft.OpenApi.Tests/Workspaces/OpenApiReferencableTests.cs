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
        private static readonly OpenApiCallback _callbackFragment = new OpenApiCallback();
        private static readonly OpenApiExample _exampleFragment = new OpenApiExample();
        private static readonly OpenApiLink _linkFragment = new OpenApiLink();
        private static readonly OpenApiHeader _headerFragment = new OpenApiHeader();
        private static readonly OpenApiParameter _parameterFragment = new OpenApiParameter
        {
            Schema = new OpenApiSchema(),
            Examples = new Dictionary<string, OpenApiExample>
            {
                { "example1", new OpenApiExample() }
            }
        };
        private static readonly OpenApiRequestBody _requestBodyFragment = new OpenApiRequestBody();
        private static readonly OpenApiResponse _responseFragment = new OpenApiResponse();
        private static readonly OpenApiSchema _schemaFragment = new OpenApiSchema();
        private static readonly OpenApiSecurityScheme _securitySchemeFragment = new OpenApiSecurityScheme();
        private static readonly OpenApiTag _tagFragment = new OpenApiTag();

        public static IEnumerable<object[]> ReferencableElementsCanResolveReferencesTestData =>
        new List<object[]>
        {
            new object[] { _callbackFragment, "/", _callbackFragment },
            new object[] { _exampleFragment, "/", _exampleFragment },
            new object[] { _linkFragment, "/", _linkFragment },
            new object[] { _headerFragment, "/", _headerFragment },
            new object[] { _parameterFragment, "/", _parameterFragment },
            new object[] { _parameterFragment, "/schema", _parameterFragment.Schema },
            new object[] { _parameterFragment, "/examples/example1", _parameterFragment.Examples["example1"] },
            new object[] { _requestBodyFragment, "/", _requestBodyFragment },
            new object[] { _responseFragment, "/", _responseFragment },
            new object[] { _schemaFragment, "/", _schemaFragment},
            new object[] { _securitySchemeFragment, "/", _securitySchemeFragment},
            new object[] { _tagFragment, "/", _tagFragment},
        };

        [Theory]
        [MemberData(nameof(ReferencableElementsCanResolveReferencesTestData))]
        public void ReferencableElementsCanResolveReferences(
            IOpenApiReferenceable element,
            string pointer,
            IOpenApiElement expectedResolvedElement)
        {
            // Act
            var actualResolvedElement = element.ResolveReference(pointer);

            // Assert
            Assert.Same(expectedResolvedElement, actualResolvedElement);
        }

        public static IEnumerable<object[]> ParameterElementShouldThrowOnInvalidReferenceIdTestData =>
        new List<object[]>
        {
            new object[] { "" },
            new object[] { "a" },
            new object[] { "examples" },
            new object[] { "examples/" },
            new object[] { "examples/a" },

        };

        [Theory]
        [MemberData(nameof(ParameterElementShouldThrowOnInvalidReferenceIdTestData))]
        public void ParameterElementShouldThrowOnInvalidReferenceId(string jsonPointer)
        {
            // Act
            Action resolveReference = () => _parameterFragment.ResolveReference(jsonPointer);

            // Assert
            var exception = Assert.Throws<OpenApiException>(resolveReference);
            Assert.Equal(string.Format(SRResource.InvalidReferenceId, jsonPointer), exception.Message);
        }
    }
}
