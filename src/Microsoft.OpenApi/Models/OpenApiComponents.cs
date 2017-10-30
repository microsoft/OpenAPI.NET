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

        /// <summary>
        /// Serialize <see cref="OpenApiComponents"/> to Open Api v3.0
        /// </summary>
        internal override void WriteAsV3(IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            writer.WriteStartObject();
            writer.WriteMap("schemas", Schemas, (w, s) => s.WriteAsV3(w));
            writer.WriteMap("responses", Responses, (w, r) => r.WriteAsV3(w));
            writer.WriteMap("parameters", Parameters, (w, p) => p.WriteAsV3(w));
            writer.WriteMap("examples", Examples, (w, e) => e.WriteAsV3(w));
            writer.WriteMap("requestBodies", RequestBodies, (w, r) => r.WriteAsV3(w));
            writer.WriteMap("headers", Headers, (w, h) => h.WriteAsV3(w));
            writer.WriteMap("securitySchemes", SecuritySchemes, (w, s) => s.WriteAsV3(w));
            writer.WriteMap("links", Links, (w, link) => link.WriteAsV3(w));
            writer.WriteMap("callbacks", Callbacks, (w, c) => c.WriteAsV3(w));
            writer.WriteEndObject();
        }

        /// <summary>
        /// Serialize <see cref="OpenApiComponents"/> to Open Api v2.0
        /// </summary>
        internal override void WriteAsV2(IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            writer.WriteStartObject();
            writer.WriteMap("definitions", Schemas, (w, s) => s.WriteAsV2(w));
            writer.WriteMap("responses", Responses, (w, r) => r.WriteAsV2(w));
            writer.WriteMap("parameters", Parameters, (w, p) => p.WriteAsV2(w));
            writer.WriteMap("securityDefinitions", SecuritySchemes, (w, s) => s.WriteAsV2(w));
            writer.WriteEndObject();
        }
    }
}
