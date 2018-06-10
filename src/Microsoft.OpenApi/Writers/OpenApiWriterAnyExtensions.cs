// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Collections.Generic;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Exceptions;
using Microsoft.OpenApi.Properties;
using Microsoft.OpenApi.Any;

namespace Microsoft.OpenApi.Writers
{
    /// <summary>
    /// Extensions methods for writing the <see cref="IOpenApiAny"/>
    /// </summary>
    public static class OpenApiWriterAnyExtensions
    {
        /// <summary>
        /// Write the specification extensions
        /// </summary>
        /// <param name="writer">The Open API writer.</param>
        /// <param name="extensions">The specification extensions.</param>
        /// <param name="specVersion">Version of the OpenAPI specification that that will be output.</param>
        public static void WriteExtensions(this IOpenApiWriter writer, IDictionary<string, IOpenApiExtension> extensions, OpenApiSpecVersion specVersion)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            if (extensions != null)
            {
                foreach (var item in extensions)
                {
                    writer.WritePropertyName(item.Key);
                    item.Value.Write(writer, specVersion);
                }
            }
        }

        /// <summary>
        /// Write the <see cref="IOpenApiAny"/> value.
        /// </summary>
        /// <typeparam name="T">The Open API Any type.</typeparam>
        /// <param name="writer">The Open API writer.</param>
        /// <param name="any">The Any value</param>
        public static void WriteAny<T>(this IOpenApiWriter writer, T any) where T : IOpenApiAny
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            if (any == null)
            {
                writer.WriteNull();
                return;
            }

            switch (any.AnyType)
            {
                case AnyType.Array: // Array
                    writer.WriteArray(any as OpenApiArray);
                    break;

                case AnyType.Object: // Object
                    writer.WriteObject(any as OpenApiObject);
                    break;

                case AnyType.Primitive: // Primitive
                    writer.WritePrimitive(any as IOpenApiPrimitive);
                    break;

                case AnyType.Null: // null
                    writer.WriteNull();
                    break;

                default:
                    break;
            }
        }

        private static void WriteArray(this IOpenApiWriter writer, OpenApiArray array)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            if (array == null)
            {
                throw Error.ArgumentNull(nameof(array));
            }

            writer.WriteStartArray();

            foreach (var item in array)
            {
                writer.WriteAny(item);
            }

            writer.WriteEndArray();
        }

        private static void WriteObject(this IOpenApiWriter writer, OpenApiObject entity)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            if (entity == null)
            {
                throw Error.ArgumentNull(nameof(entity));
            }

            writer.WriteStartObject();

            foreach (var item in entity)
            {
                writer.WritePropertyName(item.Key);
                writer.WriteAny(item.Value);
            }

            writer.WriteEndObject();
        }

        private static void WritePrimitive(this IOpenApiWriter writer, IOpenApiPrimitive primitive)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            if (primitive == null)
            {
                throw Error.ArgumentNull(nameof(primitive));
            }

            switch (primitive.PrimitiveType)
            {
                case PrimitiveType.Integer:
                    var intValue = (OpenApiInteger)primitive;
                    writer.WriteValue(intValue.Value);
                    break;

                case PrimitiveType.Long:
                    var longValue = (OpenApiLong)primitive;
                    writer.WriteValue(longValue.Value);
                    break;

                case PrimitiveType.Float:
                    var floatValue = (OpenApiFloat)primitive;
                    writer.WriteValue(floatValue.Value);
                    break;

                case PrimitiveType.Double:
                    var doubleValue = (OpenApiDouble)primitive;
                    writer.WriteValue(doubleValue.Value);
                    break;

                case PrimitiveType.String:
                    var stringValue = (OpenApiString)primitive;
                    writer.WriteValue(stringValue.Value);
                    break;

                case PrimitiveType.Byte:
                    var byteValue = (OpenApiByte)primitive;
                    writer.WriteValue(byteValue.Value);
                    break;

                case PrimitiveType.Binary:
                    var binaryValue = (OpenApiBinary)primitive;
                    writer.WriteValue(binaryValue.Value);
                    break;

                case PrimitiveType.Boolean:
                    var boolValue = (OpenApiBoolean)primitive;
                    writer.WriteValue(boolValue.Value);
                    break;

                case PrimitiveType.Date:
                    var dateValue = (OpenApiDate)primitive;
                    writer.WriteValue(dateValue.Value);
                    break;

                case PrimitiveType.DateTime:
                    var dateTimeValue = (OpenApiDateTime)primitive;
                    writer.WriteValue(dateTimeValue.Value);
                    break;

                case PrimitiveType.Password:
                    var passwordValue = (OpenApiPassword)primitive;
                    writer.WriteValue(passwordValue.Value);
                    break;

                default:
                    throw new OpenApiWriterException(
                        string.Format(
                            SRResource.PrimitiveTypeNotSupported,
                            primitive.PrimitiveType));
            }
        }
    }
}