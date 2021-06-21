// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.IO;
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
        }

        /// <summary>
        /// Reads the stream input and parses it into an Open API document.
        /// </summary>
        /// <param name="input">Stream containing OpenAPI description to parse.</param>
        /// <param name="diagnostic">Returns diagnostic object containing errors detected during parsing.</param>
        /// <returns>Instance of newly created OpenApiDocument.</returns>
        public OpenApiDocument Read(Stream input, out OpenApiDiagnostic diagnostic)
        {
            var reader = new StreamReader(input);
            var result = new OpenApiTextReaderReader(_settings).Read(reader, out diagnostic);
            if (!_settings.LeaveStreamOpen)
            {
                reader.Dispose();
            }

            return result;
        }

        /// <summary>
        /// Reads the stream input and parses it into an Open API document.
        /// </summary>
        /// <param name="input">Stream containing OpenAPI description to parse.</param>
        /// <returns>Instance result containing newly created OpenApiDocument and diagnostics object from the process</returns>
        public async Task<ReadResult> ReadAsync(Stream input)
        {
            MemoryStream bufferedStream;
            if (input is MemoryStream)
            {
                bufferedStream = (MemoryStream)input;
            }
            else
            {
                // Buffer stream so that OpenApiTextReaderReader can process it synchronously
                // YamlDocument doesn't support async reading.
                bufferedStream = new MemoryStream();
                await input.CopyToAsync(bufferedStream);
                bufferedStream.Position = 0;
            }

            var reader = new StreamReader(bufferedStream);

            return await new OpenApiTextReaderReader(_settings).ReadAsync(reader);
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
            using (var reader = new StreamReader(input))
            {
                return new OpenApiTextReaderReader(_settings).ReadFragment<T>(reader, version, out diagnostic);
            }
        }
    }
}
