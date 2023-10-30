// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.OpenApi.Exceptions;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers.Interface;
using Microsoft.OpenApi.Readers.Services;
using Microsoft.OpenApi.Services;
using Microsoft.OpenApi.Validations;
using SharpYaml.Serialization;

namespace Microsoft.OpenApi.Readers
{
    /// <summary>
    /// Service class for converting contents of TextReader into OpenApiDocument instances
    /// </summary>
    internal class OpenApiYamlDocumentReader : IOpenApiReader<YamlDocument, OpenApiDiagnostic>
    {
        private readonly OpenApiReaderSettings _settings;

        /// <summary>
        /// Create stream reader with custom settings if desired.
        /// </summary>
        /// <param name="settings"></param>
        public OpenApiYamlDocumentReader(OpenApiReaderSettings settings = null)
        {
            _settings = settings ?? new OpenApiReaderSettings();
        }

        /// <summary>
        /// Reads the stream input and parses it into an Open API document.
        /// </summary>
        /// <param name="input">TextReader containing OpenAPI description to parse.</param>
        /// <param name="diagnostic">Returns diagnostic object containing errors detected during parsing</param>
        /// <returns>Instance of newly created OpenApiDocument</returns>
        public OpenApiDocument Read(YamlDocument input, out OpenApiDiagnostic diagnostic)
        {
            diagnostic = new();
            var context = new ParsingContext(diagnostic)
            {
                ExtensionParsers = _settings.ExtensionParsers,
                BaseUrl = _settings.BaseUrl,
                DefaultContentType = _settings.DefaultContentType
            };

            OpenApiDocument document = null;
            try
            {
                // Parse the OpenAPI Document
                document = context.Parse(input);

                if (_settings.LoadExternalRefs)
                {
                    throw new InvalidOperationException("Cannot load external refs using the synchronous Read, use ReadAsync instead.");
                }

                ResolveReferences(diagnostic, document);
            }
            catch (OpenApiException ex)
            {
                diagnostic.Errors.Add(new(ex));
            }

            // Validate the document
            if (_settings.RuleSet != null && _settings.RuleSet.Rules.Count > 0)
            {
                var openApiErrors = document.Validate(_settings.RuleSet);
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

        public async Task<ReadResult> ReadAsync(YamlDocument input, CancellationToken cancellationToken = default)
        {
            var diagnostic = new OpenApiDiagnostic();
            var context = new ParsingContext(diagnostic)
            {
                ExtensionParsers = _settings.ExtensionParsers,
                BaseUrl = _settings.BaseUrl
            };

            OpenApiDocument document = null;
            try
            {
                // Parse the OpenAPI Document
                document = context.Parse(input);

                if (_settings.LoadExternalRefs)
                {
                    var diagnosticExternalRefs = await LoadExternalRefs(document, cancellationToken);
                    // Merge diagnostics of external reference
                    if (diagnosticExternalRefs != null)
                    {
                        diagnostic.Errors.AddRange(diagnosticExternalRefs.Errors);
                        diagnostic.Warnings.AddRange(diagnosticExternalRefs.Warnings);
                    }
                }

                ResolveReferences(diagnostic, document);
            }
            catch (OpenApiException ex)
            {
                diagnostic.Errors.Add(new(ex));
            }

            // Validate the document
            if (_settings.RuleSet != null && _settings.RuleSet.Rules.Count > 0)
            {
                var openApiErrors = document.Validate(_settings.RuleSet);
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

        private Task<OpenApiDiagnostic> LoadExternalRefs(OpenApiDocument document, CancellationToken cancellationToken = default)
        {
            // Create workspace for all documents to live in.
            var openApiWorkSpace = new OpenApiWorkspace();

            // Load this root document into the workspace
            var streamLoader = new DefaultStreamLoader(_settings.BaseUrl);
            var workspaceLoader = new OpenApiWorkspaceLoader(openApiWorkSpace, _settings.CustomExternalLoader ?? streamLoader, _settings);
            return workspaceLoader.LoadAsync(new() { ExternalResource = "/" }, document, null, cancellationToken);
        }

        private void ResolveReferences(OpenApiDiagnostic diagnostic, OpenApiDocument document)
        {
            var errors = new List<OpenApiError>();

            // Resolve References if requested
            switch (_settings.ReferenceResolution)
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

        /// <summary>
        /// Reads the stream input and parses the fragment of an OpenAPI description into an Open API Element.
        /// </summary>
        /// <param name="input">TextReader containing OpenAPI description to parse.</param>
        /// <param name="version">Version of the OpenAPI specification that the fragment conforms to.</param>
        /// <param name="diagnostic">Returns diagnostic object containing errors detected during parsing</param>
        /// <returns>Instance of newly created OpenApiDocument</returns>
        public T ReadFragment<T>(YamlDocument input, OpenApiSpecVersion version, out OpenApiDiagnostic diagnostic) where T : IOpenApiElement
        {
            diagnostic = new();
            var context = new ParsingContext(diagnostic)
            {
                ExtensionParsers = _settings.ExtensionParsers
            };

            IOpenApiElement element = null;
            try
            {
                // Parse the OpenAPI element
                element = context.ParseFragment<T>(input, version);
            }
            catch (OpenApiException ex)
            {
                diagnostic.Errors.Add(new(ex));
            }

            // Validate the element
            if (_settings.RuleSet != null && _settings.RuleSet.Rules.Count > 0)
            {
                var errors = element.Validate(_settings.RuleSet);
                foreach (var item in errors)
                {
                    diagnostic.Errors.Add(item);
                }
            }

            return (T)element;
        }
    }
}
