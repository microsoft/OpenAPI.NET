//---------------------------------------------------------------------
// <copyright file="OpenApiOperation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Microsoft.OpenApi
{
    /// <summary>
    /// Operation Object.
    /// </summary>
    public class OpenApiOperation
    {
        public List<OpenApiTag> Tags { get; set; } = new List<OpenApiTag>();
        public string Summary { get; set; }
        public string Description { get; set; }
        public OpenApiExternalDocs ExternalDocs { get; set; } 
        public string OperationId { get; set; }
        public List<OpenApiParameter> Parameters { get; set; } = new List<OpenApiParameter>();
        public OpenApiRequestBody RequestBody { get; set; }
        public Dictionary<string, OpenApiResponse> Responses { get; set; } = new Dictionary<string, OpenApiResponse>();
        public Dictionary<string, OpenApiCallback> Callbacks { get; set; } = new Dictionary<string, OpenApiCallback>();

        public const bool DeprecatedDefault = false;
        public bool Deprecated { get; set; } = DeprecatedDefault;
        public List<OpenApiSecurityRequirement> Security { get; set; } = new List<OpenApiSecurityRequirement>();
        public List<OpenApiServer> Servers { get; set; } = new List<OpenApiServer>();
        public Dictionary<string, IOpenApiExtension> Extensions { get; set; } = new Dictionary<string, IOpenApiExtension>();

        public void CreateResponse(string key, Action<OpenApiResponse> configure)
        {
            var response = new OpenApiResponse();
            configure(response);
            Responses.Add(key, response);
        }
    }
}
