// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Security;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OpenApi.ApiManifest;
using Microsoft.OpenApi.ApiManifest.OpenAI;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Hidi.Extensions;
using Microsoft.OpenApi.Hidi.Formatters;
using Microsoft.OpenApi.Hidi.Options;
using Microsoft.OpenApi.Hidi.Utilities;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.OData;
using Microsoft.OpenApi.Reader;
using Microsoft.OpenApi.Readers;
using Microsoft.OpenApi.Services;
using Microsoft.OpenApi.Writers;
using static Microsoft.OpenApi.Hidi.OpenApiSpecVersionHelper;

namespace Microsoft.OpenApi.Hidi
{
    internal static class OpenApiService
    {
        static OpenApiService()
        {
            OpenApiReaderRegistry.RegisterReader(OpenApiConstants.Yaml, new OpenApiYamlReader());
            OpenApiReaderRegistry.RegisterReader(OpenApiConstants.Yml, new OpenApiYamlReader());
        }

        /// <summary>
        /// Implementation of the transform command
        /// </summary>
        public static async Task TransformOpenApiDocumentAsync(HidiOptions options, ILogger logger, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(options.OpenApi) && string.IsNullOrEmpty(options.Csdl) && string.IsNullOrEmpty(options.FilterOptions?.FilterByApiManifest))
            {
                throw new ArgumentException("Please input a file path or URL");
            }

            try
            {
                if (options.Output == null)
                {
#pragma warning disable CA1308 // Normalize strings to uppercase
                    var extension = options.OpenApiFormat?.GetDisplayName().ToLowerInvariant();
                    var inputExtension = !string.IsNullOrEmpty(extension) ? string.Concat(".", extension) 
                        : GetInputPathExtension(options.OpenApi, options.Csdl);

#pragma warning restore CA1308 // Normalize strings to uppercase
                    options.Output = new($"./output{inputExtension}");
                };

                if (options.CleanOutput && options.Output.Exists)
                {
                    options.Output.Delete();
                }
                if (options.Output.Exists)
                {
                    throw new IOException($"The file {options.Output} already exists. Please input a new file path.");
                }

                // Default to yaml and OpenApiVersion 3_1 during csdl to OpenApi conversion
                var openApiFormat = options.OpenApiFormat ?? (!string.IsNullOrEmpty(options.OpenApi) ? GetOpenApiFormat(options.OpenApi, logger) : OpenApiFormat.Yaml);
                var openApiVersion = options.Version != null ? TryParseOpenApiSpecVersion(options.Version) : OpenApiSpecVersion.OpenApi3_1;

                // If ApiManifest is provided, set the referenced OpenAPI document
                var apiDependency = await FindApiDependencyAsync(options.FilterOptions.FilterByApiManifest, logger, cancellationToken).ConfigureAwait(false);
                if (apiDependency != null)
                {
                    options.OpenApi = apiDependency.ApiDescripionUrl;
                }

                // If Postman Collection is provided, load it
                JsonDocument? postmanCollection = null;
                if (!string.IsNullOrEmpty(options.FilterOptions?.FilterByCollection))
                {
                    using var collectionStream = await GetStreamAsync(options.FilterOptions.FilterByCollection, logger, cancellationToken).ConfigureAwait(false);
                    postmanCollection = await JsonDocument.ParseAsync(collectionStream, cancellationToken: cancellationToken).ConfigureAwait(false);
                }

                // Load OpenAPI document
                var format = OpenApiModelFactory.GetFormat(options.OpenApi);
                var document = await GetOpenApiAsync(options, format, logger, options.MetadataVersion, cancellationToken).ConfigureAwait(false);

                if (options.FilterOptions != null)
                {
                    document = ApplyFilters(options, logger, apiDependency, postmanCollection, document);
                }

                var languageFormat = options.SettingsConfig?.GetSection("LanguageFormat")?.Value;
                if ("PowerShell".IsEquals(languageFormat))
                {
                    // PowerShell Walker.
                    var powerShellFormatter = new PowerShellFormatter();
                    var walker = new OpenApiWalker(powerShellFormatter);
                    walker.Walk(document);
                }
                WriteOpenApi(options, openApiFormat, openApiVersion, document, logger);
            }
            catch (TaskCanceledException)
            {
                await Console.Error.WriteLineAsync("CTRL+C pressed, aborting the operation.").ConfigureAwait(false);
            }
            catch (IOException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Could not transform the document, reason: {ex.Message}", ex);
            }
        }

