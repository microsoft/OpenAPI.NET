// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.IO;
using System.Text.Json.Nodes;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Exceptions;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Validations;
using System.Linq;
using System.Net.Http;
using System.Collections.Generic;
using Microsoft.OpenApi.Services;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Reader.Services;
using System.Security;

namespace Microsoft.OpenApi.Reader
{
    /// <summary>
    /// A reader class for parsing JSON files into Open API documents.
    /// </summary>
    public class OpenApiJsonReader : IOpenApiReader
    {
        private static readonly HttpClient _httpClient = HttpClientFactory.GetHttpClient();

        /// <summary>
        /// Takes in an input URL and parses it into an Open API document
        /// </summary>
        /// <param name="url">The path to the Open API file</param>
        /// <param name="diagnostic">Returns diagnostic object containing errors detected during parsing.</param>
        /// <param name="settings">The Reader settings to be used during parsing.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public OpenApiDocument Read(string url, out OpenApiDiagnostic diagnostic, OpenApiReaderSettings settings = null)
        {
            Stream stream;
            if (url.StartsWith("http", StringComparison.OrdinalIgnoreCase) || url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    stream = _httpClient.GetStreamAsync(new Uri(url)).GetAwaiter().GetResult();
                }
                catch (HttpRequestException ex)
                {
                    throw new InvalidOperationException($"Could not download the file at {url}", ex);
                }
            }
            else
            {
                try
                {
                    var fileInput = new FileInfo(url);
                    stream = fileInput.OpenRead();
                }
                catch (Exception ex) when (
                    ex is
                        FileNotFoundException or
                        PathTooLongException or
                        DirectoryNotFoundException or
                        IOException or
                        UnauthorizedAccessException or
                        SecurityException or
                        NotSupportedException)
                {
                    throw new InvalidOperationException($"Could not open the file at {url}", ex);
                }
            }

            return Read(stream, out diagnostic, settings);
        }

        /// <summary>
        /// Reads the stream input and parses it into an Open API document.
        /// </summary>
        /// <param name="stream">The input stream.</param>
        /// <param name="diagnostic">Returns diagnostic object containing errors detected during parsing.</param>
        /// <param name="settings">The Reader settings to be used during parsing.</param>
        /// <returns></returns>
        public OpenApiDocument Read(Stream stream, out OpenApiDiagnostic diagnostic, OpenApiReaderSettings settings =  null)
        {
            var reader = new StreamReader(stream);
            var result = Read(reader, out diagnostic, settings);
            if ((bool)!settings?.LeaveStreamOpen)
            {
                reader.Dispose();
            }

            return result;
        }

        /// <summary>
        /// Reads the stream input and parses it into an Open API document.
        /// </summary>
        /// <param name="input">TextReader containing OpenAPI description to parse.</param>
        /// <param name="diagnostic">Returns diagnostic object containing errors detected during parsing.</param>
        /// <param name="settings">The Reader settings to be used during parsing.</param>
        /// <returns></returns>
        public OpenApiDocument Read(TextReader input, out OpenApiDiagnostic diagnostic, OpenApiReaderSettings settings = null)
        {
            JsonNode jsonNode;

            // Parse the YAML/JSON text in the TextReader into Json Nodes
            try
            {
                jsonNode = LoadJsonNodesFromJsonDocument(input);
            }
            catch (JsonException ex)
            {
                diagnostic = new OpenApiDiagnostic();
                diagnostic.Errors.Add(new OpenApiError($"#line={ex.LineNumber}", ex.Message));
                return new OpenApiDocument();
            }

            return Read(jsonNode, out diagnostic, settings);
        }

        /// <summary>
        /// Takes in an input URL and parses it into an Open API document.
        /// </summary>
        /// <param name="url">The path to the Open API file</param>
        /// <param name="settings">The Reader settings to be used during parsing.</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task<ReadResult> ReadAsync(string url, OpenApiReaderSettings settings = null, CancellationToken cancellationToken = default)
        {
            Stream stream;
            if (url.StartsWith("http", StringComparison.OrdinalIgnoreCase) || url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    stream = await _httpClient.GetStreamAsync(new Uri(url));
                }
                catch (HttpRequestException ex)
                {
                    throw new InvalidOperationException($"Could not download the file at {url}", ex);
                }
            }
            else
            {
                try
                {
                    var fileInput = new FileInfo(url);
                    stream = fileInput.OpenRead();
                }
                catch (Exception ex) when (
                    ex is
                        FileNotFoundException or
                        PathTooLongException or
                        DirectoryNotFoundException or
                        IOException or
                        UnauthorizedAccessException or
                        SecurityException or
                        NotSupportedException)
                {
                    throw new InvalidOperationException($"Could not open the file at {url}", ex);
                }
            }

