// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.OpenApi.Exceptions;
using Microsoft.OpenApi.Expressions;
using Microsoft.OpenApi.Properties;
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
            Assert.Throws<ArgumentNullException>("expression", test);
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
            var exception = Assert.Throws<OpenApiException>(test);
            Assert.Equal(String.Format(SRResource.RuntimeExpressionHasInvalidFormat, expression), exception.Message);
        }

        [Fact]
        public void BuildMethodRuntimeExpressionReturnsMethodExpression()
        {
            // Arrange
            var expression = "$method";

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
            var expression = "$url";

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
            var expression = "$statusCode";

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
            var expression = "$request.header.accept";

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
            var expression = "$response.body#/status";

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
        [InlineData("$response.body#/status")]
        [InlineData("$request.header.accept")]
        [InlineData("$method")]
        [InlineData("$statusCode")]
        [InlineData("$url")]
        public void BuildRuntimeExpressionTwiceCreatesNewEquivalentInstances(string expression)
        {
            // Arrange & Act
            var runtimeExpression1 = RuntimeExpression.Build(expression);
            var runtimeExpression2 = RuntimeExpression.Build(expression);

            // Assert
            Assert.NotSame(runtimeExpression1, runtimeExpression2);
            Assert.Equal(runtimeExpression1, runtimeExpression2);
        }

        [Fact]
        public void CompositeRuntimeExpressionContainsExpression()
        {
            // Arrange
            var expression = "This is a composite expression {$url} yay";

            // Act
            var runtimeExpression = RuntimeExpression.Build(expression);

            // Assert
            Assert.NotNull(runtimeExpression);
            var response = Assert.IsType<CompositeExpression>(runtimeExpression);
            Assert.Equal(expression, response.Expression);

            var compositeExpression = runtimeExpression as CompositeExpression;
            Assert.Single(compositeExpression.ContainedExpressions);
        }

        [Fact]
        public void CompositeRuntimeExpressionContainsMultipleExpressions()
        {
            // Arrange
            var expression = "This is a composite expression {$url} yay and {$request.header.foo}";

            // Act
            var runtimeExpression = RuntimeExpression.Build(expression);

            // Assert
            Assert.NotNull(runtimeExpression);
            var response = Assert.IsType<CompositeExpression>(runtimeExpression);
            Assert.Equal(expression, response.Expression);

            var compositeExpression = runtimeExpression as CompositeExpression;
            Assert.Equal(2, compositeExpression.ContainedExpressions.Count);

            compositeExpression.ContainedExpressions.Should().BeEquivalentTo(new List<RuntimeExpression>
            {
                new UrlExpression(),
                new RequestExpression(new HeaderExpression("foo"))
            });
        }

        [Fact]
        public void CompositeRuntimeExpressionForWebHook()
        {
            // Arrange
            var expression = "http://notificationServer.com?transactionId={$request.body#/id}&email={$request.body#/email}";

            // Act
            var runtimeExpression = RuntimeExpression.Build(expression);

            // Assert
            Assert.NotNull(runtimeExpression);
            var response = Assert.IsType<CompositeExpression>(runtimeExpression);
            Assert.Equal(expression, response.Expression);

            var compositeExpression = runtimeExpression as CompositeExpression;
            Assert.Equal(2, compositeExpression.ContainedExpressions.Count);

            Assert.IsType<RequestExpression>(compositeExpression.ContainedExpressions.First());
            Assert.IsType<RequestExpression>(compositeExpression.ContainedExpressions.Last());
        }

        [Fact]
        public void CompositeRuntimeExpressionWithMultipleRuntimeExpressionsAndFakeBraces()
        {
            // Arrange
            var expression = "This is a composite expression {url} {test} and {} {$url} and {$request.header.foo}";

            // Act
            var runtimeExpression = RuntimeExpression.Build(expression);

            // Assert
            runtimeExpression.Should().NotBeNull();
            runtimeExpression.Should().BeOfType(typeof(CompositeExpression));
            var response = (CompositeExpression)runtimeExpression;
            response.Expression.Should().Be(expression);

            var compositeExpression = runtimeExpression as CompositeExpression;
            compositeExpression.ContainedExpressions.Should().BeEquivalentTo(new List<RuntimeExpression>
            {
                new UrlExpression(),
                new RequestExpression(new HeaderExpression("foo"))
            });
        }

        [Theory]
        [InlineData("This is a composite expression {url} {test} and {$fakeRuntime} {$url} and {$request.header.foo}", "$fakeRuntime")]
        [InlineData("This is a composite expression {url} {test} and {$} {$url} and {$request.header.foo}", "$")]
        public void CompositeRuntimeExpressionWithInvalidRuntimeExpressions(string expression, string invalidExpression)
        {
            // Arrange & Act
            Action test = () => RuntimeExpression.Build(expression);

            // Assert
            test.Should().Throw<OpenApiException>().WithMessage(String.Format(SRResource.RuntimeExpressionHasInvalidFormat, invalidExpression));
        }

        [Theory]
        [InlineData("/simplePath")]
        [InlineData("random string")]
        [InlineData("method")]
        [InlineData("/abc")]
        [InlineData("request    {}")]
        [InlineData("response{}")]
        public void CompositeRuntimeExpressionWithoutRecognizedRuntimeExpressions(string expression)
        {
            // Arrange

            // Act
            var runtimeExpression = RuntimeExpression.Build(expression);

            // Assert
            runtimeExpression.Should().NotBeNull();
            runtimeExpression.Should().BeOfType(typeof(CompositeExpression));
            var response = (CompositeExpression)runtimeExpression;
            response.Expression.Should().Be(expression);

            var compositeExpression = runtimeExpression as CompositeExpression;

            // The whole string is treated as the template without any contained expressions.
            compositeExpression.ContainedExpressions.Should().BeEmpty();
        }
    }
}
