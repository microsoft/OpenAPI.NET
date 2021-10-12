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
        private const string Title = "Partial Graph API";
        private const string GraphVersion = "mock";
        private readonly OpenApiFilterService _openApiFilterService;
        private readonly OpenApiDocument _openApiDocumentMock;

        public OpenApiFilterServiceTests()
        {
            _openApiFilterService = new OpenApiFilterService();
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
            var predicate = _openApiFilterService.CreatePredicate(operationId);
            var subsetOpenApiDocument = _openApiFilterService.CreateFilteredDocument(_openApiDocumentMock, Title, GraphVersion, predicate);

            // Assert
            Assert.NotNull(subsetOpenApiDocument);
            Assert.Single(subsetOpenApiDocument.Paths);
        }

        [Fact]
        public void ThrowsInvalidOperationExceptionInCreatePredicateWhenInvalidOperationIdIsSpecified()
        {
            // Act and Assert
            var message = Assert.Throws<InvalidOperationException>(() =>_openApiFilterService.CreatePredicate(null)).Message;
            Assert.Equal("OperationId needs to be specified.", message);
        }
    }
}
