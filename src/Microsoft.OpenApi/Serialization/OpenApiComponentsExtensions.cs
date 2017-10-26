// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Serialization
{
    /// <summary>
    /// Extensions method for <see cref="OpenApiComponents"/> serialization.
    /// </summary>
    internal static class OpenApiComponentsExtensions
    {
        /// <summary>
        /// Serialize <see cref="OpenApiComponents"/> to Open Api v3.0
        /// </summary>
        public static void SerializeV3(this OpenApiComponents components, IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            writer.WriteStartObject();
            if (components != null)
            {
                writer.WriteMap("schemas", components.Schemas, (w, s) => s.SerializeV3(w));
                writer.WriteMap("responses", components.Responses, (w, r) => r.SerializeV3(w));
                writer.WriteMap("parameters", components.Parameters, (w, p) => p.SerializeV3(w));
                writer.WriteMap("examples", components.Examples, (w, e) => e.SerializeV3(w));
                writer.WriteMap("requestBodies", components.RequestBodies, (w, r) => r.SerializeV3(w));
                writer.WriteMap("headers", components.Headers, (w, h) => h.SerializeV3(w));
                writer.WriteMap("securitySchemes", components.SecuritySchemes, (w, s) => s.SerializeV3(w));
                writer.WriteMap("links", components.Links, (w, link) => link.SerializeV3(w));
                writer.WriteMap("callbacks", components.Callbacks, (w, c) => c.SerializeV3(w));
            }
            writer.WriteEndObject();
        }

        /// <summary>
        /// Serialize <see cref="OpenApiComponents"/> to Open Api v2.0
        /// </summary>
        public static void SerializeV2(this OpenApiComponents components, IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            writer.WriteStartObject();
            if (components != null)
            {
                writer.WriteMap("definitions", components.Schemas, (w, s) => s.SerializeV2(w));
                writer.WriteMap("responses", components.Responses, (w, r) => r.SerializeV2(w));
                writer.WriteMap("parameters", components.Parameters, (w, p) => p.SerializeV2(w));
                writer.WriteMap("securityDefinitions", components.SecuritySchemes, (w, s) => s.SerializeV2(w));
            }
            writer.WriteEndObject();
        }
    }
}
