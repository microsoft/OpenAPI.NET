// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Models
{
    /// <summary>
    /// Response object.
    /// </summary>
    public class OpenApiResponse : OpenApiElement, IOpenApiReference, IOpenApiExtension
    {
        public string Description { get; set; }
        public IDictionary<string, OpenApiMediaType> Content { get; set; }
        public IDictionary<string, OpenApiHeader> Headers { get; set; }
        public IDictionary<string, OpenApiLink> Links { get; set; }
        public IDictionary<string, IOpenApiAny> Extensions { get; set; }

        public OpenApiReference Pointer
        {
            get; set;
        }

        public void CreateContent(string mediatype, Action<OpenApiMediaType> configure)
        {
            var m = new OpenApiMediaType();
            configure(m);
            if (Content == null) {
                Content = new Dictionary<string, OpenApiMediaType>();
            }

            Content.Add(mediatype, m);
        }

        /// <summary>
        /// Serialize <see cref="OpenApiResponse"/> to Open Api v3.0
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
                writer.WriteMap("content", Content, (w, c) => c.WriteAsV3(w));

                writer.WriteMap("headers", Headers, (w, h) => h.WriteAsV3(w));
                writer.WriteMap("links", Links, (w, l) => l.WriteAsV3(w));

                //Links
                writer.WriteEndObject();
            }
        }

        /// <summary>
        /// Serialize <see cref="OpenApiResponse"/> to Open Api v2.0
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
                if (Content != null)
                {
                    var mediatype = Content.FirstOrDefault();
                    if (mediatype.Value != null)
                    {

                        writer.WriteObject("schema", mediatype.Value.Schema, (w, s) => s.WriteAsV2(w));

                        if (mediatype.Value.Example != null)
                        {
                            writer.WritePropertyName("examples");
                            writer.WriteStartObject();
                            writer.WritePropertyName(mediatype.Key);
                            writer.WriteValue(mediatype.Value.Example);
                            writer.WriteEndObject();
                        }
                    }
                }
                writer.WriteMap("headers", Headers, (w, h) => h.WriteAsV2(w));

                writer.WriteEndObject();
            }
        }
    }
}