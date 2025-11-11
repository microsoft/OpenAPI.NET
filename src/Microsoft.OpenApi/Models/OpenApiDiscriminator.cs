// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;

namespace Microsoft.OpenApi
{
    /// <summary>
    /// Discriminator object.
    /// </summary>
    public class OpenApiDiscriminator : IOpenApiSerializable, IOpenApiExtensible
    {
        /// <summary>
        /// REQUIRED. The name of the property in the payload that will hold the discriminator value.
        /// </summary>
        public string? PropertyName { get; set; }

        /// <summary>
        /// An object to hold mappings between payload values and schema names or references.
        /// </summary>
        public IDictionary<string, OpenApiSchemaReference>? Mapping { get; set; }

        /// <summary>
        /// This object MAY be extended with Specification Extensions.
        /// </summary>
        public IDictionary<string, IOpenApiExtension>? Extensions { get; set; }

        /// <summary>
        /// OAI 3.2.0: The schema name or URI reference to a schema that is expected to validate the structure of the model when the discriminating property is not present in the payload or contains a value for which there is no explicit or implicit mapping.
        /// </summary>
        public OpenApiSchemaReference? DefaultMapping { get; set; }

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
            Mapping = discriminator?.Mapping != null ? new Dictionary<string, OpenApiSchemaReference>(discriminator.Mapping) : null;
            Extensions = discriminator?.Extensions != null ? new Dictionary<string, IOpenApiExtension>(discriminator.Extensions) : null;
            DefaultMapping = discriminator?.DefaultMapping;
        }

        /// <summary>
        /// Serialize <see cref="OpenApiDiscriminator"/> to Open Api v3.2
        /// </summary>
        /// <param name="writer"></param>
        public void SerializeAsV32(IOpenApiWriter writer)
        {
            SerializeInternal(writer, OpenApiSpecVersion.OpenApi3_2);

            // Write defaultMapping property in 3.2.0
            if (DefaultMapping != null)
            {
                writer.WritePropertyName("defaultMapping");
                DefaultMapping.SerializeAsV32(writer);
            }

            // extensions
            writer.WriteExtensions(Extensions, OpenApiSpecVersion.OpenApi3_2);

            writer.WriteEndObject();
        }

        /// <summary>
        /// Serialize <see cref="OpenApiDiscriminator"/> to Open Api v3.1
        /// </summary>
        /// <param name="writer"></param>
        public void SerializeAsV31(IOpenApiWriter writer)
        {
            SerializeInternal(writer, OpenApiSpecVersion.OpenApi3_1);

            // Write as x-oas-default-mapping extension in 3.1.0
            if (DefaultMapping != null)
            {
                writer.WritePropertyName("x-oas-default-mapping");
                DefaultMapping.SerializeAsV31(writer);
            }

            // extensions
            writer.WriteExtensions(Extensions, OpenApiSpecVersion.OpenApi3_1);

            writer.WriteEndObject();
        }

        /// <summary>
        /// Serialize <see cref="OpenApiDiscriminator"/> to Open Api v3.0
        /// </summary>
        public void SerializeAsV3(IOpenApiWriter writer)
        {
            SerializeInternal(writer, OpenApiSpecVersion.OpenApi3_0);

            writer.WriteEndObject();
        }

        private void SerializeInternal(IOpenApiWriter writer, OpenApiSpecVersion version)
        {
            Utils.CheckArgumentNull(writer);

            writer.WriteStartObject();

            // propertyName
            writer.WriteProperty(OpenApiConstants.PropertyName, PropertyName);

            // mapping
            writer.WriteOptionalMap(OpenApiConstants.Mapping, Mapping, (w, s) =>
            {
                if (!string.IsNullOrEmpty(s.Reference.ReferenceV3) && s.Reference.ReferenceV3 is not null)
                {
                    w.WriteValue(s.Reference.ReferenceV3);
                }
            });
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
