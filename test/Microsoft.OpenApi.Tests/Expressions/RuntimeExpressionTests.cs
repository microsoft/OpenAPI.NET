﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.OpenApi.Exceptions;
using Microsoft.OpenApi.Expressions;
using Microsoft.OpenApi.Properties;
using System;
using Xunit;

namespace Microsoft.OpenApi.Tests.Writers
{
    [Collection("DefaultSettings")]
    public class RuntimeExpressionTests
    {
        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void BuildRuntimeExpressionThrowsNullOrWhiteSpace(string expression)
        {
            // Arrange & Act
            Action test = () => RuntimeExpression.Build(expression);

            // Assert
            Assert.Throws<ArgumentException>("expression", test);
        }

        [Theory]
        [InlineData("method")]
        [InlineData("abc")]
        [InlineData("request")]
        [InlineData("response")]
        public void BuildRuntimeExpressionThrowsWithDollarPrefix(string expression)
        {
            // Arrange & Act
            Action test = () => RuntimeExpression.Build(expression);

            // Assert
            OpenApiException exception = Assert.Throws<OpenApiException>(test);
            Assert.Equal(String.Format(SRResource.RuntimeExpressionMustBeginWithDollar, expression), exception.Message);
        }

        [Theory]
        [InlineData("$unknown")]
        [InlineData("$abc")]
        [InlineData("$$")]
        public void BuildRuntimeExpressionThrowsInvalidFormat(string expression)
        {
            // Arrange & Act
            Action test = () => RuntimeExpression.Build(expression);

            // Assert
            OpenApiException exception = Assert.Throws<OpenApiException>(test);
            Assert.Equal(String.Format(SRResource.RuntimeExpressionHasInvalidFormat, expression), exception.Message);
        }

        [Fact]
        public void BuildMethodRuntimeExpressionReturnsMethodExpression()
        {
            // Arrange
            string expression = "$method";

            // Act
            var runtimeExpression = RuntimeExpression.Build(expression);

            // Assert
            Assert.NotNull(runtimeExpression);
            var method = Assert.IsType<MethodExpression>(runtimeExpression);
            Assert.Equal(expression, method.Expression);
        }

        [Fact]
        public void BuildUrlRuntimeExpressionReturnsUrlExpression()
        {
            // Arrange
            string expression = "$url";

            // Act
            var runtimeExpression = RuntimeExpression.Build(expression);

            // Assert
            Assert.NotNull(runtimeExpression);
            var url = Assert.IsType<UrlExpression>(runtimeExpression);
            Assert.Equal(expression, url.Expression);
        }

        [Fact]
        public void BuildStatusCodeRuntimeExpressionReturnsStatusCodeExpression()
        {
            // Arrange
            string expression = "$statusCode";

            // Act
            var runtimeExpression = RuntimeExpression.Build(expression);

            // Assert
            Assert.NotNull(runtimeExpression);
            var statusCode = Assert.IsType<StatusCodeExpression>(runtimeExpression);
            Assert.Equal(expression, statusCode.Expression);
        }

        [Fact]
        public void BuildRequestRuntimeExpressionReturnsRequestExpression()
        {
            // Arrange
            string expression = "$request.header.accept";

            // Act
            var runtimeExpression = RuntimeExpression.Build(expression);

            // Assert
            Assert.NotNull(runtimeExpression);
            var request = Assert.IsType<RequestExpression>(runtimeExpression);
            Assert.Equal(expression, request.Expression);

            Assert.NotNull(request.Source);
            Assert.Equal("header.accept", request.Source.Expression);

            var header = Assert.IsType<HeaderExpression>(request.Source);

            Assert.Equal("accept", header.Token);
            Assert.Equal("header.accept", header.Expression);
        }

        [Fact]
        public void BuildResponseRuntimeExpressionReturnsResponseExpression()
        {
            // Arrange
            string expression = "$response.body#/status";

            // Act
            var runtimeExpression = RuntimeExpression.Build(expression);

            // Assert
            Assert.NotNull(runtimeExpression);
            var response = Assert.IsType<ResponseExpression>(runtimeExpression);
            Assert.Equal(expression, response.Expression);

            Assert.Equal("body#/status", response.Source.Expression);
            Assert.NotNull(response.Source);

            var body = Assert.IsType<BodyExpression>(response.Source);
            Assert.Equal("/status", body.Fragment);
            Assert.Equal("body#/status", body.Expression);
        }

        [Theory]
        [InlineData("$method")]
        [InlineData("$statusCode")]
        [InlineData("$url")]
        public void CompareStaticRuntimeExpressionWorks(string expression)
        {
            // Arrange & Act
            var runtimeExpression1 = RuntimeExpression.Build(expression);
            var runtimeExpression2 = RuntimeExpression.Build(expression);

            // Assert
            Assert.Same(runtimeExpression1, runtimeExpression2);
        }

        [Theory]
        [InlineData("$response.body#/status")]
        [InlineData("$request.header.accept")]
        public void CompareRuntimeExpressionWorks(string expression)
        {
            // Arrange & Act
            var runtimeExpression1 = RuntimeExpression.Build(expression);
            var runtimeExpression2 = RuntimeExpression.Build(expression);

            // Assert
            Assert.NotSame(runtimeExpression1, runtimeExpression2);
            Assert.Equal(runtimeExpression1, runtimeExpression2);
        }
    }
}
