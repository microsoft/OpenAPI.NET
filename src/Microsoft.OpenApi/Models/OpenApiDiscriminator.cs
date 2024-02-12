// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.IO;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Reader;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Models
{
    /// <summary>
    /// Discriminator object.
    /// </summary>
    public class OpenApiDiscriminator : IOpenApiSerializable, IOpenApiExtensible
    {
        /// <summary>
        /// REQUIRED. The name of the property in the payload that will hold the discriminator value.
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        /// An object to hold mappings between payload values and schema names or references.
        /// </summary>
        public IDictionary<string, string> Mapping { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// This object MAY be extended with Specification Extensions.
        /// </summary>
        public IDictionary<string, IOpenApiExtension> Extensions { get; set; } = new Dictionary<string, IOpenApiExtension>();

        /// <summary>
        /// Parameter-less constructor
        /// </summary>
        public OpenApiDiscriminator() { }

        /// <summary>
        /// Initializes a copy of an <see cref="OpenApiDiscriminator"/> instance
        /// </summary>
        public OpenApiDiscriminator(OpenApiDiscriminator discriminator)
        {
            PropertyName = discriminator?.PropertyName ?? PropertyName;
            Mapping = discriminator?.Mapping != null ? new Dictionary<string, string>(discriminator.Mapping) : null;
            Extensions = discriminator?.Extensions != null ? new Dictionary<string, IOpenApiExtension>(discriminator.Extensions) : null;
        }

        /// <summary>
        /// Serialize <see cref="OpenApiDiscriminator"/> to Open Api v3.1
        /// </summary>
        /// <param name="writer"></param>
        public void SerializeAsV31(IOpenApiWriter writer)
        {
            SerializeInternal(writer);

            // extensions
            writer.WriteExtensions(Extensions, OpenApiSpecVersion.OpenApi3_1);

            writer.WriteEndObject();
        }

        /// <summary>
        /// Serialize <see cref="OpenApiDiscriminator"/> to Open Api v3.0
        /// </summary>
        public void SerializeAsV3(IOpenApiWriter writer)
        {
            SerializeInternal(writer);

            writer.WriteEndObject();
        }

        /// <summary>
        /// Serialize <see cref="OpenApiDiscriminator"/> to Open Api v3.0
        /// </summary>
        /// <param name="writer"></param>
        private void SerializeInternal(IOpenApiWriter writer)
        {
            Utils.CheckArgumentNull(writer);

            writer.WriteStartObject();

            // propertyName
            writer.WriteProperty(OpenApiConstants.PropertyName, PropertyName);

            // mapping
            writer.WriteOptionalMap(OpenApiConstants.Mapping, Mapping, (w, s) => w.WriteValue(s));
        }

        /// <summary>
        /// Serialize <see cref="OpenApiDiscriminator"/> to Open Api v2.0
        /// </summary>
        public void SerializeAsV2(IOpenApiWriter writer)
        {
            // Discriminator object does not exist in V2.
        }

        /// <summary>
        /// Parses a local file path or Url into an Open API document.
        /// </summary>
        /// <param name="url"> The path to the OpenAPI file.</param>
        /// <param name="version">The OpenAPI specification version.</param>
        /// <param name="diagnostic"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static OpenApiDiscriminator Load(string url, OpenApiSpecVersion version, out OpenApiDiagnostic diagnostic, OpenApiReaderSettings settings = null)
        {
            return OpenApiModelFactory.Load<OpenApiDiscriminator>(url, version, out diagnostic, settings);
        }

        /// <summary>
        /// Reads the stream input and parses it into an Open API document.
        /// </summary>
        /// <param name="stream">Stream containing OpenAPI description to parse.</param>
        /// <param name="format">The OpenAPI format to use during parsing.</param>
        /// <param name="version"></param>
        /// <param name="diagnostic"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static OpenApiDiscriminator Load(Stream stream,
                                           string format,
                                           OpenApiSpecVersion version,
                                           out OpenApiDiagnostic diagnostic,
                                           OpenApiReaderSettings settings = null)
        {
            return OpenApiModelFactory.Load<OpenApiDiscriminator>(stream, version, out diagnostic, format, settings);
        }

        /// <summary>
        /// Reads the text reader content and parses it into an Open API document.
        /// </summary>
        /// <param name="input">TextReader containing OpenAPI description to parse.</param>
        /// <param name="format"> The OpenAPI format to use during parsing.</param>
        /// <param name="version"></param>
        /// <param name="diagnostic"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static OpenApiDiscriminator Load(TextReader input,
                                           string format,
                                           OpenApiSpecVersion version,
                                           out OpenApiDiagnostic diagnostic,
                                           OpenApiReaderSettings settings = null)
        {
            return OpenApiModelFactory.Load<OpenApiDiscriminator>(input, version, out diagnostic, format, settings);
        }


        /// <summary>
        /// Parses a string into a <see cref="OpenApiDocument"/> object.
        /// </summary>
        /// <param name="input"> The string input.</param>
        /// <param name="version"></param>
        /// <param name="diagnostic"></param>
        /// <param name="format"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static OpenApiDiscriminator Parse(string input,
                                            OpenApiSpecVersion version,
                                            out OpenApiDiagnostic diagnostic,
                                            string format = null,
                                            OpenApiReaderSettings settings = null)
        {
            return OpenApiModelFactory.Parse<OpenApiDiscriminator>(input, version, out diagnostic, format, settings);
        }
    }
}
