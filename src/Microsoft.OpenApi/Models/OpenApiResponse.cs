﻿// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

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
        /// <summary>
        /// REQUIRED. A short description of the response.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Maps a header name to its definition.
        /// </summary>
        public IDictionary<string, OpenApiHeader> Headers { get; set; }

        /// <summary>
        /// A map containing descriptions of potential response payloads.
        /// The key is a media type or media type range and the value describes it.
        /// </summary>
        public IDictionary<string, OpenApiMediaType> Content { get; set; }

        /// <summary>
        /// A map of operations links that can be followed from the response.
        /// The key of the map is a short name for the link,
        /// following the naming constraints of the names for Component Objects.
        /// </summary>
        public IDictionary<string, OpenApiLink> Links { get; set; }

        /// <summary>
        /// This object MAY be extended with Specification Extensions.
        /// </summary>
        public IDictionary<string, IOpenApiAny> Extensions { get; set; }

        /// <summary>
        /// Reference pointer.
        /// </summary>
        public OpenApiReference Pointer
        {
            get; set;
        }

        /// <summary>
        /// Serialize <see cref="OpenApiResponse"/> to Open Api v3.0.
        /// </summary>
        internal override void WriteAsV3(IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            if (Pointer != null)
            {
                Pointer.WriteAsV3(writer);
            }
            else
            {
                writer.WriteStartObject();

                // description
                writer.WriteStringProperty(OpenApiConstants.Description, Description);

                // headers
                writer.WriteOptionalMap(OpenApiConstants.Headers, Headers, (w, h) => h.WriteAsV3(w));

                // content
                writer.WriteOptionalMap(OpenApiConstants.Content, Content, (w, c) => c.WriteAsV3(w));

                // links
                writer.WriteOptionalMap(OpenApiConstants.Links, Links, (w, l) => l.WriteAsV3(w));

                // extension
                writer.WriteExtensions(Extensions);

                writer.WriteEndObject();
            }
        }

        /// <summary>
        /// Serialize <see cref="OpenApiResponse"/> to Open Api v2.0.
        /// </summary>
        internal override void WriteAsV2(IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            if (Pointer != null)
            {
                Pointer.WriteAsV2(writer);
            }
            else
            {
                writer.WriteStartObject();

                // description
                writer.WriteStringProperty(OpenApiConstants.Description, Description);
                if (Content != null)
                {
                    var mediatype = Content.FirstOrDefault();
                    if (mediatype.Value != null)
                    {
                        // schema
                        writer.WriteOptionalObject(OpenApiConstants.Schema, mediatype.Value.Schema, (w, s) => s.WriteAsV2(w));

                        // examples
                        if (mediatype.Value.Example != null)
                        {
                            writer.WritePropertyName(OpenApiConstants.Examples);
                            writer.WriteStartObject();
                            writer.WritePropertyName(mediatype.Key);
                            writer.WriteValue(mediatype.Value.Example);
                            writer.WriteEndObject();
                        }
                    }
                }

                // headers
                writer.WriteOptionalMap(OpenApiConstants.Headers, Headers, (w, h) => h.WriteAsV2(w));

                // extension
                writer.WriteExtensions(Extensions);

                writer.WriteEndObject();
            }
        }
    }
}
