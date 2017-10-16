//---------------------------------------------------------------------
// <copyright file="OpenApiComponents.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;

namespace Microsoft.OpenApi
{
    /// <summary>
    /// Components Object.
    /// </summary>
    public class OpenApiComponents
    {
        public Dictionary<string, OpenApiSchema> Schemas { get; set; } = new Dictionary<string, OpenApiSchema>();
        public Dictionary<string, OpenApiResponse> Responses { get; set; } = new Dictionary<string, OpenApiResponse>();
        public Dictionary<string, OpenApiParameter> Parameters { get; set; } = new Dictionary<string, OpenApiParameter>();
        public Dictionary<string, OpenApiExample> Examples { get; set; } = new Dictionary<string, OpenApiExample>();
        public Dictionary<string, OpenApiRequestBody> RequestBodies { get; set; } = new Dictionary<string, OpenApiRequestBody>();
        public Dictionary<string, OpenApiHeader> Headers { get; set; } = new Dictionary<string, OpenApiHeader>();
        public Dictionary<string, OpenApiSecurityScheme> SecuritySchemes { get; set; } = new Dictionary<string, OpenApiSecurityScheme>();
        public Dictionary<string, OpenApiLink> Links { get; set; } = new Dictionary<string, OpenApiLink>();
        public Dictionary<string, OpenApiCallback> Callbacks { get; set; } = new Dictionary<string, OpenApiCallback>();
        public Dictionary<string, IOpenApiExtension> Extensions { get; set; } = new Dictionary<string, IOpenApiExtension>();

        public bool IsEmpty()
        {
            return !(this.Schemas.Count > 0
                || this.Responses.Count > 0
                || this.Parameters.Count > 0
                || this.Examples.Count > 0
                || this.RequestBodies.Count > 0
                || this.Headers.Count > 0 
                || this.SecuritySchemes.Count > 0
                || this.Links.Count > 0
                || this.Callbacks.Count > 0 
                || this.Extensions.Count > 0);

        }
    }
}
