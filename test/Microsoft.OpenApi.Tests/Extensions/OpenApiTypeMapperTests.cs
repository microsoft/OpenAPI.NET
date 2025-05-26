// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using Microsoft.OpenApi.Extensions;
using Xunit;

namespace Microsoft.OpenApi.Tests.Extensions
{
    public class OpenApiTypeMapperTests
    {
        public static IEnumerable<object[]> PrimitiveTypeData => new List<object[]>
        {
            new object[] { typeof(int), new OpenApiSchema { Type = JsonSchemaType.Integer, Format = "int32" } },
            new object[] { typeof(decimal), new OpenApiSchema { Type = JsonSchemaType.Number, Format = "double" } },
            new object[] { typeof(decimal?), new OpenApiSchema { Type = JsonSchemaType.Number | JsonSchemaType.Null, Format = "double" } },
            new object[] { typeof(bool?), new OpenApiSchema { Type = JsonSchemaType.Boolean | JsonSchemaType.Null } },
            new object[] { typeof(Guid), new OpenApiSchema { Type = JsonSchemaType.String, Format = "uuid" } },
            new object[] { typeof(Guid?), new OpenApiSchema { Type = JsonSchemaType.String | JsonSchemaType.Null, Format = "uuid" } },
            new object[] { typeof(uint), new OpenApiSchema { Type = JsonSchemaType.Integer, Format = "int32" } },
            new object[] { typeof(long), new OpenApiSchema { Type = JsonSchemaType.Integer, Format = "int64" } },
            new object[] { typeof(long?), new OpenApiSchema { Type = JsonSchemaType.Integer | JsonSchemaType.Null, Format = "int64" } },
            new object[] { typeof(ulong), new OpenApiSchema { Type = JsonSchemaType.Integer, Format = "int64" } },
            new object[] { typeof(string), new OpenApiSchema { Type = JsonSchemaType.String } },
            new object[] { typeof(double), new OpenApiSchema { Type = JsonSchemaType.Number, Format = "double" } },
            new object[] { typeof(float?), new OpenApiSchema { Type = JsonSchemaType.Number | JsonSchemaType.Null, Format = "float" } },
            new object[] { typeof(byte?), new OpenApiSchema { Type = JsonSchemaType.String | JsonSchemaType.Null, Format = "byte" } },
            new object[] { typeof(int?), new OpenApiSchema { Type = JsonSchemaType.Integer | JsonSchemaType.Null, Format = "int32" } },
            new object[] { typeof(uint?), new OpenApiSchema { Type = JsonSchemaType.Integer | JsonSchemaType.Null, Format = "int32" } },
            new object[] { typeof(DateTimeOffset?), new OpenApiSchema { Type = JsonSchemaType.String | JsonSchemaType.Null, Format = "date-time" } },
            new object[] { typeof(double?), new OpenApiSchema { Type = JsonSchemaType.Number | JsonSchemaType.Null, Format = "double" } },
            new object[] { typeof(char?), new OpenApiSchema { Type = JsonSchemaType.String | JsonSchemaType.Null } },
            new object[] { typeof(DateTimeOffset), new OpenApiSchema { Type = JsonSchemaType.String, Format = "date-time" } }
        };

        public static IEnumerable<object[]> OpenApiDataTypes => new List<object[]>
        {
            new object[] { new OpenApiSchema { Type = JsonSchemaType.Integer, Format = "int32" }, typeof(int) },
            new object[] { new OpenApiSchema { Type = JsonSchemaType.Integer | JsonSchemaType.Null, Format = "int32"}, typeof(int?) },
            new object[] { new OpenApiSchema { Type = JsonSchemaType.Integer, Format = "int64" }, typeof(long) },
            new object[] { new OpenApiSchema { Type = JsonSchemaType.Integer | JsonSchemaType.Null, Format = "int64"}, typeof(long?) },
            new object[] { new OpenApiSchema { Type = JsonSchemaType.Number, Format = "decimal"}, typeof(decimal) },
            new object[] { new OpenApiSchema { Type = JsonSchemaType.Integer, Format = null }, typeof(long) },
            new object[] { new OpenApiSchema { Type = JsonSchemaType.Integer | JsonSchemaType.Null, Format = null}, typeof(long?) },
            new object[] { new OpenApiSchema { Type = JsonSchemaType.Number, Format = null }, typeof(double) },
            new object[] { new OpenApiSchema { Type = JsonSchemaType.Number | JsonSchemaType.Null, Format = null}, typeof(double?) },
            new object[] { new OpenApiSchema { Type = JsonSchemaType.Number | JsonSchemaType.Null, Format = "decimal"}, typeof(decimal?) },
            new object[] { new OpenApiSchema { Type = JsonSchemaType.Number | JsonSchemaType.Null, Format = "double"}, typeof(double?) },
            new object[] { new OpenApiSchema { Type = JsonSchemaType.String | JsonSchemaType.Null, Format = "date-time"}, typeof(DateTimeOffset?) },
            new object[] { new OpenApiSchema { Type = JsonSchemaType.String | JsonSchemaType.Null, Format = "char"}, typeof(char?) },
            new object[] { new OpenApiSchema { Type = JsonSchemaType.String | JsonSchemaType.Null, Format = "uuid"}, typeof(Guid?) },
            new object[] { new OpenApiSchema { Type = JsonSchemaType.String }, typeof(string) },
            new object[] { new OpenApiSchema { Type = JsonSchemaType.Number, Format = "double" }, typeof(double) },
            new object[] { new OpenApiSchema { Type = JsonSchemaType.Number | JsonSchemaType.Null, Format = "float" }, typeof(float?) },
            new object[] { new OpenApiSchema { Type = JsonSchemaType.String, Format = "date-time" }, typeof(DateTimeOffset) }
        };

        [Theory]
        [MemberData(nameof(PrimitiveTypeData))]
        public void MapTypeToOpenApiPrimitiveTypeShouldSucceed(Type type, OpenApiSchema expected)
        {
            // Arrange & Act
            var actual = OpenApiTypeMapper.MapTypeToOpenApiPrimitiveType(type);

            // Assert
            Assert.Equivalent(expected, actual);
        }

        [Theory]
        [MemberData(nameof(OpenApiDataTypes))]
        public void MapOpenApiSchemaTypeToSimpleTypeShouldSucceed(OpenApiSchema schema, Type expected)
        {
            // Arrange & Act
            var actual = OpenApiTypeMapper.MapOpenApiPrimitiveTypeToSimpleType(schema);

            // Assert
            Assert.Equal(expected, actual);
        }
    }
}
