//---------------------------------------------------------------------
// <copyright file="OpenApiWriterSettings.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;

namespace Microsoft.OpenApi.Writers
{
    /// <summary>
    /// Configuration settings for Open API writers.
    /// </summary>
    public sealed class OpenApiWriterSettings
    {
        public Uri BaseUri { get; set; } = new Uri("http://localhost");

        public Version Version { get; set; } = new Version(1, 0, 1);
    }
}
