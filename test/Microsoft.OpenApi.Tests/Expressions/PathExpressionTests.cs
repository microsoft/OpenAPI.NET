// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using Microsoft.OpenApi.Expressions;
using Xunit;

namespace Microsoft.OpenApi.Tests.Writers
{
    [Collection("DefaultSettings")]
    public class PathExpressionTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("        ")]
        public void PathExpressionConstructorThrows(string name)
        {
            // Arrange
            Action test = () => new PathExpression(name);

            // Act
            Assert.Throws<ArgumentException>("name", test);
        }

        [Fact]
        public void PathExpressionConstructorWorks()
        {
            // Arrange
            string name = "anyValue";

            // Act
            var path = new PathExpression(name);

            // Assert
            Assert.Equal("path.anyValue", path.Expression);
            Assert.Equal("anyValue", path.Name);
        }
    }
}
