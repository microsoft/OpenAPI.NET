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
    /// Server Variable Object.
    /// </summary>
    public class OpenApiServerVariable : IOpenApiExtension
    {
        public string Description { get; set; }
        public string Default { get; set; }
        public List<string> Enum { get; set; } = new List<string>();

        public IDictionary<string, IOpenApiAny> Extensions { get; set; }

        /// <summary>
        /// Serialize <see cref="OpenApiServerVariable"/> to Open Api v3.0
        /// </summary>
        public virtual void WriteAsV3(IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            writer.WriteStartObject();
            writer.WriteStringProperty("default", Default);
            writer.WriteStringProperty("description", Description);
            writer.WriteList("enum", Enum, (w, s) => w.WriteValue(s));
            writer.WriteEndObject();
        }

        /// <summary>
        /// Serialize <see cref="OpenApiServerVariable"/> to Open Api v2.0
        /// </summary>
        public virtual void WriteAsV2(IOpenApiWriter writer)
        {
            // nothing here
        }
    }
}
