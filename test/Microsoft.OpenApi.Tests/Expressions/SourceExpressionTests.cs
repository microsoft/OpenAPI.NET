// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using Microsoft.OpenApi.Exceptions;
using Microsoft.OpenApi.Expressions;
using Microsoft.OpenApi.Properties;
using Xunit;

namespace Microsoft.OpenApi.Tests.Writers
{
    [Collection("DefaultSettings")]
    public class SourceExpressionTests
    {
        [Theory]
        [InlineData("unknown.body")]
        [InlineData(" ")]
        [InlineData(null)]
        [InlineData("$header")]
        [InlineData("$header.accept")]
        public void BuildSourceExpressionThrowsInvalidFormat(string expression)
        {
            // Arrange & Act
            Action test = () => SourceExpression.Build(expression);

            // Assert
            OpenApiException exception = Assert.Throws<OpenApiException>(test);
            Assert.Equal(String.Format(SRResource.SourceExpressionHasInvalidFormat, expression), exception.Message);
        }

        [Fact]
        public void BuildHeaderExpressionReturnsHeaderExpression()
        {
            // Arrange
            string expression = "header.accept";

            // Act
            var sourceExpression = SourceExpression.Build(expression);

            // Assert
            Assert.NotNull(sourceExpression);
            var header = Assert.IsType<HeaderExpression>(sourceExpression);
            Assert.Equal(expression, header.Expression);
            Assert.Equal("accept", header.Token);
        }

        [Fact]
        public void BuildQueryExpressionReturnsQueryExpression()
        {
            // Arrange
            string expression = "query.anyValue";

            // Act
            var sourceExpression = SourceExpression.Build(expression);

            // Assert
            Assert.NotNull(sourceExpression);
            var query = Assert.IsType<QueryExpression>(sourceExpression);
            Assert.Equal(expression, query.Expression);
            Assert.Equal("anyValue", query.Name);
        }

        [Fact]
        public void BuildPathExpressionReturnsPathExpression()
        {
            // Arrange
            string expression = "path.anyValue";

            // Act
            var sourceExpression = SourceExpression.Build(expression);

            // Assert
            Assert.NotNull(sourceExpression);
            var path = Assert.IsType<PathExpression>(sourceExpression);
            Assert.Equal(expression, path.Expression);
            Assert.Equal("anyValue", path.Name);
        }

        [Fact]
        public void BuildBodyExpressionReturnsBodyExpression()
        {
            // Arrange
            string expression = "body#/user/uuid";

            // Act
            var sourceExpression = SourceExpression.Build(expression);

            // Assert
            Assert.NotNull(sourceExpression);
            var body = Assert.IsType<BodyExpression>(sourceExpression);
            Assert.Equal(expression, body.Expression);
            Assert.Equal("/user/uuid", body.Fragment);
        }
    }
}
