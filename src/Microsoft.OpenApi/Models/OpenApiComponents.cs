// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Models
{
    /// <summary>
    /// Components Object.
    /// </summary>
    public class OpenApiComponents : IOpenApiExtensible
    {
        /// <summary>
        /// An object to hold reusable <see cref="OpenApiSchema"/> Objects.
        /// </summary>
        public IDictionary<string, OpenApiSchema> Schemas { get; set; } = new Dictionary<string, OpenApiSchema>();

        /// <summary>
        /// An object to hold reusable <see cref="OpenApiResponse"/> Objects.
        /// </summary>
        public IDictionary<string, OpenApiResponse> Responses { get; set; } = new Dictionary<string, OpenApiResponse>();

        /// <summary>
        /// An object to hold reusable <see cref="OpenApiParameter"/> Objects.
        /// </summary>
        public IDictionary<string, OpenApiParameter> Parameters { get; set; } =
            new Dictionary<string, OpenApiParameter>();

        /// <summary>
        /// An object to hold reusable <see cref="OpenApiExample"/> Objects.
        /// </summary>
        public IDictionary<string, OpenApiExample> Examples { get; set; } = new Dictionary<string, OpenApiExample>();

        /// <summary>
        /// An object to hold reusable <see cref="OpenApiRequestBody"/> Objects.
        /// </summary>
        public IDictionary<string, OpenApiRequestBody> RequestBodies { get; set; } =
            new Dictionary<string, OpenApiRequestBody>();

        /// <summary>
        /// An object to hold reusable <see cref="OpenApiHeader"/> Objects.
        /// </summary>
        public IDictionary<string, OpenApiHeader> Headers { get; set; } = new Dictionary<string, OpenApiHeader>();

        /// <summary>
        /// An object to hold reusable <see cref="OpenApiSecurityScheme"/> Objects.
        /// </summary>
        public IDictionary<string, OpenApiSecurityScheme> SecuritySchemes { get; set; } =
            new Dictionary<string, OpenApiSecurityScheme>();

        /// <summary>
        /// An object to hold reusable <see cref="OpenApiLink"/> Objects.
        /// </summary>
        public IDictionary<string, OpenApiLink> Links { get; set; } = new Dictionary<string, OpenApiLink>();

        /// <summary>
        /// An object to hold reusable <see cref="OpenApiCallback"/> Objects.
        /// </summary>
        public IDictionary<string, OpenApiCallback> Callbacks { get; set; } = new Dictionary<string, OpenApiCallback>();

        /// <summary>
        /// This object MAY be extended with Specification Extensions.
        /// </summary>
        public IDictionary<string, IOpenApiExtension> Extensions { get; set; } = new Dictionary<string, IOpenApiExtension>();
    }
}
