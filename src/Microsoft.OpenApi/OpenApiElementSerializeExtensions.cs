﻿// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.IO;
using Microsoft.OpenApi.Exceptions;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Properties;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi
{
    /// <summary>
    /// Extension methods for <see cref="IOpenApiSerializable"/> serialization.
    /// </summary>
    public static class OpenApiElementSerializeExtensions
    {
        /// <summary>
        /// Serialize the <see cref="IOpenApiSerializable"/> to the Open API document (JSON, v3.0) using the given stream.
        /// </summary>
        /// <typeparam name="T">the <see cref="IOpenApiSerializable"/></typeparam>
        /// <param name="element">The Open API element.</param>
        /// <param name="stream">The output stream.</param>
        public static void SerializeAsJson<T>(this T element, Stream stream)
            where T : IOpenApiSerializable
        {
            element.SerializeAsJson(stream, OpenApiSpecVersion.OpenApi3_0);
        }

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
        /// Serialize the <see cref="IOpenApiSerializable"/> to the Open API document (YAML, v3.0) using the given stream.
        /// </summary>
        /// <typeparam name="T">the <see cref="IOpenApiSerializable"/></typeparam>
        /// <param name="element">The Open API element.</param>
        /// <param name="stream">The output stream.</param>
        public static void SerializeAsYaml<T>(this T element, Stream stream)
            where T : IOpenApiSerializable
        {
            element.SerializeAsYaml(stream, OpenApiSpecVersion.OpenApi3_0);
        }

        /// <summary>
        /// Serialize the <see cref="IOpenApiSerializable"/> to the Open API document (YAML) using the given stream and specification version.
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
        /// Serialize the <see cref="IOpenApiSerializable"/> to the Open API document using
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
        /// Serialize the <see cref="IOpenApiSerializable"/> to Open API document (v3.0) using the given writer.
        /// </summary>
        /// <typeparam name="T">the <see cref="IOpenApiSerializable"/></typeparam>
        /// <param name="element">The Open API element.</param>
        /// <param name="writer">The output writer.</param>
        public static void Serialize<T>(this T element, IOpenApiWriter writer)
            where T : IOpenApiSerializable
        {
            element.Serialize(writer, OpenApiSpecVersion.OpenApi3_0);
        }

        /// <summary>
        /// Serialize the <see cref="IOpenApiSerializable"/> to Open API document using the given specification version and writer.
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
                case OpenApiSpecVersion.OpenApi3_0:
                    element.WriteAsV3(writer);
                    break;

                case OpenApiSpecVersion.OpenApi2_0:
                    element.WriteAsV2(writer);
                    break;

                default:
                    throw new OpenApiException(string.Format(SRResource.OpenApiSpecVersionNotSupported, specVersion));
            }

            writer.Flush();
        }
    }
}