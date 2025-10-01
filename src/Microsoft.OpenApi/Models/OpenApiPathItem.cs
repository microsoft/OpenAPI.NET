// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Microsoft.OpenApi
{
    /// <summary>
    /// Path Item Object: to describe the operations available on a single path.
    /// </summary>
    public class OpenApiPathItem : IOpenApiExtensible, IOpenApiPathItem
    {
        /// <inheritdoc/>
        public string? Summary { get; set; }

        /// <inheritdoc/>
        public string? Description { get; set; }

        /// <inheritdoc/>
        public Dictionary<HttpMethod, OpenApiOperation>? Operations { get; set; }

        /// <inheritdoc/>
        public IList<OpenApiServer>? Servers { get; set; }

        /// <inheritdoc/>
        public IList<IOpenApiParameter>? Parameters { get; set; }

        /// <inheritdoc/>
        public OpenApiOperation? Query { get; set; }

        /// <inheritdoc/>
        public IDictionary<string, OpenApiOperation>? AdditionalOperations { get; set; }

        /// <inheritdoc/>
        public IDictionary<string, IOpenApiExtension>? Extensions { get; set; }

        /// <summary>
        /// Add one operation into this path item.
        /// </summary>
        /// <param name="operationType">The operation type kind.</param>
        /// <param name="operation">The operation item.</param>
        public void AddOperation(HttpMethod operationType, OpenApiOperation operation)
        {
            Operations ??= [];
            Operations[operationType] = operation;
        }

        /// <summary>
        /// Parameterless constructor
        /// </summary>
        public OpenApiPathItem() { }

        /// <summary>
        /// Initializes a clone of an <see cref="OpenApiPathItem"/> object
        /// </summary>
        internal OpenApiPathItem(IOpenApiPathItem pathItem)
        {
            Utils.CheckArgumentNull(pathItem);
            Summary = pathItem.Summary ?? Summary;
            Description = pathItem.Description ?? Description;
            Operations = pathItem.Operations != null ? new Dictionary<HttpMethod, OpenApiOperation>(pathItem.Operations) : null;
            Servers = pathItem.Servers != null ? [.. pathItem.Servers] : null;
            Parameters = pathItem.Parameters != null ? [.. pathItem.Parameters] : null;
            Query = pathItem.Query != null ? new OpenApiOperation(pathItem.Query) : null;
            AdditionalOperations = pathItem.AdditionalOperations != null ? new Dictionary<string, OpenApiOperation>(pathItem.AdditionalOperations) : null;
            Extensions = pathItem.Extensions != null ? new Dictionary<string, IOpenApiExtension>(pathItem.Extensions) : null;
        }

        /// <summary>
        /// Serialize <see cref="OpenApiPathItem"/> to Open Api v3.2
        /// </summary>
        public virtual void SerializeAsV32(IOpenApiWriter writer)
        {
            SerializeInternal(writer, OpenApiSpecVersion.OpenApi3_2, (writer, element) => element.SerializeAsV32(writer));
        }

        /// <summary>
        /// Serialize <see cref="OpenApiPathItem"/> to Open Api v3.1
        /// </summary>
        public virtual void SerializeAsV31(IOpenApiWriter writer)
        {
            SerializeInternal(writer, OpenApiSpecVersion.OpenApi3_1, (writer, element) => element.SerializeAsV31(writer), downgradeFrom32: true);
        }

        /// <summary>
        /// Serialize <see cref="OpenApiPathItem"/> to Open Api v3.0
        /// </summary>
        public virtual void SerializeAsV3(IOpenApiWriter writer)
        {
            SerializeInternal(writer, OpenApiSpecVersion.OpenApi3_0, (writer, element) => element.SerializeAsV3(writer), downgradeFrom32: true);
        }

        /// <summary>
        /// Serialize inline PathItem in OpenAPI V2
        /// </summary>
        /// <param name="writer"></param>
        public virtual void SerializeAsV2(IOpenApiWriter writer)
        {
            Utils.CheckArgumentNull(writer);

            writer.WriteStartObject();

            // operations except "trace"
            if (Operations != null)
            {
                foreach (var operation in Operations)
                {
                  if (operation.Key != HttpMethod.Trace)
                  {
                      writer.WriteOptionalObject(
                          operation.Key.Method.ToLowerInvariant(),
                          operation.Value,
                          (w, o) => o.SerializeAsV2(w));
                  }
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

            // Write Query and AdditionalOperations as extensions when downgrading to v2
            WriteV32FieldsAsExtensions(writer);

            // specification extensions
            writer.WriteExtensions(Extensions, OpenApiSpecVersion.OpenApi2_0);

            writer.WriteEndObject();
        }

        internal virtual void SerializeInternal(IOpenApiWriter writer, OpenApiSpecVersion version,
            Action<IOpenApiWriter, IOpenApiSerializable> callback, bool downgradeFrom32 = false)
        {
            Utils.CheckArgumentNull(writer);

            writer.WriteStartObject();

            // summary
            writer.WriteProperty(OpenApiConstants.Summary, Summary);

            // description
            writer.WriteProperty(OpenApiConstants.Description, Description);

            // operations
            if (Operations != null)
            {
                foreach (var operation in Operations)
                {
                    writer.WriteOptionalObject(
                    operation.Key.Method.ToLowerInvariant(),
                    operation.Value,
                    callback);
                }
            }

            // OpenAPI 3.2 specific fields
            if (version == OpenApiSpecVersion.OpenApi3_2)
            {
                // query operation
                writer.WriteOptionalObject(OpenApiConstants.Query, Query, callback);

                // additional operations
                writer.WriteOptionalMap(OpenApiConstants.AdditionalOperations, AdditionalOperations, callback);
            }
            else if (downgradeFrom32)
            {
                // When downgrading from 3.2 to 3.1/3.0, serialize as extensions
                WriteV32FieldsAsExtensions(writer);
            }

            // servers
            writer.WriteOptionalCollection(OpenApiConstants.Servers, Servers, callback);

            // parameters
            writer.WriteOptionalCollection(OpenApiConstants.Parameters, Parameters, callback);

            // specification extensions
            writer.WriteExtensions(Extensions, version);

            writer.WriteEndObject();
        }

        /// <summary>
        /// Writes OpenAPI 3.2 specific fields as extensions when downgrading to older versions
        /// </summary>
        private void WriteV32FieldsAsExtensions(IOpenApiWriter writer)
        {
            if (Query != null)
            {
                writer.WritePropertyName(OpenApiConstants.ExtensionFieldNamePrefix + "oas-" + OpenApiConstants.Query);
                Query.SerializeAsV31(writer);
            }

            if (AdditionalOperations != null && AdditionalOperations.Count > 0)
            {
                writer.WritePropertyName(OpenApiConstants.ExtensionFieldNamePrefix + "oas-" + OpenApiConstants.AdditionalOperations);
                writer.WriteStartObject();
                foreach (var kvp in AdditionalOperations)
                {
                    writer.WriteOptionalObject(kvp.Key, kvp.Value, (w, o) => o.SerializeAsV31(w));
                }
                writer.WriteEndObject();
            }
        }

        /// <inheritdoc/>
        public IOpenApiPathItem CreateShallowCopy()
        {
            return new OpenApiPathItem(this);
        }
    }
}
