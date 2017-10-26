// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Serialization
{
    /// <summary>
    /// Extensions method for <see cref="OpenApiSecurityScheme"/> serialization.
    /// </summary>
    internal static class OpenApiSecuritySchemeExtensions
    {
        /// <summary>
        /// Serialize <see cref="OpenApiSecurityScheme"/> to Open Api v3.0
        /// </summary>
        public static void SerializeV3(this OpenApiSecurityScheme securityScheme, IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            writer.WriteStartObject();
            if (securityScheme != null)
            {
                writer.WriteStringProperty("type", securityScheme.Type.ToString());
                switch (securityScheme.Type)
                {
                    case SecuritySchemeTypeKind.http:
                        writer.WriteStringProperty("scheme", securityScheme.Scheme);
                        writer.WriteStringProperty("bearerFormat", securityScheme.BearerFormat);
                        break;
                    case SecuritySchemeTypeKind.oauth2:
                    //writer.WriteStringProperty("scheme", this.Scheme);
                    //TODO:
                    case SecuritySchemeTypeKind.apiKey:
                        writer.WriteStringProperty("in", securityScheme.In.ToString());
                        writer.WriteStringProperty("name", securityScheme.Name);

                        break;
                }

                writer.WriteObject("flows", securityScheme.Flows, (w, o) => o.SerializeV3(w));

                writer.WriteStringProperty("openIdConnectUrl", securityScheme.OpenIdConnectUrl?.ToString());
            }

            writer.WriteEndObject();
        }

        /// <summary>
        /// Serialize <see cref="OpenApiSecurityScheme"/> to Open Api v2.0
        /// </summary>
        public static void SerializeV2(this OpenApiSecurityScheme securityScheme, IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            writer.WriteStartObject();
            if (securityScheme != null)
            {
                if (securityScheme.Type == SecuritySchemeTypeKind.http)
                {
                    if (securityScheme.Scheme == "basic")
                    {
                        writer.WriteStringProperty("type", "basic");
                    }
                }
                else
                {
                    writer.WriteStringProperty("type", securityScheme.Type.ToString());
                }
                switch (securityScheme.Type)
                {
                    case SecuritySchemeTypeKind.oauth2:
                    //writer.WriteStringProperty("scheme", this.Scheme);
                    //TODO:
                    case SecuritySchemeTypeKind.apiKey:
                        writer.WriteStringProperty("in", securityScheme.In.ToString());
                        writer.WriteStringProperty("name", securityScheme.Name);

                        break;
                }

                if (securityScheme.Flows != null)
                {
                    if (securityScheme.Flows.Implicit != null)
                    {
                        writer.WriteStringProperty("flow", "implicit");
                        securityScheme.Flows.Implicit.SerializeV2(writer);
                    }
                    else if (securityScheme.Flows.Password != null)
                    {
                        writer.WriteStringProperty("flow", "password");
                        securityScheme.Flows.Password.SerializeV2(writer);
                    }
                    else if (securityScheme.Flows.ClientCredentials != null)
                    {
                        writer.WriteStringProperty("flow", "application");
                        securityScheme.Flows.ClientCredentials.SerializeV2(writer);
                    }
                    else if (securityScheme.Flows.AuthorizationCode != null)
                    {
                        writer.WriteStringProperty("flow", "accessCode");
                        securityScheme.Flows.AuthorizationCode.SerializeV2(writer);
                    }
                }
            }
            writer.WriteEndObject();
        }
    }
}
