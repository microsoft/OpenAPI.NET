// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Collections.Generic;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Models
{
    /// <summary>
    /// Link Object.
    /// </summary>
    public class OpenApiLink : IOpenApiReferenceable, IOpenApiExtensible
    {
        /// <summary>
        /// A relative or absolute reference to an OAS operation.
        /// This field is mutually exclusive of the operationId field, and MUST point to an Operation Object.
        /// </summary>
        public string OperationRef { get; set; }

        /// <summary>
        /// The name of an existing, resolvable OAS operation, as defined with a unique operationId.
        /// This field is mutually exclusive of the operationRef field.
        /// </summary>
        public string OperationId { get; set; }

        /// <summary>
        /// A map representing parameters to pass to an operation as specified with operationId or identified via operationRef.
        /// </summary>
        public Dictionary<string, RuntimeExpressionAnyWrapper> Parameters { get; set; } =
            new Dictionary<string, RuntimeExpressionAnyWrapper>();

        /// <summary>
        /// A literal value or {expression} to use as a request body when calling the target operation.
        /// </summary>
        public RuntimeExpressionAnyWrapper RequestBody { get; set; }

        /// <summary>
        /// A description of the link.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// A server object to be used by the target operation.
        /// </summary>
        public OpenApiServer Server { get; set; }

        /// <summary>
        /// This object MAY be extended with Specification Extensions.
        /// </summary>
        public IDictionary<string, IOpenApiExtension> Extensions { get; set; } = new Dictionary<string, IOpenApiExtension>();

        /// <summary>
        /// Indicates if object is populated with data or is just a reference to the data
        /// </summary>
        public bool UnresolvedReference { get; set; }

        /// <summary>
        /// Reference pointer.
        /// </summary>
        public OpenApiReference Reference { get; set; }
    }
}
