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
        public IDictionary<string, IOpenApiExtension>? Extensions { get; set; }

        /// <inheritdoc/>
        public string? Summary { get; set; }

        /// <inheritdoc/>
        public OpenApiTagReference? Parent { get; set; }

        /// <inheritdoc/>
        public string? Kind { get; set; }

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
            Summary = tag.Summary ?? Summary;
            Parent = tag.Parent ?? Parent;
            Kind = tag.Kind ?? Kind;
        }

        /// <summary>
        /// Serialize <see cref="OpenApiTag"/> to Open Api v3.2
        /// </summary>
        public virtual void SerializeAsV32(IOpenApiWriter writer)
        {
            SerializeInternal(writer, OpenApiSpecVersion.OpenApi3_2,
                (writer, element) => element.SerializeAsV32(writer));
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

            // summary - version specific handling
            if (Summary != null)
            {
                if (version == OpenApiSpecVersion.OpenApi3_2)
                {
                    writer.WriteProperty("summary", Summary);
                }
                else if (version == OpenApiSpecVersion.OpenApi3_1 || version == OpenApiSpecVersion.OpenApi3_0)
                {
                    writer.WriteProperty("x-oas-summary", Summary);
                }
            }

            // parent - version specific handling
            if (Parent != null)
            {
                if (version == OpenApiSpecVersion.OpenApi3_2)
                {
                    writer.WritePropertyName("parent");
                    Parent.SerializeAsV32(writer);
                }
                else if (version == OpenApiSpecVersion.OpenApi3_1)
                {
                    writer.WritePropertyName("x-oas-parent");
                    Parent.SerializeAsV31(writer);
                }
                else if (version == OpenApiSpecVersion.OpenApi3_0)
                {
                    writer.WritePropertyName("x-oas-parent");
                    Parent.SerializeAsV3(writer);
                }
            }

            // kind - version specific handling
            if (Kind != null)
            {
                if (version == OpenApiSpecVersion.OpenApi3_2)
                {
                    writer.WriteProperty("kind", Kind);
                }
                else if (version == OpenApiSpecVersion.OpenApi3_1 || version == OpenApiSpecVersion.OpenApi3_0)
                {
                    writer.WriteProperty("x-oas-kind", Kind);
                }
            }

            // extensions.
            writer.WriteExtensions(Extensions, version);

            writer.WriteEndObject();
        }

        /// <summary>
        /// Serialize <see cref="OpenApiTag"/> to Open Api v2.0
        /// </summary>
        public virtual void SerializeAsV2(IOpenApiWriter writer)
        {
            SerializeInternal(writer, OpenApiSpecVersion.OpenApi2_0,
               (writer, element) => element.SerializeAsV2(writer));
        }

        /// <inheritdoc/>
        public IOpenApiTag CreateShallowCopy()
        {
            return new OpenApiTag(this);
        }
    }
}
