﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json.Nodes;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Models.Interfaces;
using Microsoft.OpenApi.Models.References;

namespace Microsoft.OpenApi.Services
{
    /// <summary>
    /// Open API visitor base provides common logic for concrete visitors
    /// </summary>
    public abstract class OpenApiVisitorBase
    {
        private readonly Stack<string> _path = new();

        /// <summary>
        /// Properties available to identify context of where an object is within OpenAPI Document
        /// </summary>
        public CurrentKeys CurrentKeys { get; } = new();

        /// <summary>
        /// Allow Rule to indicate validation error occured at a deeper context level.
        /// </summary>
        /// <param name="segment">Identifier for context</param>
        public virtual void Enter(string segment)
        {
            this._path.Push(segment);
        }

        /// <summary>
        /// Exit from path context level.  Enter and Exit calls should be matched.
        /// </summary>
        public virtual void Exit()
        {
            this._path.Pop();
        }

        /// <summary>
        /// Pointer to source of validation error in document
        /// </summary>
        public string PathString { get => "#/" + String.Join("/", _path.Reverse()); }

        /// <summary>
        /// Visits <see cref="OpenApiDocument"/>
        /// </summary>
        public virtual void Visit(OpenApiDocument doc)
        {
        }

        /// <summary>
        /// Visits <see cref="JsonNode"/>
        /// </summary>
        /// <param name="node"></param>
        public virtual void Visit(JsonNode node)
        {
        }

        /// <summary>
        /// Visits <see cref="OpenApiInfo"/>
        /// </summary>
        public virtual void Visit(OpenApiInfo info)
        {
        }

        /// <summary>
        /// Visits <see cref="OpenApiContact"/>
        /// </summary>
        public virtual void Visit(OpenApiContact contact)
        {
        }

        /// <summary>
        /// Visits <see cref="OpenApiLicense"/>
        /// </summary>
        public virtual void Visit(OpenApiLicense license)
        {
        }

        /// <summary>
        /// Visits list of <see cref="OpenApiServer"/>
        /// </summary>
        public virtual void Visit(IList<OpenApiServer> servers)
        {
        }

        /// <summary>
        /// Visits <see cref="OpenApiServer"/>
        /// </summary>
        public virtual void Visit(OpenApiServer server)
        {
        }

        /// <summary>
        /// Visits <see cref="OpenApiPaths"/>
        /// </summary>
        public virtual void Visit(OpenApiPaths paths)
        {
        }

        /// <summary>
        /// Visits Webhooks>
        /// </summary>
        public virtual void Visit(IDictionary<string, IOpenApiPathItem> webhooks)
        {
        }

        /// <summary>
        /// Visits <see cref="OpenApiPathItem"/>
        /// </summary>
        public virtual void Visit(IOpenApiPathItem pathItem)
        {
        }

        /// <summary>
        /// Visits <see cref="OpenApiServerVariable"/>
        /// </summary>
        public virtual void Visit(OpenApiServerVariable serverVariable)
        {
        }

        /// <summary>
        /// Visits the operations.
        /// </summary>
        public virtual void Visit(IDictionary<HttpMethod, OpenApiOperation> operations)
        {
        }

        /// <summary>
        /// Visits <see cref="OpenApiOperation"/>
        /// </summary>
        public virtual void Visit(OpenApiOperation operation)
        {
        }

        /// <summary>
        /// Visits list of <see cref="OpenApiParameter"/>
        /// </summary>
        public virtual void Visit(IList<IOpenApiParameter> parameters)
        {
        }

        /// <summary>
        /// Visits <see cref="OpenApiParameter"/>
        /// </summary>
        public virtual void Visit(IOpenApiParameter parameter)
        {
        }

        /// <summary>
        /// Visits <see cref="IOpenApiRequestBody"/>
        /// </summary>
        public virtual void Visit(IOpenApiRequestBody requestBody)
        {
        }

        /// <summary>
        /// Visits headers.
        /// </summary>
        public virtual void Visit(IDictionary<string, IOpenApiHeader> headers)
        {
        }

        /// <summary>
        /// Visits callbacks.
        /// </summary>
        public virtual void Visit(IDictionary<string, IOpenApiCallback> callbacks)
        {
        }

        /// <summary>
        /// Visits <see cref="OpenApiResponse"/>
        /// </summary>
        public virtual void Visit(IOpenApiResponse response)
        {
        }

        /// <summary>
        /// Visits <see cref="OpenApiResponses"/>
        /// </summary>
        public virtual void Visit(OpenApiResponses response)
        {
        }

        /// <summary>
        /// Visits media type content.
        /// </summary>
        public virtual void Visit(IDictionary<string, OpenApiMediaType> content)
        {
        }

