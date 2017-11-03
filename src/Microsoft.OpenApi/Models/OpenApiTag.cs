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
    /// Tag Object.
    /// </summary>
    public class OpenApiTag : OpenApiElement, IOpenApiReference, IOpenApiExtension
    {
        /// <summary>
        /// The name of the tag.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// A short description for the tag.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Additional external documentation for this tag.
        /// </summary>
        public OpenApiExternalDocs ExternalDocs { get; set; }

        /// <summary>
        /// This object MAY be extended with Specification Extensions.
        /// </summary>
        public IDictionary<string, IOpenApiAny> Extensions { get; set; }

        OpenApiReference IOpenApiReference.Pointer
        {
            get; set;
        }

        /// <summary>
        /// Serialize <see cref="OpenApiTag"/> to Open Api v3.0
        /// </summary>
        internal override void WriteAsV3(IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            writer.WriteStartObject();

            writer.WriteStringProperty(OpenApiConstants.Name, Name);

            writer.WriteStringProperty(OpenApiConstants.Description, Description);

            writer.WriteObject(OpenApiConstants.ExternalDocs, ExternalDocs, (w, e) => e.WriteAsV3(w));

            writer.WriteExtensions(Extensions);

            writer.WriteEndObject();
        }

        /// <summary>
        /// Serialize <see cref="OpenApiTag"/> to Open Api v2.0
        /// </summary>
        internal override void WriteAsV2(IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            writer.WriteStartObject();

            writer.WriteStringProperty(OpenApiConstants.Name, Name);

            writer.WriteStringProperty(OpenApiConstants.Description, Description);

            writer.WriteObject(OpenApiConstants.ExternalDocs, ExternalDocs, (w, e) => e.WriteAsV2(w));

            writer.WriteExtensions(Extensions);

            writer.WriteEndObject();
        }
    }
}
