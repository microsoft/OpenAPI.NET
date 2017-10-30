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
    /// Example Object.
    /// </summary>
    public class OpenApiExample : OpenApiElement, IOpenApiReference, IOpenApiExtension
    {
        /// <summary>
        /// Short description for the example.
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// Long description for the example.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Embedded literal example.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// This object MAY be extended with Specification Extensions.
        /// </summary>
        public IDictionary<string, IOpenApiAny> Extensions { get; set; }

        public OpenApiReference Pointer
        {
            get; set;
        }

        /// <summary>
        /// Serialize <see cref="OpenApiExample"/> to Open Api v3.0
        /// </summary>
        internal override void WriteAsV3(IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            if (this.IsReference())
            {
                this.WriteRef(writer);
            }
            else
            {
                writer.WriteStartObject();
                writer.WriteStringProperty("summary", Summary);
                writer.WriteStringProperty("description", Description);
                if (Value != null)
                {
                    writer.WritePropertyName("value");
                    writer.WriteRaw(Value);
                }
                writer.WriteEndObject();
            }
        }

        /// <summary>
        /// Serialize <see cref="OpenApiExample"/> to Open Api v2.0
        /// </summary>
        internal override void WriteAsV2(IOpenApiWriter writer)
        {
            // nothing here
        }
    }
}
