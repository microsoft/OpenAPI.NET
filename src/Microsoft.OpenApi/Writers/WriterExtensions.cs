//---------------------------------------------------------------------
// <copyright file="WriterExtensions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OpenApi.Writers
{
    using Microsoft.OpenApi;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    public static class WriterExtensions
    {
        public static void Save(this OpenApiDocument doc, Stream stream, IOpenApiStructureWriter openApiWriter = null)
        {
            if (openApiWriter == null)
            {
                openApiWriter = new OpenApiV3Writer();
            }

            openApiWriter.Write(stream, doc);
        }

        public static void WriteObject<T>(this IOpenApiWriter writer, string propertyName, T entity, Action<IOpenApiWriter, T> parser)
        {
            if (entity == null)
            {
                return;
            }

            writer.WritePropertyName(propertyName);
            parser(writer, entity);
        }

        public static void WriteList<T>(this IOpenApiWriter writer, string propertyName, IList<T> list, Action<IOpenApiWriter, T> parser)
        {
            if (list == null || !list.Any())
            {
                return;
            }

            writer.WritePropertyName(propertyName);
            writer.WriteStartArray();
            foreach (var item in list)
            {
                parser(writer,item);
            }

            writer.WriteEndArray();
        }

        public static void WriteMap<T>(this IOpenApiWriter writer, string propertyName, IDictionary<string, T> list, Action<IOpenApiWriter, T> parser)
        {
            if (list == null || !list.Any())
            {
                return;
            }

            writer.WritePropertyName(propertyName);
            writer.WriteStartObject();
            foreach (var item in list)
            {
                writer.WritePropertyName(item.Key);
                if (item.Value != null)
                {
                    parser(writer, item.Value);
                }
                else
                {
                    writer.WriteNull();
                }
            }
            writer.WriteEndObject();
        }

        public static void WriteStringProperty(this IOpenApiWriter writer, string name, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return;
            }

            writer.WritePropertyName(name);
            writer.WriteValue(value);
        }
        public static void WriteBoolProperty(this IOpenApiWriter writer, string name, bool value, bool? defaultValue = null)
        {
            if (defaultValue != null && value == defaultValue)
            {
                return;
            }

            writer.WritePropertyName(name);
            writer.WriteValue(value);
        }

        public static void WriteNumberProperty(this IOpenApiWriter writer, string name, decimal value, decimal? defaultValue = null)
        {
            if (defaultValue != null && value == defaultValue)
            {
                return;
            }

            writer.WritePropertyName(name);
            writer.WriteValue(value);
        }

        public static void WriteNumberProperty(this IOpenApiWriter writer, string name, int? value)
        {
            if (value == null)
            {
                return;
            }

            writer.WritePropertyName(name);
            writer.WriteValue((int)value);
        }

        public static void WriteNumberProperty(this IOpenApiWriter writer, string name, decimal? value)
        {
            if (value == null)
            {
                return;
            }

            writer.WritePropertyName(name);
            writer.WriteValue((decimal)value);
        }
       
    }
}
