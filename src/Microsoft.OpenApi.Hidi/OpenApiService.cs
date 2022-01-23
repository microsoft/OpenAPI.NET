// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;
using Microsoft.OpenApi.Services;
using Microsoft.OpenApi.Validations;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Hidi
{
    public static class OpenApiService
    {
        public static void ProcessOpenApiDocument(
            string input,
            FileInfo output,
            OpenApiSpecVersion? version,
            OpenApiFormat? format,
            string filterByOperationIds,
            string filterByTags,
            string filterByCollection,
            bool inline,
            bool resolveExternal)
        {
            if (string.IsNullOrEmpty(input))
            {
                throw new ArgumentNullException(nameof(input));
            }
            if(output == null)
            {
                throw new ArgumentException(nameof(output));
            }
            if (output.Exists)
            {
                throw new IOException("The file you're writing to already exists. Please input a new output path.");
            }

            var inputUrl = GetInputUrl(input);
            var stream = GetStream(inputUrl);

            OpenApiDocument document;

            var result = new OpenApiStreamReader(new OpenApiReaderSettings
            {
                ReferenceResolution = resolveExternal == true ? ReferenceResolutionSetting.ResolveAllReferences : ReferenceResolutionSetting.ResolveLocalReferences,
                RuleSet = ValidationRuleSet.GetDefaultRuleSet(),
                BaseUrl = new Uri(inputUrl.AbsoluteUri)
            }
            ).ReadAsync(stream).GetAwaiter().GetResult();

            document = result.OpenApiDocument;
            Func<string, OperationType?, OpenApiOperation, bool> predicate;

            // Check if filter options are provided, then execute
            if (!string.IsNullOrEmpty(filterByOperationIds) && !string.IsNullOrEmpty(filterByTags))
            {
                throw new InvalidOperationException("Cannot filter by operationIds and tags at the same time.");
            }
            if (!string.IsNullOrEmpty(filterByOperationIds))
            {
                predicate = OpenApiFilterService.CreatePredicate(operationIds: filterByOperationIds);
                document = OpenApiFilterService.CreateFilteredDocument(document, predicate);
            }
            if (!string.IsNullOrEmpty(filterByTags))
            {
                predicate = OpenApiFilterService.CreatePredicate(tags: filterByTags);
                document = OpenApiFilterService.CreateFilteredDocument(document, predicate);
            }

            if (!string.IsNullOrEmpty(filterByCollection))
            {
                var fileStream = GetStream(GetInputUrl(filterByCollection));
                var requestUrls = ParseJsonCollectionFile(fileStream);
                predicate = OpenApiFilterService.CreatePredicate(requestUrls: requestUrls, source:document);
                document = OpenApiFilterService.CreateFilteredDocument(document, predicate);
            }

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

            using var outputStream = output?.Create();

            var textWriter = outputStream != null ? new StreamWriter(outputStream) : Console.Out;

            var settings = new OpenApiWriterSettings()
            {
                ReferenceInline = inline ? ReferenceInlineSetting.InlineLocalReferences : ReferenceInlineSetting.DoNotInlineReferences
            };

            var openApiFormat = format ?? GetOpenApiFormat(input);
            var openApiVersion = version ?? result.OpenApiDiagnostic.SpecificationVersion;
            IOpenApiWriter writer = openApiFormat switch
            {
                OpenApiFormat.Json => new OpenApiJsonWriter(textWriter, settings),
                OpenApiFormat.Yaml => new OpenApiYamlWriter(textWriter, settings),
                _ => throw new ArgumentException("Unknown format"),
            };
            document.Serialize(writer, openApiVersion);

            textWriter.Flush();
        }

        private static Uri GetInputUrl(string input)
        {
            if (input.StartsWith("http"))
            {
                return new Uri(input);
            } 
            else
            {
                return new Uri("file://" + Path.GetFullPath(input));
            }
        }

        private static Stream GetStream(Uri input)
        {
            Stream stream;
            if (input.Scheme == "http" || input.Scheme == "https")
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
            else if (input.Scheme == "file")
            {
                var fileInput = new FileInfo(input.AbsolutePath);
                stream = fileInput.OpenRead();
            } 
            else
            {
                throw new ArgumentException("Unrecognized exception");
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

        internal static async Task ValidateOpenApiDocument(string input, bool resolveExternal)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }
            var inputUrl = GetInputUrl(input);
            var stream = GetStream(GetInputUrl(input));

            OpenApiDocument document;

            var result = await new OpenApiStreamReader(new OpenApiReaderSettings
            {
                ReferenceResolution = resolveExternal == true ? ReferenceResolutionSetting.ResolveAllReferences : ReferenceResolutionSetting.ResolveLocalReferences,
                RuleSet = ValidationRuleSet.GetDefaultRuleSet(),
                BaseUrl = new Uri(inputUrl.AbsoluteUri)
            }
            ).ReadAsync(stream);

            document = result.OpenApiDocument;
            var context = result.OpenApiDiagnostic;

            if (context.Errors.Count != 0)
            {
                foreach (var error in context.Errors)
                {
                    Console.WriteLine(error.ToString());
                }
            }

            if (document.Workspace == null) { 
                var statsVisitor = new StatsVisitor();
                var walker = new OpenApiWalker(statsVisitor);
                walker.Walk(document);
                Console.WriteLine(statsVisitor.GetStatisticsReport());
            } 
            else
            {
                foreach (var memberDocument in document.Workspace.Documents)
                {
                    Console.WriteLine("Stats for " + memberDocument.Info.Title);
                    var statsVisitor = new StatsVisitor();
                    var walker = new OpenApiWalker(statsVisitor);
                    walker.Walk(memberDocument);
                    Console.WriteLine(statsVisitor.GetStatisticsReport());
                }
            }

            
        }

        private static OpenApiFormat GetOpenApiFormat(string input)
        {
            return !input.StartsWith("http") && Path.GetExtension(input) == ".json" ? OpenApiFormat.Json : OpenApiFormat.Yaml;
        }
    }
}
