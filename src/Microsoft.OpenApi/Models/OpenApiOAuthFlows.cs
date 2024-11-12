// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Models
{
    /// <summary>
    /// OAuth Flows Object.
    /// </summary>
    public class OpenApiOAuthFlows : IOpenApiSerializable, IOpenApiExtensible
    {
        /// <summary>
        /// Configuration for the OAuth Implicit flow
        /// </summary>
        public OpenApiOAuthFlow Implicit { get; set; }

        /// <summary>
        /// Configuration for the OAuth Resource Owner Password flow.
        /// </summary>
        public OpenApiOAuthFlow Password { get; set; }

        /// <summary>
        /// Configuration for the OAuth Client Credentials flow.
        /// </summary>
        public OpenApiOAuthFlow ClientCredentials { get; set; }

        /// <summary>
        /// Configuration for the OAuth Authorization Code flow.
        /// </summary>
        public OpenApiOAuthFlow AuthorizationCode { get; set; }

        /// <summary>
        /// Specification Extensions.
        /// </summary>
        public IDictionary<string, IOpenApiExtension> Extensions { get; set; } = new Dictionary<string, IOpenApiExtension>();

        /// <summary>
        /// Parameterless constructor
        /// </summary>
        public OpenApiOAuthFlows() { }

        /// <summary>
        /// Initializes a copy of an <see cref="OpenApiOAuthFlows"/> object
        /// </summary>
        /// <param name="oAuthFlows"></param>
        public OpenApiOAuthFlows(OpenApiOAuthFlows oAuthFlows)
        {
            Implicit = oAuthFlows?.Implicit != null ? new(oAuthFlows?.Implicit) : null;
            Password = oAuthFlows?.Password != null ? new(oAuthFlows?.Password) : null;
            ClientCredentials = oAuthFlows?.ClientCredentials != null ? new(oAuthFlows?.ClientCredentials) : null;
            AuthorizationCode = oAuthFlows?.AuthorizationCode != null ? new(oAuthFlows?.AuthorizationCode) : null;
            Extensions = oAuthFlows?.Extensions != null ? new Dictionary<string, IOpenApiExtension>(oAuthFlows.Extensions) : null;
        }

        /// <summary>
        /// Serialize <see cref="OpenApiOAuthFlows"/> to Open Api v3.1
        /// </summary>
        public void SerializeAsV31(IOpenApiWriter writer)
        {
            SerializeInternal(writer, OpenApiSpecVersion.OpenApi3_1, (writer, element) => element.SerializeAsV31(writer));
        }

        /// <summary>
        /// Serialize <see cref="OpenApiOAuthFlows"/> to Open Api v3.0
        /// </summary>
        public void SerializeAsV3(IOpenApiWriter writer)
        {
            SerializeInternal(writer, OpenApiSpecVersion.OpenApi3_0, (writer, element) => element.SerializeAsV3(writer));
        }

        /// <summary>
        /// Serialize <see cref="OpenApiOAuthFlows"/>
        /// </summary>
        private void SerializeInternal(IOpenApiWriter writer, OpenApiSpecVersion version,
            Action<IOpenApiWriter, IOpenApiSerializable> callback)
        {
            Utils.CheckArgumentNull(writer);;

            writer.WriteStartObject();

            // implicit
            writer.WriteOptionalObject(OpenApiConstants.Implicit, Implicit, callback);

            // password
            writer.WriteOptionalObject(OpenApiConstants.Password, Password, callback);

            // clientCredentials
            writer.WriteOptionalObject(
                OpenApiConstants.ClientCredentials,
                ClientCredentials,
                callback);

            // authorizationCode
            writer.WriteOptionalObject(
                OpenApiConstants.AuthorizationCode,
                AuthorizationCode,
                callback);

            // extensions
            writer.WriteExtensions(Extensions, version);

            writer.WriteEndObject();
        }

        /// <summary>
        /// Serialize <see cref="OpenApiOAuthFlows"/> to Open Api v2.0
        /// </summary>
        public void SerializeAsV2(IOpenApiWriter writer)
        {
            // OAuthFlows object does not exist in V2.
        }
    }
}
