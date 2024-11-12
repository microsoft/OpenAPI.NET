// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Models
{
    /// <summary>
    /// License Object.
    /// </summary>
    public class OpenApiLicense : IOpenApiSerializable, IOpenApiExtensible
    {
        /// <summary>
        /// REQUIRED. The license name used for the API.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// An SPDX license expression for the API. The identifier field is mutually exclusive of the url field.
        /// </summary>
        public string Identifier { get; set; }

        /// <summary>
        /// The URL pointing to the contact information. MUST be in the format of a URL.
        /// </summary>
        public Uri Url { get; set; }

        /// <summary>
        /// This object MAY be extended with Specification Extensions.
        /// </summary>
        public IDictionary<string, IOpenApiExtension> Extensions { get; set; } = new Dictionary<string, IOpenApiExtension>();

        /// <summary>
        /// Parameterless constructor
        /// </summary>
        public OpenApiLicense() { }

        /// <summary>
        /// Initializes a copy of an <see cref="OpenApiLicense"/> object
        /// </summary>
        public OpenApiLicense(OpenApiLicense license)
        {
            Name = license?.Name ?? Name;
            Identifier = license?.Identifier ?? Identifier;
            Url = license?.Url != null ? new Uri(license.Url.OriginalString, UriKind.RelativeOrAbsolute) : null;
            Extensions = license?.Extensions != null ? new Dictionary<string, IOpenApiExtension>(license.Extensions) : null;
        }

        /// <summary>
        /// Serialize <see cref="OpenApiLicense"/> to Open Api v3.1
        /// </summary>
        public void SerializeAsV31(IOpenApiWriter writer)
        {
            WriteInternal(writer, OpenApiSpecVersion.OpenApi3_1);
            writer.WriteProperty(OpenApiConstants.Identifier, Identifier);
            writer.WriteEndObject();
        }

        /// <summary>
        /// Serialize <see cref="OpenApiLicense"/> to Open Api v3.0
        /// </summary>
        public void SerializeAsV3(IOpenApiWriter writer)
        {
            WriteInternal(writer, OpenApiSpecVersion.OpenApi3_0);
            writer.WriteEndObject();
        }

        /// <summary>
        /// Serialize <see cref="OpenApiLicense"/> to Open Api v2.0
        /// </summary>
        public void SerializeAsV2(IOpenApiWriter writer)
        {
            WriteInternal(writer, OpenApiSpecVersion.OpenApi2_0);
            writer.WriteEndObject();
        }

        private void WriteInternal(IOpenApiWriter writer, OpenApiSpecVersion specVersion)
        {
            Utils.CheckArgumentNull(writer);;
            writer.WriteStartObject();

            // name
            writer.WriteProperty(OpenApiConstants.Name, Name);

            // url
            writer.WriteProperty(OpenApiConstants.Url, Url?.OriginalString);

            // specification extensions
            writer.WriteExtensions(Extensions, specVersion);
        }
    }
}
