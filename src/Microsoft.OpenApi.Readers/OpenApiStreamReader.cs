// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.IO;
using System.Linq;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers.Interface;
using SharpYaml;
using SharpYaml.Serialization;

namespace Microsoft.OpenApi.Readers
{
    /// <summary>
    /// Service class for converting streams into OpenApiDocument instances
    /// </summary>
    public class OpenApiStreamReader : IOpenApiReader<Stream, OpenApiDiagnostic>
    {

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

            try
            {
                yamlDocument = LoadYamlDocument(input);
            }
            catch (SyntaxErrorException ex)
            {
                diagnostic.Errors.Add(new OpenApiError(string.Empty, ex.Message));
                return new OpenApiDocument();
            }

            context = new ParsingContext();
            return context.Parse(yamlDocument, diagnostic);
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