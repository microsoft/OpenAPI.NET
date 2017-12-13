﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.IO;
using Microsoft.OpenApi.Exceptions;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Properties;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="IOpenApiSerializable"/> serialization.
    /// </summary>
    public static class OpenApiSerializableExtensions
    {
        /// <summary>
        /// Serialize the <see cref="IOpenApiSerializable"/> to the Open API document (JSON) using the given stream and specification version.
        /// </summary>
        /// <typeparam name="T">the <see cref="IOpenApiSerializable"/></typeparam>
        /// <param name="element">The Open API element.</param>
        /// <param name="stream">The output stream.</param>
        /// <param name="specVersion">The Open API specification version.</param>
        public static void SerializeAsJson<T>(this T element, Stream stream, OpenApiSpecVersion specVersion)
            where T : IOpenApiSerializable
        {
            element.Serialize(stream, specVersion, OpenApiFormat.Json);
        }

        /// <summary>
        /// Serializes the <see cref="IOpenApiSerializable"/> to the Open API document (YAML) using the given stream and specification version.
        /// </summary>
        /// <typeparam name="T">the <see cref="IOpenApiSerializable"/></typeparam>
        /// <param name="element">The Open API element.</param>
        /// <param name="stream">The output stream.</param>
        /// <param name="specVersion">The Open API specification version.</param>
        public static void SerializeAsYaml<T>(this T element, Stream stream, OpenApiSpecVersion specVersion)
            where T : IOpenApiSerializable
        {
            element.Serialize(stream, specVersion, OpenApiFormat.Yaml);
        }

        /// <summary>
        /// Serializes the <see cref="IOpenApiSerializable"/> to the Open API document using
        /// the given stream, specification version and the format.
        /// </summary>
        /// <typeparam name="T">the <see cref="IOpenApiSerializable"/></typeparam>
        /// <param name="element">The Open API element.</param>
        /// <param name="stream">The given stream.</param>
        /// <param name="specVersion">The Open API specification version.</param>
        /// <param name="format">The output format (JSON or YAML).</param>
        public static void Serialize<T>(
            this T element,
            Stream stream,
            OpenApiSpecVersion specVersion,
            OpenApiFormat format)
            where T : IOpenApiSerializable
        {
            if (stream == null)
            {
                throw Error.ArgumentNull(nameof(stream));
            }

            IOpenApiWriter writer;
            switch (format)
            {
                case OpenApiFormat.Json:
                    writer = new OpenApiJsonWriter(new StreamWriter(stream));
                    break;
                case OpenApiFormat.Yaml:
                    writer = new OpenApiYamlWriter(new StreamWriter(stream));
                    break;
                default:
                    throw new OpenApiException(string.Format(SRResource.OpenApiFormatNotSupported, format));
            }

            element.Serialize(writer, specVersion);
        }

        /// <summary>
        /// Serializes the <see cref="IOpenApiSerializable"/> to Open API document using the given specification version and writer.
        /// </summary>
        /// <typeparam name="T">the <see cref="IOpenApiSerializable"/></typeparam>
        /// <param name="element">The Open API element.</param>
        /// <param name="writer">The output writer.</param>
        /// <param name="specVersion">The Open API specification version.</param>
        public static void Serialize<T>(this T element, IOpenApiWriter writer, OpenApiSpecVersion specVersion)
            where T : IOpenApiSerializable
        {
            if (element == null)
            {
                throw Error.ArgumentNull(nameof(element));
            }

            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            switch (specVersion)
            {
                case OpenApiSpecVersion.OpenApi3_0_0:
                    element.SerializeAsV3(writer);
                    break;

                case OpenApiSpecVersion.OpenApi2_0:
                    element.SerializeAsV2(writer);
                    break;

                default:
                    throw new OpenApiException(string.Format(SRResource.OpenApiSpecVersionNotSupported, specVersion));
            }

            writer.Flush();
        }

        /// <summary>
        /// Serializes the <see cref="IOpenApiSerializable"/> to the Open API document as a string in JSON format.
        /// </summary>
        /// <typeparam name="T">the <see cref="IOpenApiSerializable"/></typeparam>
        /// <param name="element">The Open API element.</param>
        /// <param name="specVersion">The Open API specification version.</param>
        public static string SerializeAsJson<T>(
            this T element,
            OpenApiSpecVersion specVersion)
            where T : IOpenApiSerializable
        {
            return element.Serialize(specVersion, OpenApiFormat.Json);
        }

        /// <summary>
        /// Serializes the <see cref="IOpenApiSerializable"/> to the Open API document as a string in YAML format.
        /// </summary>
        /// <typeparam name="T">the <see cref="IOpenApiSerializable"/></typeparam>
        /// <param name="element">The Open API element.</param>
        /// <param name="specVersion">The Open API specification version.</param>
        public static string SerializeAsYaml<T>(
            this T element,
            OpenApiSpecVersion specVersion)
            where T : IOpenApiSerializable
        {
            return element.Serialize(specVersion, OpenApiFormat.Yaml);
        }

        /// <summary>
        /// Serializes the <see cref="IOpenApiSerializable"/> to the Open API document as a string in the given format.
        /// </summary>
        /// <typeparam name="T">the <see cref="IOpenApiSerializable"/></typeparam>
        /// <param name="element">The Open API element.</param>
        /// <param name="specVersion">The Open API specification version.</param>
        /// <param name="format">Open API document format.</param>
        public static string Serialize<T>(
            this T element,
            OpenApiSpecVersion specVersion,
            OpenApiFormat format)
            where T : IOpenApiSerializable
        {
            if (element == null)
            {
                throw Error.ArgumentNull(nameof(element));
            }

            using (var stream = new MemoryStream())
            {
                element.Serialize(stream, specVersion, format);
                stream.Position = 0;

                using (var streamReader = new StreamReader(stream))
                {
                    return streamReader.ReadToEnd();
                }
            }
        }
    }
}