        private static async Task<ApiDependency?> FindApiDependencyAsync(string? apiManifestPath, ILogger logger, CancellationToken cancellationToken = default)
        {
            ApiDependency? apiDependency = null;
            // If API Manifest is provided, load it, use it get the OpenAPI path
            ApiManifestDocument? apiManifest = null;
            if (!string.IsNullOrEmpty(apiManifestPath))
            {
                // Extract fragment identifier if passed as the name of the ApiDependency
                var apiManifestRef = apiManifestPath.Split('#');
                var apiDependencyName = string.Empty;
                if (apiManifestRef.Length > 1)
                {
                    apiDependencyName = apiManifestRef[1];
                }
                using (var fileStream = await GetStreamAsync(apiManifestRef[0], logger, cancellationToken).ConfigureAwait(false))
                {
                    var document = await JsonDocument.ParseAsync(fileStream, cancellationToken: cancellationToken).ConfigureAwait(false);
                    apiManifest = ApiManifestDocument.Load(document.RootElement);
                }

                apiDependency = !string.IsNullOrEmpty(apiDependencyName) && apiManifest.ApiDependencies.TryGetValue(apiDependencyName, out var dependency) ? dependency : apiManifest.ApiDependencies.First().Value;
            }

            return apiDependency;
        }

        private static OpenApiDocument ApplyFilters(HidiOptions options, ILogger logger, ApiDependency? apiDependency, JsonDocument? postmanCollection, OpenApiDocument document)
        {
            Dictionary<string, List<string>> requestUrls;
            if (apiDependency != null)
            {
                requestUrls = GetRequestUrlsFromManifest(apiDependency);
            }
            else if (postmanCollection != null)
            {
                requestUrls = EnumerateJsonDocument(postmanCollection.RootElement, new());
                logger.LogTrace("Finished fetching the list of paths and Http methods defined in the Postman collection.");
            }
            else
            {
                requestUrls = new();
                logger.LogTrace("No filter options provided.");
            }

            logger.LogTrace("Creating predicate from filter options.");
            var predicate = FilterOpenApiDocument(options.FilterOptions.FilterByOperationIds,
                                                    options.FilterOptions.FilterByTags,
                                                    requestUrls,
                                                    document,
                                                     logger);
            if (predicate != null)
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                document = OpenApiFilterService.CreateFilteredDocument(document, predicate);
                stopwatch.Stop();
                logger.LogTrace("{Timestamp}ms: Creating filtered OpenApi document with {Paths} paths.", stopwatch.ElapsedMilliseconds, document.Paths.Count);
            }

