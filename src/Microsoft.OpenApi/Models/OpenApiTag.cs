// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Writers;
using static Microsoft.OpenApi.Extensions.OpenApiSerializableExtensions;

namespace Microsoft.OpenApi.Models
{
    /// <summary>
    /// Tag Object.
    /// </summary>
    public class OpenApiTag : IOpenApiSerializable, IOpenApiReferenceable, IOpenApiExtensible
    {
        /// <summary>
        /// The name of the tag.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// A short description for the tag.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Additional external documentation for this tag.
        /// </summary>
        public OpenApiExternalDocs ExternalDocs { get; set; }

        /// <summary>
        /// This object MAY be extended with Specification Extensions.
        /// </summary>
        public IDictionary<string, IOpenApiExtension> Extensions { get; set; } = new Dictionary<string, IOpenApiExtension>();

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
            ExternalDocs = tag?.ExternalDocs != null ? new(tag?.ExternalDocs) : null;
            Extensions = tag?.Extensions != null ? new Dictionary<string, IOpenApiExtension>(tag.Extensions) : null;
            UnresolvedReference = tag?.UnresolvedReference ?? UnresolvedReference;
            Reference = tag?.Reference != null ? new(tag?.Reference) : null;
        }
        
        /// <summary>
        /// Serialize <see cref="OpenApiTag"/> to Open Api v3.1
        /// </summary>
        public void SerializeAsV31(IOpenApiWriter writer)
        {
            SerializeInternal(writer, (writer, element) => element.SerializeAsV31(writer));
        }
        
        /// <summary>
        /// Serialize <see cref="OpenApiTag"/> to Open Api v3.0
        /// </summary>
        public void SerializeAsV3(IOpenApiWriter writer)
        {
            SerializeInternal(writer, (writer, element) => element.SerializeAsV3(writer));
        }
        
        /// <summary>
        /// Serialize <see cref="OpenApiTag"/> to Open Api v3.0
        /// </summary>
        private void SerializeInternal(IOpenApiWriter writer, Action<IOpenApiWriter, IOpenApiSerializable> callback)
        {
            writer = writer ?? throw Error.ArgumentNull(nameof(writer));

            if (Reference != null)
            {
                callback(writer, Reference);
                return;
            }

            writer.WriteValue(Name);
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
        public void SerializeAsV2(IOpenApiWriter writer)
        {
            writer = writer ?? throw Error.ArgumentNull(nameof(writer));

            if (Reference != null)
            {
                Reference.SerializeAsV2(writer);
                return;
            }

            writer.WriteValue(Name);
        }

        /// <summary>
        /// Serialize to OpenAPI V2 document without using reference.
        /// </summary>
        public void SerializeAsV2WithoutReference(IOpenApiWriter writer)
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
