// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Writers;
using static Microsoft.OpenApi.Extensions.OpenApiSerializableExtensions;

namespace Microsoft.OpenApi.Models
{
    /// <summary>
    /// Path Item Object: to describe the operations available on a single path.
    /// </summary>
    public class OpenApiPathItem : Dictionary<OperationType, OpenApiOperation>, IOpenApiSerializable, IOpenApiExtensible, IOpenApiReferenceable, IEffective<OpenApiPathItem>
    {
        /// <summary>
        /// An optional, string summary, intended to apply to all operations in this path.
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// An optional, string description, intended to apply to all operations in this path.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets the definition of operations on this path.
        /// </summary>
        public IDictionary<OperationType, OpenApiOperation> Operations { get; set; }

        /// <summary>
        /// An alternative server array to service all operations in this path.
        /// </summary>
        public IList<OpenApiServer> Servers { get; set; }

        /// <summary>
        /// A list of parameters that are applicable for all the operations described under this path.
        /// These parameters can be overridden at the operation level, but cannot be removed there.
        /// </summary>
        public IList<OpenApiParameter> Parameters { get; set; }

        /// <summary>
        /// This object MAY be extended with Specification Extensions.
        /// </summary>
        public IDictionary<string, IOpenApiExtension> Extensions { get; set; }

        /// <summary>
        /// Indicates if object is populated with data or is just a reference to the data
        /// </summary>
        [JsonIgnore]
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
        public OpenApiPathItem() {}

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
        public void SerializeAsV31(IOpenApiWriter writer)
        {
            SerializeInternal(writer, (writer, element) => element.SerializeAsV31(writer), 
                (writer, element) => element.SerializeAsV31WithoutReference(writer));
        }

        /// <summary>
        /// Serialize <see cref="OpenApiPathItem"/> to Open Api v3.0
        /// </summary>
        public void SerializeAsV3(IOpenApiWriter writer)
        {
            SerializeInternal(writer, (writer, element) => element.SerializeAsV3(writer), 
                (writer, element) => element.SerializeAsV3WithoutReference(writer));
        }
        
        /// <summary>
        /// Serialize <see cref="OpenApiPathItem"/> to Open Api v3.0
        /// </summary>
        private void SerializeInternal(IOpenApiWriter writer, Action<IOpenApiWriter, IOpenApiSerializable> callback,
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
        /// Returns an effective OpenApiPathItem object based on the presence of a $ref 
        /// </summary>
        /// <param name="doc">The host OpenApiDocument that contains the reference.</param>
        /// <returns>OpenApiPathItem</returns>
        public OpenApiPathItem GetEffective(OpenApiDocument doc)
        {
            if (this.Reference != null)
            {
                return doc.ResolveReferenceTo<OpenApiPathItem>(this.Reference);
            }
            else
            {
                return this;
            }
        }

        /// <summary>
        /// Serialize <see cref="OpenApiPathItem"/> to Open Api v2.0
        /// </summary>
        public void SerializeAsV2(IOpenApiWriter writer)
        {
            writer = writer ?? throw Error.ArgumentNull(nameof(writer));

            var target = this;

            if (Reference != null)
            {
                if (!writer.GetSettings().ShouldInlineReference(Reference))
                {
                    Reference.SerializeAsV2(writer);
                    return;
                } 
                else
                {
                    target = this.GetEffective(Reference.HostDocument);
                }
            }

            target.SerializeAsV2WithoutReference(writer);
        }

        /// <summary>
        /// Serialize inline PathItem in OpenAPI V2
        /// </summary>
        /// <param name="writer"></param>
        public void SerializeAsV2WithoutReference(IOpenApiWriter writer)
        {
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
        
        /// <summary>
        /// Serialize inline PathItem in OpenAPI V31
        /// </summary>
        /// <param name="writer"></param>
        public void SerializeAsV31WithoutReference(IOpenApiWriter writer)
        {
            SerializeInternalWithoutReference(writer, OpenApiSpecVersion.OpenApi3_1, (writer, element) => element.SerializeAsV31(writer));
        }

        /// <summary>
        /// Serialize inline PathItem in OpenAPI V3
        /// </summary>
        /// <param name="writer"></param>
        public void SerializeAsV3WithoutReference(IOpenApiWriter writer)
        {
            SerializeInternalWithoutReference(writer, OpenApiSpecVersion.OpenApi3_0, (writer, element) => element.SerializeAsV3(writer));
            
        }

        private void SerializeInternalWithoutReference(IOpenApiWriter writer, OpenApiSpecVersion version,
            Action<IOpenApiWriter, IOpenApiSerializable> callback)
        {

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
