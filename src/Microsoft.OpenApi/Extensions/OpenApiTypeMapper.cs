// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OpenApi
{
    /// <summary>
    /// Extension methods for <see cref="Type"/>.
    /// </summary>
    public static class OpenApiTypeMapper
    {
        /// <summary>
        /// Maps a JsonSchema data type to an identifier.
        /// </summary>
        /// <param name="schemaType"></param>
        /// <returns></returns>
        public static string[]? ToIdentifiers(this JsonSchemaType? schemaType)
        {
            if (schemaType is null)
            {
                return null;
            }
            return schemaType.Value.ToIdentifiers();
        }

        /// <summary>
        /// Maps a JsonSchema data type to an identifier.
        /// </summary>
        /// <param name="schemaType"></param>
        /// <returns></returns>
        public static string[] ToIdentifiers(this JsonSchemaType schemaType)
        {
            return schemaType.ToIdentifiersInternal().ToArray();
        }

        private static readonly Dictionary<JsonSchemaType, string> allSchemaTypes = new()
        {
            { JsonSchemaType.Boolean, "boolean" },
            { JsonSchemaType.Integer, "integer" },
            { JsonSchemaType.Number, "number" },
            { JsonSchemaType.String, "string" },
            { JsonSchemaType.Object, "object" },
            { JsonSchemaType.Array, "array" },
            { JsonSchemaType.Null, "null" }
        };

        private static IEnumerable<string> ToIdentifiersInternal(this JsonSchemaType schemaType)
        {
            return allSchemaTypes.Where(kvp => schemaType.HasFlag(kvp.Key)).Select(static kvp => kvp.Value);
        }

        /// <summary>
        /// Returns the first identifier from a string array.
        /// </summary>
        /// <param name="schemaType"></param>
        /// <returns></returns>
        internal static string ToFirstIdentifier(this JsonSchemaType schemaType)
        {
            return schemaType.ToIdentifiersInternal().First();
        }

        /// <summary>
        /// Returns a single identifier from an array with only one item.
        /// </summary>
        /// <param name="schemaType"></param>
        /// <returns></returns>
        internal static string ToSingleIdentifier(this JsonSchemaType schemaType)
        {
            return schemaType.ToIdentifiersInternal().Single();
        }

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

        /// <summary>
        /// Converts a schema type's identifier into the enum equivalent
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        public static JsonSchemaType? ToJsonSchemaType(this string[] identifier)
        {
            if (identifier == null)
            {
                return null;
            }

            JsonSchemaType type = 0;
            foreach (var id in identifier)
            {
                type |= id.ToJsonSchemaType();
            }
            return type;
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
        /// Maps a JsonSchema data type and format to a simple type.
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

            var type = (schema.Type, schema.Format?.ToLowerInvariant()) switch
            {
                (JsonSchemaType.Integer | JsonSchemaType.Null or JsonSchemaType.Number | JsonSchemaType.Null, "int32") => typeof(int?),
                (JsonSchemaType.Integer | JsonSchemaType.Null or JsonSchemaType.Number | JsonSchemaType.Null, "int64") => typeof(long?),
                (JsonSchemaType.Integer | JsonSchemaType.Null, null) => typeof(long?),
                (JsonSchemaType.Number | JsonSchemaType.Null, "float") => typeof(float?),
                (JsonSchemaType.Number | JsonSchemaType.Null, "double") => typeof(double?),
                (JsonSchemaType.Number | JsonSchemaType.Null, null) => typeof(double?),
                (JsonSchemaType.Number | JsonSchemaType.Null, "decimal") => typeof(decimal?),
                (JsonSchemaType.String | JsonSchemaType.Null, "byte") => typeof(byte?),
                (JsonSchemaType.String | JsonSchemaType.Null, "date-time") => typeof(DateTimeOffset?),
                (JsonSchemaType.String | JsonSchemaType.Null, "uuid") => typeof(Guid?),
                (JsonSchemaType.String | JsonSchemaType.Null, "char") => typeof(char?),
                (JsonSchemaType.Boolean | JsonSchemaType.Null, null) => typeof(bool?),
                (JsonSchemaType.Boolean, null) => typeof(bool),
                // integer is technically not valid with format, but we must provide some compatibility
                (JsonSchemaType.Integer or JsonSchemaType.Number, "int32") => typeof(int),
                (JsonSchemaType.Integer or JsonSchemaType.Number, "int64") => typeof(long),
                (JsonSchemaType.Integer, null) => typeof(long),
                (JsonSchemaType.Number, "float") => typeof(float),
                (JsonSchemaType.Number, "double") => typeof(double),
                (JsonSchemaType.Number, "decimal") => typeof(decimal),
                (JsonSchemaType.Number, null) => typeof(double),
                (JsonSchemaType.String, "byte") => typeof(byte),
                (JsonSchemaType.String, "date-time") => typeof(DateTimeOffset),
                (JsonSchemaType.String, "uuid") => typeof(Guid),
                (JsonSchemaType.String, "char") => typeof(char),
                (JsonSchemaType.String, null) => typeof(string),
                (JsonSchemaType.Object, null) => typeof(object),
                (JsonSchemaType.String, "uri") => typeof(Uri),
                _ => typeof(string),
            };

            return type;
        }
    }
}
