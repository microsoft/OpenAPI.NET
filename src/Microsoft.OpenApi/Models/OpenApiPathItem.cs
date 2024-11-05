// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Models
{
    /// <summary>
    /// Path Item Object: to describe the operations available on a single path.
    /// </summary>
    public class OpenApiPathItem : IOpenApiExtensible, IOpenApiReferenceable
    {
        /// <summary>
        /// An optional, string summary, intended to apply to all operations in this path.
        /// </summary>
        public virtual string Summary { get; set; }

        /// <summary>
        /// An optional, string description, intended to apply to all operations in this path.
        /// </summary>
        public virtual string Description { get; set; }

        /// <summary>
        /// Gets the definition of operations on this path.
        /// </summary>
        public virtual IDictionary<OperationType, OpenApiOperation> Operations { get; set; }
            = new Dictionary<OperationType, OpenApiOperation>();

        /// <summary>
        /// An alternative server array to service all operations in this path.
        /// </summary>
        public virtual IList<OpenApiServer> Servers { get; set; } = new List<OpenApiServer>();

        /// <summary>
        /// A list of parameters that are applicable for all the operations described under this path.
        /// These parameters can be overridden at the operation level, but cannot be removed there.
        /// </summary>
        public virtual IList<OpenApiParameter> Parameters { get; set; } = new List<OpenApiParameter>();

        /// <summary>
        /// This object MAY be extended with Specification Extensions.
        /// </summary>
        public virtual IDictionary<string, IOpenApiExtension> Extensions { get; set; } = new Dictionary<string, IOpenApiExtension>();

        /// <summary>
        /// Indicates if object is populated with data or is just a reference to the data
        /// </summary>
        public bool UnresolvedReference { get; set; }

        /// <summary>
        /// Reference object.
        /// </summary>
        public OpenApiReference Reference { get; set; }

        /// <summary>
        /// Add one operation into this path item.
        /// </summary>
        /// <param name="operationType">The operation type kind.</param>
        /// <param name="operation">The operation item.</param>
        public void AddOperation(OperationType operationType, OpenApiOperation operation)
        {
            Operations[operationType] = operation;
        }

        /// <summary>
        /// Parameterless constructor
        /// </summary>
        public OpenApiPathItem() { }

        /// <summary>
        /// Initializes a clone of an <see cref="OpenApiPathItem"/> object
        /// </summary>
        public OpenApiPathItem(OpenApiPathItem pathItem)
        {
            Summary = pathItem?.Summary ?? Summary;
            Description = pathItem?.Description ?? Description;
            Operations = pathItem?.Operations != null ? new Dictionary<OperationType, OpenApiOperation>(pathItem.Operations) : null;
            Servers = pathItem?.Servers != null ? new List<OpenApiServer>(pathItem.Servers) : null;
            Parameters = pathItem?.Parameters != null ? new List<OpenApiParameter>(pathItem.Parameters) : null;
            Extensions = pathItem?.Extensions != null ? new Dictionary<string, IOpenApiExtension>(pathItem.Extensions) : null;
            UnresolvedReference = pathItem?.UnresolvedReference ?? UnresolvedReference;
            Reference = pathItem?.Reference != null ? new(pathItem?.Reference) : null;
        }

        /// <summary>
        /// Serialize <see cref="OpenApiPathItem"/> to Open Api v3.1
        /// </summary>
        public virtual void SerializeAsV31(IOpenApiWriter writer)
        {
            SerializeInternal(writer, OpenApiSpecVersion.OpenApi3_1, (writer, element) => element.SerializeAsV31(writer));
        }

        /// <summary>
        /// Serialize <see cref="OpenApiPathItem"/> to Open Api v3.0
        /// </summary>
        public virtual void SerializeAsV3(IOpenApiWriter writer)
        {
            SerializeInternal(writer, OpenApiSpecVersion.OpenApi3_0, (writer, element) => element.SerializeAsV3(writer));
        }

        /// <summary>
        /// Serialize inline PathItem in OpenAPI V2
        /// </summary>
        /// <param name="writer"></param>
        public void SerializeAsV2(IOpenApiWriter writer)
        {
            Utils.CheckArgumentNull(writer);

            writer.WriteStartObject();

            // operations except "trace"
            foreach (var operation in Operations)
            {
                if (operation.Key != OperationType.Trace)
                {
                    writer.WriteOptionalObject(
                        operation.Key.GetDisplayName(),
                        operation.Value,
                        (w, o) => o.SerializeAsV2(w));
                }
            }

            // parameters
            writer.WriteOptionalCollection(OpenApiConstants.Parameters, Parameters, (w, p) => p.SerializeAsV2(w));

            // write "summary" as extensions
            writer.WriteProperty(OpenApiConstants.ExtensionFieldNamePrefix + OpenApiConstants.Summary, Summary);

            // write "description" as extensions
            writer.WriteProperty(
                OpenApiConstants.ExtensionFieldNamePrefix + OpenApiConstants.Description,
                Description);

            // specification extensions
            writer.WriteExtensions(Extensions, OpenApiSpecVersion.OpenApi2_0);

            writer.WriteEndObject();
        }

        internal virtual void SerializeInternal(IOpenApiWriter writer, OpenApiSpecVersion version,
            Action<IOpenApiWriter, IOpenApiSerializable> callback)
        {
            Utils.CheckArgumentNull(writer);

            writer.WriteStartObject();

            // summary
            writer.WriteProperty(OpenApiConstants.Summary, Summary);

            // description
            writer.WriteProperty(OpenApiConstants.Description, Description);

            // operations
            foreach (var operation in Operations)
            {
                writer.WriteOptionalObject(
                    operation.Key.GetDisplayName(),
                    operation.Value,
                    callback);
            }

            // servers
            writer.WriteOptionalCollection(OpenApiConstants.Servers, Servers, callback);

            // parameters
            writer.WriteOptionalCollection(OpenApiConstants.Parameters, Parameters, callback);

            // specification extensions
            writer.WriteExtensions(Extensions, version);

            writer.WriteEndObject();
        }
    }
}
