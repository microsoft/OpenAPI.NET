// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Models
{
    /// <summary>
    /// Tag Object.
    /// </summary>
    public class OpenApiTag : IOpenApiReferenceable, IOpenApiExtensible
    {
        /// <summary>
        /// The name of the tag.
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// A short description for the tag.
        /// </summary>
        public virtual string Description { get; set; }

        /// <summary>
        /// Additional external documentation for this tag.
        /// </summary>
        public virtual OpenApiExternalDocs ExternalDocs { get; set; }

        /// <summary>
        /// This object MAY be extended with Specification Extensions.
        /// </summary>
        public virtual IDictionary<string, IOpenApiExtension> Extensions { get; set; } = new Dictionary<string, IOpenApiExtension>();

        /// <summary>
        /// Indicates if object is populated with data or is just a reference to the data
        /// </summary>
        public bool UnresolvedReference { get; set; }

        /// <summary>
        /// Reference.
        /// </summary>
        public OpenApiReference Reference { get; set; }

        /// <summary>
        /// Parameterless constructor
        /// </summary>
        public OpenApiTag() { }

        /// <summary>
        /// Initializes a copy of an <see cref="OpenApiTag"/> object
        /// </summary>
        public OpenApiTag(OpenApiTag tag)
        {
            Name = tag?.Name ?? Name;
            Description = tag?.Description ?? Description;
            ExternalDocs = tag?.ExternalDocs != null ? new(tag.ExternalDocs) : null;
            Extensions = tag?.Extensions != null ? new Dictionary<string, IOpenApiExtension>(tag.Extensions) : null;
            UnresolvedReference = tag?.UnresolvedReference ?? UnresolvedReference;
            Reference = tag?.Reference != null ? new(tag.Reference) : null;
        }

        /// <summary>
        /// Serialize <see cref="OpenApiTag"/> to Open Api v3.1
        /// </summary>
        public virtual void SerializeAsV31(IOpenApiWriter writer) 
        {
            SerializeInternal(writer, OpenApiSpecVersion.OpenApi3_1,
                (writer, element) => element.SerializeAsV31(writer));
        }

        /// <summary>
        /// Serialize <see cref="OpenApiTag"/> to Open Api v3.0
        /// </summary>
        public virtual void SerializeAsV3(IOpenApiWriter writer) 
        {
            SerializeInternal(writer, OpenApiSpecVersion.OpenApi3_0,
                (writer, element) => element.SerializeAsV3(writer));
        }

        internal virtual void SerializeInternal(IOpenApiWriter writer, OpenApiSpecVersion version, 
            Action<IOpenApiWriter, IOpenApiSerializable> callback)
        {
            writer.WriteStartObject();

            // name
            writer.WriteProperty(OpenApiConstants.Name, Name);

            // description
            writer.WriteProperty(OpenApiConstants.Description, Description);

            // external docs
            writer.WriteOptionalObject(OpenApiConstants.ExternalDocs, ExternalDocs, callback);

            // extensions.
            writer.WriteExtensions(Extensions, version);

            writer.WriteEndObject();
        }

        /// <summary>
        /// Serialize <see cref="OpenApiTag"/> to Open Api v2.0
        /// </summary>
        public virtual void SerializeAsV2(IOpenApiWriter writer)
        {
            writer.WriteStartObject();

            // name
            writer.WriteProperty(OpenApiConstants.Name, Name);

            // description
            writer.WriteProperty(OpenApiConstants.Description, Description);

            // external docs
            writer.WriteOptionalObject(OpenApiConstants.ExternalDocs, ExternalDocs, (w, e) => e.SerializeAsV2(w));

            // extensions
            writer.WriteExtensions(Extensions, OpenApiSpecVersion.OpenApi2_0);

            writer.WriteEndObject();
        }
    }
}
