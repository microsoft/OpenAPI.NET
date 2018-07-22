// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.IO;
using System.Linq;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers.Interface;
using SharpYaml;
using SharpYaml.Serialization;

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
        /// <param name="diagnostic">Returns diagnostic object containing errors detected during parsing</param>
        /// <returns>Instance of newly created OpenApiDocument</returns>
        public OpenApiDocument Read(Stream input, out OpenApiDiagnostic diagnostic)
        {
            YamlDocument yamlDocument;
            diagnostic = new OpenApiDiagnostic();

            // Parse the YAML/JSON
            try
            {
                yamlDocument = LoadYamlDocument(input);
            }
            catch (SyntaxErrorException ex)
            {
                diagnostic.Errors.Add(new OpenApiError($"#char={ex.Start.Line}", ex.Message));
                return new OpenApiDocument();
            }

            return new OpenApiDocumentReader(_settings).Read(yamlDocument, out diagnostic);
        }

        /// <summary>
        /// Reads the stream input and parses the fragment of an OpenAPI description into an Open API Element.
        /// </summary>
        /// <param name="input">Stream containing OpenAPI description to parse.</param>
        /// <param name="version">Version of the OpenAPI specification that the fragment conforms to.</param>
        /// <param name="diagnostic">Returns diagnostic object containing errors detected during parsing</param>
        /// <returns>Instance of newly created OpenApiDocument</returns>
        public T ReadFragment<T>(Stream input, OpenApiSpecVersion version, out OpenApiDiagnostic diagnostic) where T : IOpenApiElement
        {
            YamlDocument yamlDocument;
            diagnostic = new OpenApiDiagnostic();

            // Parse the YAML/JSON
            try
            {
                yamlDocument = LoadYamlDocument(input);
            }
            catch (YamlException ex)
            {
                diagnostic.Errors.Add(new OpenApiError($"#line={ex.Start.Line}", ex.Message));
                return default(T);
            }

            return new OpenApiDocumentReader(_settings).ReadFragment<T>(yamlDocument, version, out diagnostic);
        }

        /// <summary>
        /// Helper method to turn streams into YamlDocument
        /// </summary>
        /// <param name="input">Stream containing YAML formatted text</param>
        /// <returns>Instance of a YamlDocument</returns>
        internal static YamlDocument LoadYamlDocument(Stream input)
        {
            YamlDocument yamlDocument;
            using (var streamReader = new StreamReader(input))
            {
                var yamlStream = new YamlStream();
                yamlStream.Load(streamReader);
                yamlDocument = yamlStream.Documents.First();
            }
            return yamlDocument;
        }
    }
}