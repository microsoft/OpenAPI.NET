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
    /// Link Object.
    /// </summary>
    public class OpenApiLink :  IOpenApiReference, IOpenApiExtension
    {
        public string Href { get; set; }
        public string OperationId { get; set; }
        public Dictionary<string, RuntimeExpression> Parameters { get; set; }
        public RuntimeExpression RequestBody { get; set; }

        public string Description { get; set; }

        public IDictionary<string, IOpenApiAny> Extensions { get; set; }

        public OpenApiReference Pointer { get; set; }

        /// <summary>
        /// Serialize <see cref="OpenApiLink"/> to Open Api v3.0
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
                writer.WriteStringProperty("href", Href);
                writer.WriteStringProperty("operationId", OperationId);
                writer.WriteMap("parameters", Parameters, (w, x) => { w.WriteValue(x.ToString()); });
                writer.WriteEndObject();
            }
        }

        /// <summary>
        /// Serialize <see cref="OpenApiLink"/> to Open Api v3.0
        /// </summary>
        public virtual void WriteAsV2(IOpenApiWriter writer)
        {
            // nothing here
        }
    }
}
