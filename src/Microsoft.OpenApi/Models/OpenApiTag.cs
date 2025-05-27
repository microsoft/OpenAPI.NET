// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;

namespace Microsoft.OpenApi
{
    /// <summary>
    /// Tag Object.
    /// </summary>
    public class OpenApiTag : IOpenApiExtensible, IOpenApiTag, IOpenApiDescribedElement
    {
        /// <inheritdoc/>
        public string? Name { get; set; }

        /// <inheritdoc/>
        public string? Description { get; set; }

        /// <inheritdoc/>
        public OpenApiExternalDocs? ExternalDocs { get; set; }

        /// <inheritdoc/>
        public Dictionary<string, IOpenApiExtension>? Extensions { get; set; }

        /// <summary>
        /// Parameterless constructor
        /// </summary>
        public OpenApiTag() { }

        /// <summary>
        /// Initializes a copy of an <see cref="IOpenApiTag"/> object
        /// </summary>
        internal OpenApiTag(IOpenApiTag tag)
        {
            Utils.CheckArgumentNull(tag);
            Name = tag.Name ?? Name;
            Description = tag.Description ?? Description;
            ExternalDocs = tag.ExternalDocs != null ? new(tag.ExternalDocs) : null;
            Extensions = tag.Extensions != null ? new Dictionary<string, IOpenApiExtension>(tag.Extensions) : null;
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

        internal void SerializeInternal(IOpenApiWriter writer, OpenApiSpecVersion version, 
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

        /// <inheritdoc/>
        public IOpenApiTag CreateShallowCopy()
        {
            return new OpenApiTag(this);
        }
    }
}