            return document;
        }

        private static void WriteOpenApi(HidiOptions options, OpenApiFormat openApiFormat, OpenApiSpecVersion openApiVersion, OpenApiDocument document, ILogger logger)
        {
            using (logger.BeginScope("Output"))
            {
                if (options.Output is null) throw new InvalidOperationException("Output file path is null");
                using var outputStream = options.Output.Create();
                using var textWriter = new StreamWriter(outputStream);

                var settings = new OpenApiWriterSettings
                {
                    InlineLocalReferences = options.InlineLocal,
                    InlineExternalReferences = options.InlineExternal
                };

                IOpenApiWriter writer = openApiFormat switch
                {
                    OpenApiFormat.Json => options.TerseOutput ? new(textWriter, settings, options.TerseOutput) : new OpenApiJsonWriter(textWriter, settings, false),
                    OpenApiFormat.Yaml => new OpenApiYamlWriter(textWriter, settings),
                    _ => throw new ArgumentException("Unknown format"),
                };

                logger.LogTrace("Serializing to OpenApi document using the provided spec version and writer");

                var stopwatch = new Stopwatch();
                stopwatch.Start();
                document.Serialize(writer, openApiVersion);
                stopwatch.Stop();

                logger.LogTrace("Finished serializing in {ElapsedMilliseconds}ms", stopwatch.ElapsedMilliseconds);
                textWriter.Flush();
            }
        }

        // Get OpenAPI document either from OpenAPI or CSDL
        private static async Task<OpenApiDocument> GetOpenApiAsync(HidiOptions options, string format, ILogger logger, string? metadataVersion = null, CancellationToken cancellationToken = default)
        {
            OpenApiDocument document;
            Stream stream;

            if (!string.IsNullOrEmpty(options.Csdl))
            {
                var stopwatch = new Stopwatch();
                using (logger.BeginScope("Convert CSDL: {Csdl}", options.Csdl))
                {
                    stopwatch.Start();
                    stream = await GetStreamAsync(options.Csdl, logger, cancellationToken).ConfigureAwait(false);
                    Stream? filteredStream = null;
                    if (!string.IsNullOrEmpty(options.CsdlFilter))
                    {
                        var transform = GetFilterTransform();
                        filteredStream = ApplyFilterToCsdl(stream, options.CsdlFilter, transform);
                        filteredStream.Position = 0;
                        await stream.DisposeAsync().ConfigureAwait(false);
                    }

                    document = await ConvertCsdlToOpenApiAsync(filteredStream ?? stream, format, metadataVersion, options.SettingsConfig, cancellationToken).ConfigureAwait(false);
                    stopwatch.Stop();
                    logger.LogTrace("{Timestamp}ms: Generated OpenAPI with {Paths} paths.", stopwatch.ElapsedMilliseconds, document.Paths.Count);
                }
            }
            else if (!string.IsNullOrEmpty(options.OpenApi))
            {
                stream = await GetStreamAsync(options.OpenApi, logger, cancellationToken).ConfigureAwait(false);
                var result = await ParseOpenApiAsync(options.OpenApi, options.InlineExternal, logger, stream, cancellationToken).ConfigureAwait(false);
                document = result.Document;
            }
            else throw new InvalidOperationException("No input file path or URL provided");

            return document;
        }

        private static Func<string, OperationType?, OpenApiOperation, bool>? FilterOpenApiDocument(string? filterByOperationIds, string? filterByTags, Dictionary<string, List<string>> requestUrls, OpenApiDocument document, ILogger logger)
        {
            Func<string, OperationType?, OpenApiOperation, bool>? predicate = null;

            using (logger.BeginScope("Create Filter"))
            {
                // Check if filter options are provided, then slice the OpenAPI document
                if (!string.IsNullOrEmpty(filterByOperationIds) && !string.IsNullOrEmpty(filterByTags))
                {
                    throw new InvalidOperationException("Cannot filter by operationIds and tags at the same time.");
                }
                if (!string.IsNullOrEmpty(filterByOperationIds))
                {
                    logger.LogTrace("Creating predicate based on the operationIds supplied.");
                    predicate = OpenApiFilterService.CreatePredicate(operationIds: filterByOperationIds);

                }
                if (!string.IsNullOrEmpty(filterByTags))
                {
                    logger.LogTrace("Creating predicate based on the tags supplied.");
                    predicate = OpenApiFilterService.CreatePredicate(tags: filterByTags);

                }
                if (requestUrls.Count > 0)
                {
                    logger.LogTrace("Creating predicate based on the paths and Http methods defined in the Postman collection.");
                    predicate = OpenApiFilterService.CreatePredicate(requestUrls: requestUrls, source: document);
                }
            }

            return predicate;
        }

        private static Dictionary<string, List<string>> GetRequestUrlsFromManifest(ApiDependency apiDependency)
        {
            // Get the request URLs from the API Dependencies in the API manifest
            var requests = apiDependency
                    .Requests.Where(static r => !r.Exclude && !string.IsNullOrEmpty(r.UriTemplate) && !string.IsNullOrEmpty(r.Method))
                                .Select(static r => new { UriTemplate = r.UriTemplate!, Method = r.Method! })
                    .GroupBy(static r => r.UriTemplate)
                    .ToDictionary(static g => g.Key, static g => g.Select(static r => r.Method).ToList());
            // This makes the assumption that the UriTemplate in the ApiManifest matches exactly the UriTemplate in the OpenAPI document
            // This does not need to be the case.  The URI template in the API manifest could map to a set of OpenAPI paths.
            // Additional logic will be required to handle this scenario.  I suggest we build this into the OpenAPI.Net library at some point.
            return requests;
        }

        private static XslCompiledTransform GetFilterTransform()
        {
            XslCompiledTransform transform = new();
            var assembly = typeof(OpenApiService).GetTypeInfo().Assembly;
            using var xslt = assembly.GetManifestResourceStream("Microsoft.OpenApi.Hidi.CsdlFilter.xslt") ?? throw new InvalidOperationException("Could not find the Microsoft.OpenApi.Hidi.CsdlFilter.xslt file in the assembly. Check build configuration.");
            using var streamReader = new StreamReader(xslt);
            using var textReader = new XmlTextReader(streamReader);
            transform.Load(textReader);
            return transform;
        }

        private static MemoryStream ApplyFilterToCsdl(Stream csdlStream, string entitySetOrSingleton, XslCompiledTransform transform)
        {
            using StreamReader inputReader = new(csdlStream, leaveOpen: true);
            using var inputXmlReader = XmlReader.Create(inputReader);
            MemoryStream filteredStream = new();
            using StreamWriter writer = new(filteredStream, leaveOpen: true);
            XsltArgumentList args = new();
            args.AddParam("entitySetOrSingleton", "", entitySetOrSingleton);
            transform.Transform(inputXmlReader, args, writer);
            return filteredStream;
        }

        /// <summary>
        /// Implementation of the validate command
        /// </summary>
        /// <returns><see langword="true"/> when valid, <see langword="false"/> when invalid and <see langword="null"/> when cancelled</returns>
        public static async Task<bool?> ValidateOpenApiDocumentAsync(
            string openApi,
            ILogger logger,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(openApi))
            {
                throw new ArgumentNullException(nameof(openApi));
            }

            ReadResult? result = null;

            try
            {
                using var stream = await GetStreamAsync(openApi, logger, cancellationToken).ConfigureAwait(false);

                result = await ParseOpenApiAsync(openApi, false, logger, stream, cancellationToken).ConfigureAwait(false);

                using (logger.BeginScope("Calculating statistics"))
                {
                    var statsVisitor = new StatsVisitor();
                    var walker = new OpenApiWalker(statsVisitor);
                    walker.Walk(result.Document);

                    logger.LogTrace("Finished walking through the OpenApi document. Generating a statistics report..");
                    #pragma warning disable CA2254
                    logger.LogInformation(statsVisitor.GetStatisticsReport());
                    #pragma warning restore CA2254
                }
            }
            catch (TaskCanceledException)
            {
                await Console.Error.WriteLineAsync("CTRL+C pressed, aborting the operation.").ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Could not validate the document, reason: {ex.Message}", ex);
            }

            if (result is null) return null;

            return result.Diagnostic.Errors.Count == 0;
        }

        private static async Task<ReadResult> ParseOpenApiAsync(string openApiFile, bool inlineExternal, ILogger logger, Stream stream, CancellationToken cancellationToken = default)
        {
            ReadResult result;
            var stopwatch = Stopwatch.StartNew();
            using (logger.BeginScope("Parsing OpenAPI: {OpenApiFile}", openApiFile))
            {
                stopwatch.Start();

                var settings = new OpenApiReaderSettings
                {
                    LoadExternalRefs = inlineExternal,
                    BaseUrl = openApiFile.StartsWith("http", StringComparison.OrdinalIgnoreCase) ?
                        new(openApiFile) :
                        new Uri("file://" + new FileInfo(openApiFile).DirectoryName + Path.DirectorySeparatorChar)
                };

                var format = OpenApiModelFactory.GetFormat(openApiFile);
                result = await OpenApiDocument.LoadAsync(stream, format, settings, cancellationToken).ConfigureAwait(false);

                logger.LogTrace("{Timestamp}ms: Completed parsing.", stopwatch.ElapsedMilliseconds);

                LogErrors(logger, result);
                stopwatch.Stop();
            }

            return result;
        }

        /// <summary>
        /// Converts CSDL to OpenAPI
        /// </summary>
        /// <param name="csdl">The CSDL stream.</param>
        /// <returns>An OpenAPI document.</returns>
        public static async Task<OpenApiDocument> ConvertCsdlToOpenApiAsync(Stream csdl, string format, string? metadataVersion = null, IConfiguration? settings = null, CancellationToken token = default)
        {
            using var reader = new StreamReader(csdl);
            var csdlText = await reader.ReadToEndAsync(token).ConfigureAwait(false);
            var edmModel = CsdlReader.Parse(XElement.Parse(csdlText).CreateReader());
            settings ??= SettingsUtilities.GetConfiguration();

            var document = edmModel.ConvertToOpenApi(SettingsUtilities.GetOpenApiConvertSettings(settings, metadataVersion));
            document = FixReferences(document, format);

            return document;
        }

        /// <summary>
        /// Fixes the references in the resulting OpenApiDocument.
        /// </summary>
        /// <param name="document"> The converted OpenApiDocument.</param>
        /// <returns> A valid OpenApiDocument instance.</returns>
        public static OpenApiDocument FixReferences(OpenApiDocument document, string format)
        {
            // This method is only needed because the output of ConvertToOpenApi isn't quite a valid OpenApiDocument instance.
            // So we write it out, and read it back in again to fix it up.

            var sb = new StringBuilder();
            document.SerializeAsV3(new OpenApiYamlWriter(new StringWriter(sb)));

            var doc = OpenApiDocument.Parse(sb.ToString(), format).Document;

            return doc;
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

            requestUrls = EnumerateJsonDocument(root, requestUrls);
            logger.LogTrace("Finished fetching the list of paths and Http methods defined in the Postman collection.");

            return requestUrls;
        }

        private static Dictionary<string, List<string>> EnumerateJsonDocument(JsonElement itemElement, Dictionary<string, List<string>> paths)
        {
            var itemsArray = itemElement.GetProperty("item");

            foreach (var item in itemsArray.EnumerateArray())
            {
                if (item.ValueKind == JsonValueKind.Object)
                {
                    if (item.TryGetProperty("request", out var request))
                    {
                        // Fetch list of methods and urls from collection, store them in a dictionary
                        var path = request.GetProperty("url").GetProperty("raw").ToString();
                        var method = request.GetProperty("method").ToString();
                        if (paths.TryGetValue(path, out var value))
                        {
                            value.Add(method);
                        }
                        else
                        {
                            paths.Add(path, new() {method});
                        }
                    }
                    else
                    {
                        EnumerateJsonDocument(item, paths);
                    }
                }
                else
                {
                    EnumerateJsonDocument(item, paths);
                }
            }

            return paths;
        }

        /// <summary>
        /// Reads stream from file system or makes HTTP request depending on the input string
        /// </summary>
        private static async Task<Stream> GetStreamAsync(string input, ILogger logger, CancellationToken cancellationToken = default)
        {
            Stream stream;
            using (logger.BeginScope("Reading input stream"))
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();

                if (input.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                {
                    try
                    {
                        using var httpClient = new HttpClient
                        {
                            DefaultRequestVersion = HttpVersion.Version20
                        };
                        stream = await httpClient.GetStreamAsync(new Uri(input), cancellationToken).ConfigureAwait(false);
                    }
                    catch (HttpRequestException ex)
                    {
                        throw new InvalidOperationException($"Could not download the file at {input}", ex);
                    }
                }
                else
                {
                    try
                    {
                        var fileInput = new FileInfo(input);
                        stream = fileInput.OpenRead();
                    }
                    catch (Exception ex) when (
                        ex is
                            FileNotFoundException or
                            PathTooLongException or
                            DirectoryNotFoundException or
                            IOException or
                            UnauthorizedAccessException or
                            SecurityException or
                            NotSupportedException)
                    {
                        throw new InvalidOperationException($"Could not open the file at {input}", ex);
                    }
                }
                stopwatch.Stop();
                logger.LogTrace("{Timestamp}ms: Read file {Input}", stopwatch.ElapsedMilliseconds, input);
            }
            return stream;
        }

        /// <summary>
        /// Attempt to guess OpenAPI format based in input URL
        /// </summary>
        /// <param name="input"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        private static OpenApiFormat GetOpenApiFormat(string input, ILogger logger)
        {
            logger.LogTrace("Getting the OpenApi format");
            return !input.StartsWith("http", StringComparison.OrdinalIgnoreCase) && Path.GetExtension(input) == ".json" ? OpenApiFormat.Json : OpenApiFormat.Yaml;
        }

        private static string GetInputPathExtension(string? openapi = null, string? csdl = null)
        {
            var extension = string.Empty;
            if (!string.IsNullOrEmpty(openapi))
            {
                extension = Path.GetExtension(openapi);
            }
            else if (!string.IsNullOrEmpty(csdl))
            {
                extension = ".yml";
            }

            return extension;
        }

        internal static async Task<string?> ShowOpenApiDocumentAsync(HidiOptions options, ILogger logger, CancellationToken cancellationToken = default)
        {
            try
            {
                if (string.IsNullOrEmpty(options.OpenApi) && string.IsNullOrEmpty(options.Csdl))
                {
                    throw new ArgumentException("Please input a file path or URL");
                }

                var format = OpenApiModelFactory.GetFormat(options.OpenApi);
                var document = await GetOpenApiAsync(options, format, logger, null, cancellationToken).ConfigureAwait(false);

                using (logger.BeginScope("Creating diagram"))
                {
                    // If output is null, create a HTML file in the user's temporary directory
                    var sourceUrl = (string.IsNullOrEmpty(options.OpenApi), string.IsNullOrEmpty(options.Csdl)) switch {
                        (false, _) => options.OpenApi!,
                        (_, false) => options.Csdl!,
                        _ => throw new InvalidOperationException("No input file path or URL provided")
                    };
                    if (options.Output == null)
                    {
                        var tempPath = Path.GetTempPath() + "/hidi/";
                        if (!File.Exists(tempPath))
                        {
                            Directory.CreateDirectory(tempPath);
                        }

                        var fileName = Path.GetRandomFileName();

                        var output = new FileInfo(Path.Combine(tempPath, fileName + ".html"));
                        using (var file = new FileStream(output.FullName, FileMode.Create))
                        {
                            using var writer = new StreamWriter(file);
                            WriteTreeDocumentAsHtml(sourceUrl, document, writer);
                        }
                        logger.LogTrace("Created Html document with diagram ");

                        // Launch a browser to display the output html file
                        using var process = new Process();
                        process.StartInfo.FileName = output.FullName;
                        process.StartInfo.UseShellExecute = true;
                        process.Start();

                        return output.FullName;
                    }
                    else  // Write diagram as Markdown document to output file
                    {
                        using (var file = new FileStream(options.Output.FullName, FileMode.Create))
                        {
                            using var writer = new StreamWriter(file);
                            WriteTreeDocumentAsMarkdown(sourceUrl, document, writer);
                        }
                        logger.LogTrace("Created markdown document with diagram ");
                        return options.Output.FullName;
                    }
                }
            }
            catch (TaskCanceledException)
            {
                await Console.Error.WriteLineAsync("CTRL+C pressed, aborting the operation.").ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Could not generate the document, reason: {ex.Message}", ex);
            }
            return null;
        }

        private static void LogErrors(ILogger logger, ReadResult result)
        {
            var context = result.Diagnostic;
            if (context.Errors.Count != 0)
            {
                using (logger.BeginScope("Detected errors"))
                {
                    foreach (var error in context.Errors)
                    {
                        logger.LogError("Detected error during parsing: {Error}", error.ToString());
                    }
                }
            }
        }

        internal static void WriteTreeDocumentAsMarkdown(string openapiUrl, OpenApiDocument document, StreamWriter writer)
        {
            var rootNode = OpenApiUrlTreeNode.Create(document, "main");

            writer.WriteLine("# " + document.Info.Title);
            writer.WriteLine();
            writer.WriteLine("API Description: " + openapiUrl);

            writer.WriteLine(@"<div>");
            // write a span for each mermaidcolorscheme
            foreach (var style in OpenApiUrlTreeNode.MermaidNodeStyles)
            {
                writer.WriteLine($"<span style=\"padding:2px;background-color:{style.Value.Color};border: 2px solid\">{style.Key.Replace("_", " ", StringComparison.OrdinalIgnoreCase)}</span>");
            }
            writer.WriteLine("</div>");
            writer.WriteLine();
            writer.WriteLine("```mermaid");
            rootNode.WriteMermaid(writer);
            writer.WriteLine("```");
        }

        internal static void WriteTreeDocumentAsHtml(string sourceUrl, OpenApiDocument document, StreamWriter writer, bool asHtmlFile = false)
        {
            var rootNode = OpenApiUrlTreeNode.Create(document, "main");

            writer.WriteLine(
                """
                <!doctype html>
                <html>
                <head>
                  <meta charset="utf-8"/>
                  <script src="https://cdnjs.cloudflare.com/ajax/libs/mermaid/8.0.0/mermaid.min.js"></script>
                </head>
                <style>
                    body {
                        font-family: Verdana, sans-serif;
                    }
                </style>
                <body>
                """);
            writer.WriteLine("<h1>" + document.Info.Title + "</h1>");
            writer.WriteLine();
            writer.WriteLine($"<h3> API Description: <a href='{sourceUrl}'>{sourceUrl}</a></h3>");

            writer.WriteLine(@"<div>");
            // write a span for each mermaidcolorscheme
            foreach (var style in OpenApiUrlTreeNode.MermaidNodeStyles)
            {
                writer.WriteLine($"<span style=\"padding:2px;background-color:{style.Value.Color};border: 2px solid\">{style.Key.Replace("_", " ", StringComparison.OrdinalIgnoreCase)}</span>");
            }

            writer.WriteLine("</div>");
            writer.WriteLine("<hr/>");
            writer.WriteLine("<code class=\"language-mermaid\">");
            rootNode.WriteMermaid(writer);
            writer.WriteLine("</code>");

            // Write script tag to include JS library for rendering markdown
            writer.WriteLine(
                """
                <script>
                  var config = {
                      startOnLoad:true,
                      theme: 'forest',
                      flowchart:{
                              useMaxWidth:false,
                              htmlLabels:true
                          }
                  };
                  mermaid.initialize(config);
                  window.mermaid.init(undefined, document.querySelectorAll('.language-mermaid'));
                </script>
                """);
            // Write script tag to include JS library for rendering mermaid
            writer.WriteLine("</html");
        }

        internal static async Task PluginManifestAsync(HidiOptions options, ILogger logger, CancellationToken cancellationToken = default)
        {
            // If ApiManifest is provided, set the referenced OpenAPI document
            var apiDependency = await FindApiDependencyAsync(options.FilterOptions?.FilterByApiManifest, logger, cancellationToken).ConfigureAwait(false);
            if (apiDependency != null)
            {
                options.OpenApi = apiDependency.ApiDescripionUrl;
            }

            // Load OpenAPI document
            var format = OpenApiModelFactory.GetFormat(options.OpenApi);
            var document = await GetOpenApiAsync(options, format, logger, options.MetadataVersion, cancellationToken).ConfigureAwait(false);

            cancellationToken.ThrowIfCancellationRequested();

            if (options.FilterOptions != null)
            {
                document = ApplyFilters(options, logger, apiDependency, null, document);
            }

            // Ensure path in options.OutputFolder exists
            var outputFolder = new DirectoryInfo(options.OutputFolder);
            if (!outputFolder.Exists)
            {
                outputFolder.Create();
            }
            // Write OpenAPI to Output folder
            options.Output = new(Path.Combine(options.OutputFolder, "openapi.json"));
            options.TerseOutput = true;
            WriteOpenApi(options, OpenApiFormat.Json, OpenApiSpecVersion.OpenApi3_1, document, logger);

            // Create OpenAIPluginManifest from ApiDependency and OpenAPI document
            var manifest = new OpenAIPluginManifest
            {
                NameForHuman = document.Info.Title,
                DescriptionForHuman = document.Info.Description,
                Api = new()
                {
                    Type = "openapi",
                    Url = "./openapi.json"
                }
            };
            manifest.NameForModel = manifest.NameForHuman;
            manifest.DescriptionForModel = manifest.DescriptionForHuman;

            // Write OpenAIPluginManifest to Output folder
            var manifestFile = new FileInfo(Path.Combine(options.OutputFolder, "ai-plugin.json"));
            using var file = new FileStream(manifestFile.FullName, FileMode.Create);
            using var jsonWriter = new Utf8JsonWriter(file, new() { Indented = true });
            manifest.Write(jsonWriter);
            await jsonWriter.FlushAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
