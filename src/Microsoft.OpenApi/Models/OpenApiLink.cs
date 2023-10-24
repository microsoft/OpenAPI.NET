// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Models
{
    /// <summary>
    /// Link Object.
    /// </summary>
    public class OpenApiLink : IOpenApiReferenceable, IOpenApiExtensible, IEffective<OpenApiLink>
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
            new();

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

        /// <summary>
        /// Parameterless constructor
        /// </summary>
        public OpenApiLink() {}

        /// <summary>
        /// Initializes a copy of an <see cref="OpenApiLink"/> object
        /// </summary>
        public OpenApiLink(OpenApiLink link)
        {
            OperationRef = link?.OperationRef ?? OperationRef;
            OperationId = link?.OperationId ?? OperationId;
            Parameters = link?.Parameters != null ? new(link?.Parameters) : null;
            RequestBody = link?.RequestBody != null ? new(link?.RequestBody) : null;
            Description = link?.Description ?? Description;
            Server = link?.Server != null ? new(link?.Server) : null;
            Extensions = link?.Extensions != null ? new Dictionary<string, IOpenApiExtension>(link.Extensions) : null;
            UnresolvedReference = link?.UnresolvedReference ?? UnresolvedReference;
            Reference = link?.Reference != null ? new(link?.Reference) : null;
        }

        /// <summary>
        /// Serialize <see cref="OpenApiLink"/> to Open Api v3.0
        /// </summary>
        public void SerializeAsV3(IOpenApiWriter writer)
        {
            Utils.CheckArgumentNull(writer);

            var target = this;

            if (Reference != null)
            {
                if (!writer.GetSettings().ShouldInlineReference(Reference))
                {
                    Reference.SerializeAsV3(writer);
                    return;
                }
                else
                {
                    target = GetEffective(Reference.HostDocument);
                }
            }
            target.SerializeAsV3WithoutReference(writer);
        }

        /// <summary>
        /// Returns an effective OpenApiLink object based on the presence of a $ref
        /// </summary>
        /// <param name="doc">The host OpenApiDocument that contains the reference.</param>
        /// <returns>OpenApiLink</returns>
        public OpenApiLink GetEffective(OpenApiDocument doc)
        {
            if (this.Reference != null)
            {
                return doc.ResolveReferenceTo<OpenApiLink>(this.Reference);
            }
            else
            {
                return this;
            }
        }

        /// <summary>
        /// Serialize to OpenAPI V3 document without using reference.
        /// </summary>
        public void SerializeAsV3WithoutReference(IOpenApiWriter writer)
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
            writer.WriteOptionalObject(OpenApiConstants.Server, Server, (w, s) => s.SerializeAsV3(w));

            // specification extensions
            writer.WriteExtensions(Extensions, OpenApiSpecVersion.OpenApi3_0);

            writer.WriteEndObject();
        }

        /// <summary>
        /// Serialize <see cref="OpenApiLink"/> to Open Api v2.0
        /// </summary>
        public void SerializeAsV2(IOpenApiWriter writer)
        {
            // Link object does not exist in V2.
        }

        /// <summary>
        /// Serialize to OpenAPI V2 document without using reference.
        /// </summary>
        public void SerializeAsV2WithoutReference(IOpenApiWriter writer)
        {
            // Link object does not exist in V2.
        }
    }
}
