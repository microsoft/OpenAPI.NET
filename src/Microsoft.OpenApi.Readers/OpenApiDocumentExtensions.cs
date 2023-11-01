// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;

namespace Microsoft.OpenApi
{
    /// <summary>
    /// Loads an OpenApiDocument instance through Load/LoadAsync/Parse pattern
    /// </summary>
    public static class OpenApiDocumentExtensions
    {
        /// <summary>
        /// Loads an OpenApiDocument from a file stream
        /// </summary>
        /// <param name="document"></param>
        /// <param name="stream"></param>
        /// <param name="diagnostic"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static OpenApiDocument Load(this OpenApiDocument document,
                                           Stream stream,
                                           out OpenApiDiagnostic diagnostic,
                                           OpenApiReaderSettings settings = null)
        {
            return new OpenApiStreamReader(settings).Read(stream, out diagnostic);
        }

        /// <summary>
        /// Loads an OpenApiDocument from a TextReader
        /// </summary>
        /// <param name="document"></param>
        /// <param name="reader"></param>
        /// <param name="diagnostic"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static OpenApiDocument Load(this OpenApiDocument document,
                                           TextReader reader,
                                           out OpenApiDiagnostic diagnostic,
                                           OpenApiReaderSettings settings = null)
        {
            return new OpenApiTextReaderReader(settings).Read(reader, out diagnostic);
        }

        /// <summary>
        /// Loads an OpenApiDocument from a string input
        /// </summary>
        /// <param name="document"></param>
        /// <param name="input"></param>
        /// <param name="diagnostic"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static OpenApiDocument Load(this OpenApiDocument document,
                                           string input,
                                           out OpenApiDiagnostic diagnostic,
                                           OpenApiReaderSettings settings = null)
        {
            return new OpenApiStringReader(settings).Read(input, out diagnostic);
        }

        /// <summary>
        /// Loads an OpenApiDocument from a string input
        /// </summary>
        /// <param name="document"></param>
        /// <param name="input"></param>
        /// <param name="diagnostic"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static OpenApiDocument Parse(this OpenApiDocument document,
                                            string input,
                                            out OpenApiDiagnostic diagnostic,
                                            OpenApiReaderSettings settings = null)
        {
            return new OpenApiStringReader(settings).Read(input, out diagnostic);
        }

        /// <summary>
        /// Loads an OpenApiDocument asynchronously from a TextReader
        /// </summary>
        /// <param name="document"></param>
        /// <param name="reader"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static Task<ReadResult> LoadAsync(this OpenApiDocument document,
                                                 TextReader reader,
                                                 OpenApiReaderSettings settings = null,
                                                 CancellationToken cancellationToken = default)
        {
            return new OpenApiTextReaderReader(settings).ReadAsync(reader, cancellationToken);
        }

        /// <summary>
        /// Loads an OpenApiDocument from a file stream
        /// </summary>
        /// <param name="document"></param>
        /// <param name="stream"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static Task<ReadResult> LoadAsync(this OpenApiDocument document,
                                                 Stream stream,
                                                 OpenApiReaderSettings settings = null,
                                                 CancellationToken cancellationToken = default)
        {
            return new OpenApiStreamReader(settings).ReadAsync(stream, cancellationToken);
        }
    }
}
