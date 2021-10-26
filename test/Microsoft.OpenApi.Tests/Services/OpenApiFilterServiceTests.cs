// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Services;
using OpenAPIService.Test;
using Xunit;

namespace Microsoft.OpenApi.Tests.Services
{
    public class OpenApiFilterServiceTests
    {
        private readonly OpenApiDocument _openApiDocumentMock;

        public OpenApiFilterServiceTests()
        {
            _openApiDocumentMock = OpenApiDocumentMock.CreateOpenApiDocument();
        }

        [Theory]
        [InlineData("users.user.ListUser", null)]
        [InlineData("users.user.GetUser", null)]
        [InlineData("administrativeUnits.restore", null)]
        [InlineData("graphService.GetGraphService", null)]
        [InlineData(null, "users.user")]
        [InlineData(null, "applications.application")]
        [InlineData(null, "reports.Functions")]
        public void ReturnFilteredOpenApiDocumentBasedOnOperationIds(string operationIds, string tags)
        {
            // Act
            var predicate = OpenApiFilterService.CreatePredicate(operationIds, tags);
            var subsetOpenApiDocument = OpenApiFilterService.CreateFilteredDocument(_openApiDocumentMock, predicate);

            // Assert
            Assert.NotNull(subsetOpenApiDocument);
            if (!string.IsNullOrEmpty(operationIds))
            {
                Assert.Single(subsetOpenApiDocument.Paths);
            }
            else if (!string.IsNullOrEmpty(tags))
            {
                Assert.NotEmpty(subsetOpenApiDocument.Paths);
            }
        }

        [Fact]
        public void ThrowsInvalidOperationExceptionInCreatePredicateWhenInvalidOperationIdIsSpecified()
        {
            // Act and Assert
            var message = Assert.Throws<InvalidOperationException>(() =>OpenApiFilterService.CreatePredicate(null, null)).Message;
            Assert.Equal("Either operationId(s) or tag(s) need to be specified.", message);
        }
    }
}
