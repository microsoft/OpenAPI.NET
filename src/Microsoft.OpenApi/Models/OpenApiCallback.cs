// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using Microsoft.OpenApi.Expressions;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Models
{
    /// <summary>
    /// Callback Object: A map of possible out-of band callbacks related to the parent operation.
    /// </summary>
    public class OpenApiCallback : IOpenApiReferenceable, IOpenApiExtensible, IEffective<OpenApiCallback>
    {
        /// <summary>
        /// A Path Item Object used to define a callback request and expected responses.
        /// </summary>
        public Dictionary<RuntimeExpression, OpenApiPathItem> PathItems { get; set; }
            = new();

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
        public IDictionary<string, IOpenApiExtension> Extensions { get; set; } = new Dictionary<string, IOpenApiExtension>();

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

            if (PathItems == null)
            {
                PathItems = new();
            }

            PathItems.Add(expression, pathItem);
        }

        /// <summary>
        /// Serialize <see cref="OpenApiCallback"/> to Open Api v3.0
        /// </summary>
        public void SerializeAsV3(IOpenApiWriter writer)
        {
            Utils.CheckArgumentNull(writer);

            var target = this;

            if (Reference != null)
            {
                if (!writer.GetSettings().ShouldInlineReference(Reference))
                {
                    Reference.SerializeAsV3(writer);
                    return;
                }
                else
                {
                    target = GetEffective(Reference.HostDocument);
                }
            }
            target.SerializeAsV3WithoutReference(writer);
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
        /// Serialize to OpenAPI V3 document without using reference.
        /// </summary>

        public void SerializeAsV3WithoutReference(IOpenApiWriter writer)
        {
            writer.WriteStartObject();

            // path items
            foreach (var item in PathItems)
            {
                writer.WriteRequiredObject(item.Key.Expression, item.Value, (w, p) => p.SerializeAsV3(w));
            }

            // extensions
            writer.WriteExtensions(Extensions, OpenApiSpecVersion.OpenApi3_0);

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
