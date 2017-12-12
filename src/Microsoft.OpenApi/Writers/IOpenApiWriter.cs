// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

namespace Microsoft.OpenApi.Writers
{
    /// <summary>
    /// Interface for writing Open API documentation.
    /// </summary>
    public interface IOpenApiWriter
    {
        /// <summary>
        /// Write the start object.
        /// </summary>
        void WriteStartObject();

        /// <summary>
        /// Write the end object.
        /// </summary>
        void WriteEndObject();

        /// <summary>
        /// Write the start array.
        /// </summary>
        void WriteStartArray();

        /// <summary>
        /// Write the end array.
        /// </summary>
        void WriteEndArray();

        /// <summary>
        /// Write the property name.
        /// </summary>
        void WritePropertyName(string name);

        /// <summary>
        /// Write the string value.
        /// </summary>
        void WriteValue(string value);

        /// <summary>
        /// Write the decimal value.
        /// </summary>
        void WriteValue(decimal value);

        /// <summary>
        /// Write the int value.
        /// </summary>
        void WriteValue(int value);

        /// <summary>
        /// Write the boolean value.
        /// </summary>
        void WriteValue(bool value);

        /// <summary>
        /// Write the null value.
        /// </summary>
        void WriteNull();

        /// <summary>
        /// Write the raw content value.
        /// </summary>
        void WriteRaw(string value);

        /// <summary>
        /// Write the object value.
        /// </summary>
        void WriteValue(object value);

        /// <summary>
        /// Flush the writer.
        /// </summary>
        void Flush();
    }
}