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
    /// Callback Object: A map of possible out-of band callbacks related to the parent operation.
    /// </summary>
    public class OpenApiCallback : IOpenApiReference, IOpenApiExtension
    {
        public Dictionary<RuntimeExpression, OpenApiPathItem> PathItems { get; set; } = new Dictionary<RuntimeExpression, OpenApiPathItem>();

        public OpenApiReference Pointer { get; set; }

        public IDictionary<string, IOpenApiAny> Extensions { get; set; }

        /// <summary>
        /// Serialize <see cref="OpenApiCallback"/> to Open Api v3.0
        /// </summary>
        public virtual void WriteAsV3(IOpenApiWriter writer)
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
                foreach (var item in PathItems)
                {
                    writer.WriteObject<OpenApiPathItem>(item.Key.Expression, item.Value, (w, p) => p.WriteAsV3(w));
                }
                writer.WriteEndObject();
            }
        }

        /// <summary>
        /// Serialize <see cref="OpenApiCallback"/> to Open Api v2.0
        /// </summary>
        public virtual void WriteAsV2(IOpenApiWriter writer)
        {
            // nothing here
        }
    }
}
