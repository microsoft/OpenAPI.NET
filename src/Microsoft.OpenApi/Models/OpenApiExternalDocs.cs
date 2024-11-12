// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Models
{
    /// <summary>
    /// ExternalDocs object.
    /// </summary>
    public class OpenApiExternalDocs : IOpenApiSerializable, IOpenApiExtensible
    {
        /// <summary>
        /// A short description of the target documentation.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// REQUIRED. The URL for the target documentation. Value MUST be in the format of a URL.
        /// </summary>
        public Uri Url { get; set; }

        /// <summary>
        /// This object MAY be extended with Specification Extensions.
        /// </summary>
        public IDictionary<string, IOpenApiExtension> Extensions { get; set; } = new Dictionary<string, IOpenApiExtension>();

        /// <summary>
        /// Parameter-less constructor
        /// </summary>
        public OpenApiExternalDocs() { }

        /// <summary>
        /// Initializes a copy of an <see cref="OpenApiExternalDocs"/> object
        /// </summary>
        public OpenApiExternalDocs(OpenApiExternalDocs externalDocs)
        {
            Description = externalDocs?.Description ?? Description;
            Url = externalDocs?.Url != null ? new Uri(externalDocs.Url.OriginalString, UriKind.RelativeOrAbsolute) : null;
            Extensions = externalDocs?.Extensions != null ? new Dictionary<string, IOpenApiExtension>(externalDocs.Extensions) : null;
        }

        /// <summary>
        /// Serialize <see cref="OpenApiExternalDocs"/> to Open Api v3.1.
        /// </summary>
        public void SerializeAsV31(IOpenApiWriter writer)
        {
            WriteInternal(writer, OpenApiSpecVersion.OpenApi3_1);
        }

        /// <summary>
        /// Serialize <see cref="OpenApiExternalDocs"/> to Open Api v3.0.
        /// </summary>
        public void SerializeAsV3(IOpenApiWriter writer)
        {
            WriteInternal(writer, OpenApiSpecVersion.OpenApi3_0);
        }

        /// <summary>
        /// Serialize <see cref="OpenApiExternalDocs"/> to Open Api v2.0.
        /// </summary>
        public void SerializeAsV2(IOpenApiWriter writer)
        {
            WriteInternal(writer, OpenApiSpecVersion.OpenApi2_0);
        }

        private void WriteInternal(IOpenApiWriter writer, OpenApiSpecVersion specVersion)
        {
            Utils.CheckArgumentNull(writer);;

            writer.WriteStartObject();

            // description
            writer.WriteProperty(OpenApiConstants.Description, Description);

            // url
            writer.WriteProperty(OpenApiConstants.Url, Url?.OriginalString);

            // extensions
            writer.WriteExtensions(Extensions, specVersion);

            writer.WriteEndObject();
        }
    }
}
