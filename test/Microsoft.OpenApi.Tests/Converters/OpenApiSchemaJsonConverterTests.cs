// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.Text.Json;
using FluentAssertions;
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

        private static readonly JsonSerializerOptions _optionsV3 = new()
        {
            Converters = { new OpenApiSchemaJsonConverter(OpenApiSpecVersion.OpenApi3_0) }
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
            doc.RootElement.GetProperty("type").GetString().Should().Be("string");
            doc.RootElement.GetProperty("description").GetString().Should().Be("A simple string");
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
            doc.RootElement.GetProperty("type").GetString().Should().Be("object");
            doc.RootElement.GetProperty("properties").EnumerateObject().Should().HaveCount(2);
        }

        [Fact]
        public void Serialize_DefaultConstructor_TargetsV31()
        {
            var converter = new OpenApiSchemaJsonConverter();
            var options = new JsonSerializerOptions { Converters = { converter } };

            var schema = new OpenApiSchema { Type = JsonSchemaType.Boolean };
            var json = JsonSerializer.Serialize(schema, options);

            json.Should().Contain("\"type\"");
        }

        [Fact]
        public void Deserialize_SimpleStringSchema_ReturnsCorrectSchema()
        {
            const string json = """{"type":"string","description":"A simple string"}""";

            var schema = JsonSerializer.Deserialize<OpenApiSchema>(json, _optionsV31);

            schema.Should().NotBeNull();
            schema!.Type.Should().Be(JsonSchemaType.String);
            schema.Description.Should().Be("A simple string");
        }

        [Fact]
        public void Deserialize_SchemaWithEnum_ReturnsCorrectSchema()
        {
            const string json = """{"type":"string","enum":["active","inactive"]}""";

            var schema = JsonSerializer.Deserialize<OpenApiSchema>(json, _optionsV31);

            schema.Should().NotBeNull();
            schema!.Enum.Should().HaveCount(2);
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

            deserialized.Should().NotBeNull();
            deserialized!.Title.Should().Be("User");
            deserialized.Description.Should().Be("A user object");
            deserialized.Properties.Should().ContainKey("name");
            deserialized.Properties.Should().ContainKey("age");
        }

        [Fact]
        public void Serialize_NullSchema_WritesNullLiteral()
        {
            // System.Text.Json handles null at the serializer level before invoking the converter,
            // producing a JSON null literal rather than throwing.
            var json = JsonSerializer.Serialize<OpenApiSchema>(null!, _optionsV31);

            json.Should().Be("null");
        }

        [Fact]
        public void Serialize_V31Schema_IncludesJsonSchemaKeywords()
        {
            var schema = new OpenApiSchema
            {
                Type = JsonSchemaType.String,
                Id = "https://example.com/schema"
            };

            var jsonV31 = JsonSerializer.Serialize(schema, _optionsV31);

            using var doc = JsonDocument.Parse(jsonV31);
            // $id is a JSON Schema 2020-12 keyword only written in v3.1+
            doc.RootElement.TryGetProperty("$id", out _).Should().BeTrue("$id is a v3.1 JSON Schema keyword");
        }
    }
}
