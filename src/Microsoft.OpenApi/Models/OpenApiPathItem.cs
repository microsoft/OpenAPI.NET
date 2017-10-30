// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Models
{
    /// <summary>
    /// Path Item Object: to describe the operations available on a single path.
    /// </summary>
    public class OpenApiPathItem : OpenApiElement, IOpenApiExtension
    {
        public string Summary { get; set; }
        public string Description { get; set; }

        public IDictionary<string, OpenApiOperation> Operations { get { return operations; } }
        private Dictionary<string, OpenApiOperation> operations = new Dictionary<string, OpenApiOperation>();

        public IList<OpenApiServer> Servers { get; set; } = new List<OpenApiServer>();
        public IList<OpenApiParameter> Parameters { get; set; } = new List<OpenApiParameter>();
        public IDictionary<string, IOpenApiAny> Extensions { get; set; }

        public void CreateOperation(OperationType operationType, Action<OpenApiOperation> configure)
        {
            var operation = new OpenApiOperation();
            configure(operation);
            AddOperation(operationType, operation);
        }

        public void AddOperation(OperationType operationType, OpenApiOperation operation)
        {
            var successResponse = operation.Responses.Keys.Where(k => k.StartsWith("2")).Any();
            if (!successResponse)
            {
             throw new OpenApiException("An operation requires a successful response");
            }

            this.operations.Add(operationType.GetOperationTypeName(), operation);
        }

        /// <summary>
        /// Serialize <see cref="OpenApiPathItem"/> to Open Api v3.0
        /// </summary>
        internal override void WriteAsV3(IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            writer.WriteStartObject();
            writer.WriteStringProperty("summary", Summary);
            writer.WriteStringProperty("description", Description);
            if (Parameters != null && Parameters.Count > 0)
            {
                writer.WritePropertyName("parameters");
                writer.WriteStartArray();
                foreach (var parameter in Parameters)
                {
                    parameter.WriteAsV3(writer);
                }
                writer.WriteEndArray();

            }
            writer.WriteList("servers", Servers, (w, s) => s.WriteAsV3(w));

            foreach (var operationPair in Operations)
            {
                writer.WritePropertyName(operationPair.Key);
                operationPair.Value.WriteAsV3(writer);
            }
            writer.WriteEndObject();
        }

        /// <summary>
        /// Serialize <see cref="OpenApiPathItem"/> to Open Api v2.0
        /// </summary>
        internal override void WriteAsV2(IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            writer.WriteStartObject();
            writer.WriteStringProperty("x-summary", Summary);
            writer.WriteStringProperty("x-description", Description);
            if (Parameters != null && Parameters.Count > 0)
            {
                writer.WritePropertyName("parameters");
                writer.WriteStartArray();
                foreach (var parameter in Parameters)
                {
                    parameter.WriteAsV2(writer);
                }
                writer.WriteEndArray();

            }
            //writer.WriteList("x-servers", Servers, WriteServer);

            foreach (var operationPair in Operations)
            {
                writer.WritePropertyName(operationPair.Key);
                operationPair.Value.WriteAsV2(writer);
            }
            writer.WriteEndObject();
        }
    }
}
