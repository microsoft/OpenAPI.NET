// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Microsoft.OpenApi.Writers
{
    /// <summary>
    /// Extension methods for writing Open API documentation.
    /// </summary>
    public static class OpenApiWriterExtensions
    {
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
        /// <param name="writer">The Open API writer.</param>
        /// <param name="name">The property name.</param>
        /// <param name="elements">The collection values.</param>
        /// <param name="action">The collection element writer action.</param>
        public static void WriteOptionalCollection(this IOpenApiWriter writer, string name, IEnumerable<string> elements, Action<IOpenApiWriter, string> action)
        {
            if (elements != null)
            {
                writer.WriteCollectionInternal(name, elements, action);
            }
        }

        /// <summary>
        /// Write the required Open API object/element collection.
        /// </summary>
        /// <param name="writer">The Open API writer.</param>
        /// <param name="name">The property name.</param>
        /// <param name="elements">The collection values.</param>
        /// <param name="action">The collection element writer action.</param>
        public static void WriteRequiredCollection(this IOpenApiWriter writer, string name, IEnumerable<string> elements, Action<IOpenApiWriter, string> action)
        {
            writer.WriteCollectionInternal(name, elements, action);
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
                writer.WriteCollectionInternal(name, elements, action);
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
            writer.WriteCollectionInternal(name, elements, action);
        }

        /// <summary>
        /// Write the optional Open API element map.
        /// </summary>
        /// <param name="writer">The Open API writer.</param>
        /// <param name="name">The property name.</param>
        /// <param name="elements">The map values.</param>
        /// <param name="action">The map element writer action.</param>
        public static void WriteOptionalMap(this IOpenApiWriter writer, string name, IDictionary<string, string> elements, Action<IOpenApiWriter, string> action)
        {
            if (elements != null)
            {
                writer.WriteMapInternal(name, elements, action);
            }
        }

        /// <summary>
        /// Write the required Open API element map.
        /// </summary>
        /// <param name="writer">The Open API writer.</param>
        /// <param name="name">The property name.</param>
        /// <param name="elements">The map values.</param>
        /// <param name="action">The map element writer action.</param>
        public static void WriteRequiredMap(this IOpenApiWriter writer, string name, IDictionary<string, string> elements, Action<IOpenApiWriter, string> action)
        {
            writer.WriteMapInternal(name, elements, action);
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

        /// <summary>
        /// Write the optional property with boolean value.
        /// </summary>
        /// <param name="writer">The Open Api writer.</param>
        /// <param name="name">The property name.</param>
        /// <param name="value">The property value.</param>
        /// <param name="defaultValue">The default boolean value.</param>
        public static void WriteOptionalProperty(this IOpenApiWriter writer, string name, bool value, bool defaultValue = false)
        {
            if (value == defaultValue)
            {
                return;
            }

            writer.WritePropertyInternal(name, value);
        }

        /// <summary>
        /// Write the optional property with boolean value.
        /// </summary>
        /// <param name="writer">The Open Api writer.</param>
        /// <param name="name">The property name.</param>
        /// <param name="value">The property value.</param>
        /// <param name="defaultValue">The default boolean value.</param>
        public static void WriteOptionalProperty(this IOpenApiWriter writer, string name, bool? value, bool defaultValue = false)
        {
            if (value == null || value.Value == defaultValue)
            {
                return;
            }

            writer.WritePropertyInternal(name, value);
        }

        /// <summary>
        /// Write the optional property with string value.
        /// </summary>
        /// <param name="writer">The Open Api writer.</param>
        /// <param name="name">The property name.</param>
        /// <param name="value">The property value.</param>
        public static void WriteOptionalProperty(this IOpenApiWriter writer, string name, string value)
        {
            if (value == null)
            {
                return;
            }

            writer.WritePropertyInternal(name, value);
        }

        /// <summary>
        /// Write the required property with string value.
        /// </summary>
        /// <param name="writer">The Open Api writer.</param>
        /// <param name="name">The property name.</param>
        /// <param name="value">The property value.</param>
        public static void WriteRequiredProperty(this IOpenApiWriter writer, string name, string value)
        {
            writer.WritePropertyInternal(name, value);
        }

        /// <summary>
        /// Write the required property with <typeparamref name="T"/> value.
        /// </summary>
        /// <typeparam name="T">The struct type.</typeparam>
        /// <param name="writer">The Open Api writer.</param>
        /// <param name="name">The property name.</param>
        /// <param name="value">The property value.</param>
        public static void WriteRequiredProperty<T>(this IOpenApiWriter writer, string name, T value)
            where T : struct
        {
            writer.WritePropertyInternal(name, value);
        }

        /// <summary>
        /// Write the required property with <typeparamref name="T"/> value.
        /// </summary>
        /// <typeparam name="T">The struct type.</typeparam>
        /// <param name="writer">The Open Api writer.</param>
        /// <param name="name">The property name.</param>
        /// <param name="value">The property value.</param>
        public static void WriteRequiredProperty<T>(this IOpenApiWriter writer, string name, T? value)
            where T : struct
        {
            writer.WritePropertyInternal(name, value);
        }

        /// <summary>
        /// Write the optional property with <typeparamref name="T"/> value.
        /// </summary>
        /// <typeparam name="T">The struct type.</typeparam>
        /// <param name="writer">The Open Api writer.</param>
        /// <param name="name">The property name.</param>
        /// <param name="value">The property value.</param>
        public static void WriteOptionalProperty<T>(this IOpenApiWriter writer, string name, T? value)
            where T : struct
        {
            if (value == null)
            {
                return;
            }

            writer.WritePropertyInternal(name, value.Value);
        }

        private static void WritePropertyInternal<T>(this IOpenApiWriter writer, string name, T value)
        {
            CheckArguments(writer, name);
            writer.WritePropertyName(name);
            writer.WriteValue(value);
        }

        public static void WriteCollectionInternal<T>(this IOpenApiWriter writer, string name, IEnumerable<T> elements, Action<IOpenApiWriter, T> action)
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

        public static void WriteMapInternal<T>(this IOpenApiWriter writer, string name, IDictionary<string, T> elements, Action<IOpenApiWriter, T> action)
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

        private static void CheckArguments(IOpenApiWriter writer, string name)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            if (String.IsNullOrWhiteSpace(name))
            {
                throw Error.ArgumentNullOrWhiteSpace(nameof(name));
            }
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
