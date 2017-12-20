// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Collections.Generic;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Services
{
    /// <summary>
    /// Open API visitor base providing base validation logic for each <see cref="IOpenApiElement"/>
    /// </summary>
    public abstract class OpenApiVisitorBase
    {
        /// <summary>
        /// Visits <see cref="OpenApiDocument"/>
        /// </summary>
        public virtual void Visit(OpenApiDocument doc)
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
        /// Visits <see cref="OpenApiPathItem"/>
        /// </summary>
        public virtual void Visit(OpenApiPathItem pathItem)
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
        public virtual void Visit(IDictionary<OperationType, OpenApiOperation> operations)
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
        public virtual void Visit(IList<OpenApiParameter> parameters)
        {
        }

        /// <summary>
        /// Visits <see cref="OpenApiParameter"/>
        /// </summary>
        public virtual void Visit(OpenApiParameter parameter)
        {
        }

        /// <summary>
        /// Visits <see cref="OpenApiRequestBody"/>
        /// </summary>
        public virtual void Visit(OpenApiRequestBody requestBody)
        {
        }

        /// <summary>
        /// Visits responses.
        /// </summary>
        public virtual void Visit(IDictionary<string, OpenApiResponse> responses)
        {
        }

        /// <summary>
        /// Visits <see cref="OpenApiResponse"/>
        /// </summary>
        public virtual void Visit(OpenApiResponse response)
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
        /// Visits the examples.
        /// </summary>
        public virtual void Visit(IDictionary<string, OpenApiExample> examples)
        {
        }

        /// <summary>
        /// Visits <see cref="OpenApiSchema"/>
        /// </summary>
        public virtual void Visit(OpenApiSchema schema)
        {
        }

        /// <summary>
        /// Visits the links.
        /// </summary>
        public virtual void Visit(IDictionary<string, OpenApiLink> links)
        {
        }

        /// <summary>
        /// Visits <see cref="OpenApiLink"/>
        /// </summary>
        public virtual void Visit(OpenApiLink link)
        {
        }
    }
}