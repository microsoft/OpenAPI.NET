// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Xml.Linq;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.OData;
using Microsoft.OpenApi.Readers;
using Microsoft.OpenApi.Services;
using Microsoft.OpenApi.Validations;
using Microsoft.OpenApi.Writers;
using System.Threading;

namespace Microsoft.OpenApi.Hidi
{
    public class OpenApiService
    {
        public static async Task ProcessOpenApiDocument(
            string openapi,
            string csdl,
            FileInfo output,
            OpenApiSpecVersion? version,
            OpenApiFormat? format,
            LogLevel loglevel,
            bool inline,
            bool resolveexternal,
            string filterbyoperationids,
            string filterbytags,
            string filterbycollection,
            CancellationToken cancellationToken
           )
        {
            var logger = ConfigureLoggerInstance(loglevel);

            try
            {
                if (string.IsNullOrEmpty(openapi) && string.IsNullOrEmpty(csdl))
                {
                    throw new ArgumentNullException("Please input a file path");
                }
            }
            catch (ArgumentNullException ex)
            {
#if DEBUG
                logger.LogCritical(ex, ex.Message);
#else
                logger.LogCritical(ex.Message);
#endif
                return;
            }
            try
            {
                if(output == null)
                {
                    throw new ArgumentException(nameof(output));
                }
            }
            catch (ArgumentException ex)
            {
#if DEBUG
                logger.LogCritical(ex, ex.Message);
#else
                logger.LogCritical(ex.Message);
#endif
                return;
            }
            try
            {
                if (output.Exists)
                {
                    throw new IOException($"The file {output} already exists. Please input a new file path.");
                }
            }
            catch (IOException ex)
            {
#if DEBUG  
                logger.LogCritical(ex, ex.Message);
#else
                logger.LogCritical(ex.Message);
#endif
                return;
            }

            Stream stream;
            OpenApiDocument document;
            OpenApiFormat openApiFormat;
            var stopwatch = new Stopwatch();

            if (!string.IsNullOrEmpty(csdl))
            {
                // Default to yaml and OpenApiVersion 3 during csdl to OpenApi conversion
                openApiFormat = format ?? GetOpenApiFormat(csdl, logger);
                version ??= OpenApiSpecVersion.OpenApi3_0;

                stream = await GetStream(csdl, logger, cancellationToken);
                document = await ConvertCsdlToOpenApi(stream);
            }
            else
            {
                stream = await GetStream(openapi, logger, cancellationToken);

                // Parsing OpenAPI file
                stopwatch.Start();
                logger.LogTrace("Parsing OpenApi file");
                var result = new OpenApiStreamReader(new OpenApiReaderSettings
                {
                    ReferenceResolution = resolveexternal ? ReferenceResolutionSetting.ResolveAllReferences : ReferenceResolutionSetting.ResolveLocalReferences,
                    RuleSet = ValidationRuleSet.GetDefaultRuleSet()
                }
                ).ReadAsync(stream).GetAwaiter().GetResult();

                document = result.OpenApiDocument;
                stopwatch.Stop();

                var context = result.OpenApiDiagnostic;
                if (context.Errors.Count > 0)
                {
                    logger.LogTrace("{timestamp}ms: Parsed OpenAPI with errors. {count} paths found.", stopwatch.ElapsedMilliseconds, document.Paths.Count);

                    var errorReport = new StringBuilder();

                    foreach (var error in context.Errors)
                    {
                        logger.LogError("OpenApi Parsing error: {message}", error.ToString());
                        errorReport.AppendLine(error.ToString());
                    }
                    logger.LogError($"{stopwatch.ElapsedMilliseconds}ms: OpenApi Parsing errors {string.Join(Environment.NewLine, context.Errors.Select(e => e.Message).ToArray())}");
                }
                else
                {
                    logger.LogTrace("{timestamp}ms: Parsed OpenApi successfully. {count} paths found.", stopwatch.ElapsedMilliseconds, document.Paths.Count);
                }

                openApiFormat = format ?? GetOpenApiFormat(openapi, logger);
                version ??= result.OpenApiDiagnostic.SpecificationVersion;
            }

            Func<string, OperationType?, OpenApiOperation, bool> predicate;

            // Check if filter options are provided, then slice the OpenAPI document
            if (!string.IsNullOrEmpty(filterbyoperationids) && !string.IsNullOrEmpty(filterbytags))
            {
                throw new InvalidOperationException("Cannot filter by operationIds and tags at the same time.");
            }
            if (!string.IsNullOrEmpty(filterbyoperationids))
            {
                logger.LogTrace("Creating predicate based on the operationIds supplied.");
                predicate = OpenApiFilterService.CreatePredicate(operationIds: filterbyoperationids);

                logger.LogTrace("Creating subset OpenApi document.");
                document = OpenApiFilterService.CreateFilteredDocument(document, predicate);
            }
            if (!string.IsNullOrEmpty(filterbytags))
            {
                logger.LogTrace("Creating predicate based on the tags supplied.");
                predicate = OpenApiFilterService.CreatePredicate(tags: filterbytags);

                logger.LogTrace("Creating subset OpenApi document.");
                document = OpenApiFilterService.CreateFilteredDocument(document, predicate);
            }
            if (!string.IsNullOrEmpty(filterbycollection))
            {
                var fileStream = await GetStream(filterbycollection, logger, cancellationToken);
                var requestUrls = ParseJsonCollectionFile(fileStream, logger);

                logger.LogTrace("Creating predicate based on the paths and Http methods defined in the Postman collection.");
                predicate = OpenApiFilterService.CreatePredicate(requestUrls: requestUrls, source:document);

                logger.LogTrace("Creating subset OpenApi document.");
                document = OpenApiFilterService.CreateFilteredDocument(document, predicate);
            }
            
            logger.LogTrace("Creating a new file");
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

            logger.LogTrace("Serializing to OpenApi document using the provided spec version and writer");
            
            stopwatch.Start();
            document.Serialize(writer, (OpenApiSpecVersion)version);
            stopwatch.Stop();

            logger.LogTrace($"Finished serializing in {stopwatch.ElapsedMilliseconds}ms");

            textWriter.Flush();
        }

