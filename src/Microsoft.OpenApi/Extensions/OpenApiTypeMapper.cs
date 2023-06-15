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
        /// Maps an OpenAPI data type and format to a simple type.
        /// </summary>
        /// <param name="schema">The OpenApi data type</param>
        /// <returns>The simple type</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static Type MapJsonPrimitiveTypeToSimpleType(this JsonSchema schema)
        {
            if (schema == null)
            {
                throw new ArgumentNullException(nameof(schema));
            }            

            var type = schema.GetType();
            var format = schema.GetFormat();
            var result = (type.ToString(), format.ToString()) switch
            {
                (("boolean"), null) => typeof(bool),
                ("integer", "int32") => typeof(int),
                ("integer", "int64") => typeof(long),
                ("number", "float") => typeof(float),
                ("number", "double") => typeof(double),
                ("number", "decimal") => typeof(decimal),
                ("string", "byte") => typeof(byte),
                ("string", "date-time") => typeof(DateTimeOffset),
                ("string", "uuid") => typeof(Guid),
                ("string", "duration") => typeof(TimeSpan),
                ("string", "char") => typeof(char),
                ("string", null) => typeof(string),
                ("object", null) => typeof(object),
                ("string", "uri") => typeof(Uri),
                ("integer" or null, "int32") => typeof(int?),
                ("integer" or null, "int64") => typeof(long?),
                ("number" or null, "float") => typeof(float?),
                ("number" or null, "double") => typeof(double?),
                ("number" or null, "decimal") => typeof(decimal?),
                ("string" or null, "byte") => typeof(byte?),
                ("string" or null, "date-time") => typeof(DateTimeOffset?),
                ("string" or null, "uuid") => typeof(Guid?),
                ("string" or null, "char") => typeof(char?),
                ("boolean" or null, null) => typeof(bool?),
                _ => typeof(string),
            };
            type = result;

            return type;
        }

        internal static string ConvertSchemaValueTypeToString(SchemaValueType value)
        {
            if (value == null)
            {
                return null;
            }

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
