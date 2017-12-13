﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Collections.Generic;
using Microsoft.OpenApi.Any;
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
        public IDictionary<string, IOpenApiAny> Extensions { get; set; } = new Dictionary<string, IOpenApiAny>();

        /// <summary>
        /// Serialize <see cref="OpenApiServer"/> to Open Api v3.0
        /// </summary>
        public void SerializeAsV3(IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            writer.WriteStartObject();

            // url
            writer.WriteProperty(OpenApiConstants.Url, Url);

            // description
            writer.WriteProperty(OpenApiConstants.Description, Description);

            // variables
            writer.WriteOptionalMap(OpenApiConstants.Variables, Variables, (w, v) => v.SerializeAsV3(w));

            // specification extensions
            writer.WriteExtensions(Extensions);

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