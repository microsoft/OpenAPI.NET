// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.OData;
using Microsoft.OpenApi.Readers;
using Microsoft.OpenApi.Services;
using Microsoft.OpenApi.Validations;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Hidi
{
    public static class OpenApiService
    {
        public static void ProcessOpenApiDocument(
            string openapi,
            string csdl,
            FileInfo output,
            OpenApiSpecVersion? version,
            OpenApiFormat? format,
            string filterbyoperationids,
            string filterbytags,
            string filterbycollection,
            bool inline,
            bool resolveexternal)
        {
            if (string.IsNullOrEmpty(openapi) && string.IsNullOrEmpty(csdl))
            {
                throw new ArgumentNullException("Please input a file path");
            }
            if(output == null)
            {
                throw new ArgumentException(nameof(output));
            }
            if (output.Exists)
            {
                throw new IOException("The file you're writing to already exists. Please input a new output path.");
            }

            Stream stream;
            OpenApiDocument document;
            OpenApiFormat openApiFormat;

            if (!string.IsNullOrEmpty(csdl))
            {
                // Default to yaml during csdl to OpenApi conversion
                openApiFormat = format ?? GetOpenApiFormat(csdl);
                 
                stream = GetStream(csdl);                
                document = ConvertCsdlToOpenApi(stream);                
            }
            else
            {
                stream = GetStream(openapi);

                var result = new OpenApiStreamReader(new OpenApiReaderSettings
                {
                    ReferenceResolution = resolveexternal ? ReferenceResolutionSetting.ResolveAllReferences : ReferenceResolutionSetting.ResolveLocalReferences,
                    RuleSet = ValidationRuleSet.GetDefaultRuleSet()
                }
                ).ReadAsync(stream).GetAwaiter().GetResult();

                document = result.OpenApiDocument;

                var context = result.OpenApiDiagnostic;

                if (context.Errors.Count > 0)
                {
                    var errorReport = new StringBuilder();

                    foreach (var error in context.Errors)
                    {
                        errorReport.AppendLine(error.ToString());
                    }

                    throw new ArgumentException(string.Join(Environment.NewLine, context.Errors.Select(e => e.Message).ToArray()));
                }

                openApiFormat = format ?? GetOpenApiFormat(openapi);
                version ??= result.OpenApiDiagnostic.SpecificationVersion;
            }

            Func<string, OperationType?, OpenApiOperation, bool> predicate;

            // Check if filter options are provided, then execute
            if (!string.IsNullOrEmpty(filterbyoperationids) && !string.IsNullOrEmpty(filterbytags))
            {
                throw new InvalidOperationException("Cannot filter by operationIds and tags at the same time.");
            }
            if (!string.IsNullOrEmpty(filterbyoperationids))
            {
                predicate = OpenApiFilterService.CreatePredicate(operationIds: filterbyoperationids);
                document = OpenApiFilterService.CreateFilteredDocument(document, predicate);
            }
            if (!string.IsNullOrEmpty(filterbytags))
            {
                predicate = OpenApiFilterService.CreatePredicate(tags: filterbytags);
                document = OpenApiFilterService.CreateFilteredDocument(document, predicate);
            }
            if (!string.IsNullOrEmpty(filterbycollection))
            {
                var fileStream = GetStream(filterbycollection);
                var requestUrls = ParseJsonCollectionFile(fileStream);
                predicate = OpenApiFilterService.CreatePredicate(requestUrls: requestUrls, source:document);
                document = OpenApiFilterService.CreateFilteredDocument(document, predicate);
            }
            
            using var outputStream = output?.Create();

            var textWriter = outputStream != null ? new StreamWriter(outputStream) : Console.Out;

            var settings = new OpenApiWriterSettings()
            {
                ReferenceInline = inline ? ReferenceInlineSetting.InlineLocalReferences : ReferenceInlineSetting.DoNotInlineReferences
            };

            IOpenApiWriter writer = openApiFormat switch
            {
                OpenApiFormat.Json => new OpenApiJsonWriter(textWriter, settings),
                OpenApiFormat.Yaml => new OpenApiYamlWriter(textWriter, settings),
                _ => throw new ArgumentException("Unknown format"),
            };
            document.Serialize(writer, (OpenApiSpecVersion)version);

            textWriter.Flush();
        }

        /// <summary>
        /// Converts CSDL to OpenAPI
        /// </summary>
        /// <param name="csdl">The CSDL stream.</param>
        /// <returns>An OpenAPI document.</returns>
        public static OpenApiDocument ConvertCsdlToOpenApi(Stream csdl)
        {
            using var reader = new StreamReader(csdl);
            var csdlText = reader.ReadToEndAsync().GetAwaiter().GetResult();           
            var edmModel = CsdlReader.Parse(XElement.Parse(csdlText).CreateReader());

            var settings = new OpenApiConvertSettings()
            {
                EnableKeyAsSegment = true,
                EnableOperationId = true,
                PrefixEntityTypeNameBeforeKey = true,
                TagDepth = 2,
                EnablePagination = true,
                EnableDiscriminatorValue = false,
                EnableDerivedTypesReferencesForRequestBody = false,
                EnableDerivedTypesReferencesForResponses = false,
                ShowRootPath = true,
                ShowLinks = true
            };
            OpenApiDocument document = edmModel.ConvertToOpenApi(settings);

            document = FixReferences(document);

            return document;
        }

        /// <summary>
        /// Fixes the references in the resulting OpenApiDocument.
        /// </summary>
        /// <param name="document"> The converted OpenApiDocument.</param>
        /// <returns> A valid OpenApiDocument instance.</returns>
        public static OpenApiDocument FixReferences(OpenApiDocument document)
        {
            // This method is only needed because the output of ConvertToOpenApi isn't quite a valid OpenApiDocument instance.
            // So we write it out, and read it back in again to fix it up.

            var sb = new StringBuilder();
            document.SerializeAsV3(new OpenApiYamlWriter(new StringWriter(sb)));
            var doc = new OpenApiStringReader().Read(sb.ToString(), out _);

            return doc;
        }

        private static Stream GetStream(string input)
        {
            Stream stream;
            if (input.StartsWith("http"))
            {
                var httpClient = new HttpClient(new HttpClientHandler()
                {
                    SslProtocols = System.Security.Authentication.SslProtocols.Tls12,
                })
                {
                    DefaultRequestVersion = HttpVersion.Version20
                };
                stream = httpClient.GetStreamAsync(input).Result;
            }
            else
            {
                var fileInput = new FileInfo(input);
                stream = fileInput.OpenRead();
            }

            return stream;
        }

        /// <summary>
        /// Takes in a file stream, parses the stream into a JsonDocument and gets a list of paths and Http methods
        /// </summary>
        /// <param name="stream"> A file stream.</param>
        /// <returns> A dictionary of request urls and http methods from a collection.</returns>
        public static Dictionary<string, List<string>> ParseJsonCollectionFile(Stream stream)
        {
            var requestUrls = new Dictionary<string, List<string>>();

            // Convert file to JsonDocument
            using var document = JsonDocument.Parse(stream);
            var root = document.RootElement;
            var itemElement = root.GetProperty("item");
            foreach (var requestObject in itemElement.EnumerateArray().Select(item => item.GetProperty("request")))
            {
                // Fetch list of methods and urls from collection, store them in a dictionary
                var path = requestObject.GetProperty("url").GetProperty("raw").ToString();
                var method = requestObject.GetProperty("method").ToString();

                if (!requestUrls.ContainsKey(path))
                {
                    requestUrls.Add(path, new List<string> { method });
                }
                else
                {
                    requestUrls[path].Add(method);
                }
            }

            return requestUrls;
        }

        internal static void ValidateOpenApiDocument(string openapi)
        {
            if (openapi == null)
            {
                throw new ArgumentNullException("openapi");
            }

            var stream = GetStream(openapi);

            OpenApiDocument document;

            document = new OpenApiStreamReader(new OpenApiReaderSettings
            {
                RuleSet = ValidationRuleSet.GetDefaultRuleSet()
            }
            ).Read(stream, out var context);

            if (context.Errors.Count != 0)
            {
                foreach (var error in context.Errors)
                {
                    Console.WriteLine(error.ToString());
                }
            }

            var statsVisitor = new StatsVisitor();
            var walker = new OpenApiWalker(statsVisitor);
            walker.Walk(document);

            Console.WriteLine(statsVisitor.GetStatisticsReport());
        }

        private static OpenApiFormat GetOpenApiFormat(string openapi)
        {
            return !openapi.StartsWith("http") && Path.GetExtension(openapi) == ".json" ? OpenApiFormat.Json : OpenApiFormat.Yaml;
        }
    }
}
