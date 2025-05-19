// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models.Interfaces;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Models
{
    /// <summary>
    /// Link Object.
    /// </summary>
    public class OpenApiLink : IOpenApiExtensible, IOpenApiLink
    {
        /// <inheritdoc/>
        public string? OperationRef { get; set; }

        /// <inheritdoc/>
        public string? OperationId { get; set; }

        /// <inheritdoc/>
        public IDictionary<string, RuntimeExpressionAnyWrapper>? Parameters { get; set; }

        /// <inheritdoc/>
        public RuntimeExpressionAnyWrapper? RequestBody { get; set; }

        /// <inheritdoc/>
        public string? Description { get; set; }

        /// <inheritdoc/>
        public OpenApiServer? Server { get; set; }

        /// <inheritdoc/>
        public IDictionary<string, IOpenApiExtension>? Extensions { get; set; }

        /// <summary>
        /// Parameterless constructor
        /// </summary>
        public OpenApiLink() { }

        /// <summary>
        /// Initializes a copy of an <see cref="OpenApiLink"/> object
        /// </summary>
        internal OpenApiLink(IOpenApiLink link)
        {
            Utils.CheckArgumentNull(link);
            OperationRef = link.OperationRef ?? OperationRef;
            OperationId = link.OperationId ?? OperationId;
            Parameters = link.Parameters != null ? new Dictionary<string, RuntimeExpressionAnyWrapper>(link.Parameters) : null;
            RequestBody = link.RequestBody != null ? new(link.RequestBody) : null;
            Description = link.Description ?? Description;
            Server = link.Server != null ? new(link.Server) : null;
            Extensions = link.Extensions != null ? new Dictionary<string, IOpenApiExtension>(link.Extensions) : null;
        }

        /// <inheritdoc/>
        public virtual void SerializeAsV31(IOpenApiWriter writer)
        {
            SerializeInternal(writer, (writer, element) => element.SerializeAsV31(writer));
        }

        /// <inheritdoc/>
        public virtual void SerializeAsV3(IOpenApiWriter writer)
        {
            SerializeInternal(writer, (writer, element) => element.SerializeAsV3(writer));
        }

        internal void SerializeInternal(IOpenApiWriter writer, Action<IOpenApiWriter, IOpenApiSerializable> callback)
        {
            Utils.CheckArgumentNull(writer);

            writer.WriteStartObject();

            // operationRef
            writer.WriteProperty(OpenApiConstants.OperationRef, OperationRef);

            // operationId
            writer.WriteProperty(OpenApiConstants.OperationId, OperationId);

            // parameters
            writer.WriteOptionalMap(OpenApiConstants.Parameters, Parameters, (w, p) => p.WriteValue(w));

            // requestBody
            writer.WriteOptionalObject(OpenApiConstants.RequestBody, RequestBody, (w, r) => r.WriteValue(w));

            // description
            writer.WriteProperty(OpenApiConstants.Description, Description);

            // server
            writer.WriteOptionalObject(OpenApiConstants.Server, Server, callback);

            // specification extensions
            writer.WriteExtensions(Extensions, OpenApiSpecVersion.OpenApi3_0);

            writer.WriteEndObject();
        }

        /// <inheritdoc/>
        public virtual void SerializeAsV2(IOpenApiWriter writer)
        {
            // Link object does not exist in V2.
        }

        /// <inheritdoc/>
        public IOpenApiLink CreateShallowCopy()
        {
            return new OpenApiLink(this);
        }
    }
}
