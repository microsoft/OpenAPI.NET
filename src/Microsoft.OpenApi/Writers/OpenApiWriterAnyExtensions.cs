// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;

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
            Utils.CheckArgumentNull(writer);

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
            Utils.CheckArgumentNull(writer);

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
            writer.WriteStartArray();

            foreach (var item in array)
            {
                writer.WriteAny(item);
            }

            writer.WriteEndArray();
        }

        private static void WriteObject(this IOpenApiWriter writer, OpenApiObject entity)
        {
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
            // The Spec version is meaning for the Any type, so it's ok to use the latest one.
            primitive.Write(writer, OpenApiSpecVersion.OpenApi3_0);
        }
    }
}
