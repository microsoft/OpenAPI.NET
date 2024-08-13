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
        public static IEnumerable<object[]> PrimitiveTypeData => new List<object[]>
        {
            new object[] { typeof(int), new OpenApiSchema { Type = "integer", Format = "int32" } },
            new object[] { typeof(string), new OpenApiSchema { Type = "string" } },
            new object[] { typeof(double), new OpenApiSchema { Type = "number", Format = "double" } },
            new object[] { typeof(float?), new OpenApiSchema { Type = "number", Format = "float", Nullable = true } },
            new object[] { typeof(DateTimeOffset), new OpenApiSchema { Type = "string", Format = "date-time" } }
        };

        public static IEnumerable<object[]> OpenApiDataTypes => new List<object[]>
        {
            new object[] { new OpenApiSchema { Type = "integer", Format = "int32"}, typeof(int) },
            new object[] { new OpenApiSchema { Type = "integer", Format = null, Nullable = false}, typeof(int) },
            new object[] { new OpenApiSchema { Type = "integer", Format = null, Nullable = true}, typeof(int?) },
            new object[] { new OpenApiSchema { Type = "string" }, typeof(string) },
            new object[] { new OpenApiSchema { Type = "number", Format = "double" }, typeof(double) },
            new object[] { new OpenApiSchema { Type = "number", Format = "float", Nullable = true }, typeof(float?) },
            new object[] { new OpenApiSchema { Type = "string", Format = "date-time" }, typeof(DateTimeOffset) }
        };

        [Theory]
        [MemberData(nameof(PrimitiveTypeData))]
        public void MapTypeToOpenApiPrimitiveTypeShouldSucceed(Type type, OpenApiSchema expected)
        {
            // Arrange & Act
            var actual = OpenApiTypeMapper.MapTypeToOpenApiPrimitiveType(type);

            // Assert
            actual.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [MemberData(nameof(OpenApiDataTypes))]
        public void MapOpenApiSchemaTypeToSimpleTypeShouldSucceed(OpenApiSchema schema, Type expected)
        {
            // Arrange & Act
            var actual = OpenApiTypeMapper.MapOpenApiPrimitiveTypeToSimpleType(schema);

            // Assert
            actual.Should().Be(expected);
        }
    }
}
