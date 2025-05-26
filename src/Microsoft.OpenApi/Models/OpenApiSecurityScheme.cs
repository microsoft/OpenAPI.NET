// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi
{
    /// <summary>
    /// Security Scheme Object.
    /// </summary>
    public class OpenApiSecurityScheme : IOpenApiExtensible, IOpenApiSecurityScheme
    {
        /// <inheritdoc/>
        public SecuritySchemeType? Type { get; set; }

        /// <inheritdoc/>
        public string? Description { get; set; }

        /// <inheritdoc/>
        public string? Name { get; set; }

        /// <inheritdoc/>
        public ParameterLocation? In { get; set; }

        /// <inheritdoc/>
        public string? Scheme { get; set; }

        /// <inheritdoc/>
        public string? BearerFormat { get; set; }

        /// <inheritdoc/>
        public OpenApiOAuthFlows? Flows { get; set; }

        /// <inheritdoc/>
        public Uri? OpenIdConnectUrl { get; set; }

        /// <inheritdoc/>
        public Dictionary<string, IOpenApiExtension>? Extensions { get; set; }

        /// <summary>
        /// Parameterless constructor
        /// </summary>
        public OpenApiSecurityScheme() { }

        /// <summary>
        /// Initializes a copy of <see cref="IOpenApiSecurityScheme"/> object
        /// </summary>
        internal OpenApiSecurityScheme(IOpenApiSecurityScheme securityScheme)
        {
            Utils.CheckArgumentNull(securityScheme);
            Type = securityScheme.Type;
            Description = securityScheme.Description ?? Description;
            Name = securityScheme.Name ?? Name;
            In = securityScheme.In;
            Scheme = securityScheme.Scheme ?? Scheme;
            BearerFormat = securityScheme.BearerFormat ?? BearerFormat;
            Flows = securityScheme.Flows != null ? new(securityScheme.Flows) : null;
            OpenIdConnectUrl = securityScheme.OpenIdConnectUrl != null ? new Uri(securityScheme.OpenIdConnectUrl.OriginalString, UriKind.RelativeOrAbsolute) : null;
            Extensions = securityScheme.Extensions != null ? new Dictionary<string, IOpenApiExtension>(securityScheme.Extensions) : null;
        }

        /// <summary>
        /// Serialize <see cref="OpenApiSecurityScheme"/> to Open Api v3.1
        /// </summary>
        public virtual void SerializeAsV31(IOpenApiWriter writer)
        {
            SerializeInternal(writer, OpenApiSpecVersion.OpenApi3_1, (writer, element) => element.SerializeAsV31(writer));
        }

        /// <summary>
        /// Serialize <see cref="OpenApiSecurityScheme"/> to Open Api v3.0
        /// </summary>
        public virtual void SerializeAsV3(IOpenApiWriter writer)
        {
            SerializeInternal(writer, OpenApiSpecVersion.OpenApi3_0, (writer, element) => element.SerializeAsV3(writer));
        }

        private void SerializeInternal(IOpenApiWriter writer, OpenApiSpecVersion version,
            Action<IOpenApiWriter, IOpenApiSerializable> callback)
        {
            Utils.CheckArgumentNull(writer);

            writer.WriteStartObject();

            // type
            writer.WriteProperty(OpenApiConstants.Type, Type?.GetDisplayName());

            // description
            writer.WriteProperty(OpenApiConstants.Description, Description);

            switch (Type)
            {
                case SecuritySchemeType.ApiKey:
                    // These properties apply to apiKey type only.
                    // name
                    // in
                    writer.WriteProperty(OpenApiConstants.Name, Name);
                    writer.WriteProperty(OpenApiConstants.In, In?.GetDisplayName());
                    break;
                case SecuritySchemeType.Http:
                    // These properties apply to http type only.
                    // scheme
                    // bearerFormat
                    writer.WriteProperty(OpenApiConstants.Scheme, Scheme);
                    writer.WriteProperty(OpenApiConstants.BearerFormat, BearerFormat);
                    break;
                case SecuritySchemeType.OAuth2:
                    // This property apply to oauth2 type only.
                    // flows
                    writer.WriteOptionalObject(OpenApiConstants.Flows, Flows, callback);
                    break;
                case SecuritySchemeType.OpenIdConnect:
                    // This property apply to openIdConnect only.
                    // openIdConnectUrl
                    writer.WriteProperty(OpenApiConstants.OpenIdConnectUrl, OpenIdConnectUrl?.ToString());
                    break;
            }

            // extensions
            writer.WriteExtensions(Extensions, version);

            writer.WriteEndObject();
        }

        /// <summary>
        /// Serialize <see cref="OpenApiSecurityScheme"/> to Open Api v2.0
        /// </summary>
        public virtual void SerializeAsV2(IOpenApiWriter writer)
        {
            Utils.CheckArgumentNull(writer);

            if (Type == SecuritySchemeType.Http && Scheme != OpenApiConstants.Basic)
            {
                // Bail because V2 does not support non-basic HTTP scheme
                writer.WriteStartObject();
                writer.WriteEndObject();
                return;
            }

            if (Type == SecuritySchemeType.OpenIdConnect)
            {
                // Bail because V2 does not support OpenIdConnect
                writer.WriteStartObject();
                writer.WriteEndObject();
                return;
            }

            writer.WriteStartObject();

            // type
            switch (Type)
            {
                case SecuritySchemeType.Http:
                    writer.WriteProperty(OpenApiConstants.Type, OpenApiConstants.Basic);
                    break;

                case SecuritySchemeType.OAuth2:
                    // These properties apply to oauth2 type only.
                    // flow
                    // authorizationUrl
                    // tokenUrl
                    // scopes
                    writer.WriteProperty(OpenApiConstants.Type, Type.GetDisplayName());
                    WriteOAuthFlowForV2(writer, Flows);
                    break;

                case SecuritySchemeType.ApiKey:
                    // These properties apply to apiKey type only.
                    // name
                    // in
                    writer.WriteProperty(OpenApiConstants.Type, Type.GetDisplayName());
                    writer.WriteProperty(OpenApiConstants.Name, Name);
                    writer.WriteProperty(OpenApiConstants.In, In?.GetDisplayName());
                    break;
            }

            // description
            writer.WriteProperty(OpenApiConstants.Description, Description);

            // extensions
            writer.WriteExtensions(Extensions, OpenApiSpecVersion.OpenApi2_0);

            writer.WriteEndObject();
        }

        /// <summary>
        /// Arbitrarily chooses one <see cref="OpenApiOAuthFlow"/> object from the <see cref="OpenApiOAuthFlows"/>
        /// to populate in V2 security scheme.
        /// </summary>
        private static void WriteOAuthFlowForV2(IOpenApiWriter writer, OpenApiOAuthFlows? flows)
        {
            if (flows != null)
            {
                if (flows.Implicit != null)
                {
                    WriteOAuthFlowForV2(writer, OpenApiConstants.Implicit, flows.Implicit);
                }
                else if (flows.Password != null)
                {
                    WriteOAuthFlowForV2(writer, OpenApiConstants.Password, flows.Password);
                }
                else if (flows.ClientCredentials != null)
                {
                    WriteOAuthFlowForV2(writer, OpenApiConstants.Application, flows.ClientCredentials);
                }
                else if (flows.AuthorizationCode != null)
                {
                    WriteOAuthFlowForV2(writer, OpenApiConstants.AccessCode, flows.AuthorizationCode);
                }
            }
        }

        private static void WriteOAuthFlowForV2(IOpenApiWriter writer, string flowValue, OpenApiOAuthFlow flow)
        {
            // flow
            writer.WriteProperty(OpenApiConstants.Flow, flowValue);

            // authorizationUrl
            writer.WriteProperty(OpenApiConstants.AuthorizationUrl, flow.AuthorizationUrl?.ToString());

            // tokenUrl
            writer.WriteProperty(OpenApiConstants.TokenUrl, flow.TokenUrl?.ToString());

            // scopes
            writer.WriteOptionalMap(OpenApiConstants.Scopes, flow.Scopes, (w, s) => w.WriteValue(s));
        }

        /// <inheritdoc/>
        public IOpenApiSecurityScheme CreateShallowCopy()
        {
            return new OpenApiSecurityScheme(this);
        }
    }
}
