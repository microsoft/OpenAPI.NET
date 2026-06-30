// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.Text.Json;
using Xunit;

namespace Microsoft.OpenApi.Tests.Converters
{
    [Collection("DefaultSettings")]
    public class OpenApiSchemaJsonConverterTests
    {
        private static readonly JsonSerializerOptions _optionsV31 = new()
        {
            Converters = { new OpenApiSchemaJsonConverter(OpenApiSpecVersion.OpenApi3_1) }
        };

        private static readonly JsonSerializerOptions _optionsV32 = new()
        {
            Converters = { new OpenApiSchemaJsonConverter() }
        };

        [Fact]
        public void Serialize_SimpleStringSchema_ProducesOpenApiWireFormat()
        {
            var schema = new OpenApiSchema
            {
                Type = JsonSchemaType.String,
                Description = "A simple string"
            };

            var json = JsonSerializer.Serialize(schema, _optionsV31);

            using var doc = JsonDocument.Parse(json);
            Assert.Equal("string", doc.RootElement.GetProperty("type").GetString());
            Assert.Equal("A simple string", doc.RootElement.GetProperty("description").GetString());
        }

        [Fact]
        public void Serialize_SchemaWithProperties_ProducesCorrectJson()
        {
            var schema = new OpenApiSchema
            {
                Type = JsonSchemaType.Object,
                Properties = new Dictionary<string, IOpenApiSchema>
                {
                    ["name"] = new OpenApiSchema { Type = JsonSchemaType.String },
                    ["age"] = new OpenApiSchema { Type = JsonSchemaType.Integer }
                }
            };

            var json = JsonSerializer.Serialize(schema, _optionsV31);

            using var doc = JsonDocument.Parse(json);
            Assert.Equal("object", doc.RootElement.GetProperty("type").GetString());
            var props = doc.RootElement.GetProperty("properties");
            Assert.True(props.TryGetProperty("name", out _));
            Assert.True(props.TryGetProperty("age", out _));
        }

        [Fact]
        public void Serialize_DefaultConstructor_TargetsV32()
        {
            var schema = new OpenApiSchema { Type = JsonSchemaType.Boolean };

            var json = JsonSerializer.Serialize(schema, _optionsV32);

            using var doc = JsonDocument.Parse(json);
            Assert.True(doc.RootElement.TryGetProperty("type", out _));
        }

        [Fact]
        public void Deserialize_SimpleStringSchema_ReturnsCorrectSchema()
        {
            const string json = """{"type":"string","description":"A simple string"}""";

            var schema = JsonSerializer.Deserialize<OpenApiSchema>(json, _optionsV31);

            Assert.NotNull(schema);
            Assert.Equal(JsonSchemaType.String, schema.Type);
            Assert.Equal("A simple string", schema.Description);
        }

        [Fact]
        public void Deserialize_SchemaWithEnum_ReturnsCorrectSchema()
        {
            const string json = """{"type":"string","enum":["active","inactive"]}""";

            var schema = JsonSerializer.Deserialize<OpenApiSchema>(json, _optionsV31);

            Assert.NotNull(schema);
            Assert.Equal(2, schema.Enum?.Count);
        }

        [Fact]
        public void RoundTrip_ComplexSchema_PreservesData()
        {
            var original = new OpenApiSchema
            {
                Type = JsonSchemaType.Object,
                Title = "User",
                Description = "A user object",
                Required = new System.Collections.Generic.HashSet<string> { "name" },
                Properties = new Dictionary<string, IOpenApiSchema>
                {
                    ["name"] = new OpenApiSchema { Type = JsonSchemaType.String },
                    ["age"] = new OpenApiSchema { Type = JsonSchemaType.Integer | JsonSchemaType.Null }
                }
            };

            var json = JsonSerializer.Serialize(original, _optionsV31);
            var deserialized = JsonSerializer.Deserialize<OpenApiSchema>(json, _optionsV31);

            Assert.NotNull(deserialized);
            Assert.Equal("User", deserialized.Title);
            Assert.Equal("A user object", deserialized.Description);
            Assert.True(deserialized.Properties?.ContainsKey("name"));
            Assert.True(deserialized.Properties?.ContainsKey("age"));
        }

        [Fact]
        public void Serialize_NullSchema_WritesNullLiteral()
        {
            // System.Text.Json handles null at the serializer level before invoking the converter.
            var json = JsonSerializer.Serialize<OpenApiSchema>(null!, _optionsV31);

            Assert.Equal("null", json);
        }

        [Fact]
        public void Serialize_V31Schema_IncludesJsonSchemaKeywords()
        {
            var schema = new OpenApiSchema
            {
                Type = JsonSchemaType.String,
                Id = "https://example.com/schema"
            };

            var json = JsonSerializer.Serialize(schema, _optionsV31);

            using var doc = JsonDocument.Parse(json);
            Assert.True(doc.RootElement.TryGetProperty("$id", out _), "$id is a v3.1 JSON Schema keyword");
        }

        [Fact]
        public void Deserialize_WithV2Version_ThrowsNotSupportedException()
        {
            const string json = """{"type":"string"}""";
            var optionsV2 = new JsonSerializerOptions
            {
                Converters = { new OpenApiSchemaJsonConverter(OpenApiSpecVersion.OpenApi2_0) }
            };

            Assert.Throws<NotSupportedException>(() =>
                JsonSerializer.Deserialize<OpenApiSchema>(json, optionsV2));
        }

        [Fact]
        public void Serialize_SchemaWithAllOf_ProducesCorrectJson()
        {
            var schema = new OpenApiSchema
            {
                AllOf =
                [
                    new OpenApiSchema { Type = JsonSchemaType.String }
                ]
            };

            var json = JsonSerializer.Serialize(schema, _optionsV31);

            using var doc = JsonDocument.Parse(json);
            Assert.True(doc.RootElement.TryGetProperty("allOf", out _));
        }

        [Fact]
        public void Serialize_SchemaWithInlineReference_ProducesRefInAllOf()
        {
            // A schema that uses an OpenApiSchemaReference inside allOf produces a $ref in the output.
            var document = new OpenApiDocument();
            document.Components = new OpenApiComponents
            {
                Schemas = new Dictionary<string, IOpenApiSchema>
                {
                    ["MySchema"] = new OpenApiSchema { Type = JsonSchemaType.String }
                }
            };
            document.RegisterComponents();

            var schema = new OpenApiSchema
            {
                AllOf = [new OpenApiSchemaReference("MySchema", document)]
            };

            var json = JsonSerializer.Serialize(schema, _optionsV31);

            using var doc = JsonDocument.Parse(json);
            Assert.True(doc.RootElement.TryGetProperty("allOf", out var allOf));
            var firstItem = allOf.EnumerateArray().GetEnumerator();
            Assert.True(firstItem.MoveNext());
            Assert.True(firstItem.Current.TryGetProperty("$ref", out _), "allOf item should contain a $ref");
        }
    }
}
