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
    /// Server Object: an object representing a Server.
    /// </summary>
    public class OpenApiServer : OpenApiElement, IOpenApiExtension
    {
        public string Description { get; set; }
        public string Url { get; set; }
        public IDictionary<string, OpenApiServerVariable> Variables { get; set; } = new Dictionary<string, OpenApiServerVariable>();

        public IDictionary<string, IOpenApiAny> Extensions { get; set; }

        /// <summary>
        /// Serialize <see cref="OpenApiServer"/> to Open Api v3.0
        /// </summary>
        internal override void WriteAsV3(IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            writer.WriteStartObject();
            writer.WriteStringProperty("url", Url);
            writer.WriteStringProperty("description", Description);
            writer.WriteMap("variables", Variables, (w, v) => v.WriteAsV3(w));
            writer.WriteEndObject();
        }

        /// <summary>
        /// Serialize <see cref="OpenApiServer"/> to Open Api v2.0
        /// </summary>
        internal override void WriteAsV2(IOpenApiWriter writer)
        {
            // nothing here
        }
    }
}
