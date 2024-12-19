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
        /// <inheritdoc/>
        public async Task<ReadResult> ReadAsync(TextReader input,
                                                OpenApiReaderSettings settings = null,
                                                CancellationToken cancellationToken = default)
        {
            JsonNode jsonNode;

            // Parse the YAML text in the TextReader into a sequence of JsonNodes
            try
            {
                jsonNode = LoadJsonNodesFromYamlDocument(input);
            }
            catch (JsonException ex)
            {
                var diagnostic = new OpenApiDiagnostic();
                diagnostic.Errors.Add(new($"#line={ex.LineNumber}", ex.Message));
                return new()
                {
                    Document = null,
                    Diagnostic = diagnostic
                };
            }

            return await ReadAsync(jsonNode, settings, cancellationToken: cancellationToken);
        }

        /// <inheritdoc/>
        public T ReadFragment<T>(TextReader input,
                                 OpenApiSpecVersion version,
                                 out OpenApiDiagnostic diagnostic,
                                 OpenApiReaderSettings settings = null) where T : IOpenApiElement
        {
            JsonNode jsonNode;

            // Parse the YAML
            try
            {
                jsonNode = LoadJsonNodesFromYamlDocument(input);
            }
            catch (JsonException ex)
            {
                diagnostic = new();
                diagnostic.Errors.Add(new($"#line={ex.LineNumber}", ex.Message));
                return default;
            }

            return ReadFragment<T>(jsonNode, version, out diagnostic);
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

        /// <inheritdoc/>        
        public async Task<ReadResult> ReadAsync(JsonNode jsonNode, OpenApiReaderSettings settings, string format = null, CancellationToken cancellationToken = default)
        {
            return await OpenApiReaderRegistry.DefaultReader.ReadAsync(jsonNode, settings, OpenApiConstants.Yaml, cancellationToken);
        }

        /// <inheritdoc/>
        public T ReadFragment<T>(JsonNode input, OpenApiSpecVersion version, out OpenApiDiagnostic diagnostic, OpenApiReaderSettings settings = null) where T : IOpenApiElement
        {
            return OpenApiReaderRegistry.DefaultReader.ReadFragment<T>(input, version, out diagnostic);
        }
    }
}
