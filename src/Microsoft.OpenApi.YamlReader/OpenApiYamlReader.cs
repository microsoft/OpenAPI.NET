// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.IO;
using System.Text.Json.Nodes;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.OpenApi.Reader;
using SharpYaml.Serialization;
using System;
using System.Linq;
using System.Text;

namespace Microsoft.OpenApi.YamlReader
{
    /// <summary>
    /// Reader for parsing YAML files into an OpenAPI document.
    /// </summary>
    public class OpenApiYamlReader : IOpenApiReader
    {
        private const int copyBufferSize = 4096;
        private static readonly OpenApiJsonReader _jsonReader = new();

        /// <inheritdoc/>
        public async Task<ReadResult> ReadAsync(Stream input,
                                                Uri location,
                                                OpenApiReaderSettings settings,
                                                CancellationToken cancellationToken = default)
        {
            if (input is null) throw new ArgumentNullException(nameof(input));
            if (input is MemoryStream memoryStream)
            {
                return Read(memoryStream, location, settings);
            } 
            else 
            {
                using var preparedStream = new MemoryStream();
                await input.CopyToAsync(preparedStream, copyBufferSize, cancellationToken).ConfigureAwait(false);
                preparedStream.Position = 0;
                return Read(preparedStream, location, settings);
            }
        }

        /// <inheritdoc/>
        public ReadResult Read(MemoryStream input,
                               Uri location,
                               OpenApiReaderSettings settings)
        {
            if (input is null) throw new ArgumentNullException(nameof(input));
            if (settings is null) throw new ArgumentNullException(nameof(settings));
            JsonNode jsonNode;

            // Parse the YAML text in the stream into a sequence of JsonNodes
            try
            {
#if NET
// this represents net core, net5 and up
                using var stream = new StreamReader(input, default, true, -1, settings.LeaveStreamOpen);
#else
// the implementation differs and results in a null reference exception in NETFX
                using var stream = new StreamReader(input, Encoding.UTF8, true, 4096, settings.LeaveStreamOpen);
#endif
                jsonNode = LoadJsonNodesFromYamlDocument(stream);
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

            return Read(jsonNode, location, settings);
        }

        /// <inheritdoc/>
        public static ReadResult Read(JsonNode jsonNode, Uri location, OpenApiReaderSettings settings)
        {
            return _jsonReader.Read(jsonNode, location, settings);
        }

        /// <inheritdoc/>
        public T? ReadFragment<T>(MemoryStream input,
                                 OpenApiSpecVersion version,
                                 OpenApiDocument openApiDocument,
                                 out OpenApiDiagnostic diagnostic,
                                 OpenApiReaderSettings? settings = null) where T : IOpenApiElement
        {
            if (input is null) throw new ArgumentNullException(nameof(input));
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

            return ReadFragment<T>(jsonNode, version, openApiDocument, out diagnostic, settings);
        }

        /// <inheritdoc/>
        public static T? ReadFragment<T>(JsonNode input, OpenApiSpecVersion version, OpenApiDocument openApiDocument, out OpenApiDiagnostic diagnostic, OpenApiReaderSettings? settings = null) where T : IOpenApiElement
        {
            return _jsonReader.ReadFragment<T>(input, version, openApiDocument, out diagnostic, settings);
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
            if (yamlStream.Documents.Any())
            {
                return yamlStream.Documents[0].ToJsonNode();
            }

            throw new InvalidOperationException("No documents found in the YAML stream.");
        }
    }
}
