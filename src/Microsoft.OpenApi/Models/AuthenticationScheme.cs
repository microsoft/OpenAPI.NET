// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using Microsoft.OpenApi.Attributes;

namespace Microsoft.OpenApi.Models
{
    /// <summary>
    /// The type of HTTP authentication schemes
    /// </summary>
    public enum AuthenticationScheme
    {
        /// <summary>
        /// Use basic HTTP authentication schemes
        /// </summary>
        [Display("basic")] Basic,

        /// <summary>
        /// Use bearer Authentication scheme
        /// </summary>
        [Display("bearer")] Bearer,

        /// <summary>
        /// Use OpenIdConnectUrl
        /// </summary>
        [Display("openIdConnectUrl")] OpenIdConnectUrl

        
    }
}
