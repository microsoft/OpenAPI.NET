// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using Microsoft.OpenApi.Exceptions;
using Microsoft.OpenApi.Models;
using Xunit;

namespace Microsoft.OpenApi.Validations.Visitors.Tests
{
    public class OpenApiVisitorSetTests
    {
        [Fact]
        public void VisitorsPropertyReturnsTheCorrectVisitorList()
        {
            for (int i = 0; i < 5; i++) // 5 is just a magic number
            {
                // Arrange & Act
                var visitors = OpenApiVisitorSet.Visitors;

                // Assert
                Assert.NotNull(visitors);
                Assert.NotEmpty(visitors);
                Assert.Equal(29, visitors.Count);
            }
        }

        [Fact]
        public void GetVisitorThrowsUnknowElementType()
        {
            // Arrange & Act
            Action test = () => OpenApiVisitorSet.GetVisitor(typeof(OpenApiVisitorSetTests));

            // Assert
            var exception = Assert.Throws<OpenApiException>(test);
            Assert.Equal("Can not find visitor type registered for type 'Microsoft.OpenApi.Validations.Visitors.Tests.OpenApiVisitorSetTests'.",
                exception.Message);
        }

        [Theory]
        [InlineData(typeof(OpenApiDocument), typeof(DocumentVisitor))]
        [InlineData(typeof(OpenApiInfo), typeof(InfoVisitor))]
        [InlineData(typeof(OpenApiXml), typeof(XmlVisitor))]
        [InlineData(typeof(OpenApiComponents), typeof(ComponentsVisitor))]
        public void GetVisitorReturnsTheCorrectVisitor(Type elementType, Type visitorType)
        {
            // Arrange & Act
            var visitor = OpenApiVisitorSet.GetVisitor(elementType);

            // Assert
            Assert.NotNull(visitor);
            Assert.Same(visitorType, visitor.GetType());
        }
    }
}
