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
        /// Base Uri for the Open API services.
        /// </summary>
        public Uri BaseUri { get; set; } = new Uri("http://localhost");

        /// <summary>
        /// Metadata version for the <see cref="OpenApiInfo"/>
        /// </summary>
        public Version MetadataVersion { get; set; } = new Version(1, 0, 1);

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
