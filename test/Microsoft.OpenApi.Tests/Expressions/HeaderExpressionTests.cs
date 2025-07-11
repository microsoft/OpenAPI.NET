﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using Xunit;

namespace Microsoft.OpenApi.Tests.Writers
{
    [Collection("DefaultSettings")]
    public class HeaderExpressionTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("        ")]
        public void HeaderExpressionConstructorThrows(string token)
        {
            // Arrange
            Action test = () => new HeaderExpression(token);

            // Act
            Assert.Throws<ArgumentNullException>("token", test);
        }

        [Fact]
        public void BodyExpressionWorksWithConstructor()
        {
            // Arrange
            var expression = "accept";

            // Act
            var header = new HeaderExpression(expression);

            // Assert
            Assert.Equal("header.accept", header.Expression);
            Assert.Equal("accept", header.Token);
        }
    }
}
