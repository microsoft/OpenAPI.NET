// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers.Interface;

namespace Microsoft.OpenApi.Readers
{
    /// <summary>
    /// Service class for converting streams into OpenApiDocument instances
    /// </summary>
    public class OpenApiStreamReader : IOpenApiReader<Stream, OpenApiDiagnostic>
    {
        private readonly OpenApiReaderSettings _settings;

        /// <summary>
        /// Create stream reader with custom settings if desired.
        /// </summary>
        /// <param name="settings"></param>
        public OpenApiStreamReader(OpenApiReaderSettings settings = null)
        {
            _settings = settings ?? new OpenApiReaderSettings();

            if((_settings.ReferenceResolution == ReferenceResolutionSetting.ResolveAllReferences || _settings.LoadExternalRefs)
                && _settings.BaseUrl == null)
            {
                throw new ArgumentException("BaseUrl must be provided to resolve external references.");
            }
        }

        /// <summary>
        /// Reads the stream input and parses it into an Open API document.
        /// </summary>
        /// <param name="input">Stream containing OpenAPI description to parse.</param>
        /// <param name="diagnostic">Returns diagnostic object containing errors detected during parsing.</param>
        /// <returns>Instance of newly created OpenApiDocument.</returns>
        public OpenApiDocument Read(Stream input, out OpenApiDiagnostic diagnostic)
        {
            using var reader = new StreamReader(input, default, true, -1, _settings.LeaveStreamOpen);
            return new OpenApiTextReaderReader(_settings).Read(reader, out diagnostic);
        }

        /// <summary>
        /// Reads the stream input and parses it into an Open API document.
        /// </summary>
        /// <param name="input">Stream containing OpenAPI description to parse.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Instance result containing newly created OpenApiDocument and diagnostics object from the process</returns>
        public async Task<ReadResult> ReadAsync(Stream input, CancellationToken cancellationToken = default)
        {
            MemoryStream bufferedStream;
            if (input is MemoryStream stream)
            {
                bufferedStream = stream;
            }
            else
            {
                // Buffer stream so that OpenApiTextReaderReader can process it synchronously
                // YamlDocument doesn't support async reading.
                bufferedStream = new();
                await input.CopyToAsync(bufferedStream, 81920, cancellationToken);
                bufferedStream.Position = 0;
            }

            using var reader = new StreamReader(bufferedStream, default, true, -1, _settings.LeaveStreamOpen);
            return await new OpenApiTextReaderReader(_settings).ReadAsync(reader, cancellationToken);
        }

        /// <summary>
        /// Reads the stream input and parses the fragment of an OpenAPI description into an Open API Element.
        /// </summary>
        /// <param name="input">Stream containing OpenAPI description to parse.</param>
        /// <param name="version">Version of the OpenAPI specification that the fragment conforms to.</param>
        /// <param name="diagnostic">Returns diagnostic object containing errors detected during parsing</param>
        /// <returns>Instance of newly created OpenApiDocument</returns>
        public T ReadFragment<T>(Stream input, OpenApiSpecVersion version, out OpenApiDiagnostic diagnostic) where T : IOpenApiReferenceable
        {
            using var reader = new StreamReader(input, default, true, -1, _settings.LeaveStreamOpen);
            return new OpenApiTextReaderReader(_settings).ReadFragment<T>(reader, version, out diagnostic);
        }
    }
}
