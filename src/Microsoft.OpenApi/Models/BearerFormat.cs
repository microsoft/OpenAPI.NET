// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using Microsoft.OpenApi.Attributes;

namespace Microsoft.OpenApi.Models
{
    /// <summary>
    /// The type of Bearer authentication scheme
    /// </summary>
    public enum BearerFormat
    {
        /// <summary>
        /// Use JWT bearer format
        /// </summary>
        [Display("jwt")] JWT,

    }
}
