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
        [InlineData("users.user.ListUser")]
        [InlineData("users.user.GetUser")]
        [InlineData("administrativeUnits.restore")]
        [InlineData("graphService.GetGraphService")]
        public void ReturnFilteredOpenApiDocumentBasedOnOperationIds(string operationId)
        {
            // Act
            var predicate = OpenApiFilterService.CreatePredicate(operationId);
            var subsetOpenApiDocument = OpenApiFilterService.CreateFilteredDocument(_openApiDocumentMock, predicate);

            // Assert
            Assert.NotNull(subsetOpenApiDocument);
            Assert.Single(subsetOpenApiDocument.Paths);
        }

        [Fact]
        public void ThrowsInvalidOperationExceptionInCreatePredicateWhenInvalidOperationIdIsSpecified()
        {
            // Act and Assert
            var message = Assert.Throws<InvalidOperationException>(() =>OpenApiFilterService.CreatePredicate(null)).Message;
            Assert.Equal("OperationId needs to be specified.", message);
        }
    }
}
