// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

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
using Microsoft.OpenApi.Services;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Reader.Services;
using System;

namespace Microsoft.OpenApi.Reader
{
    /// <summary>
    /// A reader class for parsing JSON files into Open API documents.
    /// </summary>
    public class OpenApiJsonReader : IOpenApiReader
    {
        /// <summary>
        /// Reads the stream input and parses it into an Open API document.
        /// </summary>
        /// <param name="input">TextReader containing OpenAPI description to parse.</param>
        /// <param name="settings">The Reader settings to be used during parsing.</param>
        /// <param name="cancellationToken">Propagates notifications that operations should be cancelled.</param>
        /// <returns></returns>
        public async Task<ReadResult> ReadAsync(TextReader input,
                                                OpenApiReaderSettings settings = null,
                                                CancellationToken cancellationToken = default)
        {
            JsonNode jsonNode;
            var diagnostic = new OpenApiDiagnostic();
            settings ??= new OpenApiReaderSettings();

            // Parse the JSON text in the TextReader into JsonNodes
            try
            {
                jsonNode = await LoadJsonNodesAsync(input);
            }
            catch (JsonException ex)
            {
                diagnostic.Errors.Add(new OpenApiError($"#line={ex.LineNumber}", $"Please provide the correct format, {ex.Message}"));
                return new ReadResult
                {
                    OpenApiDocument = null,
                    OpenApiDiagnostic = diagnostic
                };
            }

            return await ReadAsync(jsonNode, settings, cancellationToken: cancellationToken);            
        }

        /// <summary>
        /// Parses the JsonNode input into an Open API document.
        /// </summary>
        /// <param name="jsonNode">The JsonNode input.</param>
        /// <param name="settings">The Reader settings to be used during parsing.</param>
        /// <param name="cancellationToken">Propagates notifications that operations should be cancelled.</param>
        /// <returns></returns>
        public async Task<ReadResult> ReadAsync(JsonNode jsonNode,                                                
                                                OpenApiReaderSettings settings,
                                                CancellationToken cancellationToken = default)
        {
            var diagnostic = new OpenApiDiagnostic();
            var context = new ParsingContext(diagnostic)
            {
                ExtensionParsers = settings.ExtensionParsers,
                BaseUrl = settings.BaseUrl,
                DefaultContentType = settings.DefaultContentType
            };

            OpenApiDocument document = null;
            try
            {
                // Parse the OpenAPI Document
                document = context.Parse(jsonNode);

                if (settings.LoadExternalRefs)
                {
                    var diagnosticExternalRefs = await LoadExternalRefsAsync(document, cancellationToken, settings);
                    // Merge diagnostics of external reference
                    if (diagnosticExternalRefs != null)
                    {
                        diagnostic.Errors.AddRange(diagnosticExternalRefs.Errors);
                        diagnostic.Warnings.AddRange(diagnosticExternalRefs.Warnings);
                    }
                }

                document.SetReferenceHostDocument();
            }
            catch (OpenApiException ex)
            {
                diagnostic.Errors.Add(new(ex));
            }

            // Validate the document
            if (settings.RuleSet != null && settings.RuleSet.Rules.Any())
            {
                var openApiErrors = document.Validate(settings.RuleSet);
                foreach (var item in openApiErrors.OfType<OpenApiValidatorError>())
                {
                    diagnostic.Errors.Add(item);
                }
                foreach (var item in openApiErrors.OfType<OpenApiValidatorWarning>())
                {
                    diagnostic.Warnings.Add(item);
                }
            }

            return new()
            {
                OpenApiDocument = document,
                OpenApiDiagnostic = diagnostic
            };
        }

        /// <inheritdoc/>
        public async Task<ReadFragmentResult<T>> ReadFragmentAsync<T>(TextReader input,
                                                                      OpenApiSpecVersion version,
                                                                      OpenApiReaderSettings settings = null) where T: IOpenApiElement
        {
            JsonNode jsonNode;

            // Parse the JSON
            try
            {
                jsonNode = await LoadJsonNodesAsync(input);
            }
            catch (JsonException ex)
            {
                var diagnostic = new OpenApiDiagnostic();
                diagnostic.Errors.Add(new($"#line={ex.LineNumber}", ex.Message));
                return default;
            }

            return ReadFragment<T>(jsonNode, version, settings);
        }

        /// <inheritdoc/>
        public ReadFragmentResult<T> ReadFragment<T>(JsonNode input,
                                                     OpenApiSpecVersion version,
                                                     OpenApiReaderSettings settings = null) where T : IOpenApiElement
        {
            var diagnostic = new OpenApiDiagnostic();
            settings ??= new OpenApiReaderSettings();
            var context = new ParsingContext(diagnostic)
            {
                ExtensionParsers = settings.ExtensionParsers
            };

            IOpenApiElement element = null;
            try
            {
                // Parse the OpenAPI element asynchronously
                element = await Task.Run(() => context.ParseFragment<T>(input, version));
            }
            catch (OpenApiException ex)
            {
                diagnostic.Errors.Add(new(ex));
            }

            // Validate the element
            if (settings.RuleSet != null && settings.RuleSet.Rules.Any())
            {
                var errors = element.Validate(settings.RuleSet);
                foreach (var item in errors)
                {
                    diagnostic.Errors.Add(item);
                }
            }

            return new ReadFragmentResult<T>
            {
                Element = (T)element,
                OpenApiDiagnostic = diagnostic
            };
        }

        private async Task<JsonNode> LoadJsonNodesAsync(TextReader input)
        {
            var content = await input.ReadToEndAsync();
            return JsonNode.Parse(content);
        }

        private async Task<OpenApiDiagnostic> LoadExternalRefsAsync(OpenApiDocument document, CancellationToken cancellationToken, OpenApiReaderSettings settings)
        {
            // Create workspace for all documents to live in.
            var baseUrl = settings.BaseUrl ?? new Uri(OpenApiConstants.BaseRegistryUri);
            var openApiWorkSpace = new OpenApiWorkspace(baseUrl);

            // Load this root document into the workspace
            var streamLoader = new DefaultStreamLoader(settings.BaseUrl);
            var workspaceLoader = new OpenApiWorkspaceLoader(openApiWorkSpace, settings.CustomExternalLoader ?? streamLoader, settings);
            return await workspaceLoader.LoadAsync(new OpenApiReference() { ExternalResource = "/" }, document, null, cancellationToken);
        }
    }
}
