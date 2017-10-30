﻿// ------------------------------------------------------------
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
    /// ExternalDocs object.
    /// </summary>
    public class OpenApiExternalDocs : OpenApiElement, IOpenApiExtension
    {
        /// <summary>
        /// REQUIRED.The URL for the target documentation. Value MUST be in the format of a URL.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// A short description of the target documentation.
        /// </summary>
        public Uri Url { get; set; }

        /// <summary>
        /// This object MAY be extended with Specification Extensions.
        /// </summary>
        public IDictionary<string, IOpenApiAny> Extensions { get; set; }

        /// <summary>
        /// Serialize <see cref="OpenApiExternalDocs"/> to Open Api v3.0
        /// </summary>
        internal override void WriteAsV3(IOpenApiWriter writer)
        {
            WriteInternal(writer);
        }

        /// <summary>
        /// Serialize <see cref="OpenApiExternalDocs"/> to Open Api v2.0
        /// </summary>
        internal override void WriteAsV2(IOpenApiWriter writer)
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
            writer.WriteStringProperty("description", Description);
            writer.WriteStringProperty("url", Url?.OriginalString);
            writer.WriteEndObject();
        }
    }
}
