// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.ComponentModel;
using System.Text.Json.Serialization;
using Microsoft.OpenApi.Attributes;

namespace Microsoft.OpenApi.Models
{
    /// <summary>
    /// The type of the security scheme
    /// </summary>
    [JsonConverter(typeof(Json.More.EnumStringConverter<SecuritySchemeType>))]
    public enum SecuritySchemeType
    {
        /// <summary>
        /// Use API key
        /// </summary>
        [Description("apiKey")] ApiKey,

        /// <summary>
        /// Use basic or bearer token authorization header.
        /// </summary>
        [Description("http")] Http,

        /// <summary>
        /// Use OAuth2
        /// </summary>
        [Description("oauth2")] OAuth2,

        /// <summary>
        /// Use OAuth2 with OpenId Connect URL to discover OAuth2 configuration value.
        /// </summary>
        [Description("openIdConnect")] OpenIdConnect
    }
}
