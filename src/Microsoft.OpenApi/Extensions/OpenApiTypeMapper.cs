// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="Type"/>.
    /// </summary>
    public static class OpenApiTypeMapper
    {
        private static readonly Dictionary<Type, Func<OpenApiSchema>> _simpleTypeToOpenApiSchema = new()
        {
            [typeof(bool)] = () => new() { Type = "boolean" },
            [typeof(byte)] = () => new() { Type = "string", Format = "byte" },
            [typeof(int)] = () => new() { Type = "integer", Format = "int32" },
            [typeof(uint)] = () => new() { Type = "integer", Format = "int32" },
            [typeof(long)] = () => new() { Type = "integer", Format = "int64" },
            [typeof(ulong)] = () => new() { Type = "integer", Format = "int64" },
            [typeof(float)] = () => new() { Type = "number", Format = "float" },
            [typeof(double)] = () => new() { Type = "number", Format = "double" },
            [typeof(decimal)] = () => new() { Type = "number", Format = "double" },
            [typeof(DateTime)] = () => new() { Type = "string", Format = "date-time" },
            [typeof(DateTimeOffset)] = () => new() { Type = "string", Format = "date-time" },
            [typeof(Guid)] = () => new() { Type = "string", Format = "uuid" },
            [typeof(char)] = () => new() { Type = "string" },

            // Nullable types
            [typeof(bool?)] = () => new() { Type = "boolean", Nullable = true },
            [typeof(byte?)] = () => new() { Type = "string", Format = "byte", Nullable = true },
            [typeof(int?)] = () => new() { Type = "integer", Format = "int32", Nullable = true },
            [typeof(uint?)] = () => new() { Type = "integer", Format = "int32", Nullable = true },
            [typeof(long?)] = () => new() { Type = "integer", Format = "int64", Nullable = true },
            [typeof(ulong?)] = () => new() { Type = "integer", Format = "int64", Nullable = true },
            [typeof(float?)] = () => new() { Type = "number", Format = "float", Nullable = true },
            [typeof(double?)] = () => new() { Type = "number", Format = "double", Nullable = true },
            [typeof(decimal?)] = () => new() { Type = "number", Format = "double", Nullable = true },
            [typeof(DateTime?)] = () => new() { Type = "string", Format = "date-time", Nullable = true },
            [typeof(DateTimeOffset?)] = () => new() { Type = "string", Format = "date-time", Nullable = true },
            [typeof(Guid?)] = () => new() { Type = "string", Format = "uuid", Nullable = true },
            [typeof(char?)] = () => new() { Type = "string", Nullable = true },

            [typeof(Uri)] = () => new() { Type = "string", Format = "uri"}, // Uri is treated as simple string
            [typeof(string)] = () => new() { Type = "string" },
            [typeof(object)] = () => new() { Type = "object" }
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
        /// integer          integer int32       signed 32 bits
        /// long             integer int64       signed 64 bits
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
                : new() { Type = "string" };
        }

        /// <summary>
        /// Maps an OpenAPI data type and format to a simple type.
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

            var type = (schema.Type?.ToLowerInvariant(), schema.Format?.ToLowerInvariant(), schema.Nullable) switch
            {
                ("boolean", null, false) => typeof(bool),
                ("integer", "int32", false) => typeof(int),
                ("integer", "int64", false) => typeof(long),
                ("integer", null, false) => typeof(int),
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
                ("integer", "int32", true) => typeof(int?),
                ("integer", "int64", true) => typeof(long?),
                ("integer", null, true) => typeof(int?),
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
