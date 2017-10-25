// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System;

namespace Microsoft.OpenApi
{
    /// <summary>
    /// Configuration settings for Open API writers.
    /// </summary>
    public sealed class OpenApiSerializerSettings
    {
        /// <summary>
        /// Open Api specification version
        /// </summary>
        public OpenApiSpecVersion SpecVersion { get; set; } = OpenApiSpecVersion.OpenApi3_0;

        /// <summary>
        /// Open Api document format.
        /// </summary>
        public OpenApiFormat Format { get; set; } = OpenApiFormat.Json;
    }
}
