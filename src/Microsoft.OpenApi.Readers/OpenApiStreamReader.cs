// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers.Interface;
using Microsoft.OpenApi.Readers.Services;
using Microsoft.OpenApi.Services;
using Microsoft.OpenApi.Validations;
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
            diagnostic = new OpenApiDiagnostic();

            // Parse the OpenAPI Document
            OpenApiDocument document = ParseStream(input, diagnostic);

            // Resolve References if requested
            switch (_settings.ReferenceResolution)
            {
                case ReferenceResolutionSetting.ResolveRemoteReferences:
                    throw new ArgumentException("Cannot resolve remote references using synchronous Read method, use ReadAsync instead");
                case ReferenceResolutionSetting.ResolveLocalReferences:
                    ResolveLocalReferences(document);
                    break;
                case
                    ReferenceResolutionSetting.DoNotResolveReferences:
                    break;
            }

            ValidateDocument(diagnostic, document);

            return document;
        }

        /// <summary>
        /// Reads the stream input and parses it into an Open API document. 
        /// Remote references are also loaded as OpenApiDocuments and OpenApiElements and stored in the OpenApiWorkspace
        /// </summary>
        /// <param name="input">Stream containing OpenAPI description to parse.</param>
        /// <param name="diagnostic">Returns diagnostic object containing errors detected during parsing.  Caller must provide an instance.</param>
        /// <param name="openApiWorkspace">Container for the set of OpenAPIDocuments and OpenApiElements referenced. Provide a previously used workspace to avoid re-parsing common documents.</param>
        /// <returns></returns>
        /// <remarks>Will be made public once finalized.</remarks>
        internal async Task<OpenApiDocument> ReadAsync(Stream input, OpenApiDiagnostic diagnostic, OpenApiWorkspace openApiWorkspace = null) 
        {

            // Parse the OpenAPI Document
            OpenApiDocument document = ParseStream(input, diagnostic);

            // Resolve References if requested
            switch (_settings.ReferenceResolution)
            {
                case ReferenceResolutionSetting.ResolveRemoteReferences:
                    await ResolveAllReferences(document);
                    break;
                case ReferenceResolutionSetting.ResolveLocalReferences:
                    ResolveLocalReferences(document);
                    break;
                case
                    ReferenceResolutionSetting.DoNotResolveReferences:
                    break;
            }

            ValidateDocument(diagnostic, document);

            return document;
        }


        private OpenApiDocument ParseStream(Stream input, OpenApiDiagnostic diagnostic)
        {
            ParsingContext context;
            YamlDocument yamlDocument;

            // Parse the YAML/JSON
            try
            {
                yamlDocument = LoadYamlDocument(input);
            }
            catch (SyntaxErrorException ex)
            {
                diagnostic.Errors.Add(new OpenApiError(string.Empty, ex.Message));
                return new OpenApiDocument();
            }

            context = new ParsingContext
            {
                ExtensionParsers = _settings.ExtensionParsers
            };

            // Parse the OpenAPI Document
            return context.Parse(yamlDocument, diagnostic);
        }

        private void ValidateDocument(OpenApiDiagnostic diagnostic, OpenApiDocument document)
        {
            // Validate the document
            var errors = document.Validate(_settings.RuleSet);
            foreach (var item in errors)
            {
                diagnostic.Errors.Add(new OpenApiError(item.ErrorPath, item.ErrorMessage));
            }
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

        private static async Task ResolveAllReferences(OpenApiDocument document)
        {
            // Collect remote references by walking document
            var referenceCollector = new OpenApiRemoteReferenceCollector();
            var collectorWalker = new OpenApiWalker(referenceCollector);

            // Load References into a workspace
            var openApiWorkspace = new OpenApiWorkspace();
            await openApiWorkspace.LoadAsync(referenceCollector.References);

            // Attach Workspace to primary document
            document.Workspace = openApiWorkspace;

            // Load all remote references into OpenApiWorkspace
            var resolver = new OpenApiReferenceResolver(document, true);
            var walker = new OpenApiWalker(resolver);
            walker.Walk(document);
        }

        private static void ResolveLocalReferences(OpenApiDocument document)
        {
            var resolver = new OpenApiReferenceResolver(document, false);
            var walker = new OpenApiWalker(resolver);
            walker.Walk(document);
        }

    }
}