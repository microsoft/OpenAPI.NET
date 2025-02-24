// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using Microsoft.OpenApi.Interfaces;

namespace Microsoft.OpenApi.Reader
{
    /// <summary>
    /// Registry for managing different OpenAPI format providers.
    /// </summary>
    public static class OpenApiReaderRegistry
    {
        /// <summary>
        /// Registers an IOpenApiReader for a given OpenAPI format.
        /// </summary>
        /// <param name="format">The OpenApi file format.</param>
        /// <param name="reader">The reader instance.</param>
        public static void RegisterReader(string format, IOpenApiReader reader)
        {
        }
    }
}
