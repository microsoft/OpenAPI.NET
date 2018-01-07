// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using Microsoft.OpenApi.Expressions;
using Xunit;

namespace Microsoft.OpenApi.Tests.Writers
{
    [Collection("DefaultSettings")]
    public class RequestExpressionTests
    {
        [Fact]
        public void RequestExpressionConstructorThrows()
        {
            // Arrange & Act
            Action test = () => new RequestExpression(source: null);

            // Assert
            Assert.Throws<ArgumentNullException>("source", test);
        }

        [Fact]
        public void RequestExpressionConstructorWorks()
        {
            // Arrange
            SourceExpression source = SourceExpression.Build("header.accept");

            // Act
            RequestExpression request = new RequestExpression(source);

            // Assert
            Assert.Same(source, request.Source);
            Assert.Equal("$request.header.accept", request.Expression);
            Assert.Equal("$request.header.accept", request.ToString());
        }
    }
}
