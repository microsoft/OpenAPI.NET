// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Xunit;

namespace Microsoft.OpenApi.Tests.Models
{
    /// <summary>
    /// Tests for the transformations applied when an <see cref="OpenApiSchema"/> that uses
    /// JSON Schema DRAFT 2020-12 / OpenAPI 3.2 features is serialized down to OpenAPI 3.0 (which lacks those features),
    /// and the corresponding behavior when such documents are deserialized back.
    /// </summary>
    [Collection("DefaultSettings")]
    public class OpenApiSchemaV30CompatibilityTests
    {
        private static OpenApiSchema ParseSchemaFromV30Document(string schemaJson)
        {
            var jsonContent = $$"""
            {
              "openapi": "3.0.0",
              "info": { "title": "Test", "version": "1.0" },
              "paths": {},
              "components": {
                "schemas": {
                  "TestSchema": {{schemaJson}}
                }
              }
            }
            """;

            var readResult = OpenApiDocument.Parse(jsonContent, "json");
            Assert.Empty(readResult.Diagnostic.Errors);
            return Assert.IsType<OpenApiSchema>(readResult.Document.Components.Schemas["TestSchema"]);
        }

        [Fact]
        public async Task NullableEnumShouldRoundTripCorrectly()
        {
            var schema = new OpenApiSchema
            {
                Enum =
                [
                    JsonValue.Create(1),
                    JsonValue.Create(2),
                    JsonValue.Create(3),
                    JsonNullSentinel.JsonNull,
                ]
            };

            var actual = await schema.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_0);

            var expected =
                """
                {
                  "enum": [
                    1,
                    2,
                    3,
                    null
                  ]
                }
                """;
            Assert.True(JsonNode.DeepEquals(JsonNode.Parse(expected), JsonNode.Parse(actual)));

            var deserializedSchema = ParseSchemaFromV30Document(actual);

            Assert.Equal(4, deserializedSchema.Enum.Count);
            Assert.Equal(1, deserializedSchema.Enum[0].GetValue<int>());
            Assert.Equal(2, deserializedSchema.Enum[1].GetValue<int>());
            Assert.Equal(3, deserializedSchema.Enum[2].GetValue<int>());
            Assert.True(deserializedSchema.Enum[3].IsJsonNullSentinel());
        }

        [Fact]
        public async Task TypeNullAloneAsV3ShouldRoundTripCorrectly()
        {
            var schema = new OpenApiSchema { Type = JsonSchemaType.Null };

            var actual = await schema.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_0);

            var expected =
                """
                {
                  "enum": [
                    null
                  ],
                  "nullable": true
                }
                """;
            Assert.True(JsonNode.DeepEquals(JsonNode.Parse(expected), JsonNode.Parse(actual)));

            var deserializedSchema = ParseSchemaFromV30Document(actual);

