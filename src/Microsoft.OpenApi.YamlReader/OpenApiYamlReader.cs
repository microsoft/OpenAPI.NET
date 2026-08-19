// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.IO;
using System.Text.Json.Nodes;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.OpenApi.Reader;
using SharpYaml;
using System;
using System.Text;

namespace Microsoft.OpenApi.YamlReader
{
    /// <summary>
    /// Reader for parsing YAML files into an OpenAPI document.
    /// </summary>
    /// <remarks>
    /// Input is converted directly from SharpYaml parser events so resource limits are enforced
    /// before SharpYaml's recursive YAML model loader can compose or expand the document.
    /// </remarks>
    public class OpenApiYamlReader : IOpenApiReader
    {
        private const int copyBufferSize = 4096;
        private static readonly OpenApiJsonReader _jsonReader = new();
        private readonly OpenApiYamlReaderSettings _yamlSettings;

        /// <summary>
        /// Initializes a YAML reader using the current legacy global conversion limits.
        /// </summary>
        public OpenApiYamlReader()
            : this(new()
            {
                MaxDepth = YamlConverter.MaxDepth,
                MaxNodeCount = YamlConverter.MaxNodeCount,
                MaxAliasExpansionNodeCount = YamlConverter.MaxAliasExpansionNodeCount,
            })
        {
        }

        /// <summary>
        /// Initializes a YAML reader with immutable per-reader resource limits.
        /// </summary>
        /// <param name="settings">The YAML reader settings.</param>
        public OpenApiYamlReader(OpenApiYamlReaderSettings settings)
        {
            if (settings is null) throw new ArgumentNullException(nameof(settings));
            settings.Validate();
            _yamlSettings = new()
            {
                MaxDepth = settings.MaxDepth,
                MaxNodeCount = settings.MaxNodeCount,
                MaxAliasExpansionNodeCount = settings.MaxAliasExpansionNodeCount,
                MaxInputByteCount = settings.MaxInputByteCount,
                MaxScalarLength = settings.MaxScalarLength,
            };
        }

        /// <inheritdoc/>
        public async Task<ReadResult> ReadAsync(Stream input,
                                                Uri location,
                                                OpenApiReaderSettings settings,
                                                CancellationToken cancellationToken = default)
        {
            if (input is null) throw new ArgumentNullException(nameof(input));
            if (settings is null) throw new ArgumentNullException(nameof(settings));
            if (input is MemoryStream memoryStream)
            {
                return ReadCore(memoryStream, location, settings, cancellationToken);
            } 
            else 
            {
                using var preparedStream = new MemoryStream();
                try
                {
                    await CopyToMemoryStreamAsync(
                        input,
                        preparedStream,
                        _yamlSettings.MaxInputByteCount,
                        cancellationToken).ConfigureAwait(false);
                }
                catch (OpenApiReaderException ex)
                {
                    return new()
                    {
                        Document = null,
                        Diagnostic = CreateDiagnostic(new(ex)),
                    };
                }

                preparedStream.Position = 0;
                return ReadCore(preparedStream, location, settings, cancellationToken);
            }
        }

        /// <inheritdoc/>
        public ReadResult Read(MemoryStream input,
                               Uri location,
                               OpenApiReaderSettings settings)
            => ReadCore(input, location, settings, CancellationToken.None);

        private ReadResult ReadCore(MemoryStream input,
                                    Uri location,
                                    OpenApiReaderSettings settings,
                                    CancellationToken cancellationToken)
        {
            if (input is null) throw new ArgumentNullException(nameof(input));
            if (settings is null) throw new ArgumentNullException(nameof(settings));
            cancellationToken.ThrowIfCancellationRequested();
            JsonNode jsonNode;

            // Parse the YAML text in the stream into a sequence of JsonNodes
            try
            {
                EnsureInputWithinLimit(input, _yamlSettings.MaxInputByteCount);
#if NET
// this represents net core, net5 and up
                using var stream = new StreamReader(input, default, true, -1, settings.LeaveStreamOpen);
#else
// the implementation differs and results in a null reference exception in NETFX
                using var stream = new StreamReader(input, Encoding.UTF8, true, 4096, settings.LeaveStreamOpen);
#endif
                jsonNode = LoadJsonNodesFromYamlDocument(stream, cancellationToken);
            }
            catch (JsonException ex)
            {
                return new()
                {
                    Document = null,
                    Diagnostic = CreateDiagnostic(new($"#line={ex.LineNumber}", ex.Message)),
                };
            }
            catch (OpenApiReaderException ex)
            {
                return new()
                {
                    Document = null,
                    Diagnostic = CreateDiagnostic(new(ex)),
                };
            }

            cancellationToken.ThrowIfCancellationRequested();
            return UpdateFormat(Read(jsonNode, location, settings));
        }

