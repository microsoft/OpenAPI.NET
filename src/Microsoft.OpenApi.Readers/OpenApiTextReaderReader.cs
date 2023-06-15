// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Collections;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers.Interface;
using SharpYaml;
using SharpYaml.Serialization;
//using YamlDotNet.Core;
//using YamlDotNet.RepresentationModel;

namespace Microsoft.OpenApi.Readers
{
    /// <summary>
    /// Service class for converting contents of TextReader into OpenApiDocument instances
    /// </summary>
    public class OpenApiTextReaderReader : IOpenApiReader<TextReader, OpenApiDiagnostic>
    {
        private readonly OpenApiReaderSettings _settings;

        /// <summary>
        /// Create stream reader with custom settings if desired.
        /// </summary>
        /// <param name="settings"></param>
        public OpenApiTextReaderReader(OpenApiReaderSettings settings = null)
        {
            _settings = settings ?? new OpenApiReaderSettings();
        }

        /// <summary>
        /// Reads the stream input and parses it into an Open API document.
        /// </summary>
        /// <param name="input">TextReader containing OpenAPI description to parse.</param>
        /// <param name="diagnostic">Returns diagnostic object containing errors detected during parsing</param>
        /// <returns>Instance of newly created OpenApiDocument</returns>
        public OpenApiDocument Read(TextReader input, out OpenApiDiagnostic diagnostic)
        {
            JsonNode jsonNode;

            // Parse the YAML/JSON text in the TextReader into Json Nodes
            try
            {
                jsonNode = LoadJsonNodesFromYamlDocument(input);
            }
            catch (YamlException ex)
            {
                diagnostic = new OpenApiDiagnostic();
                diagnostic.Errors.Add(new OpenApiError($"#line={ex.Start.Line}", ex.Message));
                return new OpenApiDocument();
            }            

            return new OpenApiYamlDocumentReader(this._settings).Read(jsonNode, out diagnostic);
        }

        /// <summary>
        /// Reads the content of the TextReader.  If there are references to external documents then they will be read asynchronously.
        /// </summary>
        /// <param name="input">TextReader containing OpenAPI description to parse.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A ReadResult instance that contains the resulting OpenApiDocument and a diagnostics instance.</returns>
        public async Task<ReadResult> ReadAsync(TextReader input, CancellationToken cancellationToken = default)
        {
            JsonNode jsonNode;

            // Parse the YAML/JSON text in the TextReader into the YamlDocument
            try
            {
                jsonNode = LoadJsonNodesFromYamlDocument(input);
            }
            catch (JsonException ex)
            {
                var diagnostic = new OpenApiDiagnostic();
                diagnostic.Errors.Add(new OpenApiError($"#line={ex.LineNumber}", ex.Message));
                return new ReadResult
                {
                    OpenApiDocument = null,
                    OpenApiDiagnostic = diagnostic
                };
            }

            return await new OpenApiYamlDocumentReader(this._settings).ReadAsync(jsonNode, cancellationToken);
        }


        /// <summary>
        /// Reads the stream input and parses the fragment of an OpenAPI description into an Open API Element.
        /// </summary>
        /// <param name="input">TextReader containing OpenAPI description to parse.</param>
        /// <param name="version">Version of the OpenAPI specification that the fragment conforms to.</param>
        /// <param name="diagnostic">Returns diagnostic object containing errors detected during parsing</param>
        /// <returns>Instance of newly created OpenApiDocument</returns>
        public T ReadFragment<T>(TextReader input, OpenApiSpecVersion version, out OpenApiDiagnostic diagnostic) where T : IOpenApiElement
        {
            JsonNode jsonNode;

            // Parse the YAML/JSON
            try
            {
                jsonNode = LoadJsonNodesFromYamlDocument(input);
            }
            catch (JsonException ex)
            {
                diagnostic = new OpenApiDiagnostic();
                diagnostic.Errors.Add(new OpenApiError($"#line={ex.LineNumber}", ex.Message));
                return default(T);
            }

            return new OpenApiYamlDocumentReader(this._settings).ReadFragment<T>(jsonNode, version, out diagnostic);
        }

        /// <summary>
        /// Helper method to turn streams into YamlDocument
        /// </summary>
        /// <param name="input">Stream containing YAML formatted text</param>
        /// <returns>Instance of a YamlDocument</returns>
        static JsonNode LoadJsonNodesFromYamlDocument(TextReader input)
        {
            var yamlStream = new YamlStream();
            yamlStream.Load(input);
            var yamlDocument = yamlStream.Documents.First();
            return yamlDocument.ToJsonNode();
        }
    }
}
