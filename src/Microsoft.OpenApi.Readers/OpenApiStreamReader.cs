// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.IO;
using System.Linq;
using Microsoft.OpenApi.Exceptions;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers.Interface;
using Microsoft.OpenApi.Readers.Services;
using Microsoft.OpenApi.Services;
using SharpYaml;
using SharpYaml.Serialization;

namespace Microsoft.OpenApi.Readers
{
    /// <summary>
    /// Service class for converting streams into OpenApiDocument instances
    /// </summary>
    public class OpenApiStreamReader : IOpenApiReader<Stream, OpenApiDiagnostic>
    {
        private OpenApiReaderSettings _settings;

        /// <summary>
        /// Create stream reader with custom settings if desired.
        /// </summary>
        /// <param name="settings"></param>
        public OpenApiStreamReader(OpenApiReaderSettings settings = null)
        {
            _settings = settings ?? new OpenApiReaderSettings();

        }
        /// <summary>
        /// Reads the stream input and parses it into an Open API document.
        /// </summary>
        /// <param name="input">Stream containing OpenAPI description to parse.</param>
        /// <param name="diagnostic">Returns diagnostic object containing errors detected during parsing</param>
        /// <returns>Instance of newly created OpenApiDocument</returns>
        public OpenApiDocument Read(Stream input, out OpenApiDiagnostic diagnostic)
        {
            ParsingContext context;
            YamlDocument yamlDocument;
            diagnostic = new OpenApiDiagnostic();

            // Parse the YAML/JSON
            try
            {
                yamlDocument = LoadYamlDocument(input);
            }
            catch (YamlException ex)
            {
                diagnostic.Errors.Add(new OpenApiError($"#char={ex.Start.Line}", ex.Message));
                return new OpenApiDocument();
            }

            context = new ParsingContext
            {
                ExtensionParsers = _settings.ExtensionParsers,
                BaseUrl = _settings.BaseUrl
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
        /// Reads the stream input and parses the fragment of an OpenAPI description into an Open API Element.
        /// </summary>
        /// <param name="input">Stream containing OpenAPI description to parse.</param>
        /// <param name="version">Version of the OpenAPI specification that the fragment conforms to.</param>
        /// <param name="diagnostic">Returns diagnostic object containing errors detected during parsing</param>
        /// <returns>Instance of newly created OpenApiDocument</returns>
        public T ReadFragment<T>(Stream input, OpenApiSpecVersion version, out OpenApiDiagnostic diagnostic) where T : IOpenApiElement
        {
            ParsingContext context;
            YamlDocument yamlDocument;
            diagnostic = new OpenApiDiagnostic();

            // Parse the YAML/JSON
            try
            {
                yamlDocument = LoadYamlDocument(input);
            }
            catch (YamlException ex)
            {
                diagnostic.Errors.Add(new OpenApiError($"#line={ex.Start.Line}", ex.Message));
                return default(T);
            }

            context = new ParsingContext
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

        /// <summary>
        /// Helper method to turn streams into YamlDocument
        /// </summary>
        /// <param name="input">Stream containing YAML formatted text</param>
        /// <returns>Instance of a YamlDocument</returns>
        internal static YamlDocument LoadYamlDocument(Stream input)
        {
            YamlDocument yamlDocument;
            using (var streamReader = new StreamReader(input))
            {
                var yamlStream = new YamlStream();
                yamlStream.Load(streamReader);
                yamlDocument = yamlStream.Documents.First();
            }
            return yamlDocument;
        }
    }
}