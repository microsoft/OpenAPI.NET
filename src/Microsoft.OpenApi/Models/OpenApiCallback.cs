﻿// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Expressions;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Models
{
    /// <summary>
    /// Callback Object: A map of possible out-of band callbacks related to the parent operation.
    /// </summary>
    public class OpenApiCallback : OpenApiElement, IOpenApiReference, IOpenApiExtension
    {
        /// <summary>
        /// A Path Item Object used to define a callback request and expected responses. 
        /// </summary>
        public Dictionary<RuntimeExpression, OpenApiPathItem> PathItems { get; set; }

        /// <summary>
        /// Reference pointer.
        /// </summary>
        public OpenApiReference Pointer { get; set; }

        /// <summary>
        /// This object MAY be extended with Specification Extensions.
        /// </summary>
        public IDictionary<string, IOpenApiAny> Extensions { get; set; }

        /// <summary>
        /// Add a <see cref="pathItem"/> into the <see cref="OpenApiCallback"/>.
        /// </summary>
        /// <param name="name">The runtime expression.</param>
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
        /// Serialize <see cref="OpenApiCallback"/> to Open Api v3.0
        /// </summary>
        internal override void WriteAsV3(IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            if (this.IsReference())
            {
                this.WriteRef(writer);
            }
            else
            {
                writer.WriteStartObject();

                // path items
                foreach (var item in PathItems)
                {
                    writer.WriteRequiredObject(item.Key.Expression, item.Value, (w, p) => p.WriteAsV3(w));
                }

                // extensions
                writer.WriteExtensions(Extensions);

                writer.WriteEndObject();
            }
        }

        /// <summary>
        /// Serialize <see cref="OpenApiCallback"/> to Open Api v2.0
        /// </summary>
        internal override void WriteAsV2(IOpenApiWriter writer)
        {
            // Callback object does not exist in V2.
        }
    }
}
