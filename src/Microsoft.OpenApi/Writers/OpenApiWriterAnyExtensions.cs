// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
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
        public static void WriteExtensions(this IOpenApiWriter writer, IDictionary<string, IOpenApiAny> extensions)
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
                    writer.WriteAny(item.Value);
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

            switch (any.AnyKind)
            {
                case AnyTypeKind.Array: // Array
                    writer.WriteArray(any as OpenApiArray);
                    break;

                case AnyTypeKind.Object: // Object
                    writer.WriteObject(any as OpenApiObject);
                    break;

                case AnyTypeKind.Primitive: // Primitive
                    writer.WritePrimitive(any as IOpenApiPrimitive);
                    break;

                case AnyTypeKind.Null: // null
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

            switch (primitive.PrimitiveKind)
            {
                case PrimitiveTypeKind.Integer:
                    OpenApiInteger intValue = (OpenApiInteger)primitive;
                    writer.WriteValue(intValue.Value);
                    break;

                case PrimitiveTypeKind.Long:
                    OpenApiLong longValue = (OpenApiLong)primitive;
                    writer.WriteValue(longValue.Value);
                    break;

                case PrimitiveTypeKind.Float:
                    OpenApiFloat floatValue = (OpenApiFloat)primitive;
                    writer.WriteValue(floatValue.Value);
                    break;

                case PrimitiveTypeKind.Double:
                    OpenApiDouble doubleValue = (OpenApiDouble)primitive;
                    writer.WriteValue(doubleValue.Value);
                    break;

                case PrimitiveTypeKind.String:
                    OpenApiString stringValue = (OpenApiString)primitive;
                    writer.WriteValue(stringValue.Value);
                    break;

                case PrimitiveTypeKind.Byte:
                    OpenApiByte byteValue = (OpenApiByte)primitive;
                    writer.WriteValue(byteValue.Value);
                    break;

                case PrimitiveTypeKind.Binary:
                    OpenApiBinary binaryValue = (OpenApiBinary)primitive;
                    writer.WriteValue(binaryValue.Value);
                    break;

                case PrimitiveTypeKind.Boolean:
                    OpenApiBoolean boolValue = (OpenApiBoolean)primitive;
                    writer.WriteValue(boolValue.Value);
                    break;

                case PrimitiveTypeKind.Date:
                    OpenApiDate dateValue = (OpenApiDate)primitive;
                    writer.WriteValue(dateValue.Value);
                    break;

                case PrimitiveTypeKind.DateTime:
                    OpenApiDateTime dateTimeValue = (OpenApiDateTime)primitive;
                    writer.WriteValue(dateTimeValue.Value);
                    break;

                case PrimitiveTypeKind.Password:
                    OpenApiPassword passwordValue = (OpenApiPassword)primitive;
                    writer.WriteValue(passwordValue.Value);
                    break;

                default:
                    throw new OpenApiException("Not supported primitive type.");
            }
        }
    }
}
