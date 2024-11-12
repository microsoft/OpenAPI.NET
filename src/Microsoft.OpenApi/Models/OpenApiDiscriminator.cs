// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Models
{
    /// <summary>
    /// Discriminator object.
    /// </summary>
    public class OpenApiDiscriminator : IOpenApiSerializable, IOpenApiExtensible
    {
        /// <summary>
        /// REQUIRED. The name of the property in the payload that will hold the discriminator value.
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        /// An object to hold mappings between payload values and schema names or references.
        /// </summary>
        public IDictionary<string, string> Mapping { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// This object MAY be extended with Specification Extensions.
        /// </summary>
        public IDictionary<string, IOpenApiExtension> Extensions { get; set; } = new Dictionary<string, IOpenApiExtension>();

        /// <summary>
        /// Parameter-less constructor
        /// </summary>
        public OpenApiDiscriminator() { }

        /// <summary>
        /// Initializes a copy of an <see cref="OpenApiDiscriminator"/> instance
        /// </summary>
        public OpenApiDiscriminator(OpenApiDiscriminator discriminator)
        {
            PropertyName = discriminator?.PropertyName ?? PropertyName;
            Mapping = discriminator?.Mapping != null ? new Dictionary<string, string>(discriminator.Mapping) : null;
            Extensions = discriminator?.Extensions != null ? new Dictionary<string, IOpenApiExtension>(discriminator.Extensions) : null;
        }

        /// <summary>
        /// Serialize <see cref="OpenApiDiscriminator"/> to Open Api v3.1
        /// </summary>
        /// <param name="writer"></param>
        public void SerializeAsV31(IOpenApiWriter writer)
        {
            SerializeInternal(writer);

            // extensions
            writer.WriteExtensions(Extensions, OpenApiSpecVersion.OpenApi3_1);

            writer.WriteEndObject();
        }

        /// <summary>
        /// Serialize <see cref="OpenApiDiscriminator"/> to Open Api v3.0
        /// </summary>
        public void SerializeAsV3(IOpenApiWriter writer)
        {
            SerializeInternal(writer);

            writer.WriteEndObject();
        }

        /// <summary>
        /// Serialize <see cref="OpenApiDiscriminator"/> to Open Api v3.0
        /// </summary>
        /// <param name="writer"></param>
        private void SerializeInternal(IOpenApiWriter writer)
        {
            Utils.CheckArgumentNull(writer);

            writer.WriteStartObject();

            // propertyName
            writer.WriteProperty(OpenApiConstants.PropertyName, PropertyName);

            // mapping
            writer.WriteOptionalMap(OpenApiConstants.Mapping, Mapping, (w, s) => w.WriteValue(s));
        }

        /// <summary>
        /// Serialize <see cref="OpenApiDiscriminator"/> to Open Api v2.0
        /// </summary>
        public void SerializeAsV2(IOpenApiWriter writer)
        {
            // Discriminator object does not exist in V2.
        }
    }
}
