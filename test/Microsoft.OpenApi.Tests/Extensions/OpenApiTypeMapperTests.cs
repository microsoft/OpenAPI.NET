// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Xunit;

namespace Microsoft.OpenApi.Tests.Extensions
{
    public class OpenApiTypeMapperTests
    {
        [Theory]
        [MemberData(nameof(PrimitiveTypeData))]
        public void MapTypeToOpenApiPrimitiveTypeShouldSucceed(Type type, OpenApiSchema expected)
        {
            // Arrange & Act
            var actual = OpenApiTypeMapper.MapTypeToOpenApiPrimitiveType(type);

            // Assert
            actual.Should().BeEquivalentTo(expected);
        }

        public static IEnumerable<object[]> PrimitiveTypeData => new List<object[]>
        {
            new object[] { typeof(int), new OpenApiSchema { Type = "integer", Format = "int32" } },
            new object[] { typeof(string), new OpenApiSchema { Type = "string" } },
            new object[] { typeof(double), new OpenApiSchema { Type = "number", Format = "double" } },
            new object[] { typeof(float?), new OpenApiSchema { Type = "number", Format = "float", Nullable = true } },
            new object[] { typeof(DateTimeOffset), new OpenApiSchema { Type = "string", Format = "date-time" } }
        };
    }
}