            Assert.Equal(JsonSchemaType.Null, deserializedSchema.Type);
            Assert.Null(deserializedSchema.Enum);
        }

        [Fact]
        public async Task NullableTypeAsV3ShouldRoundTripCorrectly()
        {
            var schema = new OpenApiSchema { Type = JsonSchemaType.String | JsonSchemaType.Null };

            var actual = await schema.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_0);

            var expected =
                """
                {
                  "type": "string",
                  "nullable": true
                }
                """;
            Assert.True(JsonNode.DeepEquals(JsonNode.Parse(expected), JsonNode.Parse(actual)));

            var deserializedSchema = ParseSchemaFromV30Document(actual);
            Assert.Equal(JsonSchemaType.String | JsonSchemaType.Null, deserializedSchema.Type);
        }

        [Fact]
        public async Task SerializeMultipleNonNullTypesAsV3DoesNotOmitType()
        {
            var schema = new OpenApiSchema { Type = JsonSchemaType.String | JsonSchemaType.Integer };

            var actual = await schema.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_0);

            var expected = """
                {
                  "anyOf": [
                    {
                      "type": "integer"
                    },
                    {
                      "type": "string"
                    }
                  ]
                }
                """;

            Assert.True(JsonNode.DeepEquals(JsonNode.Parse(expected), JsonNode.Parse(actual)));

            var deserializedSchema = ParseSchemaFromV30Document(actual);
            Assert.Equal(schema.Type, deserializedSchema.Type);
        }

        [Fact]
        public async Task SerializeMultipleNonNullTypesWithNullAsV3DoesNotOmitType()
        {
            var schema = new OpenApiSchema
            {
                Type = JsonSchemaType.String | JsonSchemaType.Integer | JsonSchemaType.Null
            };

            var actual = await schema.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_0);

            var expected = """
                {
                  "anyOf": [
                    {
                      "type": "integer"
                    },
                    {
                      "type": "string"
                    },
                    {
                      "enum": [
                        null
                      ],
                      "nullable": true
                    }
                  ],
                  "nullable": true
                }
                """;

            Assert.True(JsonNode.DeepEquals(JsonNode.Parse(expected), JsonNode.Parse(actual)));

            var deserializedSchema = ParseSchemaFromV30Document(actual);
            Assert.Equal(schema.Type, deserializedSchema.Type);
        }

        [Fact]
        public void DeserializeAnyOfWithConstraintsAsV3PreservesConstraints()
        {
            var schemaJson = """
                {
                  "anyOf": [
                    {
                      "type": "string",
                      "format": "email",
                      "maxLength": 10
                    },
                    {
                      "type": "integer",
                      "minimum": 0
                    }
                  ]
                }
                """;

            var deserializedSchema = ParseSchemaFromV30Document(schemaJson);

            Assert.Null(deserializedSchema.Type);
            Assert.NotNull(deserializedSchema.AnyOf);
            Assert.Collection(deserializedSchema.AnyOf,
                first =>
                {
                    Assert.Equal(JsonSchemaType.String, first.Type);
                    Assert.Equal("email", first.Format);
                    Assert.Equal(10, first.MaxLength);
                },
                second =>
                {
                    Assert.Equal(JsonSchemaType.Integer, second.Type);
                    Assert.Equal("0", second.Minimum);
                });
        }

        [Fact]
        public async Task SerializeConstAsV3EmitsSingleValueEnum()
        {
            var schema = new OpenApiSchema { Type = JsonSchemaType.String, Const = "foo" };

            var actual = await schema.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_0);

            var node = JsonNode.Parse(actual)!.AsObject();
            Assert.False(node.ContainsKey("const"));
            Assert.True(node["enum"] is JsonArray enumArray
                && enumArray.Count == 1
                && enumArray[0]!.ToString() == "foo");

            var deserializedSchema = ParseSchemaFromV30Document(actual);
            Assert.Null(deserializedSchema.Const);
            Assert.NotNull(deserializedSchema.Enum);
            Assert.Single(deserializedSchema.Enum);
            Assert.Equal("foo", deserializedSchema.Enum[0]!.ToString());
        }

        [Fact]
        public async Task SerializeExclusiveMaximumAsV3EmitsMaximumWithBooleanFlag()
        {
            var schema = new OpenApiSchema
            {
                Type = JsonSchemaType.Integer,
                ExclusiveMaximum = "5"
            };

            var actual = await schema.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_0);

            var expected =
                """
                {
                  "type": "integer",
                  "maximum": 5,
                  "exclusiveMaximum": true
                }
                """;
            Assert.True(JsonNode.DeepEquals(JsonNode.Parse(expected), JsonNode.Parse(actual)));

            var deserializedSchema = ParseSchemaFromV30Document(actual);
            Assert.True(deserializedSchema.IsExclusiveMaximum);
            Assert.Null(deserializedSchema.Maximum);
            Assert.Equal("5", deserializedSchema.ExclusiveMaximum);
        }

        [Fact]
        public async Task SerializeExclusiveMinimumAsV3EmitsMinimumWithBooleanFlag()
        {
            var schema = new OpenApiSchema
            {
                Type = JsonSchemaType.Integer,
                ExclusiveMinimum = "1"
            };

            var actual = await schema.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_0);

            var expected =
                """
                {
                  "type": "integer",
                  "minimum": 1,
                  "exclusiveMinimum": true
                }
                """;
            Assert.True(JsonNode.DeepEquals(JsonNode.Parse(expected), JsonNode.Parse(actual)));

            var deserializedSchema = ParseSchemaFromV30Document(actual);
            Assert.True(deserializedSchema.IsExclusiveMinimum);
            Assert.Null(deserializedSchema.Minimum);
            Assert.Equal("1", deserializedSchema.ExclusiveMinimum);
        }

        // https://spec.openapis.org/oas/v3.2.0.html#migrating-binary-descriptions-from-oas-3-0
        [Fact]
        public async Task SerializeContentEncodingAsV3EmitsByteFormatAndRoundTrips()
        {
            var schema = new OpenApiSchema
            {
                Type = JsonSchemaType.String,
                ContentEncoding = "base64"
            };

            var actual = await schema.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_0);

            var expected =
                """
                {
                  "type": "string",
                  "format": "byte",
                  "x-jsonschema-contentEncoding": "base64"
                }
                """;
            Assert.True(JsonNode.DeepEquals(JsonNode.Parse(expected), JsonNode.Parse(actual)));

            var deserializedSchema = ParseSchemaFromV30Document(actual);
            Assert.Equal(JsonSchemaType.String, deserializedSchema.Type);
            Assert.Equal("base64", deserializedSchema.ContentEncoding);
            Assert.Null(deserializedSchema.Format);
        }

        // The content media type is what carries the binary-ness, so the type is dropped entirely.
        [Fact]
        public async Task SerializeContentMediaTypeAsV3EmitsBinaryFormatAndRoundTrips()
        {
            var schema = new OpenApiSchema
            {
                ContentMediaType = "image/png"
            };

            var actual = await schema.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_0);

            var expected =
                """
                {
                  "type": "string",
                  "format": "binary",
                  "x-jsonschema-contentMediaType": "image/png"
                }
                """;
            Assert.True(JsonNode.DeepEquals(JsonNode.Parse(expected), JsonNode.Parse(actual)));

            var deserializedSchema = ParseSchemaFromV30Document(actual);
            Assert.Null(deserializedSchema.Type);
            Assert.Equal("image/png", deserializedSchema.ContentMediaType);
            Assert.Null(deserializedSchema.Format);
        }

        [Fact]
        public async Task SerializeContentEncodingWithMediaTypeAsV3EmitsByteFormatAndRoundTrips()
        {
            var schema = new OpenApiSchema
            {
                Type = JsonSchemaType.String,
                ContentEncoding = "base64",
                ContentMediaType = "image/png"
            };

            var actual = await schema.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_0);

            var expected =
                """
                {
                  "type": "string",
                  "format": "byte",
                  "x-jsonschema-contentEncoding": "base64",
                  "x-jsonschema-contentMediaType": "image/png"
                }
                """;
            Assert.True(JsonNode.DeepEquals(JsonNode.Parse(expected), JsonNode.Parse(actual)));

            var deserializedSchema = ParseSchemaFromV30Document(actual);
            Assert.Equal(JsonSchemaType.String, deserializedSchema.Type);
            Assert.Equal("base64", deserializedSchema.ContentEncoding);
            Assert.Equal("image/png", deserializedSchema.ContentMediaType);
            Assert.Null(deserializedSchema.Format);
        }

        [Fact]
        public async Task SerializeNullableContentEncodingAsV3EmitsByteFormatAndRoundTrips()
        {
            var schema = new OpenApiSchema
            {
                Type = JsonSchemaType.String | JsonSchemaType.Null,
                ContentEncoding = "base64"
            };

            var actual = await schema.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_0);

            var expected =
                """
                {
                  "type": "string",
                  "format": "byte",
                  "nullable": true,
                  "x-jsonschema-contentEncoding": "base64"
                }
                """;
            Assert.True(JsonNode.DeepEquals(JsonNode.Parse(expected), JsonNode.Parse(actual)));

            var deserializedSchema = ParseSchemaFromV30Document(actual);
            Assert.Equal(JsonSchemaType.String | JsonSchemaType.Null, deserializedSchema.Type);
            Assert.Equal("base64", deserializedSchema.ContentEncoding);
            Assert.Null(deserializedSchema.Format);
        }

        [Fact]
        public async Task SerializeExplicitFormatAsV3IsNotOverriddenByContentKeywords()
        {
            var schema = new OpenApiSchema
            {
                Type = JsonSchemaType.String,
                Format = "password",
                ContentEncoding = "base64"
            };

            var actual = await schema.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_0);

            var expected =
                """
                {
                  "type": "string",
                  "format": "password",
                  "x-jsonschema-contentEncoding": "base64"
                }
                """;
            Assert.True(JsonNode.DeepEquals(JsonNode.Parse(expected), JsonNode.Parse(actual)));
        }

        [Fact]
        public void DeserializeByteFormatAsV3NormalizesToContentEncoding()
        {
            var deserializedSchema = ParseSchemaFromV30Document($$"""
                {
                  "type": "string",
                  "format": "byte"
                }
                """);

            Assert.Equal(JsonSchemaType.String, deserializedSchema.Type);
            Assert.Equal("base64", deserializedSchema.ContentEncoding);
            Assert.Null(deserializedSchema.Format);
        }

        [Fact]
        public void DeserializeBinaryFormatAsV3NormalizesToContentMediaType()
        {
            var deserializedSchema = ParseSchemaFromV30Document($$"""
                {
                  "type": "string",
                  "format": "binary"
                }
                """);

            Assert.Null(deserializedSchema.Type);
            Assert.Equal("application/octet-stream", deserializedSchema.ContentMediaType);
            Assert.Null(deserializedSchema.Format);
        }

        [Fact]
        public void DeserializeByteFormatAsV3WithConflictingEncodingKeepsFormat()
        {
            var deserializedSchema = ParseSchemaFromV30Document("""
                {
                  "type": "string",
                  "format": "byte",
                  "x-jsonschema-contentEncoding": "base64url"
                }
                """);

            Assert.Equal(JsonSchemaType.String, deserializedSchema.Type);
            Assert.Equal("base64url", deserializedSchema.ContentEncoding);
            Assert.Equal("byte", deserializedSchema.Format);
        }

        [Fact]
        public void DeserializeUnrelatedFormatAsV3IsNotNormalized()
        {
            var deserializedSchema = ParseSchemaFromV30Document("""
                {
                  "type": "string",
                  "format": "date-time"
                }
                """);

            Assert.Equal(JsonSchemaType.String, deserializedSchema.Type);
            Assert.Equal("date-time", deserializedSchema.Format);
            Assert.Null(deserializedSchema.ContentEncoding);
            Assert.Null(deserializedSchema.ContentMediaType);
        }
    }
}
