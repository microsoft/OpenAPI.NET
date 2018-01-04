// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using Microsoft.OpenApi.Expressions;
using Xunit;

namespace Microsoft.OpenApi.Tests.Writers
{
    [Collection("DefaultSettings")]
    public class QueryExpressionTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("        ")]
        public void QueryExpressionConstructorThrows(string name)
        {
            // Arrange
            Action test = () => new QueryExpression(name);

            // Act
            Assert.Throws<ArgumentException>("name", test);
        }

        [Fact]
        public void QueryExpressionConstructorWorks()
        {
            // Arrange
            string name = "anyValue";

            // Act
            var query = new QueryExpression(name);

            // Assert
            Assert.Equal("query.anyValue", query.Expression);
            Assert.Equal("anyValue", query.Name);
        }
    }
}
