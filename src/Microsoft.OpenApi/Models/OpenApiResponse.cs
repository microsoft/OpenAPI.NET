﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models.Interfaces;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Models
{
    /// <summary>
    /// Response object.
    /// </summary>
    public class OpenApiResponse : IOpenApiExtensible, IOpenApiResponse
    {
        /// <inheritdoc/>
        public string? Description { get; set; }

        private Lazy<IDictionary<string, IOpenApiHeader>>? _headers = new(() => new Dictionary<string, IOpenApiHeader>(StringComparer.Ordinal));
        /// <inheritdoc/>
        public IDictionary<string, IOpenApiHeader>? Headers
        {
            get => _headers?.Value;
            set => _headers = value is null ? null : new(() => value);
        }

        private Lazy<IDictionary<string, OpenApiMediaType>>? _content = new(() => new Dictionary<string, OpenApiMediaType>(StringComparer.Ordinal));
        /// <inheritdoc/>
        public IDictionary<string, OpenApiMediaType>? Content
        {
            get => _content?.Value;
            set => _content = value is null ? null : new(() => value);
        }

        private Lazy<IDictionary<string, IOpenApiLink>>? _links = new(() => new Dictionary<string, IOpenApiLink>(StringComparer.Ordinal));
        /// <inheritdoc/>
        public IDictionary<string, IOpenApiLink>? Links
        {
            get => _links?.Value;
            set => _links = value is null ? null : new(() => value);
        }

        private Lazy<IDictionary<string, IOpenApiExtension>>? _extensions = new(() => new Dictionary<string, IOpenApiExtension>(StringComparer.Ordinal));
        /// <inheritdoc/>
        public IDictionary<string, IOpenApiExtension>? Extensions
        {
            get => _extensions?.Value;
            set => _extensions = value is null ? null : new(() => value);
        }

        /// <summary>
        /// Parameterless constructor
        /// </summary>
        public OpenApiResponse() { }

        /// <summary>
        /// Initializes a copy of <see cref="IOpenApiResponse"/> object
        /// </summary>
        internal OpenApiResponse(IOpenApiResponse response)
        {
            Utils.CheckArgumentNull(response);
            Description = response.Description ?? Description;
            Headers = response.Headers != null ? new Dictionary<string, IOpenApiHeader>(response.Headers) : null;
            Content = response.Content != null ? new Dictionary<string, OpenApiMediaType>(response.Content) : null;
            Links = response.Links != null ? new Dictionary<string, IOpenApiLink>(response.Links) : null;
            Extensions = response.Extensions != null ? new Dictionary<string, IOpenApiExtension>(response.Extensions) : null;
        }

        /// <summary>
        /// Serialize <see cref="OpenApiResponse"/> to Open Api v3.1
        /// </summary>
        public void SerializeAsV31(IOpenApiWriter writer)
        {
            SerializeInternal(writer, OpenApiSpecVersion.OpenApi3_1, (writer, element) => element.SerializeAsV31(writer));
        }

        /// <summary>
        /// Serialize <see cref="OpenApiResponse"/> to Open Api v3.0.
        /// </summary>
        public void SerializeAsV3(IOpenApiWriter writer)
        {
            SerializeInternal(writer, OpenApiSpecVersion.OpenApi3_0, (writer, element) => element.SerializeAsV3(writer));
        }

        private void SerializeInternal(IOpenApiWriter writer, OpenApiSpecVersion version, 
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
        public void SerializeAsV2(IOpenApiWriter writer)
        {
            Utils.CheckArgumentNull(writer);

            writer.WriteStartObject();

            // description
            writer.WriteRequiredProperty(OpenApiConstants.Description, Description);

            var extensionsClone = Extensions is not null ? new Dictionary<string, IOpenApiExtension>(Extensions) : null;

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
                            .Select(static x => x.Value.Examples)
                            .OfType<IDictionary<string, IOpenApiExample>>()
                            .SelectMany(static x => x))
                        {
                            writer.WritePropertyName(example.Key);
                            example.Value.SerializeAsV2(writer);
                        }

                        writer.WriteEndObject();
                    }

                    writer.WriteExtensions(mediatype.Value.Extensions, OpenApiSpecVersion.OpenApi2_0);

                    if (mediatype.Value.Extensions is not null)
                    {
                        foreach (var key in mediatype.Value.Extensions.Keys)
                        {
                            // The extension will already have been serialized as part of the call above,
                            // so remove it from the cloned collection so we don't write it again.
                            extensionsClone?.Remove(key);
                        }
                    }                    
                }
            }

            // headers
            writer.WriteOptionalMap(OpenApiConstants.Headers, Headers, (w, h) => h.SerializeAsV2(w));

            // extension
            writer.WriteExtensions(extensionsClone, OpenApiSpecVersion.OpenApi2_0);

            writer.WriteEndObject();
        }

        /// <inheritdoc/>
        public IOpenApiResponse CreateShallowCopy()
        {
            return new OpenApiResponse(this);
        }
    }
}
