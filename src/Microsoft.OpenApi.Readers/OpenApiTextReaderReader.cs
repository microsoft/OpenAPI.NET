// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers.Interface;
using Microsoft.OpenApi.Readers.Exceptions;
using SharpYaml;
using SharpYaml.Serialization;

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
            YamlDocument yamlDocument;

            // Parse the YAML/JSON text in the TextReader into the YamlDocument
            try
            {
                yamlDocument = LoadYamlDocument(input);
            }
            catch (YamlException ex)
            {
                diagnostic = new();
                diagnostic.Errors.Add(new($"#line={ex.Start.Line}", ex.Message));
                return new();
            }
            catch (OpenApiReaderException ex)
            {
                diagnostic = new();
                diagnostic.Errors.Add(new(ex));
                return new();
            }

            return new OpenApiYamlDocumentReader(this._settings).Read(yamlDocument, out diagnostic);
        }

        /// <summary>
        /// Reads the content of the TextReader.  If there are references to external documents then they will be read asynchronously.
        /// </summary>
        /// <param name="input">TextReader containing OpenAPI description to parse.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A ReadResult instance that contains the resulting OpenApiDocument and a diagnostics instance.</returns>
        public async Task<ReadResult> ReadAsync(TextReader input, CancellationToken cancellationToken = default)
        {
            YamlDocument yamlDocument;

            // Parse the YAML/JSON text in the TextReader into the YamlDocument
            try
            {
                yamlDocument = LoadYamlDocument(input, cancellationToken);
            }
            catch (YamlException ex)
            {
                var diagnostic = new OpenApiDiagnostic();
                diagnostic.Errors.Add(new($"#line={ex.Start.Line}", ex.Message));
                return new()
                {
                    OpenApiDocument = null,
                    OpenApiDiagnostic = diagnostic
                };
            }
            catch (OpenApiReaderException ex)
            {
                var diagnostic = new OpenApiDiagnostic();
                diagnostic.Errors.Add(new(ex));
                return new()
                {
                    OpenApiDocument = null,
                    OpenApiDiagnostic = diagnostic
                };
            }

            return await new OpenApiYamlDocumentReader(this._settings).ReadAsync(yamlDocument, cancellationToken);
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
            YamlDocument yamlDocument;

            // Parse the YAML/JSON
            try
            {
                yamlDocument = LoadYamlDocument(input);
            }
            catch (YamlException ex)
            {
                diagnostic = new();
                diagnostic.Errors.Add(new($"#line={ex.Start.Line}", ex.Message));
                return default;
            }
            catch (OpenApiReaderException ex)
            {
                diagnostic = new();
                diagnostic.Errors.Add(new(ex));
                return default;
            }

            return new OpenApiYamlDocumentReader(this._settings).ReadFragment<T>(yamlDocument, version, out diagnostic);
        }

        /// <summary>
        /// Helper method to turn streams into YamlDocument
        /// </summary>
        /// <param name="input">Stream containing YAML formatted text</param>
        /// <param name="cancellationToken">Propagates notification that parsing should be cancelled.</param>
        /// <returns>Instance of a YamlDocument</returns>
        private YamlDocument LoadYamlDocument(
            TextReader input,
            CancellationToken cancellationToken = default)
        {
            return new BoundedYamlDocumentParser(_settings).Parse(
                input,
                _settings.MaxInputByteCount,
                cancellationToken);
        }
    }
}
