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
    /// Parameter Object.
    /// </summary>
    public class OpenApiParameter : IOpenApiReference, IOpenApiExtension
    {
        public OpenApiReference Pointer { get; set; }
        public string Name { get; set; }
        public ParameterLocation In
        {
            get { return @in; }
            set
            {
                @in = value;
                if (@in == ParameterLocation.path)
                {
                    Required = true;
                }
            }
        }
        private ParameterLocation @in;
        public string Description { get; set; }
        public bool Required
        {
            get { return required; }
            set
            {
                if (In == ParameterLocation.path && value == false)
                {
                    throw new ArgumentException("Required cannot be set to false when in is path");
                }
                required = value;
            }
        }
        private bool required = false;
        public bool Deprecated { get; set; } = false;
        public bool AllowEmptyValue { get; set; } = false;
        public string Style { get; set; }
        public bool Explode { get; set; }
        public bool AllowReserved { get; set; }
        public OpenApiSchema Schema { get; set; }
        public IList<OpenApiExample> Examples { get; set; } = new List<OpenApiExample>();
        public string Example { get; set; }
        public IDictionary<string, OpenApiMediaType> Content { get; set; }
        public IDictionary<string, IOpenApiAny> Extensions { get; set; }

        /// <summary>
        /// Serialize <see cref="OpenApiParameter"/> to Open Api v3.0
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
                writer.WriteStringProperty("name", Name);
                writer.WriteStringProperty("in", In.ToString());
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
        /// Serialize <see cref="OpenApiParameter"/> to Open Api v2.0
        /// </summary>
        public virtual void WriteAsV2(IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            if (this.IsReference())
            {
                this.WriteRef(writer);
                return;
            }
            else
            {
                writer.WriteStartObject();
                writer.WriteStringProperty("name", Name);
                if (this is BodyParameter)
                {
                    writer.WriteStringProperty("in", "body");   // form?
                }
                else
                {
                    writer.WriteStringProperty("in", In.ToString());
                }
                writer.WriteStringProperty("description", Description);
                writer.WriteBoolProperty("required", Required, false);
                writer.WriteBoolProperty("deprecated", Deprecated, false);
                writer.WriteBoolProperty("allowEmptyValue", AllowEmptyValue, false);

                writer.WriteBoolProperty("allowReserved", AllowReserved, false);
                if (this is BodyParameter)
                {
                    writer.WriteObject("schema", Schema, (w, s) => s.WriteAsV2(w));
                }
                else
                {
                    Schema.SerializeSchemaProperties(writer);
                }
                //            writer.WriteList("examples", Examples, AnyNode.Write);
                //            writer.WriteObject("example", Example, AnyNode.Write);
                writer.WriteEndObject();
            }
        }
    }

    internal class BodyParameter : OpenApiParameter
    {
    }
}
