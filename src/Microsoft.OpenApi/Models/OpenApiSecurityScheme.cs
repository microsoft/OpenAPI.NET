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
        public SecuritySchemeType Type { get; set; }

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

        /// <summary>
        /// Reference object.
        /// </summary>
        public OpenApiReference Pointer { get; set; }

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
            writer.WriteStringProperty(OpenApiConstants.OpenApiDocType, Type.ToString());
            writer.WriteStringProperty(OpenApiConstants.OpenApiDocDescription, Description);

            switch (Type)
            {
                case SecuritySchemeType.http:
                    writer.WriteStringProperty(OpenApiConstants.OpenApiDocScheme, Scheme);
                    writer.WriteStringProperty(OpenApiConstants.OpenApiDocBearerFormat, BearerFormat);
                    break;
                case SecuritySchemeType.oauth2:
                    writer.WriteObject(OpenApiConstants.OpenApiDocFlows, Flows, (w, o) => o.WriteAsV3(w));
                    break;
                case SecuritySchemeType.apiKey:
                    writer.WriteStringProperty(OpenApiConstants.OpenApiDocIn, In.ToString());
                    writer.WriteStringProperty(OpenApiConstants.OpenApiDocName, Name);
                    break;
                case SecuritySchemeType.openIdConnect:
                    writer.WriteStringProperty(OpenApiConstants.OpenApiDocOpenIdConnectUrl, OpenIdConnectUrl?.ToString());
                    break;
            }

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

            if (Type == SecuritySchemeType.http && Scheme != OpenApiConstants.OpenApiDocBasic)
            {
                // Bail because V2 does not support non-basic HTTP scheme
                writer.WriteStartObject();
                writer.WriteEndObject();
                return;
            }

            if (Type == SecuritySchemeType.openIdConnect)
            {
                // Bail because V2 does not support OpenIdConnect
                writer.WriteStartObject();
                writer.WriteEndObject();
                return;
            }

            writer.WriteStartObject();

            switch (Type)
            {
                case SecuritySchemeType.http:
                    writer.WriteStringProperty(OpenApiConstants.OpenApiDocType, OpenApiConstants.OpenApiDocBasic);
                    break;

                case SecuritySchemeType.oauth2:
                    writer.WriteStringProperty(OpenApiConstants.OpenApiDocType, Type.ToString());
                    WriteOAuthFlowForV2(writer, Flows);
                    break;

                case SecuritySchemeType.apiKey:
                    writer.WriteStringProperty(OpenApiConstants.OpenApiDocType, Type.ToString());
                    writer.WriteStringProperty(OpenApiConstants.OpenApiDocIn, In.ToString());
                    writer.WriteStringProperty(OpenApiConstants.OpenApiDocName, Name);
                    break;
            }

            writer.WriteStringProperty(OpenApiConstants.OpenApiDocDescription, Description);
            writer.WriteEndObject();
        }

        private static void WriteOAuthFlowForV2(IOpenApiWriter writer, OpenApiOAuthFlows flows)
        {
            if (flows != null)
            {
                if (flows.Implicit != null)
                {
                    WriteOAuthFlowForV2(writer, OpenApiConstants.OpenApiDocImplicit, flows.Implicit);
                }
                else if (flows.Password != null)
                {
                    WriteOAuthFlowForV2(writer, OpenApiConstants.OpenApiDocPassword, flows.Password);
                }
                else if (flows.ClientCredentials != null)
                {
                    WriteOAuthFlowForV2(writer, OpenApiConstants.OpenApiDocApplication, flows.ClientCredentials);
                }
                else if (flows.AuthorizationCode != null)
                {
                    WriteOAuthFlowForV2(writer, OpenApiConstants.OpenApiDocAccessCode, flows.AuthorizationCode);
                }
            }
        }

        private static void WriteOAuthFlowForV2(IOpenApiWriter writer, string flowValue, OpenApiOAuthFlow flow)
        {
            // flow
            writer.WriteStringProperty(OpenApiConstants.OpenApiDocFlow, flowValue);

            // authorizationUrl
            writer.WriteStringProperty(OpenApiConstants.OpenApiDocAuthorizationUrl, flow.AuthorizationUrl?.ToString());

            // tokenUrl
            writer.WriteStringProperty(OpenApiConstants.OpenApiDocTokenUrl, flow.TokenUrl?.ToString());

            // scopes
            writer.WriteMap(OpenApiConstants.OpenApiDocScopes, flow.Scopes, (w, s) => w.WriteValue(s));
        }
    }
}