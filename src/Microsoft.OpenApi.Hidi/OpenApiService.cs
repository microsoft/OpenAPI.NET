// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
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
    static class OpenApiService
    {
        public static void ProcessOpenApiDocument(
            string input,
            FileInfo output,
            OpenApiSpecVersion version,
            OpenApiFormat format,
            string filterByOperationIds,
            string filterByTags,
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

            var stream = GetStream(input);
            var result = new OpenApiStreamReader(new OpenApiReaderSettings
            {
                ReferenceResolution = resolveExternal ? ReferenceResolutionSetting.ResolveAllReferences : ReferenceResolutionSetting.ResolveLocalReferences,
                RuleSet = ValidationRuleSet.GetDefaultRuleSet()
            }
            ).ReadAsync(stream).GetAwaiter().GetResult();

            OpenApiDocument document;
            document = result.OpenApiDocument;

            // Check if filter options are provided, then execute
            if (!string.IsNullOrEmpty(filterByOperationIds) && !string.IsNullOrEmpty(filterByTags))
            {
                throw new InvalidOperationException("Cannot filter by operationIds and tags at the same time.");
            }

            if (!string.IsNullOrEmpty(filterByOperationIds))
            {
                var predicate = OpenApiFilterService.CreatePredicate(operationIds: filterByOperationIds);
                document = OpenApiFilterService.CreateFilteredDocument(document, predicate);
            }
            if (!string.IsNullOrEmpty(filterByTags))
            {
                var predicate = OpenApiFilterService.CreatePredicate(tags: filterByTags);
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
            IOpenApiWriter writer = format switch
            {
                OpenApiFormat.Json => new OpenApiJsonWriter(textWriter, settings),
                OpenApiFormat.Yaml => new OpenApiYamlWriter(textWriter, settings),
                _ => throw new ArgumentException("Unknown format"),
            };
            document.Serialize(writer, version);

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

        internal static void ValidateOpenApiDocument(string input)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }

            var stream = GetStream(input);

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
    }
}
