// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Models
{
    /// <summary>
    /// Response object.
    /// </summary>
    public class OpenApiResponse : IOpenApiReferenceable, IOpenApiExtensible
    {
        /// <summary>
        /// REQUIRED. A short description of the response.
        /// </summary>
        public virtual string Description { get; set; }

        /// <summary>
        /// Maps a header name to its definition.
        /// </summary>
        public virtual IDictionary<string, OpenApiHeader> Headers { get; set; } = new Dictionary<string, OpenApiHeader>();

        /// <summary>
        /// A map containing descriptions of potential response payloads.
        /// The key is a media type or media type range and the value describes it.
        /// </summary>
        public virtual IDictionary<string, OpenApiMediaType> Content { get; set; } = new Dictionary<string, OpenApiMediaType>();

        /// <summary>
        /// A map of operations links that can be followed from the response.
        /// The key of the map is a short name for the link,
        /// following the naming constraints of the names for Component Objects.
        /// </summary>
        public virtual IDictionary<string, OpenApiLink> Links { get; set; } = new Dictionary<string, OpenApiLink>();

        /// <summary>
        /// This object MAY be extended with Specification Extensions.
        /// </summary>
        public virtual IDictionary<string, IOpenApiExtension> Extensions { get; set; } = new Dictionary<string, IOpenApiExtension>();

        /// <summary>
        /// Indicates if object is populated with data or is just a reference to the data
        /// </summary>
        public bool UnresolvedReference { get; set; }

        /// <summary>
        /// Reference pointer.
        /// </summary>
        public OpenApiReference Reference { get; set; }

        /// <summary>
        /// Parameterless constructor
        /// </summary>
        public OpenApiResponse() { }

        /// <summary>
        /// Initializes a copy of <see cref="OpenApiResponse"/> object
        /// </summary>
        public OpenApiResponse(OpenApiResponse response)
        {
            Description = response?.Description ?? Description;
            Headers = response?.Headers != null ? new Dictionary<string, OpenApiHeader>(response.Headers) : null;
            Content = response?.Content != null ? new Dictionary<string, OpenApiMediaType>(response.Content) : null;
            Links = response?.Links != null ? new Dictionary<string, OpenApiLink>(response.Links) : null;
            Extensions = response?.Extensions != null ? new Dictionary<string, IOpenApiExtension>(response.Extensions) : null;
            UnresolvedReference = response?.UnresolvedReference ?? UnresolvedReference;
            Reference = response?.Reference != null ? new(response?.Reference) : null;
        }

        /// <summary>
        /// Serialize <see cref="OpenApiResponse"/> to Open Api v3.1
        /// </summary>
        public virtual void SerializeAsV31(IOpenApiWriter writer)
        {
            SerializeInternal(writer, OpenApiSpecVersion.OpenApi3_1, (writer, element) => element.SerializeAsV31(writer));
        }

        /// <summary>
        /// Serialize <see cref="OpenApiResponse"/> to Open Api v3.0.
        /// </summary>
        public virtual void SerializeAsV3(IOpenApiWriter writer)
        {
            SerializeInternal(writer, OpenApiSpecVersion.OpenApi3_0, (writer, element) => element.SerializeAsV3(writer));
        }

        internal virtual void SerializeInternal(IOpenApiWriter writer, OpenApiSpecVersion version, 
            Action<IOpenApiWriter, IOpenApiSerializable> callback)
        {
            Utils.CheckArgumentNull(writer);

            writer.WriteStartObject();

            // description
            writer.WriteRequiredProperty(OpenApiConstants.Description, Description);

            // headers
            writer.WriteOptionalMap(OpenApiConstants.Headers, Headers, callback);

            // content
            writer.WriteOptionalMap(OpenApiConstants.Content, Content, callback);

            // links
            writer.WriteOptionalMap(OpenApiConstants.Links, Links, callback);

            // extension
            writer.WriteExtensions(Extensions, version);

            writer.WriteEndObject();
        }

        /// <summary>
        /// Serialize to OpenAPI V2 document without using reference.
        /// </summary>
        public virtual void SerializeAsV2(IOpenApiWriter writer)
        {
            Utils.CheckArgumentNull(writer);

            writer.WriteStartObject();

            // description
            writer.WriteRequiredProperty(OpenApiConstants.Description, Description);

            var extensionsClone = new Dictionary<string, IOpenApiExtension>(Extensions);

            if (Content != null)
            {
                var mediatype = Content.FirstOrDefault();
                if (mediatype.Value != null)
                {
                    // schema
                    writer.WriteOptionalObject(OpenApiConstants.Schema, mediatype.Value.Schema, (w, s) => s.SerializeAsV2(w));

                    // examples
                    if (Content.Values.Any(m => m.Example != null))
                    {
                        writer.WritePropertyName(OpenApiConstants.Examples);
                        writer.WriteStartObject();

                        foreach (var mediaTypePair in Content)
                        {
                            if (mediaTypePair.Value.Example != null)
                            {
                                writer.WritePropertyName(mediaTypePair.Key);
                                writer.WriteAny(mediaTypePair.Value.Example);
                            }
                        }

                        writer.WriteEndObject();
                    }

                    if (Content.Values.Any(m => m.Examples != null && m.Examples.Any()))
                    {
                        writer.WritePropertyName(OpenApiConstants.ExamplesExtension);
                        writer.WriteStartObject();

                        foreach (var example in Content
                            .Where(mediaTypePair => mediaTypePair.Value.Examples != null && mediaTypePair.Value.Examples.Any())
                            .SelectMany(mediaTypePair => mediaTypePair.Value.Examples))
                        {
                            writer.WritePropertyName(example.Key);
                            example.Value.SerializeInternal(writer, OpenApiSpecVersion.OpenApi2_0);
                        }

                        writer.WriteEndObject();
                    }

                    writer.WriteExtensions(mediatype.Value.Extensions, OpenApiSpecVersion.OpenApi2_0);

                    foreach (var key in mediatype.Value.Extensions.Keys)
                    {
                        // The extension will already have been serialized as part of the call above,
                        // so remove it from the cloned collection so we don't write it again.
                        extensionsClone.Remove(key);
                    }
                }
            }

            // headers
            writer.WriteOptionalMap(OpenApiConstants.Headers, Headers, (w, h) => h.SerializeAsV2(w));

            // extension
            writer.WriteExtensions(extensionsClone, OpenApiSpecVersion.OpenApi2_0);

            writer.WriteEndObject();
        }
    }
}
