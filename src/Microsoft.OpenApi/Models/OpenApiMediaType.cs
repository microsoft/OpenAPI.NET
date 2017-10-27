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
    /// Media Type Object.
    /// </summary>
    public class OpenApiMediaType : IOpenApiExtension
    {
        public OpenApiSchema Schema { get; set; }
        public IDictionary<string, OpenApiExample> Examples { get; set; }
        public string Example { get; set; }

        public IDictionary<string, IOpenApiAny> Extensions { get; set; }

        /// <summary>
        /// Serialize <see cref="OpenApiMediaType"/> to Open Api v3.0
        /// </summary>
        public virtual void WriteAsV3(IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            writer.WriteStartObject();
            writer.WriteObject("schema", Schema, (w, s) => s.WriteAsV3(w));
            writer.WriteObject("example", Example, (w, e) => w.WriteRaw(e));
            writer.WriteMap("examples", Examples, (w, e) => e.WriteAsV3(w));
            writer.WriteEndObject();
        }

        /// <summary>
        /// Serialize <see cref="OpenApiMediaType"/> to Open Api v2.0
        /// </summary>
        public virtual void WriteAsV2(IOpenApiWriter writer)
        {
            // nothing here
        }
    }
}
