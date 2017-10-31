// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Models
{
    /// <summary>
    /// ExternalDocs object.
    /// </summary>
    public class OpenApiEncoding : OpenApiElement, IOpenApiExtension
    {
        /// <summary>
        /// The Content-Type for encoding a specific property.
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// A map allowing additional information to be provided as headers.
        /// </summary>
        public IDictionary<string, OpenApiHeader> Headers { get; set; }

        /// <summary>
        /// Describes how a specific property value will be serialized depending on its type. 
        /// </summary>
        public ParameterStyle? Style { get; set; }

        /// <summary>
        /// Explode
        /// </summary>
        public bool? Explode { get; set; }

        /// <summary>
        /// AllowReserved
        /// </summary>
        public bool? AllowReserved { get; set; }

        /// <summary>
        /// This object MAY be extended with Specification Extensions.
        /// </summary>
        public IDictionary<string, IOpenApiAny> Extensions { get; set; }

        /// <summary>
        /// Serialize <see cref="OpenApiExternalDocs"/> to Open Api v3.0.
        /// </summary>
        internal override void WriteAsV3(IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull("writer");
            }

            // { for json, empty for YAML
            writer.WriteStartObject();

            // contentType
            writer.WriteStringProperty(OpenApiConstants.OpenApiDocContentType, ContentType);

            // headers
            writer.WriteMap(OpenApiConstants.OpenApiDocHeaders, Headers, (w, h) => h.WriteAsV3(w));

            // style
            writer.WriteStringProperty(OpenApiConstants.OpenApiDocStyle, Style?.ToString());

            // explode
            if (Explode != null)
            {
                writer.WriteBoolProperty(OpenApiConstants.OpenApiDocExplode, Explode.Value, false);
            }

            // allowReserved
            if (AllowReserved != null)
            {
                writer.WriteBoolProperty(OpenApiConstants.OpenApiDocAllowReserved, AllowReserved.Value, false);
            }

            // specification extensions
            writer.WriteExtensions(Extensions);

            // } for json, empty for YAML
            writer.WriteEndObject();
        }

        /// <summary>
        /// Serialize <see cref="OpenApiExternalDocs"/> to Open Api v2.0.
        /// </summary>
        internal override void WriteAsV2(IOpenApiWriter writer)
        {
            // nothing here
        }
    }
}
