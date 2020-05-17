// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.OpenApi.Expressions;
using Xunit;

namespace Microsoft.OpenApi.Tests.Writers
{
    [Collection("DefaultSettings")]
    public class UrlExpressionTests
    {
        [Fact]
        public void UrlExpressionReturnsCorrectExpression()
        {
            // Arrange & Act
            var url = new UrlExpression();

            // Assert
            Assert.Equal("$url", url.Expression);
        }
    }
}
