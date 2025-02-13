﻿// Copyright (c) Microsoft Corporation. All rights reserved.
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
using Microsoft.OpenApi.Interfaces;
using System;

namespace Microsoft.OpenApi.Reader
{
    /// <summary>
    /// A reader class for parsing JSON files into Open API documents.
    /// </summary>
    public class OpenApiJsonReader : IOpenApiReader
    {
        /// <summary>
        /// Reads the memory stream input and parses it into an Open API document.
        /// </summary>
        /// <param name="input">Memory stream containing OpenAPI description to parse.</param>
        /// <param name="settings">The Reader settings to be used during parsing.</param>
        /// <returns></returns>
        public ReadResult Read(MemoryStream input,
                               OpenApiReaderSettings settings)
        {
            if (input is null) throw new ArgumentNullException(nameof(input));
            if (settings is null) throw new ArgumentNullException(nameof(settings));

            JsonNode? jsonNode;
            var diagnostic = new OpenApiDiagnostic();
            settings ??= new OpenApiReaderSettings();

            // Parse the JSON text in the stream into JsonNodes
            try
            {
                jsonNode = JsonNode.Parse(input);
            }
            catch (JsonException ex)
            {
                diagnostic.Errors.Add(new OpenApiError($"#line={ex.LineNumber}", $"Please provide the correct format, {ex.Message}"));
                return new ReadResult
                {
                    Document = null,
                    Diagnostic = diagnostic
                };
            }

            return Read(jsonNode, settings);
        }

        /// <summary>
        /// Parses the JsonNode input into an Open API document.
        /// </summary>
        /// <param name="jsonNode">The JsonNode input.</param>
        /// <param name="settings">The Reader settings to be used during parsing.</param>
        /// <returns></returns>
        public ReadResult Read(JsonNode? jsonNode,
                               OpenApiReaderSettings settings)
        {
            if (jsonNode is null) throw new ArgumentNullException(nameof(jsonNode));
            if (settings is null) throw new ArgumentNullException(nameof(settings));

            var diagnostic = new OpenApiDiagnostic();
            var context = new ParsingContext(diagnostic)
            {
                ExtensionParsers = settings.ExtensionParsers,
                BaseUrl = settings.BaseUrl,
                DefaultContentType = settings.DefaultContentType
            };

            OpenApiDocument? document = null;
            try
            {
                // Parse the OpenAPI Document
                document = context.Parse(jsonNode);
                document.SetReferenceHostDocument();
            }
            catch (OpenApiException ex)
            {
                diagnostic.Errors.Add(new(ex));
            }

            // Validate the document
            if (settings.RuleSet != null && settings.RuleSet.Rules.Any())
            {
                var openApiErrors = document?.Validate(settings.RuleSet);
                if(openApiErrors is not null)
                {
                    foreach (var item in openApiErrors.OfType<OpenApiValidatorError>())
                    {
                        diagnostic.Errors.Add(item);
                    }
                    foreach (var item in openApiErrors.OfType<OpenApiValidatorWarning>())
                    {
                        diagnostic.Warnings.Add(item);
                    }
                }                
            }

            return new()
            {
                Document = document,
                Diagnostic = diagnostic
            };
        }

        /// <summary>
        /// Reads the stream input asynchronously and parses it into an Open API document.
        /// </summary>
        /// <param name="input">Memory stream containing OpenAPI description to parse.</param>
        /// <param name="settings">The Reader settings to be used during parsing.</param>
        /// <param name="cancellationToken">Propagates notifications that operations should be cancelled.</param>
        /// <returns></returns>
        public async Task<ReadResult> ReadAsync(Stream input,
                                                OpenApiReaderSettings settings,
                                                CancellationToken cancellationToken = default)
        {
            if (input is null) throw new ArgumentNullException(nameof(input));
            if (settings is null) throw new ArgumentNullException(nameof(settings));

            JsonNode? jsonNode;
            var diagnostic = new OpenApiDiagnostic();

            // Parse the JSON text in the stream into JsonNodes
            try
            {
                jsonNode = await JsonNode.ParseAsync(input, cancellationToken: cancellationToken).ConfigureAwait(false);
            }
            catch (JsonException ex)
            {
                diagnostic.Errors.Add(new OpenApiError($"#line={ex.LineNumber}", $"Please provide the correct format, {ex.Message}"));
                return new ReadResult
                {
                    Document = null,
                    Diagnostic = diagnostic
                };
            }

            return Read(jsonNode, settings);
        }

        /// <inheritdoc/>
        public T? ReadFragment<T>(MemoryStream input,
                                 OpenApiSpecVersion version,
                                 OpenApiDocument openApiDocument,
                                 out OpenApiDiagnostic diagnostic,
                                 OpenApiReaderSettings? settings = null) where T : IOpenApiElement
        {
            Utils.CheckArgumentNull(input);
            Utils.CheckArgumentNull(openApiDocument);

            JsonNode? jsonNode;

            // Parse the JSON
            try
            {
                jsonNode = JsonNode.Parse(input);
            }
            catch (JsonException ex)
            {
                diagnostic = new();
                diagnostic.Errors.Add(new($"#line={ex.LineNumber}", ex.Message));
                return default;
            }

            return ReadFragment<T>(jsonNode, version, openApiDocument, out diagnostic);
        }

        /// <inheritdoc/>
        public T ReadFragment<T>(JsonNode? input,
                 OpenApiSpecVersion version,
                 OpenApiDocument openApiDocument,
                 out OpenApiDiagnostic diagnostic,
                 OpenApiReaderSettings? settings = null) where T : IOpenApiElement
        {
            diagnostic = new();
            settings ??= new OpenApiReaderSettings();
            var context = new ParsingContext(diagnostic)
            {
                ExtensionParsers = settings.ExtensionParsers
            };

            IOpenApiElement? element = null;
            try
            {
                // Parse the OpenAPI element
                element = context.ParseFragment<T>(input, version, openApiDocument);
            }
            catch (OpenApiException ex)
            {
                diagnostic.Errors.Add(new(ex));
            }

            // Validate the element
            if (settings.RuleSet != null && settings.RuleSet.Rules.Any())
            {
                var errors = element?.Validate(settings.RuleSet);
                if (errors is not null)
                {
                    foreach (var item in errors)
                    {
                        diagnostic.Errors.Add(item);
                    }
                }
            }

            return (T?)element!;
        }
    }
}
