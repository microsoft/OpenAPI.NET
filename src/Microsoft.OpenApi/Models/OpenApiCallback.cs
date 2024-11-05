// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using Microsoft.OpenApi.Expressions;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Models
{
    /// <summary>
    /// Callback Object: A map of possible out-of band callbacks related to the parent operation.
    /// </summary>
    public class OpenApiCallback : IOpenApiReferenceable, IOpenApiExtensible
    {
        /// <summary>
        /// A Path Item Object used to define a callback request and expected responses.
        /// </summary>
        public virtual Dictionary<RuntimeExpression, OpenApiPathItem> PathItems { get; set; }
            = new();

        /// <summary>
        /// Indicates if object is populated with data or is just a reference to the data
        /// </summary>
        public virtual bool UnresolvedReference { get; set; }

        /// <summary>
        /// Reference pointer.
        /// </summary>
        public OpenApiReference Reference { get; set; }

        /// <summary>
        /// This object MAY be extended with Specification Extensions.
        /// </summary>
        public virtual IDictionary<string, IOpenApiExtension> Extensions { get; set; } = new Dictionary<string, IOpenApiExtension>();

        /// <summary>
        /// Parameter-less constructor
        /// </summary>
        public OpenApiCallback() { }

        /// <summary>
        /// Initializes a copy of an <see cref="OpenApiCallback"/> object
        /// </summary>
        public OpenApiCallback(OpenApiCallback callback)
        {
            PathItems = callback?.PathItems != null ? new(callback?.PathItems) : null;
            UnresolvedReference = callback?.UnresolvedReference ?? UnresolvedReference;
            Reference = callback?.Reference != null ? new(callback?.Reference) : null;
            Extensions = callback?.Extensions != null ? new Dictionary<string, IOpenApiExtension>(callback.Extensions) : null;
        }

        /// <summary>
        /// Add a <see cref="OpenApiPathItem"/> into the <see cref="PathItems"/>.
        /// </summary>
        /// <param name="expression">The runtime expression.</param>
        /// <param name="pathItem">The path item.</param>
        public void AddPathItem(RuntimeExpression expression, OpenApiPathItem pathItem)
        {
            Utils.CheckArgumentNull(expression);
            Utils.CheckArgumentNull(pathItem);

            PathItems ??= new();

            PathItems.Add(expression, pathItem);
        }

        /// <summary>
        /// Serialize <see cref="OpenApiCallback"/> to Open Api v3.1
        /// </summary>
        /// <param name="writer"></param>
        /// <exception cref="System.NotImplementedException"></exception>
        public virtual void SerializeAsV31(IOpenApiWriter writer)
        {
            SerializeInternal(writer, OpenApiSpecVersion.OpenApi3_1, (writer, element) => element.SerializeAsV31(writer));
        }

        /// <summary>
        /// Serialize <see cref="OpenApiCallback"/> to Open Api v3.0
        /// </summary>
        public virtual void SerializeAsV3(IOpenApiWriter writer)
        {
            SerializeInternal(writer, OpenApiSpecVersion.OpenApi3_0, (writer, element) => element.SerializeAsV3(writer));
        }

        internal void SerializeInternal(IOpenApiWriter writer, OpenApiSpecVersion version,
            Action<IOpenApiWriter, IOpenApiSerializable> callback)
        {
            Utils.CheckArgumentNull(writer);

            writer.WriteStartObject();

            // path items
            foreach (var item in PathItems)
            {
                writer.WriteRequiredObject(item.Key.Expression, item.Value, callback);
            }

            // extensions
            writer.WriteExtensions(Extensions, version);

            writer.WriteEndObject();
        }

        /// <summary>
        /// Serialize <see cref="OpenApiCallback"/> to Open Api v2.0
        /// </summary>
        public void SerializeAsV2(IOpenApiWriter writer)
        {
            // Callback object does not exist in V2.
        }
    }
}
