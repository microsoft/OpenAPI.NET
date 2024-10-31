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
            new object[] { typeof(int), new OpenApiSchema { Type = JsonSchemaType.Integer, Format = "int32" } },
            new object[] { typeof(decimal), new OpenApiSchema { Type = JsonSchemaType.Number, Format = "double" } },
            new object[] { typeof(decimal?), new OpenApiSchema { Type = JsonSchemaType.Number, Format = "double", Nullable = true } },
            new object[] { typeof(bool?), new OpenApiSchema { Type = JsonSchemaType.Boolean, Nullable = true } },
            new object[] { typeof(Guid), new OpenApiSchema { Type = JsonSchemaType.String, Format = "uuid" } },
            new object[] { typeof(Guid?), new OpenApiSchema { Type = JsonSchemaType.String, Format = "uuid", Nullable = true } },
            new object[] { typeof(uint), new OpenApiSchema { Type = JsonSchemaType.Integer, Format = "int32" } },
            new object[] { typeof(long), new OpenApiSchema { Type = JsonSchemaType.Integer, Format = "int64" } },
            new object[] { typeof(long?), new OpenApiSchema { Type = JsonSchemaType.Integer, Format = "int64", Nullable = true } },
            new object[] { typeof(ulong), new OpenApiSchema { Type = JsonSchemaType.Integer, Format = "int64" } },
            new object[] { typeof(string), new OpenApiSchema { Type = JsonSchemaType.String } },
            new object[] { typeof(double), new OpenApiSchema { Type = JsonSchemaType.Number, Format = "double" } },
            new object[] { typeof(float?), new OpenApiSchema { Type = JsonSchemaType.Number, Format = "float", Nullable = true } },
            new object[] { typeof(byte?), new OpenApiSchema { Type = JsonSchemaType.String, Format = "byte", Nullable = true } },
            new object[] { typeof(int?), new OpenApiSchema { Type = JsonSchemaType.Integer, Format = "int32", Nullable = true } },
            new object[] { typeof(uint?), new OpenApiSchema { Type = JsonSchemaType.Integer, Format = "int32", Nullable = true } },
            new object[] { typeof(DateTimeOffset?), new OpenApiSchema { Type = JsonSchemaType.String, Format = "date-time", Nullable = true } },
            new object[] { typeof(double?), new OpenApiSchema { Type = JsonSchemaType.Number, Format = "double", Nullable = true } },
            new object[] { typeof(char?), new OpenApiSchema { Type = JsonSchemaType.String, Nullable = true } },
            new object[] { typeof(DateTimeOffset), new OpenApiSchema { Type = JsonSchemaType.String, Format = "date-time" } }
        };

        public static IEnumerable<object[]> OpenApiDataTypes => new List<object[]>
        {
            new object[] { new OpenApiSchema { Type = JsonSchemaType.Integer, Format = "int32", Nullable = false}, typeof(int) },
            new object[] { new OpenApiSchema { Type = JsonSchemaType.Integer, Format = "int32", Nullable = true}, typeof(int?) },
            new object[] { new OpenApiSchema { Type = JsonSchemaType.Integer, Format = "int64", Nullable = false}, typeof(long) },
            new object[] { new OpenApiSchema { Type = JsonSchemaType.Integer, Format = "int64", Nullable = true}, typeof(long?) },
            new object[] { new OpenApiSchema { Type = JsonSchemaType.Number, Format = "decimal"}, typeof(decimal) },
            new object[] { new OpenApiSchema { Type = JsonSchemaType.Integer, Format = null, Nullable = false}, typeof(long) },
            new object[] { new OpenApiSchema { Type = JsonSchemaType.Integer, Format = null, Nullable = true}, typeof(long?) },
            new object[] { new OpenApiSchema { Type = JsonSchemaType.Number, Format = null, Nullable = false}, typeof(double) },
            new object[] { new OpenApiSchema { Type = JsonSchemaType.Number, Format = null, Nullable = true}, typeof(double?) },
            new object[] { new OpenApiSchema { Type = JsonSchemaType.Number, Format = "decimal", Nullable = true}, typeof(decimal?) },
            new object[] { new OpenApiSchema { Type = JsonSchemaType.Number, Format = "double", Nullable = true}, typeof(double?) },
            new object[] { new OpenApiSchema { Type = JsonSchemaType.String, Format = "date-time", Nullable = true}, typeof(DateTimeOffset?) },
            new object[] { new OpenApiSchema { Type = JsonSchemaType.String, Format = "char", Nullable = true}, typeof(char?) },
            new object[] { new OpenApiSchema { Type = JsonSchemaType.String, Format = "uuid", Nullable = true}, typeof(Guid?) },
            new object[] { new OpenApiSchema { Type = JsonSchemaType.String }, typeof(string) },
            new object[] { new OpenApiSchema { Type = JsonSchemaType.Number, Format = "double" }, typeof(double) },
            new object[] { new OpenApiSchema { Type = JsonSchemaType.Number, Format = "float", Nullable = true }, typeof(float?) },
            new object[] { new OpenApiSchema { Type = JsonSchemaType.String, Format = "date-time" }, typeof(DateTimeOffset) }
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