            return await ReadAsync(stream, settings, cancellationToken);
        }

        /// <summary>
        /// Reads the input stream and parses it into an Open API document.
        /// </summary>
        /// <param name="input">TextReader containing OpenAPI description to parse.</param>
        /// <param name="settings">The Reader settings to be used during parsing.</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ReadResult> ReadAsync(Stream input, OpenApiReaderSettings settings = null, CancellationToken cancellationToken = default)
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
                bufferedStream = new MemoryStream();
                await input.CopyToAsync(bufferedStream, 81920, cancellationToken);
                bufferedStream.Position = 0;
            }

            using var reader = new StreamReader(bufferedStream);
            return await ReadAsync(reader, settings, cancellationToken);
        }

        /// <summary>
        /// Reads the stream input and parses it into an Open API document.
        /// </summary>
        /// <param name="input">TextReader containing OpenAPI description to parse.</param>
        /// <param name="settings">The Reader settings to be used during parsing.</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ReadResult> ReadAsync(TextReader input,
                                                OpenApiReaderSettings settings = null,
                                                CancellationToken cancellationToken = default)
        {
            JsonNode jsonNode;
            var diagnostic = new OpenApiDiagnostic();

            // Parse the YAML/JSON text in the TextReader into the YamlDocument
            try
            {
                jsonNode = LoadJsonNodesFromJsonDocument(input);
            }
            catch (JsonException ex)
            {
                diagnostic.Errors.Add(new OpenApiError($"#line={ex.LineNumber}", ex.Message));
                return new ReadResult
                {
                    OpenApiDocument = null,
                    OpenApiDiagnostic = diagnostic
                };
            }

            return await ReadAsync(jsonNode, settings, cancellationToken);            
        }

        /// <summary>
        /// Parses an input string into an Open API document.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="diagnostic"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public OpenApiDocument Parse(string input, out OpenApiDiagnostic diagnostic, OpenApiReaderSettings settings = null)
        {
            using var reader = new StringReader(input);
            return Read(reader, out diagnostic, settings);
        }

        private JsonNode LoadJsonNodesFromJsonDocument(TextReader input)
        {
            var nodes = JsonNode.Parse(input.ReadToEnd());
            return nodes;
        }

        private OpenApiDocument Read(JsonNode input, out OpenApiDiagnostic diagnostic, OpenApiReaderSettings settings = null)
        {
            diagnostic = new OpenApiDiagnostic();
            var context = new ParsingContext(diagnostic)
            {
                ExtensionParsers = settings?.ExtensionParsers,
                BaseUrl = settings?.BaseUrl
            };

            OpenApiDocument document = null;
            try
            {
                // Parse the OpenAPI Document
                document = context.Parse(input);

                if ((bool)(settings?.LoadExternalRefs))
                {
                    throw new InvalidOperationException("Cannot load external refs using the synchronous Read, use ReadAsync instead.");
                }

                ResolveReferences(diagnostic, document);
            }
            catch (OpenApiException ex)
            {
                diagnostic.Errors.Add(new OpenApiError(ex));
            }

            // Validate the document
            if (settings?.RuleSet != null && settings?.RuleSet.Rules.Count() > 0)
            {
                var openApiErrors = document.Validate(settings?.RuleSet);
                foreach (var item in openApiErrors.OfType<OpenApiValidatorError>())
                {
                    diagnostic.Errors.Add(item);
                }
                foreach (var item in openApiErrors.OfType<OpenApiValidatorWarning>())
                {
                    diagnostic.Warnings.Add(item);
                }
            }

            return document;
        }

        private async Task<ReadResult> ReadAsync(JsonNode jsonNode,
                                                 OpenApiReaderSettings settings = null,
                                                 CancellationToken cancellationToken = default)
        {
            var diagnostic = new OpenApiDiagnostic();
            var context = new ParsingContext(diagnostic)
            {
                ExtensionParsers = settings?.ExtensionParsers,
                BaseUrl = settings?.BaseUrl
            };

            OpenApiDocument document = null;
            try
            {
                // Parse the OpenAPI Document
                document = context.Parse(jsonNode);

                if ((bool)(settings?.LoadExternalRefs))
                {
                    await LoadExternalRefs(document, cancellationToken, settings);
                }

                ResolveReferences(diagnostic, document, settings);
            }
            catch (OpenApiException ex)
            {
                diagnostic.Errors.Add(new OpenApiError(ex));
            }

            // Validate the document
            if (settings?.RuleSet != null && settings?.RuleSet.Rules.Count() > 0)
            {
                var openApiErrors = document.Validate(settings?.RuleSet);
                foreach (var item in openApiErrors.OfType<OpenApiValidatorError>())
                {
                    diagnostic.Errors.Add(item);
                }
                foreach (var item in openApiErrors.OfType<OpenApiValidatorWarning>())
                {
                    diagnostic.Warnings.Add(item);
                }
            }

            return new ReadResult()
            {
                OpenApiDocument = document,
                OpenApiDiagnostic = diagnostic
            };
        }

        private void ResolveReferences(OpenApiDiagnostic diagnostic, OpenApiDocument document, OpenApiReaderSettings settings = null)
        {
            List<OpenApiError> errors = new();

            // Resolve References if requested
            switch (settings?.ReferenceResolution)
            {
                case ReferenceResolutionSetting.ResolveAllReferences:
                    throw new ArgumentException("Resolving external references is not supported");
                case ReferenceResolutionSetting.ResolveLocalReferences:
                    errors.AddRange(document.ResolveReferences());
                    break;
                case ReferenceResolutionSetting.DoNotResolveReferences:
                    break;
            }

            foreach (var item in errors)
            {
                diagnostic.Errors.Add(item);
            }
        }

        private async Task LoadExternalRefs(OpenApiDocument document, CancellationToken cancellationToken, OpenApiReaderSettings settings = null)
        {
            // Create workspace for all documents to live in.
            var openApiWorkSpace = new OpenApiWorkspace();

            // Load this root document into the workspace
            var streamLoader = new DefaultStreamLoader(settings?.BaseUrl);
            var workspaceLoader = new OpenApiWorkspaceLoader(openApiWorkSpace, settings?.CustomExternalLoader ?? streamLoader, settings);
            await workspaceLoader.LoadAsync(new OpenApiReference() { ExternalResource = "/" }, document, OpenApiConstants.Json, null, cancellationToken);
        }
    }
}
