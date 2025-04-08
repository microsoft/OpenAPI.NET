// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Net.Http;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models.Interfaces;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Models
{
    /// <summary>
    /// Path Item Object: to describe the operations available on a single path.
    /// </summary>
    public class OpenApiPathItem : IOpenApiExtensible, IOpenApiReferenceable, IOpenApiPathItem
    {
        /// <inheritdoc/>
        public string? Summary { get; set; }

        /// <inheritdoc/>
        public string? Description { get; set; }

        /// <inheritdoc/>
        public IDictionary<HttpMethod, OpenApiOperation>? Operations { get; set; }
            = new Dictionary<HttpMethod, OpenApiOperation>();

        /// <inheritdoc/>
        public IList<OpenApiServer>? Servers { get; set; } = [];

        /// <inheritdoc/>
        public IList<IOpenApiParameter>? Parameters { get; set; } = [];

        /// <inheritdoc/>
        public IDictionary<string, IOpenApiExtension>? Extensions { get; set; } = new Dictionary<string, IOpenApiExtension>();

        /// <summary>
        /// Add one operation into this path item.
        /// </summary>
        /// <param name="operationType">The operation type kind.</param>
        /// <param name="operation">The operation item.</param>
        public void AddOperation(HttpMethod operationType, OpenApiOperation operation)
        {
            if (Operations is not null)
            {
                Operations[operationType] = operation;
            }
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
            Servers = pathItem.Servers != null ? new List<OpenApiServer>(pathItem.Servers) : null;
            Parameters = pathItem.Parameters != null ? new List<IOpenApiParameter>(pathItem.Parameters) : null;
            Extensions = pathItem.Extensions != null ? new Dictionary<string, IOpenApiExtension>(pathItem.Extensions) : null;
        }

        /// <summary>
        /// Serialize <see cref="OpenApiPathItem"/> to Open Api v3.1
        /// </summary>
        public void SerializeAsV31(IOpenApiWriter writer)
        {
            SerializeInternal(writer, OpenApiSpecVersion.OpenApi3_1, (writer, element) => element.SerializeAsV31(writer));
        }

        /// <summary>
        /// Serialize <see cref="OpenApiPathItem"/> to Open Api v3.0
        /// </summary>
        public void SerializeAsV3(IOpenApiWriter writer)
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

            // servers
            writer.WriteOptionalCollection(OpenApiConstants.Servers, Servers, callback);

            // parameters
            writer.WriteOptionalCollection(OpenApiConstants.Parameters, Parameters, callback);

            // specification extensions
            writer.WriteExtensions(Extensions, version);

            writer.WriteEndObject();
        }

        /// <inheritdoc/>
        public IOpenApiPathItem CreateShallowCopy()
        {
            return new OpenApiPathItem(this);
        }
    }
}
