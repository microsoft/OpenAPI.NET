// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Models
{
    /// <summary>
    /// Server Object: an object representing a Server.
    /// </summary>
    public class OpenApiServer : IOpenApiSerializable, IOpenApiExtensible
    {
        /// <summary>
        /// An optional string describing the host designated by the URL. CommonMark syntax MAY be used for rich text representation.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// REQUIRED. A URL to the target host. This URL supports Server Variables and MAY be relative,
        /// to indicate that the host location is relative to the location where the OpenAPI document is being served.
        /// Variable substitutions will be made when a variable is named in {brackets}.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// A map between a variable name and its value. The value is used for substitution in the server's URL template.
        /// </summary>
        public IDictionary<string, OpenApiServerVariable> Variables { get; set; } =
            new Dictionary<string, OpenApiServerVariable>();

        /// <summary>
        /// This object MAY be extended with Specification Extensions.
        /// </summary>
        public IDictionary<string, IOpenApiExtension> Extensions { get; set; } = new Dictionary<string, IOpenApiExtension>();

        /// <summary>
        /// Parameterless constructor
        /// </summary>
        public OpenApiServer() { }

        /// <summary>
        /// Initializes a copy of an <see cref="OpenApiServer"/> object
        /// </summary>
        public OpenApiServer(OpenApiServer server)
        {
            Description = server?.Description ?? Description;
            Url = server?.Url ?? Url;
            Variables = server?.Variables != null ? new Dictionary<string, OpenApiServerVariable>(server.Variables) : null;
            Extensions = server?.Extensions != null ? new Dictionary<string, IOpenApiExtension>(server.Extensions) : null;
        }

        /// <summary>
        /// Serialize <see cref="OpenApiServer"/> to Open Api v3.1
        /// </summary>
        public void SerializeAsV31(IOpenApiWriter writer)
        {
            SerializeInternal(writer, OpenApiSpecVersion.OpenApi3_1, (writer, element) => element.SerializeAsV31(writer));
        }

        /// <summary>
        /// Serialize <see cref="OpenApiServer"/> to Open Api v3.0
        /// </summary>
        public void SerializeAsV3(IOpenApiWriter writer)
        {
            SerializeInternal(writer, OpenApiSpecVersion.OpenApi3_0, (writer, element) => element.SerializeAsV3(writer));
        }

        /// <summary>
        /// Serialize <see cref="OpenApiServer"/> to Open Api v3.0
        /// </summary>
        private void SerializeInternal(IOpenApiWriter writer, OpenApiSpecVersion version,
            Action<IOpenApiWriter, IOpenApiSerializable> callback)
        {
            Utils.CheckArgumentNull(writer);;

            writer.WriteStartObject();

            // url
            writer.WriteProperty(OpenApiConstants.Url, Url);

            // description
            writer.WriteProperty(OpenApiConstants.Description, Description);

            // variables
            writer.WriteOptionalMap(OpenApiConstants.Variables, Variables, callback);

            // specification extensions
            writer.WriteExtensions(Extensions, version);

            writer.WriteEndObject();
        }

        /// <summary>
        /// Serialize <see cref="OpenApiServer"/> to Open Api v2.0
        /// </summary>
        public void SerializeAsV2(IOpenApiWriter writer)
        {
            // Server object does not exist in V2.
        }
    }
}