        /// <summary>
        /// Converts CSDL to OpenAPI
        /// </summary>
        /// <param name="csdl">The CSDL stream.</param>
        /// <returns>An OpenAPI document.</returns>
        public static async Task<OpenApiDocument> ConvertCsdlToOpenApi(Stream csdl)
        {
            using var reader = new StreamReader(csdl);
            var csdlText = await reader.ReadToEndAsync();
            var edmModel = CsdlReader.Parse(XElement.Parse(csdlText).CreateReader());

            var settings = new OpenApiConvertSettings()
            {
                AddSingleQuotesForStringParameters = true,
                AddEnumDescriptionExtension = true,
                DeclarePathParametersOnPathItem = true,
                EnableKeyAsSegment = true,
                EnableOperationId = true,
                ErrorResponsesAsDefault  = false,
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

        private static async Task<Stream> GetStream(string input, ILogger logger, CancellationToken cancellationToken)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            Stream stream;
            if (input.StartsWith("http"))
            {
                try
                {
                    var httpClientHandler = new HttpClientHandler()
                    {
                        SslProtocols = System.Security.Authentication.SslProtocols.Tls12,
                    };
                    using var httpClient = new HttpClient(httpClientHandler)
                    {
                      DefaultRequestVersion = HttpVersion.Version20
                    };
                    stream = await httpClient.GetStreamAsync(input, cancellationToken);
                }
                catch (HttpRequestException ex)
                {
#if DEBUG
                    logger.LogCritical(ex, "Could not download the file at {inputPath}, reason: {exMessage}", input, ex.Message);
#else
                    logger.LogCritical( "Could not download the file at {inputPath}, reason: {exMessage}", input, ex.Message);
#endif
                    return null;
                }
            }
            else
            {
                try
                {
                    var fileInput = new FileInfo(input);
                    stream = fileInput.OpenRead();
                }
                catch (Exception ex) when (ex is FileNotFoundException ||
                    ex is PathTooLongException ||
                    ex is DirectoryNotFoundException ||
                    ex is IOException ||
                    ex is UnauthorizedAccessException ||
                    ex is SecurityException ||
                    ex is NotSupportedException)
                {
#if DEBUG
                    logger.LogCritical(ex, "Could not open the file at {inputPath}, reason: {exMessage}", input, ex.Message);
#else
                    logger.LogCritical("Could not open the file at {inputPath}, reason: {exMessage}", input, ex.Message);
#endif
                    return null;
                }
            }
            stopwatch.Stop();
            logger.LogTrace("{timestamp}ms: Read file {input}", stopwatch.ElapsedMilliseconds, input);
            return stream;
        }

        /// <summary>
        /// Takes in a file stream, parses the stream into a JsonDocument and gets a list of paths and Http methods
        /// </summary>
        /// <param name="stream"> A file stream.</param>
        /// <returns> A dictionary of request urls and http methods from a collection.</returns>
        public static Dictionary<string, List<string>> ParseJsonCollectionFile(Stream stream, ILogger logger)
        {
            var requestUrls = new Dictionary<string, List<string>>();

            logger.LogTrace("Parsing the json collection file into a JsonDocument");
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
            logger.LogTrace("Finished fetching the list of paths and Http methods defined in the Postman collection.");
            return requestUrls;
        }

        internal static async Task ValidateOpenApiDocument(string openapi, LogLevel loglevel, CancellationToken cancellationToken)
        {
            var logger = ConfigureLoggerInstance(loglevel);

            try
            {
                if (string.IsNullOrEmpty(openapi))
                {
                    throw new ArgumentNullException(nameof(openapi));
                }
                var stream = await GetStream(openapi, logger, cancellationToken);

                OpenApiDocument document;
                logger.LogTrace("Parsing the OpenApi file");
                document = new OpenApiStreamReader(new OpenApiReaderSettings
                {
                    RuleSet = ValidationRuleSet.GetDefaultRuleSet()
                }
                ).Read(stream, out var context);

                if (context.Errors.Count != 0)
                {
                    foreach (var error in context.Errors)
                    {
                        logger.LogError("OpenApi Parsing error: {message}", error.ToString());
                        Console.WriteLine(error.ToString());
                    }
                }

                var statsVisitor = new StatsVisitor();
                var walker = new OpenApiWalker(statsVisitor);
                walker.Walk(document);

                logger.LogTrace("Finished walking through the OpenApi document. Generating a statistics report..");
                Console.WriteLine(statsVisitor.GetStatisticsReport());
            }
            catch(Exception ex)
            {
#if DEBUG
                logger.LogCritical(ex, ex.Message);
#else
                logger.LogCritical(ex.Message);
#endif
            }

        }

        private static OpenApiFormat GetOpenApiFormat(string input, ILogger logger)
        {
            logger.LogTrace("Getting the OpenApi format");
            return !input.StartsWith("http") && Path.GetExtension(input) == ".json" ? OpenApiFormat.Json : OpenApiFormat.Yaml;
        }

        private static ILogger ConfigureLoggerInstance(LogLevel loglevel)
        {
            // Configure logger options
#if DEBUG
            loglevel = loglevel > LogLevel.Debug ? LogLevel.Debug : loglevel;
#endif

            var logger = LoggerFactory.Create((builder) => {
                builder
                    .AddConsole()
#if DEBUG
                    .AddDebug()
#endif
                    .SetMinimumLevel(loglevel);
            }).CreateLogger<OpenApiService>();

            return logger;
        }
    }
}