        private static async Task CopyToMemoryStreamAsync(
            Stream input,
            MemoryStream output,
            uint maxInputByteCount,
            CancellationToken cancellationToken)
        {
            var buffer = new byte[copyBufferSize];
            long totalBytesRead = 0;
            int bytesRead;
            while ((bytesRead = await input.ReadAsync(
                buffer,
                0,
                buffer.Length,
                cancellationToken).ConfigureAwait(false)) > 0)
            {
                if (bytesRead > (long)maxInputByteCount - totalBytesRead)
                {
                    throw CreateInputLimitException(maxInputByteCount);
                }

                await output.WriteAsync(buffer, 0, bytesRead, cancellationToken).ConfigureAwait(false);
                totalBytesRead += bytesRead;
            }
        }

        private static void EnsureInputWithinLimit(MemoryStream input, uint maxInputByteCount)
        {
            if (input.Length - input.Position > maxInputByteCount)
            {
                throw CreateInputLimitException(maxInputByteCount);
            }
        }

        private static OpenApiReaderException CreateInputLimitException(uint maxInputByteCount)
            => new($"The YAML input exceeds the maximum supported size of {maxInputByteCount} bytes.");

        private static ReadResult UpdateFormat(ReadResult result)
        {
            result.Diagnostic ??= new OpenApiDiagnostic();
            result.Diagnostic.Format = OpenApiConstants.Yaml;
            return result;
        }

        /// <inheritdoc/>
        public static ReadResult Read(JsonNode jsonNode, Uri location, OpenApiReaderSettings settings)
        {
            return UpdateFormat(_jsonReader.Read(jsonNode, location, settings));
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
                EnsureInputWithinLimit(input, _yamlSettings.MaxInputByteCount);
#if NET
                using var stream = new StreamReader(input, default, true, -1, settings?.LeaveStreamOpen ?? false);
#else
                using var stream = new StreamReader(input, Encoding.UTF8, true, 4096, settings?.LeaveStreamOpen ?? false);
#endif
                jsonNode = LoadJsonNodesFromYamlDocument(stream, CancellationToken.None);
            }
            catch (JsonException ex)
            {
                diagnostic = CreateDiagnostic(new($"#line={ex.LineNumber}", ex.Message));
                return default;
            }
            catch (OpenApiReaderException ex)
            {
                diagnostic = CreateDiagnostic(new(ex));
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
        /// Converts the first YAML document in a stream into a JSON node.
        /// </summary>
        /// <param name="input">Stream containing YAML formatted text</param>
        /// <param name="cancellationToken">Propagates notification that parsing should be cancelled.</param>
        /// <returns>The converted JSON node.</returns>
        private JsonNode LoadJsonNodesFromYamlDocument(TextReader input, CancellationToken cancellationToken)
        {
            try
            {
                return new YamlJsonParser(_yamlSettings).Parse(input, cancellationToken);
            }
            catch (YamlException ex)
            {
                var location = ex.Start.Line >= 0
                    ? $" at line {ex.Start.Line + 1}, column {ex.Start.Column + 1}"
                    : string.Empty;
                throw new OpenApiReaderException($"Unable to parse the YAML document{location}: {ex.Message}", ex);
            }
        }

        private static OpenApiDiagnostic CreateDiagnostic(OpenApiError error)
        {
            var diagnostic = new OpenApiDiagnostic
            {
                Format = OpenApiConstants.Yaml,
            };
            diagnostic.Errors.Add(error);
            return diagnostic;
        }
    }
}
