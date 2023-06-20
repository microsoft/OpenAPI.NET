// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;
using FluentAssertions;
using Json.Schema;
using Microsoft.OpenApi.Extensions;
using Xunit;

namespace Microsoft.OpenApi.Tests.Extensions
{
    public class OpenApiTypeMapperTests
    {
        public static IEnumerable<object[]> PrimitiveTypeData => new List<object[]>
        {
            new object[] { typeof(int), new JsonSchemaBuilder().Type(SchemaValueType.Integer).Format("int32").Build() },
            new object[] { typeof(string), new JsonSchemaBuilder().Type(SchemaValueType.String).Build() },
            new object[] { typeof(double), new JsonSchemaBuilder().Type(SchemaValueType.Number).Format("double").Build() },
            new object[] { typeof(DateTimeOffset), new JsonSchemaBuilder().Type(SchemaValueType.String).Format("date-time").Build() }
        };

        public static IEnumerable<object[]> JsonSchemaDataTypes => new List<object[]>
        {
            new object[] { new JsonSchemaBuilder().Type(SchemaValueType.Integer).Format("int32").Build(), typeof(int) },
            new object[] { new JsonSchemaBuilder().Type(SchemaValueType.Number).Format("double").Build(), typeof(double) },
            new object[] { new JsonSchemaBuilder().AnyOf(
                new JsonSchemaBuilder().Type(SchemaValueType.Integer).Build(),
                new JsonSchemaBuilder().Type(SchemaValueType.Integer).Build())
                .Format("float").Build(), typeof(float?) },
            new object[] { new JsonSchemaBuilder().Type(SchemaValueType.String).Format("date-time").Build(), typeof(DateTimeOffset) }
        };

        [Theory]
        [MemberData(nameof(PrimitiveTypeData))]
        public void MapTypeToJsonPrimitiveTypeShouldSucceed(Type type, JsonSchema expected)
        {
            // Arrange & Act
            var actual = OpenApiTypeMapper.MapTypeToJsonPrimitiveType(type);

            // Assert
            actual.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [MemberData(nameof(JsonSchemaDataTypes))]
        public void MapOpenApiSchemaTypeToSimpleTypeShouldSucceed(JsonSchema schema, Type expected)
        {
            // Arrange & Act
            var actual = OpenApiTypeMapper.MapJsonSchemaValueTypeToSimpleType(schema);

            // Assert
            actual.Should().Be(expected);
        }
    }
}
