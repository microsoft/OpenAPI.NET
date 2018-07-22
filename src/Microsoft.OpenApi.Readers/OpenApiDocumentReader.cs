// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using Microsoft.OpenApi.Exceptions;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers.Interface;
using Microsoft.OpenApi.Readers.Services;
using Microsoft.OpenApi.Services;
using SharpYaml.Serialization;

namespace Microsoft.OpenApi.Readers
{
    /// <summary>
    /// Service class for converting Yaml Documents into OpenApiDocument instances
    /// </summary>
    public class OpenApiDocumentReader : IOpenApiReader<YamlDocument, OpenApiDiagnostic>
    {
        private readonly OpenApiReaderSettings _settings;

        /// <summary>
        /// Constructor tha allows reader to use non-default settings
        /// </summary>
        /// <param name="settings"></param>
        public OpenApiDocumentReader(OpenApiReaderSettings settings = null)
        {
            _settings = settings ?? new OpenApiReaderSettings();
        }

        /// <summary>
        /// Parses a YamlDocument into an Open API document.
        /// </summary>
        /// <param name="yamlDocument">Document containing Yaml File</param>
        /// <param name="diagnostic">Returns diagnostic object containing errors detected during parsing</param>
        /// <returns>Instance of newly created OpenApiDocument</returns>
        public OpenApiDocument Read(YamlDocument yamlDocument, out OpenApiDiagnostic diagnostic)
        {
            diagnostic = new OpenApiDiagnostic();
            ParsingContext context = new ParsingContext
            {
                ExtensionParsers = _settings.ExtensionParsers
            };

            OpenApiDocument document = null;

            try
            {
                // Parse the OpenAPI Document
                document = context.Parse(yamlDocument, diagnostic);

                // Resolve References if requested
                switch (_settings.ReferenceResolution)
                {
                    case ReferenceResolutionSetting.ResolveAllReferences:
                        throw new ArgumentException(Properties.SRResource.CannotResolveRemoteReferencesSynchronously);
                    case ReferenceResolutionSetting.ResolveLocalReferences:
                        var resolver = new OpenApiReferenceResolver(document);
                        var walker = new OpenApiWalker(resolver);
                        walker.Walk(document);
                        foreach (var item in resolver.Errors)
                        {
                            diagnostic.Errors.Add(item);
                        }
                        break;
                    case ReferenceResolutionSetting.DoNotResolveReferences:
                        break;
                }
            }
            catch (OpenApiException ex)
            {
                diagnostic.Errors.Add(new OpenApiError(ex));
            }

            // Validate the document
            if (_settings.RuleSet != null && _settings.RuleSet.Rules.Count > 0)
            {
                var errors = document.Validate(_settings.RuleSet);
                foreach (var item in errors)
                {
                    diagnostic.Errors.Add(item);
                }
            }

            return document;
        }


        /// <summary>
        /// Parses the fragment of an OpenAPI description into an Open API Element.
        /// </summary>
        /// <param name="yamlDocument">Yaml Document containing OpenAPI description</param>
        /// <param name="version">Version of the OpenAPI specification that the fragment conforms to.</param>
        /// <param name="diagnostic">Returns diagnostic object containing errors detected during parsing</param>
        /// <returns>Instance of newly created OpenApiDocument</returns>
        public T ReadFragment<T>(YamlDocument yamlDocument, OpenApiSpecVersion version,
            out OpenApiDiagnostic diagnostic) where T : IOpenApiElement
        {
            diagnostic = new OpenApiDiagnostic();
            ParsingContext context = new ParsingContext
            {
                ExtensionParsers = _settings.ExtensionParsers
            };

            IOpenApiElement element = null;

            try
            {
                // Parse the OpenAPI element
                element = context.ParseFragment<T>(yamlDocument, version, diagnostic);
            }
            catch (OpenApiException ex)
            {
                diagnostic.Errors.Add(new OpenApiError(ex));
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