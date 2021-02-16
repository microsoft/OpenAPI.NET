// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Globalization;
using System.IO;
using Microsoft.OpenApi.Exceptions;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Properties;
using Microsoft.OpenApi.Serializers;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="IOpenApiElement"/> serialization.
    /// </summary>
    public static class OpenApiSerializableExtensions
    {
        /// <summary>
        /// Serialize the <see cref="IOpenApiElement"/> to the Open API document (JSON) using the given stream and specification version.
        /// </summary>
        /// <typeparam name="T">the <see cref="IOpenApiElement"/></typeparam>
        /// <param name="element">The Open API element.</param>
        /// <param name="writer">The Open API writer</param>
        public static void SerializeAsV3<T>(this T element, IOpenApiWriter writer)
            where T : IOpenApiElement
        {
            var serializer = OpenApiSerializer.V3Serializer;
            serializer.Serialize(element, writer);
        }

        /// <summary>
        /// Serialize the <see cref="IOpenApiElement"/> to the Open API document (JSON) using the given stream and specification version.
        /// </summary>
        /// <typeparam name="T">the <see cref="IOpenApiElement"/></typeparam>
        /// <param name="element">The Open API element.</param>
        /// <param name="writer">The Open API writer</param>
        public static void SerializeAsV2<T>(this T element, IOpenApiWriter writer)
            where T : IOpenApiElement
        {
            var serializer = OpenApiSerializer.V2Serializer;
            serializer.Serialize(element, writer);
        }

        /// <summary>
        /// Serialize the <see cref="IOpenApiElement"/> to the Open API document (JSON) using the given stream and specification version.
        /// </summary>
        /// <typeparam name="T">the <see cref="IOpenApiElement"/></typeparam>
        /// <param name="element">The Open API element.</param>
        /// <param name="writer">The Open API writer</param>
        public static void SerializeAsV3WithoutReference<T>(this T element, IOpenApiWriter writer)
            where T : IOpenApiReferenceable
        {
            var serializer = OpenApiSerializer.V3Serializer;
            serializer.SerializeWithoutReference(element, writer);
        }

        /// <summary>
        /// Serialize the <see cref="IOpenApiElement"/> to the Open API document (JSON) using the given stream and specification version.
        /// </summary>
        /// <typeparam name="T">the <see cref="IOpenApiElement"/></typeparam>
        /// <param name="element">The Open API element.</param>
        /// <param name="writer">The Open API writer</param>
        public static void SerializeAsV2WithoutReference<T>(this T element, IOpenApiWriter writer)
            where T : IOpenApiReferenceable
        {
            var serializer = OpenApiSerializer.V2Serializer;
            serializer.SerializeWithoutReference(element, writer);
        }

        /// <summary>
        /// Serialize the <see cref="IOpenApiElement"/> to the Open API document (JSON) using the given stream and specification version.
        /// </summary>
        /// <typeparam name="T">the <see cref="IOpenApiElement"/></typeparam>
        /// <param name="element">The Open API element.</param>
        /// <param name="stream">The output stream.</param>
        /// <param name="specVersion">The Open API specification version.</param>
        public static void SerializeAsJson<T>(this T element, Stream stream, OpenApiSpecVersion specVersion)
            where T : IOpenApiElement
        {
            element.Serialize(stream, specVersion, OpenApiFormat.Json);
        }

        /// <summary>
        /// Serializes the <see cref="IOpenApiElement"/> to the Open API document (YAML) using the given stream and specification version.
        /// </summary>
        /// <typeparam name="T">the <see cref="IOpenApiElement"/></typeparam>
        /// <param name="element">The Open API element.</param>
        /// <param name="stream">The output stream.</param>
        /// <param name="specVersion">The Open API specification version.</param>
        public static void SerializeAsYaml<T>(this T element, Stream stream, OpenApiSpecVersion specVersion)
            where T : IOpenApiElement
        {
            element.Serialize(stream, specVersion, OpenApiFormat.Yaml);
        }

        /// <summary>
        /// Serializes the <see cref="IOpenApiElement"/> to the Open API document using
        /// the given stream, specification version and the format.
        /// </summary>
        /// <typeparam name="T">the <see cref="IOpenApiElement"/></typeparam>
        /// <param name="element">The Open API element.</param>
        /// <param name="stream">The given stream.</param>
        /// <param name="specVersion">The Open API specification version.</param>
        /// <param name="format">The output format (JSON or YAML).</param>
        public static void Serialize<T>(
            this T element,
            Stream stream,
            OpenApiSpecVersion specVersion,
            OpenApiFormat format)
            where T : IOpenApiElement
        {
            element.Serialize(stream, specVersion, format, null);
        }

        /// <summary>
        /// Serializes the <see cref="IOpenApiElement"/> to the Open API document using
        /// the given stream, specification version and the format.
        /// </summary>
        /// <typeparam name="T">the <see cref="IOpenApiElement"/></typeparam>
        /// <param name="element">The Open API element.</param>
        /// <param name="stream">The given stream.</param>
        /// <param name="specVersion">The Open API specification version.</param>
        /// <param name="format">The output format (JSON or YAML).</param>
        /// <param name="settings">Provide configuration settings for controlling writing output</param>
        public static void Serialize<T>(
            this T element,
            Stream stream,
            OpenApiSpecVersion specVersion,
            OpenApiFormat format, 
            OpenApiWriterSettings settings)
            where T : IOpenApiElement
        {
            if (stream == null)
            {
                throw Error.ArgumentNull(nameof(stream));
            }

            IOpenApiWriter writer;
            var streamWriter = new FormattingStreamWriter(stream, CultureInfo.InvariantCulture);
            switch (format)
            {
                case OpenApiFormat.Json:
                    writer = new OpenApiJsonWriter(streamWriter, settings);
                    break;
                case OpenApiFormat.Yaml:
                    writer = new OpenApiYamlWriter(streamWriter, settings);
                    break;
                default:
                    throw new OpenApiException(string.Format(SRResource.OpenApiFormatNotSupported, format));
            }

            element.Serialize(writer, specVersion);
        }

        /// <summary>
        /// Serializes the <see cref="IOpenApiElement"/> to Open API document using the given specification version and writer.
        /// </summary>
        /// <typeparam name="T">the <see cref="IOpenApiElement"/></typeparam>
        /// <param name="element">The Open API element.</param>
        /// <param name="writer">The output writer.</param>
        /// <param name="specVersion">Version of the specification the output should conform to</param>

        public static void Serialize<T>(this T element, IOpenApiWriter writer, OpenApiSpecVersion specVersion)
            where T : IOpenApiElement
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
        /// Serializes the <see cref="IOpenApiElement"/> to the Open API document as a string in JSON format.
        /// </summary>
        /// <typeparam name="T">the <see cref="IOpenApiElement"/></typeparam>
        /// <param name="element">The Open API element.</param>
        /// <param name="specVersion">The Open API specification version.</param>
        public static string SerializeAsJson<T>(
            this T element,
            OpenApiSpecVersion specVersion)
            where T : IOpenApiElement
        {
            return element.Serialize(specVersion, OpenApiFormat.Json);
        }

        /// <summary>
        /// Serializes the <see cref="IOpenApiElement"/> to the Open API document as a string in YAML format.
        /// </summary>
        /// <typeparam name="T">the <see cref="IOpenApiElement"/></typeparam>
        /// <param name="element">The Open API element.</param>
        /// <param name="specVersion">The Open API specification version.</param>
        public static string SerializeAsYaml<T>(
            this T element,
            OpenApiSpecVersion specVersion)
            where T : IOpenApiElement
        {
            return element.Serialize(specVersion, OpenApiFormat.Yaml);
        }

        /// <summary>
        /// Serializes the <see cref="IOpenApiElement"/> to the Open API document as a string in the given format.
        /// </summary>
        /// <typeparam name="T">the <see cref="IOpenApiElement"/></typeparam>
        /// <param name="element">The Open API element.</param>
        /// <param name="specVersion">The Open API specification version.</param>
        /// <param name="format">Open API document format.</param>
        public static string Serialize<T>(
            this T element,
            OpenApiSpecVersion specVersion,
            OpenApiFormat format)
            where T : IOpenApiElement
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
