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
    /// Header Object.
    /// The Header Object follows the structure of the Parameter Object 
    /// </summary>
    public class OpenApiHeader : OpenApiElement, IOpenApiReference, IOpenApiExtension
    {
        public OpenApiReference Pointer { get; set; }
        public string Description { get; set; }
        public bool Required { get; set; }
        public bool Deprecated { get; set; }
        public bool AllowEmptyValue { get; set; }
        public string Style { get; set; }
        public bool Explode { get; set; }
        public bool AllowReserved { get; set; }
        public OpenApiSchema Schema { get; set; }
        public string Example { get; set; }
        public IList<OpenApiExample> Examples { get; set; }
        public IDictionary<string, OpenApiMediaType> Content { get; set; }

        public IDictionary<string, IOpenApiAny> Extensions { get; set; }

        /// <summary>
        /// Serialize <see cref="OpenApiHeader"/> to Open Api v3.0
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
                writer.WriteBoolProperty("deprecated", Deprecated, false);
                writer.WriteBoolProperty("allowEmptyValue", AllowEmptyValue, false);
                writer.WriteStringProperty("style", Style);
                writer.WriteBoolProperty("explode", Explode, false);
                writer.WriteBoolProperty("allowReserved", AllowReserved, false);
                writer.WriteObject("schema", Schema, (w, s) => s.WriteAsV3(w));
                writer.WriteList("examples", Examples, (w, e) => e.WriteAsV3(w));
                writer.WriteObject("example", Example, (w, s) => w.WriteRaw(s));
                writer.WriteMap("content", Content, (w, c) => c.WriteAsV3(w));

                writer.WriteEndObject();
            }
        }

        /// <summary>
        /// Serialize <see cref="OpenApiHeader"/> to Open Api v2.0
        /// </summary>
        internal override void WriteAsV2(IOpenApiWriter writer)
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
                writer.WriteBoolProperty("deprecated", Deprecated, false);
                writer.WriteBoolProperty("allowEmptyValue", AllowEmptyValue, false);
                writer.WriteStringProperty("style", Style);
                writer.WriteBoolProperty("explode", Explode, false);
                writer.WriteBoolProperty("allowReserved", AllowReserved, false);
                writer.WriteObject("schema", Schema, (w, s) => s.WriteAsV2(w));
                writer.WriteStringProperty("example", Example);

                writer.WriteEndObject();
            }
        }
    }
}
