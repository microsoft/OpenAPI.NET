// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.IO;
using System.Linq;
using Microsoft.OpenApi.Readers.Interface;
using SharpYaml;
using SharpYaml.Serialization;

namespace Microsoft.OpenApi.Readers.YamlReaders
{
    /// <summary>
    /// Service class for converting streams into OpenApiDocument instances
    /// </summary>
    public class OpenApiStreamReader : IOpenApiReader<Stream, ParsingContext>
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
            if (versionNode != null)
            {
                return versionNode.GetScalarValue();
            }

            return null;
        }

        /// <summary>
        /// Reads the stream input and parses it into an Open API document.
        /// </summary>
        public OpenApiDocument Read(Stream input, out ParsingContext context)
        {
            RootNode rootNode;
            context = new ParsingContext();

            try
            {
                var yamlStream = new YamlStream();
                yamlStream.Load(new StreamReader(input));

                var yamlDocument = yamlStream.Documents.First();
                rootNode = new RootNode(context, yamlDocument);
            }
            catch (SyntaxErrorException ex)
            {
                context.ParseErrors.Add(new OpenApiError("", ex.Message));
                context.OpenApiDocument = new OpenApiDocument(); // Could leave this null?

                return context.OpenApiDocument;
            }

            var inputVersion = GetVersion(rootNode);

            switch (inputVersion)
            {
                case "2.0":
                    context.SetReferenceService(
                        new ReferenceService(rootNode)
                        {
                            loadReference = OpenApiV2Builder.LoadReference,
                            parseReference = p => OpenApiV2Builder.ParseReference(p)
                        });
                    context.OpenApiDocument = OpenApiV2Builder.LoadOpenApi(rootNode);
                    break;
                default:
                    context.SetReferenceService(
                        new ReferenceService(rootNode)
                        {
                            loadReference = OpenApiV3Builder.LoadReference,
                            parseReference = p => new OpenApiReference(p)
                        });
                    context.OpenApiDocument = OpenApiV3Builder.LoadOpenApi(rootNode);
                    break;
            }

            return context.OpenApiDocument;
        }
    }
}