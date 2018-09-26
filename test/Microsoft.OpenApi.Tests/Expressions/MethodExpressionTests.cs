// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.OpenApi.Expressions;
using Xunit;

namespace Microsoft.OpenApi.Tests.Writers
{
    [Collection("DefaultSettings")]
    public class MethodExpressionTests
    {
        [Fact]
        public void MethodExpressionReturnsCorrectExpression()
        {
            // Arrange & Act
            var method = new MethodExpression();

            // Assert
            Assert.Equal("$method", method.Expression);
        }
    }
}
