// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;

namespace Microsoft.OpenApi.Models
{
    /// <summary>
    /// Path Item Object: to describe the operations available on a single path.
    /// </summary>
    public class OpenApiPathItem : IOpenApiExtension
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
    }
}
