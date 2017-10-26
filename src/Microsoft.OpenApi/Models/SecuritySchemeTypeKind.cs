// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.OpenApi.Models
{
    /// <summary>
    /// The type of the security scheme
    /// </summary>
    public enum SecuritySchemeTypeKind
    {
        apiKey,

        http,

        oauth2,

        openIdConnect
    }

}
