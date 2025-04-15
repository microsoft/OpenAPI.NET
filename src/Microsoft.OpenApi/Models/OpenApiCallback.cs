// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Expressions;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models.Interfaces;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Models
{
    /// <summary>
    /// Callback Object: A map of possible out-of band callbacks related to the parent operation.
    /// </summary>
    public class OpenApiCallback : IOpenApiExtensible, IOpenApiCallback
    {
        /// <inheritdoc/>
        public Dictionary<RuntimeExpression, IOpenApiPathItem>? PathItems { get; set; }


        /// <summary>
        /// This object MAY be extended with Specification Extensions.
        /// </summary>
        public OpenApiExtensionDictionary? Extensions { get; set; }

        /// <summary>
        /// Parameter-less constructor
        /// </summary>
        public OpenApiCallback() { }

        /// <summary>
        /// Initializes a copy of an <see cref="OpenApiCallback"/> object
        /// </summary>
        internal OpenApiCallback(IOpenApiCallback callback)
        {
            Utils.CheckArgumentNull(callback);
            PathItems = callback?.PathItems != null ? new(callback.PathItems) : null;
            Extensions = callback?.Extensions != null ? new Dictionary<string, IOpenApiExtension>(callback.Extensions) : null;
        }

        /// <summary>
        /// Add a <see cref="IOpenApiPathItem"/> into the <see cref="PathItems"/>.
        /// </summary>
        /// <param name="expression">The runtime expression.</param>
        /// <param name="pathItem">The path item.</param>
        public void AddPathItem(RuntimeExpression expression, IOpenApiPathItem pathItem)
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
        public void SerializeAsV31(IOpenApiWriter writer)
        {
            SerializeInternal(writer, OpenApiSpecVersion.OpenApi3_1, (writer, element) => element.SerializeAsV31(writer));
        }

        /// <summary>
        /// Serialize <see cref="OpenApiCallback"/> to Open Api v3.0
        /// </summary>
        public void SerializeAsV3(IOpenApiWriter writer)
        {
            SerializeInternal(writer, OpenApiSpecVersion.OpenApi3_0, (writer, element) => element.SerializeAsV3(writer));
        }

        internal void SerializeInternal(IOpenApiWriter writer, OpenApiSpecVersion version,
            Action<IOpenApiWriter, IOpenApiSerializable> callback)
        {
            Utils.CheckArgumentNull(writer);

            writer.WriteStartObject();

            // path items
            if (PathItems != null)
            {
                foreach (var item in PathItems)
                {
                    writer.WriteRequiredObject(item.Key.Expression, item.Value, callback);
                }
            }

            // extensions
            writer.WriteExtensions(Extensions, version);
            Extensions!["x-extensions"] = new JsonObject(); //this works

            writer.WriteEndObject();
        }

        /// <summary>
        /// Serialize <see cref="OpenApiCallback"/> to Open Api v2.0
        /// </summary>
        public void SerializeAsV2(IOpenApiWriter writer)
        {
            // Callback object does not exist in V2.
        }

        /// <inheritdoc/>
        public IOpenApiCallback CreateShallowCopy()
        {
            return new OpenApiCallback(this);
        }
    }
}
