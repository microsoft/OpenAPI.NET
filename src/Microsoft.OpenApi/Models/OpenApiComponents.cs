// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.OpenApi.Any;

namespace Microsoft.OpenApi.Models
{
    /// <summary>
    /// Components Object.
    /// </summary>
    public class OpenApiComponents : IOpenApiExtension
    {
        public IDictionary<string, OpenApiSchema> Schemas { get; set; } = new Dictionary<string, OpenApiSchema>();
        public IDictionary<string, OpenApiResponse> Responses { get; set; } = new Dictionary<string, OpenApiResponse>();
        public IDictionary<string, OpenApiParameter> Parameters { get; set; } = new Dictionary<string, OpenApiParameter>();
        public IDictionary<string, OpenApiExample> Examples { get; set; } = new Dictionary<string, OpenApiExample>();
        public IDictionary<string, OpenApiRequestBody> RequestBodies { get; set; } = new Dictionary<string, OpenApiRequestBody>();
        public IDictionary<string, OpenApiHeader> Headers { get; set; } = new Dictionary<string, OpenApiHeader>();
        public IDictionary<string, OpenApiSecurityScheme> SecuritySchemes { get; set; } = new Dictionary<string, OpenApiSecurityScheme>();
        public IDictionary<string, OpenApiLink> Links { get; set; } = new Dictionary<string, OpenApiLink>();
        public IDictionary<string, OpenApiCallback> Callbacks { get; set; } = new Dictionary<string, OpenApiCallback>();
        public IDictionary<string, IOpenApiAny> Extensions { get; set; }

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
                || this.Callbacks.Count > 0);
        }
    }
}
