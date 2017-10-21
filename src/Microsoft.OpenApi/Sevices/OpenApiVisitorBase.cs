﻿//---------------------------------------------------------------------
// <copyright file="OpenApiVisitorBase.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;

namespace Microsoft.OpenApi
{
    public abstract class OpenApiVisitorBase
    {
        public virtual void Visit(OpenApiDocument doc) { }
        public virtual void Visit(OpenApiInfo info) { }
        public virtual void Visit(IList<OpenApiServer> servers) { }
        public virtual void Visit(OpenApiServer server) { }
        public virtual void Visit(OpenApiPaths paths) { }
        public virtual void Visit(OpenApiPathItem pathItem) { }
        public virtual void Visit(OpenApiServerVariable serverVariable) { }
        public virtual void Visit(IDictionary<string,OpenApiOperation> operations) { }
        public virtual void Visit(OpenApiOperation operation) { }
        public virtual void Visit(IList<OpenApiParameter> parameters) { }
        public virtual void Visit(OpenApiParameter parameter) { }
        public virtual void Visit(OpenApiRequestBody requestBody) { }
        public virtual void Visit(IDictionary<string, OpenApiResponse> responses) { }
        public virtual void Visit(OpenApiResponse response) { }
        public virtual void Visit(IDictionary<string,OpenApiMediaType> content) { }
        public virtual void Visit(OpenApiMediaType mediaType) { }
        public virtual void Visit(IDictionary<string, OpenApiExample> example) { }
        public virtual void Visit(OpenApiSchema example) { }
        public virtual void Visit(IDictionary<string, OpenApiLink> links) { }
        public virtual void Visit(OpenApiLink link) { }
    }
}
