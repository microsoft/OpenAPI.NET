﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Writers;
using static Microsoft.OpenApi.Extensions.OpenApiSerializableExtensions;

namespace Microsoft.OpenApi.Models
{
    /// <summary>
    /// Response object.
    /// </summary>
    public class OpenApiResponse : IOpenApiSerializable, IOpenApiReferenceable, IOpenApiExtensible, IEffective<OpenApiResponse>
    {
        /// <summary>
        /// REQUIRED. A short description of the response.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Maps a header name to its definition.
        /// </summary>
        public IDictionary<string, OpenApiHeader> Headers { get; set; } = new Dictionary<string, OpenApiHeader>();

        /// <summary>
        /// A map containing descriptions of potential response payloads.
        /// The key is a media type or media type range and the value describes it.
        /// </summary>
        public IDictionary<string, OpenApiMediaType> Content { get; set; } = new Dictionary<string, OpenApiMediaType>();

        /// <summary>
        /// A map of operations links that can be followed from the response.
        /// The key of the map is a short name for the link,
        /// following the naming constraints of the names for Component Objects.
        /// </summary>
        public IDictionary<string, OpenApiLink> Links { get; set; } = new Dictionary<string, OpenApiLink>();

        /// <summary>
        /// This object MAY be extended with Specification Extensions.
        /// </summary>
        public IDictionary<string, IOpenApiExtension> Extensions { get; set; } = new Dictionary<string, IOpenApiExtension>();

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
        public OpenApiResponse() {}

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
        public void SerializeAsV31(IOpenApiWriter writer)
        {
            SerializeInternal(writer, (writer, element) => element.SerializeAsV31(writer), 
                (writer, element) => element.SerializeAsV31WithoutReference(writer));
        }
        
        /// <summary>
        /// Serialize <see cref="OpenApiResponse"/> to Open Api v3.0.
        /// </summary>
        public void SerializeAsV3(IOpenApiWriter writer)
        {
            SerializeInternal(writer, (writer, element) => element.SerializeAsV3(writer), 
                (writer, element) => element.SerializeAsV3WithoutReference(writer));
        }

        private void SerializeInternal(IOpenApiWriter writer, Action<IOpenApiWriter, IOpenApiSerializable> callback, 
            Action<IOpenApiWriter, IOpenApiReferenceable> action)
        {
            writer = writer ?? throw Error.ArgumentNull(nameof(writer));

            var target = this;

            if (Reference != null)
            {
                if (!writer.GetSettings().ShouldInlineReference(Reference))
                {
                    callback(writer, Reference);
                    return;
                }
                else
                {
                    target = GetEffective(Reference.HostDocument);
                }
            }
            action(writer, target);
        }

        /// <summary>
        /// Returns an effective OpenApiRequestBody object based on the presence of a $ref 
        /// </summary>
        /// <param name="doc">The host OpenApiDocument that contains the reference.</param>
        /// <returns>OpenApiResponse</returns>
        public OpenApiResponse GetEffective(OpenApiDocument doc)
        {
            if (this.Reference != null)
            {
                return doc.ResolveReferenceTo<OpenApiResponse>(this.Reference);
            }
            else
            {
                return this;
            }
        }
        
        /// <summary>
        /// Serialize to OpenAPI V3 document without using reference.
        /// </summary>
        public void SerializeAsV31WithoutReference(IOpenApiWriter writer) 
        {
            SerializeInternalWithoutReference(writer, OpenApiSpecVersion.OpenApi3_1, 
                (writer, element) => element.SerializeAsV31(writer));
        }

        /// <summary>
        /// Serialize to OpenAPI V3 document without using reference.
        /// </summary>
        public void SerializeAsV3WithoutReference(IOpenApiWriter writer) 
        {
            SerializeInternalWithoutReference(writer, OpenApiSpecVersion.OpenApi3_0, 
                (writer, element) => element.SerializeAsV3(writer));
        }

        private void SerializeInternalWithoutReference(IOpenApiWriter writer, OpenApiSpecVersion version, 
            Action<IOpenApiWriter, IOpenApiSerializable> callback)
        {
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
        /// Serialize <see cref="OpenApiResponse"/> to Open Api v2.0.
        /// </summary>
        public void SerializeAsV2(IOpenApiWriter writer)
        {
            writer = writer ?? throw Error.ArgumentNull(nameof(writer));

            var target = this;

            if (Reference != null)
            {
                if (!writer.GetSettings().ShouldInlineReference(Reference))
                {
                    Reference.SerializeAsV2(writer);
                    return;
                }
                else
                {
                    target = GetEffective(Reference.HostDocument);
                }
            }
            target.SerializeAsV2WithoutReference(writer);
        }

        /// <summary>
        /// Serialize to OpenAPI V2 document without using reference.
        /// </summary>
        public void SerializeAsV2WithoutReference(IOpenApiWriter writer)
        {
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
                    writer.WriteOptionalObject(
                        OpenApiConstants.Schema,
                        mediatype.Value.Schema,
                        (w, s) => s.SerializeAsV2(w));

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
