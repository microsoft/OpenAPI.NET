﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Collections.Generic;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Models
{
    /// <summary>
    /// Path Item Object: to describe the operations available on a single path.
    /// </summary>
    public class OpenApiPathItem : IOpenApiSerializable, IOpenApiExtensible
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
            = new Dictionary<OperationType, OpenApiOperation>();

        /// <summary>
        /// An alternative server array to service all operations in this path.
        /// </summary>
        public IList<OpenApiServer> Servers { get; set; } = new List<OpenApiServer>();

        /// <summary>
        /// A list of parameters that are applicable for all the operations described under this path.
        /// These parameters can be overridden at the operation level, but cannot be removed there.
        /// </summary>
        public IList<OpenApiParameter> Parameters { get; set; } = new List<OpenApiParameter>();

        /// <summary>
        /// This object MAY be extended with Specification Extensions.
        /// </summary>
        public IDictionary<string, IOpenApiAny> Extensions { get; set; } = new Dictionary<string, IOpenApiAny>();

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
        /// Serialize <see cref="OpenApiPathItem"/> to Open Api v3.0
        /// </summary>
        public void SerializeAsV3(IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

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
                    (w, o) => o.SerializeAsV3(w));
            }

            // servers
            writer.WriteOptionalCollection(OpenApiConstants.Servers, Servers, (w, s) => s.SerializeAsV3(w));

            // parameters
            writer.WriteOptionalCollection(OpenApiConstants.Parameters, Parameters, (w, p) => p.SerializeAsV3(w));

            // specification extensions
            writer.WriteExtensions(Extensions);

            writer.WriteEndObject();
        }

        /// <summary>
        /// Serialize <see cref="OpenApiPathItem"/> to Open Api v2.0
        /// </summary>
        public void SerializeAsV2(IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

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
            writer.WriteExtensions(Extensions);

            writer.WriteEndObject();
        }
    }
}