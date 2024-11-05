// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Models
{
    /// <summary>
    /// Open API Info Object, it provides the metadata about the Open API.
    /// </summary>
    public class OpenApiInfo : IOpenApiSerializable, IOpenApiExtensible
    {
        /// <summary>
        /// REQUIRED. The title of the application.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// A short summary of the API.
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// A short description of the application.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// REQUIRED. The version of the OpenAPI document.
        /// </summary>
        public string Version { get; set; }

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
        public IDictionary<string, IOpenApiExtension> Extensions { get; set; } = new Dictionary<string, IOpenApiExtension>();

        /// <summary>
        /// Parameter-less constructor
        /// </summary>
        public OpenApiInfo() { }

        /// <summary>
        /// Initializes a copy of an <see cref="OpenApiInfo"/> object
        /// </summary>
        public OpenApiInfo(OpenApiInfo info)
        {
            Title = info?.Title ?? Title;
            Summary = info?.Summary ?? Summary;
            Description = info?.Description ?? Description;
            Version = info?.Version ?? Version;
            TermsOfService = info?.TermsOfService ?? TermsOfService;
            Contact = info?.Contact != null ? new(info?.Contact) : null;
            License = info?.License != null ? new(info?.License) : null;
            Extensions = info?.Extensions != null ? new Dictionary<string, IOpenApiExtension>(info.Extensions) : null;
        }

        /// <summary>
        /// Serialize <see cref="OpenApiInfo"/> to Open Api v3.1
        /// </summary>
        public void SerializeAsV31(IOpenApiWriter writer)
        {
            SerializeInternal(writer, OpenApiSpecVersion.OpenApi3_1, (writer, element) => element.SerializeAsV31(writer));

            // summary - present in 3.1
            writer.WriteProperty(OpenApiConstants.Summary, Summary);
            writer.WriteEndObject();
        }

        /// <summary>
        /// Serialize <see cref="OpenApiInfo"/> to Open Api v3.0
        /// </summary>
        public void SerializeAsV3(IOpenApiWriter writer)
        {
            SerializeInternal(writer, OpenApiSpecVersion.OpenApi3_0, (writer, element) => element.SerializeAsV3(writer));

            writer.WriteEndObject();
        }

        /// <summary>
        /// Serialize <see cref="OpenApiInfo"/> to Open Api v3.0
        /// </summary>
        private void SerializeInternal(IOpenApiWriter writer, OpenApiSpecVersion version, Action<IOpenApiWriter, IOpenApiSerializable> callback)
        {
            Utils.CheckArgumentNull(writer);;
            writer.WriteStartObject();

            // title
            writer.WriteProperty(OpenApiConstants.Title, Title);

            // description
            writer.WriteProperty(OpenApiConstants.Description, Description);

            // termsOfService
            writer.WriteProperty(OpenApiConstants.TermsOfService, TermsOfService?.OriginalString);

            // contact object
            writer.WriteOptionalObject(OpenApiConstants.Contact, Contact, callback);

            // license object
            writer.WriteOptionalObject(OpenApiConstants.License, License, callback);

            // version
            writer.WriteProperty(OpenApiConstants.Version, Version);

            // specification extensions
            writer.WriteExtensions(Extensions, version);
        }

        /// <summary>
        /// Serialize <see cref="OpenApiInfo"/> to Open Api v2.0
        /// </summary>
        public void SerializeAsV2(IOpenApiWriter writer)
        {
            Utils.CheckArgumentNull(writer);;

            writer.WriteStartObject();

            // title
            writer.WriteProperty(OpenApiConstants.Title, Title);

            // description
            writer.WriteProperty(OpenApiConstants.Description, Description);

            // termsOfService
            writer.WriteProperty(OpenApiConstants.TermsOfService, TermsOfService?.OriginalString);

            // contact object
            writer.WriteOptionalObject(OpenApiConstants.Contact, Contact, (w, c) => c.SerializeAsV2(w));

            // license object
            writer.WriteOptionalObject(OpenApiConstants.License, License, (w, l) => l.SerializeAsV2(w));

            // version
            writer.WriteProperty(OpenApiConstants.Version, Version);

            // specification extensions
            writer.WriteExtensions(Extensions, OpenApiSpecVersion.OpenApi2_0);

            writer.WriteEndObject();
        }
    }
}
