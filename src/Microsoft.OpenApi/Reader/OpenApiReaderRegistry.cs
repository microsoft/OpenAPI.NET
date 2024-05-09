// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.OpenApi.Interfaces;

namespace Microsoft.OpenApi.Reader
{
    /// <summary>
    /// Registry for managing different OpenAPI format providers.
    /// </summary>
    public static class OpenApiReaderRegistry
    {
        private static readonly ConcurrentDictionary<string, IOpenApiReader> _readers = new(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Defines a default OpenAPI reader.
        /// </summary>
        public static readonly IOpenApiReader DefaultReader = new OpenApiJsonReader();

        /// <summary>
        /// Registers an IOpenApiReader for a given OpenAPI format.
        /// </summary>
        /// <param name="format">The OpenApi file format.</param>
        /// <param name="reader">The reader instance.</param>
        public static void RegisterReader(string format, IOpenApiReader reader)
        {
            _readers.AddOrUpdate(format, reader, (_, _) => reader);
        }

        /// <summary>
        /// Retrieves an IOpenApiReader for a given OpenAPI format.
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        public static IOpenApiReader GetReader(string format)
        {
            if (_readers.TryGetValue(format, out var reader))
            {
                return reader;
            }

            throw new NotSupportedException($"Format '{format}' is not supported. Register your reader with the OpenApiReaderRegistry class.");
        }
    }
}
