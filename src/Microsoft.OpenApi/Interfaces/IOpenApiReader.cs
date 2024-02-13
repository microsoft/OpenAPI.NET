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

        /// <summary>
        /// Reads the input string and parses it into an Open API document.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input">Stream containing OpenAPI description to parse.</param>
        /// <param name="version">Version of the OpenAPI specification that the fragment conforms to.</param>
        /// <param name="diagnostic">Returns diagnostic object containing errors detected during parsing</param>
        /// <param name="settings">The OpenApiReader settings.</param>
        /// <returns></returns>
        T Parse<T>(string input, OpenApiSpecVersion version, out OpenApiDiagnostic diagnostic, OpenApiReaderSettings settings = null) where T : IOpenApiElement;

        /// <summary>
        /// Reads the stream input and parses the fragment of an OpenAPI description into an Open API Element.
        /// </summary>
        /// <param name="input">Stream containing OpenAPI description to parse.</param>
        /// <param name="version">Version of the OpenAPI specification that the fragment conforms to.</param>
        /// <param name="diagnostic">Returns diagnostic object containing errors detected during parsing</param>
        /// <param name="settings">The OpenApiReader settings.</param>
        /// <returns>Instance of newly created OpenApiDocument</returns>
        T Read<T>(Stream input, OpenApiSpecVersion version, out OpenApiDiagnostic diagnostic, OpenApiReaderSettings settings = null) where T : IOpenApiElement;

        /// <summary>
        /// Reads the TextReader input and parses the fragment of an OpenAPI description into an Open API Element.
        /// </summary>
        /// <param name="input">TextReader containing OpenAPI description to parse.</param>
        /// <param name="version">Version of the OpenAPI specification that the fragment conforms to.</param>
        /// <param name="diagnostic">Returns diagnostic object containing errors detected during parsing</param>
        /// <param name="settings">The OpenApiReader settings.</param>
        /// <returns>Instance of newly created OpenApiDocument</returns>
        T Read<T>(TextReader input, OpenApiSpecVersion version, out OpenApiDiagnostic diagnostic, OpenApiReaderSettings settings = null) where T : IOpenApiElement;

        /// <summary>
        /// Reads the string input and parses the fragment of an OpenAPI description into an Open API Element.
        /// </summary>
        /// <param name="url">Url pointing to the document.</param>
        /// <param name="version">Version of the OpenAPI specification that the fragment conforms to.</param>
        /// <param name="diagnostic">Returns diagnostic object containing errors detected during parsing</param>
        /// <param name="settings">The OpenApiReader settings.</param>
        /// <returns>Instance of newly created OpenApiDocument</returns>
        T Read<T>(string url, OpenApiSpecVersion version, out OpenApiDiagnostic diagnostic, OpenApiReaderSettings settings = null) where T : IOpenApiElement;
    }
}
