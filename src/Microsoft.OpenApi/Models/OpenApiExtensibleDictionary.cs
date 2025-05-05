// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Models
{
    /// <summary>
    /// Generic OrderedDictionary type for Open API OrderedDictionary element.
    /// </summary>
    /// <typeparam name="T">The Open API element, <see cref="IOpenApiElement"/></typeparam>
    public abstract class OpenApiExtensibleDictionary<T> : OrderedDictionary<string, T>,
        IOpenApiSerializable,
        IOpenApiExtensible
        where T : IOpenApiSerializable
    {
        /// <summary>
        /// Parameterless constructor
        /// </summary>
        protected OpenApiExtensibleDictionary():this([]) { }
        /// <summary>
        /// Initializes a copy of <see cref="OpenApiExtensibleDictionary{T}"/> class.
        /// </summary>
        /// <param name="OrderedDictionary">The generic OrderedDictionary.</param>
        /// <param name="extensions">The OrderedDictionary of <see cref="IOpenApiExtension"/>.</param>
        protected OpenApiExtensibleDictionary(
            OrderedDictionary<string, T> OrderedDictionary,
            OrderedDictionary<string, IOpenApiExtension>? extensions = null) : base(OrderedDictionary is null ? [] : OrderedDictionary)
        {
            Extensions = extensions != null ? new OrderedDictionary<string, IOpenApiExtension>(extensions) : [];
        }

        /// <summary>
        /// This object MAY be extended with Specification Extensions.
        /// </summary>
        public OrderedDictionary<string, IOpenApiExtension>? Extensions { get; set; }


        /// <summary>
        /// Serialize to Open Api v3.1
        /// </summary>
        /// <param name="writer"></param>
        public void SerializeAsV31(IOpenApiWriter writer)
        {
            SerializeInternal(writer, OpenApiSpecVersion.OpenApi3_1, (writer, element) => element.SerializeAsV31(writer));
        }

        /// <summary>
        /// Serialize to Open Api v3.0
        /// </summary>
        /// <param name="writer"></param>
        public void SerializeAsV3(IOpenApiWriter writer)
        {
            SerializeInternal(writer, OpenApiSpecVersion.OpenApi3_0, (writer, element) => element.SerializeAsV3(writer));
        }

        /// <summary>
        /// Serialize to Open Api v3.0
        /// </summary>
        private void SerializeInternal(IOpenApiWriter writer, OpenApiSpecVersion version,
            Action<IOpenApiWriter, IOpenApiSerializable> callback)
        {
            Utils.CheckArgumentNull(writer);

            writer.WriteStartObject();

            foreach (var item in this)
            {
                writer.WriteRequiredObject(item.Key, item.Value, callback);
            }

            writer.WriteExtensions(Extensions, version);

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
