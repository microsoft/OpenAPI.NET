//---------------------------------------------------------------------
// <copyright file="SecuritySchemeTypeKind.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OpenApi
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
