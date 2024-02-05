// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Reader;

namespace Microsoft.OpenApi.Interfaces
{
    /// <summary>
    /// Interface for Open API readers.
    /// </summary>
    public interface IOpenApiReader
    {
        /// <summary>
        /// Reads the input URL and parses it into an Open API document.
        /// </summary>
        /// <param name="url">The input to read from.</param>
        /// <param name="diagnostic">The diagnostic entity containing information from the reading process.</param>
        /// <param name="settings"> The OpenApi reader settings.</param>
        /// <returns></returns>
        OpenApiDocument Read(string url, out OpenApiDiagnostic diagnostic, OpenApiReaderSettings settings = null);

        /// <summary>
        /// Reads the input stream and parses it into an Open API document.
        /// </summary>
        /// <param name="stream"> The input stream.</param>
        /// <param name="diagnostic">The diagnostic entity containing information from the reading process.</param>
        /// <param name="settings"> The OpenApi reader settings.</param>
        /// <returns></returns>
        OpenApiDocument Read(Stream stream, out OpenApiDiagnostic diagnostic, OpenApiReaderSettings settings = null);

        /// <summary>
        /// Reads the TextReader input and parses it into an Open API document.
        /// </summary>
        /// <param name="input">The TextReader input.</param>
        /// <param name="diagnostic">The diagnostic entity containing information from the reading process.</param>
        /// <param name="settings"> The OpenApi reader settings.</param>
        /// <returns></returns>
        OpenApiDocument Read(TextReader input, out OpenApiDiagnostic diagnostic, OpenApiReaderSettings settings = null);

        /// <summary>
        /// Reads the input URL and parses it into an Open API document.
        /// </summary>
        /// <param name="url">The input URL.</param>
        /// <param name="settings"> The OpenApi reader settings.</param>
        /// <param name="cancellationToken">Propagates notification that an operation should be cancelled.</param>
        /// <returns></returns>
        Task<ReadResult> ReadAsync(string url, OpenApiReaderSettings settings = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Reads the input stream and parses it into an Open API document.
        /// </summary>
        /// <param name="stream">The input stream.</param>
        /// <param name="settings"> The OpenApi reader settings.</param>
        /// <param name="cancellationToken">Propagates notification that an operation should be cancelled.</param>
        /// <returns></returns>
        Task<ReadResult> ReadAsync(Stream stream, OpenApiReaderSettings settings = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Reads the TextReader input and parses it into an Open API document.
        /// </summary>
        /// <param name="input">The TextReader input.</param>
        /// <param name="settings"> The OpenApi reader settings.</param>
        /// <param name="cancellationToken">Propagates notification that an operation should be cancelled.</param>
        /// <returns></returns>
        Task<ReadResult> ReadAsync(TextReader input, OpenApiReaderSettings settings = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Reads the input string and parses it into an Open API document.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <param name="diagnostic">The diagnostic entity containing information from the reading process.</param>
        /// <param name="settings"> The OpenApi reader settings.</param>
        /// <returns></returns>
        OpenApiDocument Parse(string input, out OpenApiDiagnostic diagnostic, OpenApiReaderSettings settings = null);
    }
}
