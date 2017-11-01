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
        public IDictionary<string, IOpenApiAny> Extensions { get; set; }

        public bool IsEmpty()
        {
            return !(Schemas.Count > 0 ||
                Responses.Count > 0 ||
                Parameters.Count > 0 ||
                Examples.Count > 0 ||
                RequestBodies.Count > 0 ||
                Headers.Count > 0 ||
                SecuritySchemes.Count > 0 ||
                Links.Count > 0 ||
                Callbacks.Count > 0);
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

            writer.WriteMap("definitions", Schemas, (w, s) => s.WriteAsV2(w));
            writer.WriteMap("responses", Responses, (w, r) => r.WriteAsV2(w));
            writer.WriteMap("parameters", Parameters, (w, p) => p.WriteAsV2(w));
        }

        /// <summary>
        /// Serialize <see cref="SecuritySchemes"/> in <see cref="OpenApiComponents"/> to OpenApi V2
        /// </summary>
        internal void WriteSecurityDefinitionsV2(IOpenApiWriter writer)
        {
            writer.WriteMap("securityDefinitions", SecuritySchemes, (w, s) => s.WriteAsV2(w));
        }
    }
}