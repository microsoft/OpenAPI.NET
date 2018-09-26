// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.OpenApi.Expressions;
using Xunit;

namespace Microsoft.OpenApi.Tests.Writers
{
    [Collection("DefaultSettings")]
    public class StatusCodeExpressionTests
    {
        [Fact]
        public void StatusCodeExpressionReturnsCorrectExpression()
        {
            // Arrange & Act
            var statusCode = new StatusCodeExpression();

            // Assert
            Assert.Equal("$statusCode", statusCode.Expression);
        }
    }
}
