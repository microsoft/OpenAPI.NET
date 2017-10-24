// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.IO;
using System.Linq;
using Microsoft.OpenApi.Readers.Interface;
using Microsoft.OpenApi.Readers.YamlReaders.ParseNodes;
using SharpYaml;
using SharpYaml.Serialization;

namespace Microsoft.OpenApi.Readers.YamlReaders
{
    /// <summary>
    /// Service class for converting streams into OpenApiDocument instances
    /// </summary>
    public class OpenApiStreamReader : IOpenApiReader<Stream, OpenApiDiagnostic>
    {
        /// <summary>
        /// Gets the version of the Open API document.
        /// </summary>
        private static string GetVersion(RootNode rootNode)
        {
            var versionNode = rootNode.Find(new JsonPointer("/openapi"));

            if (versionNode != null)
            {
                return versionNode.GetScalarValue();
            }

            versionNode = rootNode.Find(new JsonPointer("/swagger"));

            return versionNode?.GetScalarValue();
        }

        /// <summary>
        /// Reads the stream input and parses it into an Open API document.
        /// </summary>
        public OpenApiDocument Read(Stream input, out OpenApiDiagnostic diagnostic)
        {
            RootNode rootNode;
            var context = new ParsingContext();
            diagnostic = new OpenApiDiagnostic();

                try
                {
                    using (var streamReader = new StreamReader(input))
                    {
                        var yamlStream = new YamlStream();
                        yamlStream.Load(streamReader);

                        var yamlDocument = yamlStream.Documents.First();
                        rootNode = new RootNode(context, diagnostic, yamlDocument);
                    }
                }
                catch (SyntaxErrorException ex)
                {
                    diagnostic.Errors.Add(new OpenApiError(string.Empty, ex.Message));

                    return new OpenApiDocument();
                }

                var inputVersion = GetVersion(rootNode);

                switch (inputVersion)
                {
                    case "2.0":
                        context.SetReferenceService(
                            new OpenApiReferenceService(rootNode)
                            {
                                loadReference = OpenApiV2Deserializer.LoadReference,
                                parseReference = p => OpenApiV2Deserializer.ParseReference(p)
                            });

                        return OpenApiV2Deserializer.LoadOpenApi(rootNode);

                    default:
                        context.SetReferenceService(
                            new OpenApiReferenceService(rootNode)
                            {
                                loadReference = OpenApiV3Deserializer.LoadReference,
                                parseReference = p => new OpenApiReference(p)
                            });

                        return OpenApiV3Deserializer.LoadOpenApi(rootNode);
                }
        }
    }
}