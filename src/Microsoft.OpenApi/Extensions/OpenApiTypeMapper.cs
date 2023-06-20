// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;
using Json.Schema;

namespace Microsoft.OpenApi.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="Type"/>.
    /// </summary>
    public static class OpenApiTypeMapper
    {
        private static readonly Dictionary<Type, Func<JsonSchema>> _simpleTypeToJsonSchema = new()
        {
            [typeof(bool)] = () => new JsonSchemaBuilder().Type(SchemaValueType.Boolean).Build(),
            [typeof(byte)] = () => new JsonSchemaBuilder().Type(SchemaValueType.String).Format("byte").Build(),
            [typeof(int)] = () => new JsonSchemaBuilder().Type(SchemaValueType.Integer).Format("int32").Build(),
            [typeof(uint)] = () => new JsonSchemaBuilder().Type(SchemaValueType.Integer).Format("int32").Build(),
            [typeof(long)] = () => new JsonSchemaBuilder().Type(SchemaValueType.Integer).Format("int64").Build(),
            [typeof(ulong)] = () => new JsonSchemaBuilder().Type(SchemaValueType.Integer).Format("int64").Build(),
            [typeof(float)] = () => new JsonSchemaBuilder().Type(SchemaValueType.Number).Format("float").Build(),
            [typeof(double)] = () => new JsonSchemaBuilder().Type(SchemaValueType.Number).Format("double").Build(),
            [typeof(decimal)] = () => new JsonSchemaBuilder().Type(SchemaValueType.Number).Format("double").Build(),
            [typeof(DateTime)] = () => new JsonSchemaBuilder().Type(SchemaValueType.String).Format("date-time").Build(),
            [typeof(DateTimeOffset)] = () => new JsonSchemaBuilder().Type(SchemaValueType.String).Format("date-time").Build(),
            [typeof(Guid)] = () => new JsonSchemaBuilder().Type(SchemaValueType.String).Format("uuid").Build(),
            [typeof(char)] = () => new JsonSchemaBuilder().Type(SchemaValueType.String).Format("string").Build(),

            // Nullable types
            [typeof(bool?)] = () => new JsonSchemaBuilder()
                .AnyOf(
                        new JsonSchemaBuilder().Type(SchemaValueType.Null).Build(),
                        new JsonSchemaBuilder().Type(SchemaValueType.Boolean).Build()
                ).Build(),

            [typeof(byte?)] = () => new JsonSchemaBuilder()
            .AnyOf(
                        new JsonSchemaBuilder().Type(SchemaValueType.Null).Build(),
                        new JsonSchemaBuilder().Type(SchemaValueType.String).Build()
                )
            .Format("byte").Build(),

            [typeof(int?)] = () => new JsonSchemaBuilder()
            .AnyOf(
                        new JsonSchemaBuilder().Type(SchemaValueType.Null).Build(),
                        new JsonSchemaBuilder().Type(SchemaValueType.Integer).Build()
                )
            .Format("int32").Build(),

            [typeof(uint?)] = () => new JsonSchemaBuilder().AnyOf(
                        new JsonSchemaBuilder().Type(SchemaValueType.Null).Build(),
                        new JsonSchemaBuilder().Type(SchemaValueType.Integer).Build()
                )
            .Format("int32").Build(),

            [typeof(long?)] = () => new JsonSchemaBuilder()
            .AnyOf(
                new JsonSchemaBuilder().Type(SchemaValueType.Null).Build(),
                new JsonSchemaBuilder().Type(SchemaValueType.Integer).Build()
                )
            .Format("int64").Build(),

            [typeof(ulong?)] = () => new JsonSchemaBuilder()
            .AnyOf(
                new JsonSchemaBuilder().Type(SchemaValueType.Null).Build(),
                new JsonSchemaBuilder().Type(SchemaValueType.Integer).Build()
                )
            .Format("int64").Build(),

            [typeof(float?)] = () => new JsonSchemaBuilder()
            .AnyOf(
                new JsonSchemaBuilder().Type(SchemaValueType.Null).Build(),
                new JsonSchemaBuilder().Type(SchemaValueType.Integer).Build()
                )
            .Format("float").Build(),

            [typeof(double?)] = () => new JsonSchemaBuilder()
            .AnyOf(
                new JsonSchemaBuilder().Type(SchemaValueType.Null).Build(),
                new JsonSchemaBuilder().Type(SchemaValueType.Number).Build())
            .Format("double").Build(),

            [typeof(decimal?)] = () => new JsonSchemaBuilder()
            .AnyOf(
                new JsonSchemaBuilder().Type(SchemaValueType.Null).Build(),
                new JsonSchemaBuilder().Type(SchemaValueType.Number).Build()
                )
            .Format("double").Build(),

            [typeof(DateTime?)] = () => new JsonSchemaBuilder()
            .AnyOf(
                new JsonSchemaBuilder().Type(SchemaValueType.Null).Build(),
                new JsonSchemaBuilder().Type(SchemaValueType.String).Build()
                )
            .Format("date-time").Build(),

            [typeof(DateTimeOffset?)] = () => new JsonSchemaBuilder()
            .AnyOf(
                new JsonSchemaBuilder().Type(SchemaValueType.Null).Build(),
                new JsonSchemaBuilder().Type(SchemaValueType.String).Build()
                )
            .Format("date-time").Build(),

            [typeof(Guid?)] = () => new JsonSchemaBuilder()
            .AnyOf(
                new JsonSchemaBuilder().Type(SchemaValueType.Null).Build(),
                new JsonSchemaBuilder().Type(SchemaValueType.String).Build()
                )
            .Format("string").Build(),

            [typeof(char?)] = () => new JsonSchemaBuilder()
            .AnyOf(
                new JsonSchemaBuilder().Type(SchemaValueType.Null).Build(),
                new JsonSchemaBuilder().Type(SchemaValueType.String).Build()
                )
            .Format("string").Build(),

            [typeof(Uri)] = () => new JsonSchemaBuilder().Type(SchemaValueType.String).Format("uri").Build(), // Uri is treated as simple string
            [typeof(string)] = () => new JsonSchemaBuilder().Type(SchemaValueType.String).Build(),
            [typeof(object)] = () => new JsonSchemaBuilder().Type(SchemaValueType.Object).Build(),

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
        public static JsonSchema MapTypeToJsonPrimitiveType(this Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return _simpleTypeToJsonSchema.TryGetValue(type, out var result)
                ? result()
                : new JsonSchemaBuilder().Type(SchemaValueType.String).Build();
        }

        /// <summary>
        /// Maps an JsonSchema data type and format to a simple type.
        /// </summary>
        /// <param name="schema">The OpenApi data type</param>
        /// <returns>The simple type</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static Type MapJsonSchemaValueTypeToSimpleType(this JsonSchema schema)
        {
            if (schema == null)
            {
                throw new ArgumentNullException(nameof(schema));
            }

            var type = schema.GetJsonType();
            var format = schema.GetFormat().Key;
            var result = (type, format) switch
            {
                (SchemaValueType.Boolean, null) => typeof(bool),
                (SchemaValueType.Integer, "int32") => typeof(int),
                (SchemaValueType.Integer, "int64") => typeof(long),
                (SchemaValueType.Number, "float") => typeof(float),
                (SchemaValueType.Number, "double") => typeof(double),
                (SchemaValueType.Number, "decimal") => typeof(decimal),
                (SchemaValueType.String, "byte") => typeof(byte),
                (SchemaValueType.String, "date-time") => typeof(DateTimeOffset),
                (SchemaValueType.String, "uuid") => typeof(Guid),
                (SchemaValueType.String, "duration") => typeof(TimeSpan),
                (SchemaValueType.String, "char") => typeof(char),
                (SchemaValueType.String, null) => typeof(string),
                (SchemaValueType.Object, null) => typeof(object),
                (SchemaValueType.String, "uri") => typeof(Uri),
                (SchemaValueType.Integer or null, "int32") => typeof(int?),
                (SchemaValueType.Integer or null, "int64") => typeof(long?),
                (SchemaValueType.Number or null, "float") => typeof(float?),
                (SchemaValueType.Number or null, "double") => typeof(double?),
                (SchemaValueType.Number or null, "decimal") => typeof(decimal?),
                (SchemaValueType.String or null, "byte") => typeof(byte?),
                (SchemaValueType.String or null, "date-time") => typeof(DateTimeOffset?),
                (SchemaValueType.String or null, "uuid") => typeof(Guid?),
                (SchemaValueType.String or null, "char") => typeof(char?),
                (SchemaValueType.Boolean or null, null) => typeof(bool?),
                _ => typeof(string),
            };

            return result;
        }

        internal static string ConvertSchemaValueTypeToString(SchemaValueType value)
        {
            return value switch
            {
                SchemaValueType.String => "string",
                SchemaValueType.Number => "number",
                SchemaValueType.Integer => "integer",
                SchemaValueType.Boolean => "boolean",
                SchemaValueType.Array => "array",
                SchemaValueType.Object => "object",
                SchemaValueType.Null => "null",
                _ => throw new NotSupportedException(),
            };
        }

        //internal static string GetValueType(Type type)
        //{
        //    if (type == typeof(string))
        //    {
        //        return "string";
        //    }
        //    else if (type == typeof(int) || type == typeof(int?))
        //    {
        //        return "integer";
        //    }
        //    else if (type == typeof(long) || type == typeof(long?))
        //    {
        //        return "integer";
        //    }
        //    else if (type == typeof(bool) || type == typeof(bool?))
        //    {
        //        return "bool";
        //    }
        //    else if (type == typeof(float) || type == typeof(float?))
        //    {
        //        return "float";
        //    }
        //    else if (type == typeof(double) || type == typeof(double?))
        //    {
        //        return "double";
        //    }
        //    else if (type == typeof(decimal) || type == typeof(decimal?))
        //    {
        //        return "decimal";
        //    }
        //    else if (type == typeof(DateTime) || type == typeof(DateTime?))
        //    {
        //        return "date-time";
        //    }
        //    else if (type == typeof(DateTimeOffset) || type == typeof(DateTimeOffset?))
        //    {
        //        return "date-time";
        //    }

        //    return null;
        //}
    }
}
