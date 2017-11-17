// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.OpenApi.Any;
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
        /// REQUIRED.The license name used for the API.
        /// </summary>
        public string Name { get; set; } = OpenApiConstants.DefaultName;

        /// <summary>
        /// The URL pointing to the contact information. MUST be in the format of a URL.
        /// </summary>
        public Uri Url { get; set; }

        /// <summary>
        /// This object MAY be extended with Specification Extensions.
        /// </summary>
        public IDictionary<string, IOpenApiAny> Extensions { get; set; }

        /// <summary>
        /// Serialize <see cref="OpenApiLicense"/> to Open Api v3.0
        /// </summary>
        public void SerializeAsV3(IOpenApiWriter writer)
        {
            WriteInternal(writer);
        }

        /// <summary>
        /// Serialize <see cref="OpenApiLicense"/> to Open Api v2.0
        /// </summary>
        public void SerializeAsV2(IOpenApiWriter writer)
        {
            WriteInternal(writer);
        }

        private void WriteInternal(IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            writer.WriteStartObject();

            // name
            writer.WriteProperty(OpenApiConstants.Name, Name);

            // url
            writer.WriteProperty(OpenApiConstants.Url, Url?.OriginalString);

            // specification extensions
            writer.WriteExtensions(Extensions);

            writer.WriteEndObject();
        }
    }
}
