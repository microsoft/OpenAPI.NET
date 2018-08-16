// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.IO;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers.Interface;

namespace Microsoft.OpenApi.Readers
{
    /// <summary>
    /// Service class for converting strings into OpenApiDocument instances
    /// </summary>
    public class OpenApiStringReader : IOpenApiReader<string, OpenApiDiagnostic>
    {
        private readonly OpenApiReaderSettings _settings;

        /// <summary>
        /// Constructor tha allows reader to use non-default settings
        /// </summary>
        /// <param name="settings"></param>
        public OpenApiStringReader(OpenApiReaderSettings settings = null)
        {
            _settings = settings ?? new OpenApiReaderSettings();
        }

        /// <summary>
        /// Reads the string input and parses it into an Open API document.
        /// </summary>
        public OpenApiDocument Read(string input, out OpenApiDiagnostic diagnostic)
        {
            using (var memoryStream = new MemoryStream())
            {
                var writer = new StreamWriter(memoryStream);
                writer.Write(input);
                writer.Flush();
                memoryStream.Position = 0;

                return new OpenApiStreamReader(_settings).Read(memoryStream, out diagnostic);
            }
        }

        /// <summary>
        /// Reads the string input and parses it into an Open API element.
        /// </summary>
        public T ReadFragment<T>(string input, OpenApiSpecVersion version, out OpenApiDiagnostic diagnostic) where T : IOpenApiElement
        {
            using (var memoryStream = new MemoryStream())
            {
                var writer = new StreamWriter(memoryStream);
                writer.Write(input);
                writer.Flush();
                memoryStream.Position = 0;

                return new OpenApiStreamReader(_settings).ReadFragment<T>(memoryStream, version, out diagnostic);
            }
        }
    }
}