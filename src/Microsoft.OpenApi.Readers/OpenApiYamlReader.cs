// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.IO;
using System.Text.Json.Nodes;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Reader;
using SharpYaml.Serialization;
using System.Linq;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Readers
{
    /// <summary>
    /// Reader for parsing YAML files into an OpenAPI document.
    /// </summary>
    public class OpenApiYamlReader : IOpenApiReader
    {
        private const int copyBufferSize = 4096;

        /// <inheritdoc/>
        public async Task<ReadResult> ReadAsync(Stream input,
                                                OpenApiReaderSettings settings,
                                                CancellationToken cancellationToken = default)
        {
            if (input is MemoryStream memoryStream)
            {
                return Read(memoryStream, settings);
            } 
            else 
            {
                using var preparedStream = new MemoryStream();
                await input.CopyToAsync(preparedStream, copyBufferSize, cancellationToken).ConfigureAwait(false);
                preparedStream.Position = 0;
                return Read(preparedStream, settings);
            }
        }

        /// <inheritdoc/>
        public ReadResult Read(MemoryStream input,
                               OpenApiReaderSettings settings)
        {
            JsonNode jsonNode;

            // Parse the YAML text in the TextReader into a sequence of JsonNodes
            try
            {
                using var stream = new StreamReader(input, default, true, -1, settings.LeaveStreamOpen);
                jsonNode = LoadJsonNodesFromYamlDocument(stream);
            }
            catch (JsonException ex)
            {
                var diagnostic = new OpenApiDiagnostic();
                diagnostic.Errors.Add(new($"#line={ex.LineNumber}", ex.Message));
                return new()
                {
                    OpenApiDocument = null,
                    OpenApiDiagnostic = diagnostic
                };
            }

            return Read(jsonNode, settings);
        }

        /// <inheritdoc/>
        public ReadResult Read(JsonNode jsonNode, OpenApiReaderSettings settings, string format = null)
        {
            return OpenApiReaderRegistry.DefaultReader.Read(jsonNode, settings, OpenApiConstants.Yaml);
        }

        /// <inheritdoc/>
        public T ReadFragment<T>(MemoryStream input,
                                 OpenApiSpecVersion version,
                                 out OpenApiDiagnostic diagnostic,
                                 OpenApiReaderSettings settings = null) where T : IOpenApiElement
        {
            JsonNode jsonNode;

            // Parse the YAML
            try
            {
                using var stream = new StreamReader(input);
                jsonNode = LoadJsonNodesFromYamlDocument(stream);
            }
            catch (JsonException ex)
            {
                diagnostic = new();
                diagnostic.Errors.Add(new($"#line={ex.LineNumber}", ex.Message));
                return default;
            }

            return ReadFragment<T>(jsonNode, version, out diagnostic);
        }

        /// <inheritdoc/>
        public T ReadFragment<T>(JsonNode input, OpenApiSpecVersion version, out OpenApiDiagnostic diagnostic, OpenApiReaderSettings settings = null) where T : IOpenApiElement
        {
            return OpenApiReaderRegistry.DefaultReader.ReadFragment<T>(input, version, out diagnostic);
        }

        /// <summary>
        /// Helper method to turn streams into a sequence of JsonNodes
        /// </summary>
        /// <param name="input">Stream containing YAML formatted text</param>
        /// <returns>Instance of a YamlDocument</returns>
        static JsonNode LoadJsonNodesFromYamlDocument(TextReader input)
        {
            var yamlStream = new YamlStream();
            yamlStream.Load(input);
            var yamlDocument = yamlStream.Documents[0];
            return yamlDocument.ToJsonNode();
        }
    }
}
