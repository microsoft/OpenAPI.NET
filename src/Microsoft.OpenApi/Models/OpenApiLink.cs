// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Models
{
    /// <summary>
    /// Link Object.
    /// </summary>
    public class OpenApiLink : OpenApiElement, IOpenApiReference, IOpenApiExtension
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
        public Dictionary<string, RuntimeExpressionAnyWrapper> Parameters { get; set; }

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
        public IDictionary<string, IOpenApiAny> Extensions { get; set; }

        /// <summary>
        /// Reference pointer.
        /// </summary>
        public OpenApiReference Pointer { get; set; }

        /// <summary>
        /// Serialize <see cref="OpenApiLink"/> to Open Api v3.0
        /// </summary>
        internal override void WriteAsV3(IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            if (Pointer != null)
            {
                Pointer.WriteAsV3(writer);
            }
            else
            {
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
                writer.WriteOptionalObject(OpenApiConstants.Server, Server, (w, s) => s.WriteAsV3(w));

                writer.WriteEndObject();
            }
        }

        /// <summary>
        /// Serialize <see cref="OpenApiLink"/> to Open Api v2.0
        /// </summary>
        internal override void WriteAsV2(IOpenApiWriter writer)
        {
            // link object does not exist in V2.
        }
    }
}
