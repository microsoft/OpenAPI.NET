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
    /// Components Object.
    /// </summary>
    public class OpenApiComponents : OpenApiElement, IOpenApiExtension
    {
        /// <summary>
        /// An object to hold reusable <see cref="OpenApiSchema"/> Objects.
        /// </summary>
        public IDictionary<string, OpenApiSchema> Schemas { get; set; }

        /// <summary>
        /// An object to hold reusable <see cref="OpenApiResponse"/> Objects.
        /// </summary>
        public IDictionary<string, OpenApiResponse> Responses { get; set; }

        /// <summary>
        /// An object to hold reusable <see cref="OpenApiParameter"/> Objects.
        /// </summary>
        public IDictionary<string, OpenApiParameter> Parameters { get; set; }

        /// <summary>
        /// An object to hold reusable <see cref="OpenApiExample"/> Objects.
        /// </summary>
        public IDictionary<string, OpenApiExample> Examples { get; set; }

        /// <summary>
        /// An object to hold reusable <see cref="OpenApiRequestBody"/> Objects.
        /// </summary>
        public IDictionary<string, OpenApiRequestBody> RequestBodies { get; set; }

        /// <summary>
        /// An object to hold reusable <see cref="OpenApiHeader"/> Objects.
        /// </summary>
        public IDictionary<string, OpenApiHeader> Headers { get; set; }

        /// <summary>
        /// An object to hold reusable <see cref="OpenApiSecurityScheme"/> Objects.
        /// </summary>
        public IDictionary<string, OpenApiSecurityScheme> SecuritySchemes { get; set; }

        /// <summary>
        /// An object to hold reusable <see cref="OpenApiLink"/> Objects.
        /// </summary>
        public IDictionary<string, OpenApiLink> Links { get; set; }

        /// <summary>
        /// An object to hold reusable <see cref="OpenApiCallback"/> Objects.
        /// </summary>
        public IDictionary<string, OpenApiCallback> Callbacks { get; set; }

        /// <summary>
        /// This object MAY be extended with Specification Extensions.
        /// </summary>
        public IDictionary<string, IOpenApiAny> Extensions { get; set; }

        /// <summary>
        /// Serialize <see cref="OpenApiComponents"/> to Open Api v3.0.
        /// </summary>
        internal override void WriteAsV3(IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            writer.WriteStartObject();

            // schemas
            writer.WriteOptionalMap(OpenApiConstants.Schemas, Schemas, (w, s) => s.WriteAsV3(w));

            // responses
            writer.WriteOptionalMap(OpenApiConstants.Responses, Responses, (w, r) => r.WriteAsV3(w));

            // parameters
            writer.WriteOptionalMap(OpenApiConstants.Parameters, Parameters, (w, p) => p.WriteAsV3(w));

            // examples
            writer.WriteOptionalMap(OpenApiConstants.Examples, Examples, (w, e) => e.WriteAsV3(w));

            // requestBodies
            writer.WriteOptionalMap(OpenApiConstants.RequestBodies, RequestBodies, (w, r) => r.WriteAsV3(w));

            // headers
            writer.WriteOptionalMap(OpenApiConstants.Headers, Headers, (w, h) => h.WriteAsV3(w));

            // securitySchemes
            writer.WriteOptionalMap(OpenApiConstants.SecuritySchemes, SecuritySchemes, (w, s) => s.WriteAsV3(w));

            // links
            writer.WriteOptionalMap(OpenApiConstants.Links, Links, (w, link) => link.WriteAsV3(w));

            // callbacks
            writer.WriteOptionalMap(OpenApiConstants.Callbacks, Callbacks, (w, c) => c.WriteAsV3(w));

            // extensions
            writer.WriteExtensions(Extensions);

            writer.WriteEndObject();
        }

        /// <summary>
        /// Serialize <see cref="OpenApiComponents"/> to Open Api v2.0.
        /// </summary>
        internal override void WriteAsV2(IOpenApiWriter writer)
        {
            // Components object does not exist in V2.
        }
    }
}