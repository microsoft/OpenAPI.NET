// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using Microsoft.OpenApi.Exceptions;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="Type"/>.
    /// </summary>
    public static class OpenApiTypeMapper
    {
#nullable enable
        /// <summary>
        /// Maps a JsonSchema data type to an identifier.
        /// </summary>
        /// <param name="schemaType"></param>
        /// <returns></returns>
        public static string? ToIdentifier(this JsonSchemaType? schemaType)
        {
            if (schemaType is null)
            {
                return null;
            }
            return schemaType.Value.ToIdentifier();
        }

        /// <summary>
        /// Maps a JsonSchema data type to an identifier.
        /// </summary>
        /// <param name="schemaType"></param>
        /// <returns></returns>
        public static string? ToIdentifier(this JsonSchemaType schemaType)
        {
            return schemaType switch
            {
                JsonSchemaType.Null => "null",
                JsonSchemaType.Boolean => "boolean",
                JsonSchemaType.Integer => "integer",
                JsonSchemaType.Number => "number",
                JsonSchemaType.String => "string",
                JsonSchemaType.Array => "array",
                JsonSchemaType.Object => "object",
                _ => null,
            };
        }
#nullable restore

        /// <summary>
        /// Converts a schema type's identifier into the enum equivalent
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        public static JsonSchemaType ToJsonSchemaType(this string identifier)
        {
            return identifier switch
            {
                "null" => JsonSchemaType.Null,
                "boolean" => JsonSchemaType.Boolean,
                "integer" or "int" => JsonSchemaType.Integer,
                "number" or "double" or "float" or "decimal"=> JsonSchemaType.Number,
                "string" => JsonSchemaType.String,
                "array" => JsonSchemaType.Array,
                "object" => JsonSchemaType.Object,
                "file" => JsonSchemaType.String, // File is treated as string
                _ => throw new OpenApiException(string.Format("Invalid schema type identifier: {0}", identifier))
            };
        }

        private static readonly Dictionary<Type, Func<OpenApiSchema>> _simpleTypeToOpenApiSchema = new()
        {
            [typeof(bool)] = () => new() { Type = JsonSchemaType.Boolean },
            [typeof(byte)] = () => new() { Type = JsonSchemaType.String, Format = "byte" },
            [typeof(int)] = () => new() { Type = JsonSchemaType.Integer, Format = "int32" },
            [typeof(uint)] = () => new() { Type = JsonSchemaType.Integer, Format = "int32" },
            [typeof(long)] = () => new() { Type = JsonSchemaType.Integer, Format = "int64" },
            [typeof(ulong)] = () => new() { Type = JsonSchemaType.Integer, Format = "int64" },
            [typeof(float)] = () => new() { Type = JsonSchemaType.Number, Format = "float" },
            [typeof(double)] = () => new() { Type = JsonSchemaType.Number, Format = "double" },
            [typeof(decimal)] = () => new() { Type = JsonSchemaType.Number, Format = "double" },
            [typeof(DateTime)] = () => new() { Type = JsonSchemaType.String, Format = "date-time" },
            [typeof(DateTimeOffset)] = () => new() { Type = JsonSchemaType.String, Format = "date-time" },
            [typeof(Guid)] = () => new() { Type = JsonSchemaType.String, Format = "uuid" },
            [typeof(char)] = () => new() { Type = JsonSchemaType.String },

            // Nullable types
            [typeof(bool?)] = () => new() { Type = JsonSchemaType.Boolean | JsonSchemaType.Null },
            [typeof(byte?)] = () => new() { Type = JsonSchemaType.String | JsonSchemaType.Null, Format = "byte" },
            [typeof(int?)] = () => new() { Type = JsonSchemaType.Integer | JsonSchemaType.Null, Format = "int32" },
            [typeof(uint?)] = () => new() { Type = JsonSchemaType.Integer | JsonSchemaType.Null, Format = "int32" },
            [typeof(long?)] = () => new() { Type = JsonSchemaType.Integer | JsonSchemaType.Null, Format = "int64" },
            [typeof(ulong?)] = () => new() { Type = JsonSchemaType.Integer | JsonSchemaType.Null, Format = "int64" },
            [typeof(float?)] = () => new() { Type = JsonSchemaType.Number | JsonSchemaType.Null, Format = "float" },
            [typeof(double?)] = () => new() { Type = JsonSchemaType.Number | JsonSchemaType.Null, Format = "double" },
            [typeof(decimal?)] = () => new() { Type = JsonSchemaType.Number | JsonSchemaType.Null, Format = "double" },
            [typeof(DateTime?)] = () => new() { Type = JsonSchemaType.String | JsonSchemaType.Null, Format = "date-time" },
            [typeof(DateTimeOffset?)] = () => new() { Type = JsonSchemaType.String | JsonSchemaType.Null, Format = "date-time" },
            [typeof(Guid?)] = () => new() { Type = JsonSchemaType.String | JsonSchemaType.Null, Format = "uuid" },
            [typeof(char?)] = () => new() { Type = JsonSchemaType.String | JsonSchemaType.Null },

            [typeof(Uri)] = () => new() { Type = JsonSchemaType.String, Format = "uri" }, // Uri is treated as simple string
            [typeof(string)] = () => new() { Type = JsonSchemaType.String },
            [typeof(object)] = () => new() { Type = JsonSchemaType.Object }
        };

        /// <summary>
        /// Maps a simple type to an OpenAPI data type and format.
        /// </summary>
        /// <param name="type">Simple type.</param>
        /// <remarks>
        /// All the following types from http://swagger.io/specification/#data-types-12 are supported.
        /// Other types including nullables and URL are also supported.
        /// Common Name      type    format      Comments
        /// ===========      ======= ======      =========================================
        /// integer          number int32       signed 32 bits
        /// long             number int64       signed 64 bits
        /// float            number  float
        /// double           number  double
        /// string           string  [empty]
        /// byte             string  byte        base64 encoded characters
        /// binary           string  binary      any sequence of octets
        /// boolean          boolean [empty]
        /// date             string  date        As defined by full-date - RFC3339
        /// dateTime         string  date-time   As defined by date-time - RFC3339
        /// password         string  password    Used to hint UIs the input needs to be obscured.
        /// If the type is not recognized as "simple", System.String will be returned.
        /// </remarks>
        public static OpenApiSchema MapTypeToOpenApiPrimitiveType(this Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return _simpleTypeToOpenApiSchema.TryGetValue(type, out var result)
                ? result()
                : new() { Type = JsonSchemaType.String };
        }

        /// <summary>
        /// Maps an JsonSchema data type and format to a simple type.
        /// </summary>
        /// <param name="schema">The OpenApi data type</param>
        /// <returns>The simple type</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static Type MapOpenApiPrimitiveTypeToSimpleType(this OpenApiSchema schema)
        {
            if (schema == null)
            {
                throw new ArgumentNullException(nameof(schema));
            }

            var type = ((schema.Type & ~JsonSchemaType.Null).ToIdentifier(), schema.Format?.ToLowerInvariant(), schema.Type & JsonSchemaType.Null) switch
            {
                ("integer" or "number", "int32", JsonSchemaType.Null) => typeof(int?),
                ("integer" or "number", "int64", JsonSchemaType.Null) => typeof(long?),
                ("integer", null, JsonSchemaType.Null) => typeof(long?),
                ("number", "float", JsonSchemaType.Null) => typeof(float?),
                ("number", "double", JsonSchemaType.Null) => typeof(double?),
                ("number", null, JsonSchemaType.Null) => typeof(double?),
                ("number", "decimal", JsonSchemaType.Null) => typeof(decimal?),
                ("string", "byte", JsonSchemaType.Null) => typeof(byte?),
                ("string", "date-time", JsonSchemaType.Null) => typeof(DateTimeOffset?),
                ("string", "uuid", JsonSchemaType.Null) => typeof(Guid?),
                ("string", "char", JsonSchemaType.Null) => typeof(char?),
                ("boolean", null, JsonSchemaType.Null) => typeof(bool?),
                ("boolean", null, _) => typeof(bool),
                // integer is technically not valid with format, but we must provide some compatibility
                ("integer" or "number", "int32", _) => typeof(int),
                ("integer" or "number", "int64", _) => typeof(long),
                ("integer", null, _) => typeof(long),
                ("number", "float", _) => typeof(float),
                ("number", "double", _) => typeof(double),
                ("number", "decimal", _) => typeof(decimal),
                ("number", null, _) => typeof(double),
                ("string", "byte", _) => typeof(byte),
                ("string", "date-time", _) => typeof(DateTimeOffset),
                ("string", "uuid", _) => typeof(Guid),
                ("string", "duration", _) => typeof(TimeSpan),
                ("string", "char", _) => typeof(char),
                ("string", null, _) => typeof(string),
                ("object", null, _) => typeof(object),
                ("string", "uri", _) => typeof(Uri),
                _ => typeof(string),
            };

            return type;
        }
    }
}