        /// <summary>
        /// Visits <see cref="OpenApiMediaType"/>
        /// </summary>
        public virtual void Visit(OpenApiMediaType mediaType)
        {
        }

        /// <summary>
        /// Visits <see cref="OpenApiEncoding"/>
        /// </summary>
        public virtual void Visit(OpenApiEncoding encoding)
        {
        }

        /// <summary>
        /// Visits the examples.
        /// </summary>
        public virtual void Visit(IDictionary<string, IOpenApiExample> examples)
        {
        }

        /// <summary>
        /// Visits <see cref="OpenApiComponents"/>
        /// </summary>
        public virtual void Visit(OpenApiComponents components)
        {
        }

        /// <summary>
        /// Visits <see cref="OpenApiComponents"/>
        /// </summary>
        public virtual void Visit(OpenApiExternalDocs externalDocs)
        {
        }

        /// <summary>
        /// Visits <see cref="IOpenApiSchema"/>
        /// </summary>
        public virtual void Visit(IOpenApiSchema schema)
        {
        }

        /// <summary>
        /// Visits the links.
        /// </summary>
        public virtual void Visit(IDictionary<string, IOpenApiLink> links)
        {
        }

        /// <summary>
        /// Visits <see cref="IOpenApiLink"/>
        /// </summary>
        public virtual void Visit(IOpenApiLink link)
        {
        }

        /// <summary>
        /// Visits <see cref="IOpenApiCallback"/>
        /// </summary>
        public virtual void Visit(IOpenApiCallback callback)
        {
        }

        /// <summary>
        /// Visits <see cref="OpenApiTag"/>
        /// </summary>
        public virtual void Visit(OpenApiTag tag)
        {
        }

        /// <summary>
        /// Visits <see cref="OpenApiTagReference"/>
        /// </summary>
        public virtual void Visit(OpenApiTagReference tag)
        {
        }

        /// <summary>
        /// Visits <see cref="OpenApiHeader"/>
        /// </summary>
        public virtual void Visit(IOpenApiHeader header)
        {
        }

        /// <summary>
        /// Visits <see cref="OpenApiOAuthFlow"/>
        /// </summary>
        public virtual void Visit(OpenApiOAuthFlow openApiOAuthFlow)
        {
        }

        /// <summary>
        /// Visits <see cref="OpenApiSecurityRequirement"/>
        /// </summary>
        public virtual void Visit(OpenApiSecurityRequirement securityRequirement)
        {
        }

        /// <summary>
        /// Visits <see cref="IOpenApiSecurityScheme"/>
        /// </summary>
        public virtual void Visit(IOpenApiSecurityScheme securityScheme)
        {
        }

        /// <summary>
        /// Visits <see cref="IOpenApiExample"/>
        /// </summary>
        public virtual void Visit(IOpenApiExample example)
        {
        }

        /// <summary>
        /// Visits list of <see cref="OpenApiTag"/>
        /// </summary>
        public virtual void Visit(ISet<OpenApiTag> openApiTags)
        {
        }

        /// <summary>
        /// Visits list of <see cref="OpenApiTagReference"/>
        /// </summary>
        public virtual void Visit(ISet<OpenApiTagReference> openApiTags)
        {
        }

        /// <summary>
        /// Visits list of <see cref="OpenApiSecurityRequirement"/>
        /// </summary>
        public virtual void Visit(IList<OpenApiSecurityRequirement> openApiSecurityRequirements)
        {
        }

        /// <summary>
        /// Visits <see cref="IOpenApiExtensible"/>
        /// </summary>
        public virtual void Visit(IOpenApiExtensible openApiExtensible)
        {
        }

        /// <summary>
        /// Visits <see cref="IOpenApiExtension"/>
        /// </summary>
        public virtual void Visit(IOpenApiExtension openApiExtension)
        {
        }

        /// <summary>
        /// Visits list of <see cref="IOpenApiExample"/>
        /// </summary>
        public virtual void Visit(IList<IOpenApiExample> example)
        {
        }

        /// <summary>
        /// Visits a dictionary of server variables
        /// </summary>
        public virtual void Visit(IDictionary<string, OpenApiServerVariable> serverVariables)
        {
        }

        /// <summary>
        /// Visits a dictionary of encodings
        /// </summary>
        /// <param name="encodings"></param>
        public virtual void Visit(IDictionary<string, OpenApiEncoding> encodings)
        {
        }

        /// <summary>
        /// Visits IOpenApiReferenceable instances that are references and not in components
        /// </summary>
        /// <param name="referenceHolder">Referencing object</param>
        public virtual void Visit(IOpenApiReferenceHolder referenceHolder)
        {
        }
    }
}
