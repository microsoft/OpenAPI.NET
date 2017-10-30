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
    /// Open API Info Object, it provides the metadata about the Open API.
    /// </summary>
    public class OpenApiInfo : OpenApiElement, IOpenApiExtension
    {
        /// <summary>
        /// REQUIRED. The title of the application.
        /// </summary>
        public string Title { get; set; } = OpenApiConstants.OpenApiDocDefaultTitle;

        /// <summary>
        /// A short description of the application.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// REQUIRED. The version of the OpenAPI document.
        /// </summary>
        public Version Version { get; set; } = new Version(1, 0);

        /// <summary>
        /// A URL to the Terms of Service for the API. MUST be in the format of a URL.
        /// </summary>
        public Uri TermsOfService { get; set; }

        /// <summary>
        /// The contact information for the exposed API.
        /// </summary>
        public OpenApiContact Contact { get; set; }

        /// <summary>
        /// The license information for the exposed API.
        /// </summary>
        public OpenApiLicense License { get; set; }

        /// <summary>
        /// This object MAY be extended with Specification Extensions.
        /// </summary>
        public IDictionary<string, IOpenApiAny> Extensions { get; set; } = new Dictionary<string, IOpenApiAny>();

        /// <summary>
        /// Serialize <see cref="OpenApiInfo"/> to Open Api v3.0
        /// </summary>
        internal override void WriteAsV3(IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            writer.WriteStartObject();

            // title
            writer.WriteStringProperty(OpenApiConstants.OpenApiDocTitle, Title);

            // description
            writer.WriteStringProperty(OpenApiConstants.OpenApiDocDescription, Description);

            // termsOfService
            writer.WriteStringProperty(OpenApiConstants.OpenApiDocTermsOfService, TermsOfService?.OriginalString);

            // contact object
            writer.WriteObject(OpenApiConstants.OpenApiDocContact, Contact, (w, c) => c.WriteAsV3(w));

            // license object
            writer.WriteObject(OpenApiConstants.OpenApiDocLicense, License, (w, l) => l.WriteAsV3(w));

            // version
            writer.WriteStringProperty(OpenApiConstants.OpenApiDocVersion, Version?.ToString());

            // specification extensions
            writer.WriteExtensions(Extensions);

            writer.WriteEndObject();
        }

        /// <summary>
        /// Serialize <see cref="OpenApiInfo"/> to Open Api v2.0
        /// </summary>
        internal override void WriteAsV2(IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            writer.WriteStartObject();

            // title
            writer.WriteStringProperty(OpenApiConstants.OpenApiDocTitle, Title);

            // description
            writer.WriteStringProperty(OpenApiConstants.OpenApiDocDescription, Description);

            // termsOfService
            writer.WriteStringProperty(OpenApiConstants.OpenApiDocTermsOfService, TermsOfService?.OriginalString);

            // contact object
            writer.WriteObject(OpenApiConstants.OpenApiDocContact, Contact, (w, c) => c.WriteAsV2(w));

            // license object
            writer.WriteObject(OpenApiConstants.OpenApiDocLicense, License, (w, l) => l.WriteAsV2(w));

            // version
            writer.WriteStringProperty(OpenApiConstants.OpenApiDocVersion, Version?.ToString());

            // specification extensions
            writer.WriteExtensions(Extensions);

            writer.WriteEndObject();
        }
    }
}
