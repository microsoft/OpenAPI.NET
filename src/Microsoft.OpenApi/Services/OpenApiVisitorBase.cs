// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Services
{
    /// <summary>
    /// Open API visitor base providing base validation logic for each <see cref="OpenApiElement"/>
    /// </summary>
    public abstract class OpenApiVisitorBase
    {
        /// <summary>
        /// Validates <see cref="OpenApiDocument"/>
        /// </summary>
        public virtual void Visit(OpenApiDocument doc) { }

        /// <summary>
        /// Validates <see cref="OpenApiInfo"/>
        /// </summary>
        public virtual void Visit(OpenApiInfo info) { }

        /// <summary>
        /// Validates list of <see cref="OpenApiServer"/>
        /// </summary>
        public virtual void Visit(IList<OpenApiServer> servers) { }

        /// <summary>
        /// Validates <see cref="OpenApiServer"/>
        /// </summary>
        public virtual void Visit(OpenApiServer server) { }

        /// <summary>
        /// Validates <see cref="OpenApiPaths"/>
        /// </summary>
        public virtual void Visit(OpenApiPaths paths) { }

        /// <summary>
        /// Validates <see cref="OpenApiPathItem"/>
        /// </summary>
        public virtual void Visit(OpenApiPathItem pathItem) { }

        /// <summary>
        /// Validates <see cref="OpenApiServerVariable"/>
        /// </summary>
        public virtual void Visit(OpenApiServerVariable serverVariable) { }

        /// <summary>
        /// Validates the operations.
        /// </summary>
        public virtual void Visit(IDictionary<OperationType,OpenApiOperation> operations) { }

        /// <summary>
        /// Validates <see cref="OpenApiOperation"/>
        /// </summary>
        public virtual void Visit(OpenApiOperation operation) { }

        /// <summary>
        /// Validates list of <see cref="OpenApiParameter"/>
        /// </summary>
        public virtual void Visit(IList<OpenApiParameter> parameters) { }

        /// <summary>
        /// Validates <see cref="OpenApiParameter"/>
        /// </summary>
        public virtual void Visit(OpenApiParameter parameter) { }

        /// <summary>
        /// Validates <see cref="OpenApiRequestBody"/>
        /// </summary>
        public virtual void Visit(OpenApiRequestBody requestBody) { }

        /// <summary>
        /// Validates responses.
        /// </summary>
        public virtual void Visit(IDictionary<string, OpenApiResponse> responses) { }

        /// <summary>
        /// Validates <see cref="OpenApiResponse"/>
        /// </summary>
        public virtual void Visit(OpenApiResponse response) { }

        /// <summary>
        /// Validates media type content.
        /// </summary>
        public virtual void Visit(IDictionary<string,OpenApiMediaType> content) { }

        /// <summary>
        /// Validates <see cref="OpenApiMediaType"/>
        /// </summary>
        public virtual void Visit(OpenApiMediaType mediaType) { }

        /// <summary>
        /// Validates the examples.
        /// </summary>
        public virtual void Visit(IDictionary<string, OpenApiExample> examples) { }

        /// <summary>
        /// Validates <see cref="OpenApiSchema"/>
        /// </summary>
        public virtual void Visit(OpenApiSchema schema) { }

        /// <summary>
        /// Validates the links.
        /// </summary>
        public virtual void Visit(IDictionary<string, OpenApiLink> links) { }

        /// <summary>
        /// Validates <see cref="OpenApiLink"/>
        /// </summary>
        public virtual void Visit(OpenApiLink link) { }
    }
}
