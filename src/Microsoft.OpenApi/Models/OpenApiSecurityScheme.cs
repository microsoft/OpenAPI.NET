// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Models
{
    /// <summary>
    /// Security Scheme Object.
    /// </summary>
    public class OpenApiSecurityScheme : OpenApiElement, IOpenApiReference, IOpenApiExtension
    {
        /// <summary>
        /// REQUIRED. The type of the security scheme.
        /// </summary>
        public SecuritySchemeTypeKind Type { get; set; }

        /// <summary>
        /// A short description for security scheme.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// REQUIRED. The name of the header, query or cookie parameter to be used.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// REQUIRED. The location of the API key
        /// </summary>
        public ParameterLocation In { get; set; }

        /// <summary>
        /// REQUIRED. The name of the HTTP Authorization scheme to be used.
        /// </summary>
        public string Scheme { get; set; }

        /// <summary>
        /// A hint to the client to identify how the bearer token is formatted.
        /// </summary>
        public string BearerFormat { get; set; }

        /// <summary>
        /// REQUIRED. An object containing configuration information for the flow types supported.
        /// </summary>
        public OpenApiOAuthFlows Flows { get; set; }

        /// <summary>
        /// REQUIRED. OpenId Connect URL to discover OAuth2 configuration values.
        /// </summary>
        public Uri OpenIdConnectUrl { get; set; }

        /// <summary>
        /// Specification Extensions.
        /// </summary>
        public IDictionary<string, IOpenApiAny> Extensions { get; set; }

        public OpenApiReference Pointer
        {
            get; set;
        }

        /// <summary>
        /// Serialize <see cref="OpenApiSecurityScheme"/> to Open Api v3.0
        /// </summary>
        internal override void WriteAsV3(IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            writer.WriteStartObject();
            writer.WriteStringProperty("type", Type.ToString());
            switch (Type)
            {
                case SecuritySchemeTypeKind.http:
                    writer.WriteStringProperty("scheme", Scheme);
                    writer.WriteStringProperty("bearerFormat", BearerFormat);
                    break;
                case SecuritySchemeTypeKind.oauth2:
                //writer.WriteStringProperty("scheme", this.Scheme);
                //TODO:
                case SecuritySchemeTypeKind.apiKey:
                    writer.WriteStringProperty("in", In.ToString());
                    writer.WriteStringProperty("name", Name);

                    break;
            }

            writer.WriteObject("flows", Flows, (w, o) => o.WriteAsV3(w));

            writer.WriteStringProperty("openIdConnectUrl", OpenIdConnectUrl?.ToString());

            writer.WriteEndObject();
        }

        /// <summary>
        /// Serialize <see cref="OpenApiSecurityScheme"/> to Open Api v2.0
        /// </summary>
        internal override void WriteAsV2(IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            writer.WriteStartObject();
            if (Type == SecuritySchemeTypeKind.http)
            {
                if (Scheme == "basic")
                {
                    writer.WriteStringProperty("type", "basic");
                }
            }
            else
            {
                writer.WriteStringProperty("type", Type.ToString());
            }
            switch (Type)
            {
                case SecuritySchemeTypeKind.oauth2:
                //writer.WriteStringProperty("scheme", this.Scheme);
                //TODO:
                case SecuritySchemeTypeKind.apiKey:
                    writer.WriteStringProperty("in", In.ToString());
                    writer.WriteStringProperty("name", Name);

                    break;
            }

            if (Flows != null)
            {
                if (Flows.Implicit != null)
                {
                    writer.WriteStringProperty("flow", "implicit");
                    Flows.Implicit.WriteAsV2(writer);
                }
                else if (Flows.Password != null)
                {
                    writer.WriteStringProperty("flow", "password");
                    Flows.Password.WriteAsV2(writer);
                }
                else if (Flows.ClientCredentials != null)
                {
                    writer.WriteStringProperty("flow", "application");
                    Flows.ClientCredentials.WriteAsV2(writer);
                }
                else if (Flows.AuthorizationCode != null)
                {
                    writer.WriteStringProperty("flow", "accessCode");
                    Flows.AuthorizationCode.WriteAsV2(writer);
                }
            }
            writer.WriteEndObject();
        }
    }
}
