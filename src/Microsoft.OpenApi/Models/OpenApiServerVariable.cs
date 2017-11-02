﻿// ------------------------------------------------------------
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
    /// Server Variable Object.
    /// </summary>
    public class OpenApiServerVariable : OpenApiElement, IOpenApiExtension
    {
        /// <summary>
        /// An optional description for the server variable. CommonMark syntax MAY be used for rich text representation.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// REQUIRED. The default value to use for substitution, and to send, if an alternate value is not supplied.
        /// Unlike the Schema Object's default, this value MUST be provided by the consumer.
        /// </summary>
        public string Default { get; set; }

        /// <summary>
        /// An enumeration of string values to be used if the substitution options are from a limited set.
        /// </summary>
        public List<string> Enum { get; set; }

        /// <summary>
        /// This object MAY be extended with Specification Extensions.
        /// </summary>
        public IDictionary<string, IOpenApiAny> Extensions { get; set; }

        /// <summary>
        /// Serialize <see cref="OpenApiServerVariable"/> to Open Api v3.0
        /// </summary>
        internal override void WriteAsV3(IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            writer.WriteStartObject();

            // default
            writer.WriteStringProperty(OpenApiConstants.OpenApiDocDefault, Default);

            // description
            writer.WriteStringProperty(OpenApiConstants.OpenApiDocDescription, Description);

            // enums
            writer.WriteOptionalCollection(OpenApiConstants.OpenApiDocEnum, Enum, (w, s) => w.WriteValue(s));

            // specification extensions
            writer.WriteExtensions(Extensions);

            writer.WriteEndObject();
        }

        /// <summary>
        /// Serialize <see cref="OpenApiServerVariable"/> to Open Api v2.0
        /// </summary>
        internal override void WriteAsV2(IOpenApiWriter writer)
        {
            // ServerVariable does not exist in V2.
        }
    }
}
