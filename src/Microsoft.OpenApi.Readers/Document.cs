// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Readers
{
    /// <summary>
    /// Loads an OpenApiDocument instance through Load/LoadAsync/Parse pattern
    /// </summary>
    internal static class Document
    {
        /// <summary>
        /// Loads an OpenApiDocument from a file stream
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="diagnostic"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static OpenApiDocument Load(Stream stream, out OpenApiDiagnostic diagnostic, OpenApiReaderSettings settings = null)
        {
            return new OpenApiStreamReader(settings).Read(stream, out diagnostic);
        }

        /// <summary>
        /// Loads an OpenApiDocument from a TextReader
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="diagnostic"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static OpenApiDocument Load(TextReader reader, out OpenApiDiagnostic diagnostic, OpenApiReaderSettings settings = null)
        {
            return new OpenApiTextReaderReader(settings).Read(reader, out diagnostic);
        }

        /// <summary>
        /// Loads an OpenApiDocument from a string input
        /// </summary>
        /// <param name="input"></param>
        /// <param name="diagnostic"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static OpenApiDocument Load(string input, out OpenApiDiagnostic diagnostic, OpenApiReaderSettings settings = null)
        {
            return new OpenApiStringReader(settings).Read(input, out diagnostic);
        }

        /// <summary>
        /// Loads an OpenApiDocument from a string input
        /// </summary>
        /// <param name="input"></param>
        /// <param name="diagnostic"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static OpenApiDocument Parse(string input, out OpenApiDiagnostic diagnostic, OpenApiReaderSettings settings = null)
        {
            return new OpenApiStringReader(settings).Read(input, out diagnostic);
        }

        /// <summary>
        /// Loads an OpenApiDocument asynchronously from a TextReader
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static Task<ReadResult> LoadAsync(TextReader reader, CancellationToken cancellationToken = default, OpenApiReaderSettings settings = null)
        {
            return new OpenApiTextReaderReader(settings).ReadAsync(reader, cancellationToken);
        }

        /// <summary>
        /// Loads an OpenApiDocument from a file stream
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static async Task<ReadResult> LoadAsync(Stream stream, CancellationToken cancellationToken = default, OpenApiReaderSettings settings = null)
        {
            return await new OpenApiStreamReader(settings).ReadAsync(stream, cancellationToken);
        }
    }
}
