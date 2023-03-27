// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;
using Microsoft.OpenApi.Expressions;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Writers;
using static Microsoft.OpenApi.Extensions.OpenApiSerializableExtensions;

namespace Microsoft.OpenApi.Models
{
    /// <summary>
    /// Callback Object: A map of possible out-of band callbacks related to the parent operation.
    /// </summary>
    public class OpenApiCallback : IOpenApiSerializable, IOpenApiReferenceable, IOpenApiExtensible, IEffective<OpenApiCallback>
    {
        /// <summary>
        /// A Path Item Object used to define a callback request and expected responses.
        /// </summary>
        public Dictionary<RuntimeExpression, OpenApiPathItem> PathItems { get; set; }

        /// <summary>
        /// Indicates if object is populated with data or is just a reference to the data
        /// </summary>
        public bool UnresolvedReference { get; set; }

        /// <summary>
        /// Reference pointer.
        /// </summary>
        public OpenApiReference Reference { get; set; }

        /// <summary>
        /// This object MAY be extended with Specification Extensions.
        /// </summary>
        public IDictionary<string, IOpenApiExtension> Extensions { get; set; }

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
            if (expression == null)
            {
                throw Error.ArgumentNull(nameof(expression));
            }

            if (pathItem == null)
            {
                throw Error.ArgumentNull(nameof(pathItem));
            }

            if (PathItems == null)
            {
                PathItems = new Dictionary<RuntimeExpression, OpenApiPathItem>();
            }

            PathItems.Add(expression, pathItem);
        }
        
        /// <summary>
        /// Serialize <see cref="OpenApiCallback"/> to Open Api v3.1
        /// </summary>
        /// <param name="writer"></param>
        /// <exception cref="System.NotImplementedException"></exception>
        public void SerializeAsV31(IOpenApiWriter writer)
        {
            SerializeInternal(writer, (writer, element) => element.SerializeAsV31(writer),
                (writer, referenceElement) => referenceElement.SerializeAsV31WithoutReference(writer));
        }
        
        /// <summary>
        /// Serialize <see cref="OpenApiCallback"/> to Open Api v3.0
        /// </summary>
        public void SerializeAsV3(IOpenApiWriter writer)
        {
            SerializeInternal(writer, (writer, element) => element.SerializeAsV3(writer), 
                (writer, referenceElement) => referenceElement.SerializeAsV3WithoutReference(writer));
        }

        /// <summary>
        /// Serialize <see cref="OpenApiCallback"/>
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="callback"></param>
        /// <param name="action"></param>
        private void SerializeInternal(IOpenApiWriter writer, 
            Action<IOpenApiWriter, IOpenApiSerializable> callback,
            Action<IOpenApiWriter, IOpenApiReferenceable> action)
        {
            writer = writer ?? throw Error.ArgumentNull(nameof(writer));

            var target = this;

            if (Reference != null)
            {
                if (!writer.GetSettings().ShouldInlineReference(Reference))
                {
                    callback(writer, Reference);
                    return;
                }
                else
                {
                    target = GetEffective(Reference.HostDocument);
                }
            }
            action(writer, target);
        }

        /// <summary>
        /// Returns an effective OpenApiCallback object based on the presence of a $ref 
        /// </summary>
        /// <param name="doc">The host OpenApiDocument that contains the reference.</param>
        /// <returns>OpenApiCallback</returns>
        public OpenApiCallback GetEffective(OpenApiDocument doc)
        {
            if (this.Reference != null)
            {
                return doc.ResolveReferenceTo<OpenApiCallback>(this.Reference);
            }
            else
            {
                return this;
            }
        }

        /// <summary>
        /// Serialize to OpenAPI V31 document without using reference.
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

        /// <summary>
        /// Serialize to OpenAPI V2 document without using reference.
        /// </summary>

        public void SerializeAsV2WithoutReference(IOpenApiWriter writer)
        {
            // Callback object does not exist in V2.
        }
    }
}
