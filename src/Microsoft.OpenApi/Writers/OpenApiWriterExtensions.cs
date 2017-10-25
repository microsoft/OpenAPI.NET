// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Microsoft.OpenApi.Writers
{
    /// <summary>
    /// Extension methods for writing Open API documentation.
    /// </summary>
    public static class OpenApiWriterExtensions
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

        /// <summary>
        /// Write the optional Open API object/element.
        /// </summary>
        /// <typeparam name="T">The Open API element type. <see cref="IOpenApiElement"/></typeparam>
        /// <param name="writer">The Open API writer.</param>
        /// <param name="name">The property name.</param>
        /// <param name="value">The property value.</param>
        /// <param name="action">The proprety value writer action.</param>
        public static void WriteOptionalObject<T>(this IOpenApiWriter writer, string name, T value, Action<IOpenApiWriter, T> action)
            where T : IOpenApiElement
        {
            if (value != null)
            {
                writer.WriteRequiredObject(name, value, action);
            }
        }

        /// <summary>
        /// Write the required Open API object/element.
        /// </summary>
        /// <typeparam name="T">The Open API element type. <see cref="IOpenApiElement"/></typeparam>
        /// <param name="writer">The Open API writer.</param>
        /// <param name="name">The property name.</param>
        /// <param name="value">The property value.</param>
        /// <param name="action">The proprety value writer action.</param>
        public static void WriteRequiredObject<T>(this IOpenApiWriter writer, string name, T value, Action<IOpenApiWriter, T> action)
            where T : IOpenApiElement
        {
            CheckArguments(writer, name, action);

            writer.WritePropertyName(name);
            action(writer, value);
        }

        /// <summary>
        /// Write the optional Open API object/element collection.
        /// </summary>
        /// <typeparam name="T">The Open API element type. <see cref="IOpenApiElement"/></typeparam>
        /// <param name="writer">The Open API writer.</param>
        /// <param name="name">The property name.</param>
        /// <param name="elements">The collection values.</param>
        /// <param name="action">The collection element writer action.</param>
        public static void WriteOptionalCollection<T>(this IOpenApiWriter writer, string name, IEnumerable<T> elements, Action<IOpenApiWriter, T> action)
            where T : IOpenApiElement
        {
            if (elements != null)
            {
                writer.WriteRequiredCollection(name, elements, action);
            }
        }

        /// <summary>
        /// Write the required Open API object/element collection.
        /// </summary>
        /// <typeparam name="T">The Open API element type. <see cref="IOpenApiElement"/></typeparam>
        /// <param name="writer">The Open API writer.</param>
        /// <param name="name">The property name.</param>
        /// <param name="elements">The collection values.</param>
        /// <param name="action">The collection element writer action.</param>
        public static void WriteRequiredCollection<T>(this IOpenApiWriter writer, string name, IEnumerable<T> elements, Action<IOpenApiWriter, T> action)
            where T : IOpenApiElement
        {
            CheckArguments(writer, name, action);

            writer.WritePropertyName(name);
            writer.WriteStartArray();

            if (elements != null)
            {
                foreach (var item in elements)
                {
                    action(writer, item);
                }
            }

            writer.WriteEndArray();
        }

        /// <summary>
        /// Write the optional Open API element map.
        /// </summary>
        /// <typeparam name="T">The Open API element type. <see cref="IOpenApiElement"/></typeparam>
        /// <param name="writer">The Open API writer.</param>
        /// <param name="name">The property name.</param>
        /// <param name="elements">The map values.</param>
        /// <param name="action">The map element writer action.</param>
        public static void WriteOptionalMap<T>(this IOpenApiWriter writer, string name, IDictionary<string, T> elements, Action<IOpenApiWriter, T> action)
            where T : IOpenApiElement
        {
            if (elements != null)
            {
                writer.WriteRequiredMap(name, elements, action);
            }
        }

        /// <summary>
        /// Write the required Open API element map.
        /// </summary>
        /// <typeparam name="T">The Open API element type. <see cref="IOpenApiElement"/></typeparam>
        /// <param name="writer">The Open API writer.</param>
        /// <param name="name">The property name.</param>
        /// <param name="elements">The map values.</param>
        /// <param name="action">The map element writer action.</param>
        public static void WriteRequiredMap<T>(this IOpenApiWriter writer, string name, IDictionary<string, T> elements, Action<IOpenApiWriter, T> action)
            where T : IOpenApiElement
        {
            CheckArguments(writer, name, action);

            writer.WritePropertyName(name);
            writer.WriteStartObject();

            if (elements != null)
            {
                foreach (var item in elements)
                {
                    writer.WritePropertyName(item.Key);
                    action(writer, item.Value);
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

        private static void CheckArguments<T>(IOpenApiWriter writer, string name, Action<IOpenApiWriter, T> action)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            if (String.IsNullOrWhiteSpace(name))
            {
                throw Error.ArgumentNullOrWhiteSpace(nameof(name));
            }

            if (action == null)
            {
                throw Error.ArgumentNull(nameof(action));
            }
        }
    }
}
