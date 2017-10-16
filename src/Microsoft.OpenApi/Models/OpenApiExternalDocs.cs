//---------------------------------------------------------------------
// <copyright file="OpenApiExternalDocs.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;

namespace Microsoft.OpenApi
{
    /// <summary>
    /// ExternalDocs object.
    /// </summary>
    public class OpenApiExternalDocs
    {
        /// <summary>
        /// REQUIRED.The URL for the target documentation. Value MUST be in the format of a URL.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// A short description of the target documentation.
        /// </summary>
        public Uri Url { get; set; }
    }
}
