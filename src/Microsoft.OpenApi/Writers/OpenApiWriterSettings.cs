// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.IO;

namespace Microsoft.OpenApi.Writers
{
    /// <summary>
    /// Configuration settings for Open API writers.
    /// </summary>
    public sealed class OpenApiWriterSettings
    {
        public Uri BaseUri { get; set; } = new Uri("http://localhost");

        public Version Version { get; set; } = new Version(1, 0, 1);

        public Func<Stream, IOpenApiWriter> WriterFactory { get; set; }
    }
}
