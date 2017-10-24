//---------------------------------------------------------------------
// <copyright file="OpenApiPathItem.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Any;

namespace Microsoft.OpenApi
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
