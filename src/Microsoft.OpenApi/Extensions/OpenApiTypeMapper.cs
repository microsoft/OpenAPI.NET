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
            [typeof(bool?)] = () => new() { Type = JsonSchemaType.Boolean, Nullable = true },
            [typeof(byte?)] = () => new() { Type = JsonSchemaType.String, Format = "byte", Nullable = true },
            [typeof(int?)] = () => new() { Type = JsonSchemaType.Integer, Format = "int32", Nullable = true },
            [typeof(uint?)] = () => new() { Type = JsonSchemaType.Integer, Format = "int32", Nullable = true },
            [typeof(long?)] = () => new() { Type = JsonSchemaType.Integer, Format = "int64", Nullable = true },
            [typeof(ulong?)] = () => new() { Type = JsonSchemaType.Integer, Format = "int64", Nullable = true },
            [typeof(float?)] = () => new() { Type = JsonSchemaType.Number, Format = "float", Nullable = true },
            [typeof(double?)] = () => new() { Type = JsonSchemaType.Number, Format = "double", Nullable = true },
            [typeof(decimal?)] = () => new() { Type = JsonSchemaType.Number, Format = "double", Nullable = true },
            [typeof(DateTime?)] = () => new() { Type = JsonSchemaType.String, Format = "date-time", Nullable = true },
            [typeof(DateTimeOffset?)] = () => new() { Type = JsonSchemaType.String, Format = "date-time", Nullable = true },
            [typeof(Guid?)] = () => new() { Type = JsonSchemaType.String, Format = "uuid", Nullable = true },
            [typeof(char?)] = () => new() { Type = JsonSchemaType.String, Nullable = true },

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

            var type = (schema.Type.ToIdentifier(), schema.Format?.ToLowerInvariant(), schema.Nullable) switch
            {
                ("boolean", null, false) => typeof(bool),
                // integer is technically not valid with format, but we must provide some compatibility
                ("integer" or "number", "int32", false) => typeof(int),
                ("integer" or "number", "int64", false) => typeof(long),
                ("integer", null, false) => typeof(long),
                ("number", "float", false) => typeof(float),
                ("number", "double", false) => typeof(double),
                ("number", "decimal", false) => typeof(decimal),
                ("number", null, false) => typeof(double),
                ("string", "byte", false) => typeof(byte),
                ("string", "date-time", false) => typeof(DateTimeOffset),
                ("string", "uuid", false) => typeof(Guid),
                ("string", "duration", false) => typeof(TimeSpan),
                ("string", "char", false) => typeof(char),
                ("string", null, false) => typeof(string),
                ("object", null, false) => typeof(object),
                ("string", "uri", false) => typeof(Uri),
                ("integer" or "number", "int32", true) => typeof(int?),
                ("integer" or "number", "int64", true) => typeof(long?),
                ("integer", null, true) => typeof(long?),
                ("number", "float", true) => typeof(float?),
                ("number", "double", true) => typeof(double?),
                ("number", null, true) => typeof(double?),
                ("number", "decimal", true) => typeof(decimal?),
                ("string", "byte", true) => typeof(byte?),
                ("string", "date-time", true) => typeof(DateTimeOffset?),
                ("string", "uuid", true) => typeof(Guid?),
                ("string", "char", true) => typeof(char?),
                ("boolean", null, true) => typeof(bool?),
                _ => typeof(string),
            };

            return type;
        }
    }
}
