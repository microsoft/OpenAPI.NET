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
    /// Request Body Object
    /// </summary>
    public class OpenApiRequestBody : OpenApiElement, IOpenApiReference, IOpenApiExtension
    {
        public OpenApiReference Pointer { get; set; }

        public string Description { get; set; }
        public Boolean Required { get; set; }
        public IDictionary<string, OpenApiMediaType> Content { get; set; }
        public IDictionary<string,IOpenApiAny> Extensions { get; set; }

        /// <summary>
        /// Serialize <see cref="OpenApiRequestBody"/> to Open Api v3.0
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

                writer.WriteStringProperty("description", Description);
                writer.WriteBoolProperty("required", Required, false);
                writer.WriteMap("content", Content, (w, c) => c.WriteAsV3(w));

                writer.WriteEndObject();
            }
        }

        /// <summary>
        /// Serialize <see cref="OpenApiRequestBody"/> to Open Api v2.0
        /// </summary>
        internal override void WriteAsV2(IOpenApiWriter writer)
        {
            // nothing here
        }
    }
}
