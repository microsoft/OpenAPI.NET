// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Reader;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Models
{
    /// <summary>
    /// XML Object.
    /// </summary>
    public class OpenApiXml : IOpenApiSerializable, IOpenApiExtensible
    {
        /// <summary>
        /// Replaces the name of the element/attribute used for the described schema property.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The URI of the namespace definition. Value MUST be in the form of an absolute URI.
        /// </summary>
        public Uri Namespace { get; set; }

        /// <summary>
        /// The prefix to be used for the name
        /// </summary>
        public string Prefix { get; set; }

        /// <summary>
        /// Declares whether the property definition translates to an attribute instead of an element.
        /// Default value is false.
        /// </summary>
        public bool Attribute { get; set; }

        /// <summary>
        /// Signifies whether the array is wrapped.
        /// Default value is false.
        /// </summary>
        public bool Wrapped { get; set; }

        /// <summary>
        /// Specification Extensions.
        /// </summary>
        public IDictionary<string, IOpenApiExtension> Extensions { get; set; } = new Dictionary<string, IOpenApiExtension>();

        /// <summary>
        /// Parameterless constructor
        /// </summary>
        public OpenApiXml() { }

        /// <summary>
        /// Initializes a copy of an <see cref="OpenApiXml"/> object
        /// </summary>
        public OpenApiXml(OpenApiXml xml)
        {
            Name = xml?.Name ?? Name;
            Namespace = xml?.Namespace ?? Namespace;
            Prefix = xml?.Prefix ?? Prefix;
            Attribute = xml?.Attribute ?? Attribute;
            Wrapped = xml?.Wrapped ?? Wrapped;
            Extensions = xml?.Extensions != null ? new Dictionary<string, IOpenApiExtension>(xml.Extensions) : null;
        }

        /// <summary>
        /// Serialize <see cref="OpenApiXml"/> to Open Api v3.0
        /// </summary>
        public void SerializeAsV31(IOpenApiWriter writer)
        {
            Write(writer, OpenApiSpecVersion.OpenApi3_1);
        }

        /// <summary>
        /// Serialize <see cref="OpenApiXml"/> to Open Api v3.0
        /// </summary>
        public void SerializeAsV3(IOpenApiWriter writer)
        {
            Write(writer, OpenApiSpecVersion.OpenApi3_0);
        }

        /// <summary>
        /// Serialize <see cref="OpenApiXml"/> to Open Api v2.0
        /// </summary>
        public void SerializeAsV2(IOpenApiWriter writer)
        {
            Write(writer, OpenApiSpecVersion.OpenApi2_0);
        }

        private void Write(IOpenApiWriter writer, OpenApiSpecVersion specVersion)
        {
            Utils.CheckArgumentNull(writer);;

            writer.WriteStartObject();

            // name
            writer.WriteProperty(OpenApiConstants.Name, Name);

            // namespace
            writer.WriteProperty(OpenApiConstants.Namespace, Namespace?.AbsoluteUri);

            // prefix
            writer.WriteProperty(OpenApiConstants.Prefix, Prefix);

            // attribute
            writer.WriteProperty(OpenApiConstants.Attribute, Attribute, false);

            // wrapped
            writer.WriteProperty(OpenApiConstants.Wrapped, Wrapped, false);

            // extensions
            writer.WriteExtensions(Extensions, specVersion);

            writer.WriteEndObject();
        }

        /// <summary>
        /// Parses a local file path or Url into an OpenApiXml object.
        /// </summary>
        /// <param name="url"> The path to the OpenAPI file.</param>
        /// <param name="version">The OpenAPI specification version.</param>
        /// <param name="diagnostic"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static OpenApiXml Load(string url,
                                      OpenApiSpecVersion version,
                                      out OpenApiDiagnostic diagnostic,
                                      OpenApiReaderSettings settings = null)
        {
            return OpenApiModelFactory.Load<OpenApiXml>(url, version, out diagnostic, settings);
        }

        /// <summary>
        /// Reads the stream input and parses it into an OpenApiXml object.
        /// </summary>
        /// <param name="stream">Stream containing OpenAPI description to parse.</param>
        /// <param name="format">The OpenAPI format to use during parsing.</param>
        /// <param name="version"></param>
        /// <param name="diagnostic"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static OpenApiXml Load(Stream stream,
                                      string format,
                                      OpenApiSpecVersion version,
                                      out OpenApiDiagnostic diagnostic,
                                      OpenApiReaderSettings settings = null)
        {
            return OpenApiModelFactory.Load<OpenApiXml>(stream, version, out diagnostic, format, settings);
        }

        /// <summary>
        /// Reads the text reader content and parses it into an OpenApiXml object.
        /// </summary>
        /// <param name="input">TextReader containing OpenAPI description to parse.</param>
        /// <param name="format"> The OpenAPI format to use during parsing.</param>
        /// <param name="version"></param>
        /// <param name="diagnostic"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static OpenApiXml Load(TextReader input,
                                      string format,
                                      OpenApiSpecVersion version,
                                      out OpenApiDiagnostic diagnostic,
                                      OpenApiReaderSettings settings = null)
        {
            return OpenApiModelFactory.Load<OpenApiXml>(input, version, out diagnostic, format, settings);
        }


        /// <summary>
        /// Parses a string into a <see cref="OpenApiXml"/> object.
        /// </summary>
        /// <param name="input"> The string input.</param>
        /// <param name="version"></param>
        /// <param name="diagnostic"></param>
        /// <param name="format"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static OpenApiXml Parse(string input,
                                       OpenApiSpecVersion version,
                                       out OpenApiDiagnostic diagnostic,
                                       string format = null,
                                       OpenApiReaderSettings settings = null)
        {
            return OpenApiModelFactory.Parse<OpenApiXml>(input, version, out diagnostic, format, settings);
        }
    }
}
