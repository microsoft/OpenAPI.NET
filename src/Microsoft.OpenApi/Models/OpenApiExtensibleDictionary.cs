// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Models
{
    /// <summary>
    /// Generic dictionary type for Open API dictionary element.
    /// </summary>
    /// <typeparam name="T">The Open API element, <see cref="IOpenApiElement"/></typeparam>
    public abstract class OpenApiExtensibleDictionary<T> : Dictionary<string, T>,
        IOpenApiSerializable,
        IOpenApiExtensible
        where T : IOpenApiSerializable
    {
        /// <summary>
        /// Parameterless constructor
        /// </summary>
        protected OpenApiExtensibleDictionary() { }

        /// <summary>
        /// Initializes a copy of <see cref="OpenApiExtensibleDictionary{T}"/> class.
        /// </summary>
        /// <param name="dictionary">The generic dictionary.</param>
        /// <param name="extensions">The dictionary of <see cref="IOpenApiExtension"/>.</param>
        protected OpenApiExtensibleDictionary(
            Dictionary<string, T> dictionary = null,
            IDictionary<string, IOpenApiExtension> extensions = null) : base (dictionary)
        {
            Extensions = extensions != null ? new Dictionary<string, IOpenApiExtension>(extensions) : null;
        }

        /// <summary>
        /// This object MAY be extended with Specification Extensions.
        /// </summary>
        public IDictionary<string, IOpenApiExtension> Extensions { get; set; } = new Dictionary<string, IOpenApiExtension>();

        /// <summary>
        /// Serialize to Open Api v3.0
        /// </summary>
        public void SerializeAsV3(IOpenApiWriter writer)
        {
            Utils.CheckArgumentNull(writer);

            writer.WriteStartObject();

            foreach (var item in this)
            {
                writer.WriteRequiredObject(item.Key, item.Value, (w, p) => p.SerializeAsV3(w));
            }

            writer.WriteExtensions(Extensions, OpenApiSpecVersion.OpenApi3_0);

            writer.WriteEndObject();
        }

        /// <summary>
        /// Serialize to Open Api v2.0
        /// </summary>
        public void SerializeAsV2(IOpenApiWriter writer)
        {
            Utils.CheckArgumentNull(writer);

            writer.WriteStartObject();

            foreach (var item in this)
            {
                writer.WriteRequiredObject(item.Key, item.Value, (w, p) => p.SerializeAsV2(w));
            }

            writer.WriteExtensions(Extensions, OpenApiSpecVersion.OpenApi2_0);

            writer.WriteEndObject();
        }
    }
}
