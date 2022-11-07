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
using System.Threading.Tasks;
using System.Text.Json;
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
using static Microsoft.OpenApi.Hidi.OpenApiSpecVersionHelper;
using System.Threading;
using System.Xml.Xsl;
using System.Xml;
using System.Reflection;
using Microsoft.Extensions.Configuration;

namespace Microsoft.OpenApi.Hidi
{
    public class OpenApiService
    {
        /// <summary>
        /// Implementation of the transform command
        /// </summary>
        public static async Task TransformOpenApiDocument(
            string openapi,
            string csdl,
            string csdlFilter,
            FileInfo output,
            bool cleanoutput,
            string? version,
            OpenApiFormat? format,
            bool terseOutput,
            string settingsFile,
            LogLevel logLevel,
            bool inlineLocal,
            bool inlineExternal,
            string filterbyoperationids,
            string filterbytags,
            string filterbycollection,
            CancellationToken cancellationToken
           )
        {
            using var loggerFactory = Logger.ConfigureLogger(logLevel);
            var logger = loggerFactory.CreateLogger<OpenApiService>();
            try
            {
                if (string.IsNullOrEmpty(openapi) && string.IsNullOrEmpty(csdl))
                {
                    throw new ArgumentException("Please input a file path");
                }
                if (output == null)
                {
                    var inputExtension = GetInputPathExtension(openapi, csdl);
                    output = new FileInfo($"./output{inputExtension}");
                };
                if (cleanoutput && output.Exists)
                {
                    output.Delete();
                }
                if (output.Exists)
                {
                    throw new IOException($"The file {output} already exists. Please input a new file path.");
                }

                Stream stream;
                OpenApiDocument document;
                OpenApiFormat openApiFormat;
                OpenApiSpecVersion openApiVersion;
                var stopwatch = new Stopwatch();

                if (!string.IsNullOrEmpty(csdl))
                {
                    using (logger.BeginScope($"Convert CSDL: {csdl}", csdl))
                    {
                        stopwatch.Start();
                        // Default to yaml and OpenApiVersion 3 during csdl to OpenApi conversion
                        openApiFormat = format ?? GetOpenApiFormat(csdl, logger);
                        openApiVersion = version != null ? TryParseOpenApiSpecVersion(version) : OpenApiSpecVersion.OpenApi3_0;

                        stream = await GetStream(csdl, logger, cancellationToken);

                        if (!string.IsNullOrEmpty(csdlFilter))
                        {
                            XslCompiledTransform transform = GetFilterTransform();
                            stream = ApplyFilter(csdl, csdlFilter, transform);
                            stream.Position = 0;
                        }

                        document = await ConvertCsdlToOpenApi(stream, settingsFile);
                        stopwatch.Stop();
                        logger.LogTrace("{timestamp}ms: Generated OpenAPI with {paths} paths.", stopwatch.ElapsedMilliseconds, document.Paths.Count);
                    }
                }
                else
                {
                    stream = await GetStream(openapi, logger, cancellationToken);

                    using (logger.BeginScope($"Parse OpenAPI: {openapi}",openapi))
                    {
                        stopwatch.Restart();
                        var result = await new OpenApiStreamReader(new OpenApiReaderSettings
                        {
                            RuleSet = ValidationRuleSet.GetDefaultRuleSet(),
                            LoadExternalRefs = inlineExternal,
                            BaseUrl = openapi.StartsWith("http") ? new Uri(openapi) : new Uri("file:" + new FileInfo(openapi).DirectoryName + "\\")
                        }
                        ).ReadAsync(stream);

                        document = result.OpenApiDocument;

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
                        openApiVersion = version != null ? TryParseOpenApiSpecVersion(version) : result.OpenApiDiagnostic.SpecificationVersion;
                        stopwatch.Stop();
                    }
                }

                using (logger.BeginScope("Filter"))
                {
                    Func<string, OperationType?, OpenApiOperation, bool> predicate = null;

                    // Check if filter options are provided, then slice the OpenAPI document
                    if (!string.IsNullOrEmpty(filterbyoperationids) && !string.IsNullOrEmpty(filterbytags))
                    {
                        throw new InvalidOperationException("Cannot filter by operationIds and tags at the same time.");
                    }
                    if (!string.IsNullOrEmpty(filterbyoperationids))
                    {
                        logger.LogTrace("Creating predicate based on the operationIds supplied.");
                        predicate = OpenApiFilterService.CreatePredicate(operationIds: filterbyoperationids);

                    }
                    if (!string.IsNullOrEmpty(filterbytags))
                    {
                        logger.LogTrace("Creating predicate based on the tags supplied.");
                        predicate = OpenApiFilterService.CreatePredicate(tags: filterbytags);

                    }
                    if (!string.IsNullOrEmpty(filterbycollection))
                    {
                        var fileStream = await GetStream(filterbycollection, logger, cancellationToken);
                        var requestUrls = ParseJsonCollectionFile(fileStream, logger);

                        logger.LogTrace("Creating predicate based on the paths and Http methods defined in the Postman collection.");
                        predicate = OpenApiFilterService.CreatePredicate(requestUrls: requestUrls, source: document);
                    }
                    if (predicate != null)
                    {
                        stopwatch.Restart();
                        document = OpenApiFilterService.CreateFilteredDocument(document, predicate);
                        stopwatch.Stop();
                        logger.LogTrace("{timestamp}ms: Creating filtered OpenApi document with {paths} paths.", stopwatch.ElapsedMilliseconds, document.Paths.Count);
                    }
                }

                using (logger.BeginScope("Output"))
                {
                    ;
                    using var outputStream = output?.Create();
                    var textWriter = outputStream != null ? new StreamWriter(outputStream) : Console.Out;

                    var settings = new OpenApiWriterSettings()
                    {
                        InlineLocalReferences = inlineLocal,
                        InlineExternalReferences = inlineExternal
                    };

                    IOpenApiWriter writer = openApiFormat switch
                    {
                        OpenApiFormat.Json => terseOutput ? new OpenApiJsonWriter(textWriter, settings, terseOutput) : new OpenApiJsonWriter(textWriter, settings, false),
                        OpenApiFormat.Yaml => new OpenApiYamlWriter(textWriter, settings),
                        _ => throw new ArgumentException("Unknown format"),
                    };

                    logger.LogTrace("Serializing to OpenApi document using the provided spec version and writer");

                    stopwatch.Start();
                    document.Serialize(writer, openApiVersion);
                    stopwatch.Stop();

                    logger.LogTrace($"Finished serializing in {stopwatch.ElapsedMilliseconds}ms");
                    textWriter.Flush();
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Could not transform the document, reason: {ex.Message}", ex);
            }
        }

        private static XslCompiledTransform GetFilterTransform()
        {
            XslCompiledTransform transform = new();
            Assembly assembly = typeof(OpenApiService).GetTypeInfo().Assembly;
            Stream xslt = assembly.GetManifestResourceStream("Microsoft.OpenApi.Hidi.CsdlFilter.xslt");
            transform.Load(new XmlTextReader(new StreamReader(xslt)));
            return transform;
        }

        private static Stream ApplyFilter(string csdl, string entitySetOrSingleton, XslCompiledTransform transform)
        {
            Stream stream;
            StreamReader inputReader = new(csdl);
            XmlReader inputXmlReader = XmlReader.Create(inputReader);
            MemoryStream filteredStream = new();
            StreamWriter writer = new(filteredStream);
            XsltArgumentList args = new();
            args.AddParam("entitySetOrSingleton", "", entitySetOrSingleton);
            transform.Transform(inputXmlReader, args, writer);
            stream = filteredStream;
            return stream;
        }

        /// <summary>
        /// Implementation of the validate command
        /// </summary>
        public static async Task ValidateOpenApiDocument(
            string openapi, 
            LogLevel logLevel, 
            CancellationToken cancellationToken)
        {
            using var loggerFactory = Logger.ConfigureLogger(logLevel);
            var logger = loggerFactory.CreateLogger<OpenApiService>();
            try
            {
                if (string.IsNullOrEmpty(openapi))
                {
                    throw new ArgumentNullException(nameof(openapi));
                }
                var stream = await GetStream(openapi, logger, cancellationToken);

                OpenApiDocument document;
                Stopwatch stopwatch = Stopwatch.StartNew();
                using (logger.BeginScope($"Parsing OpenAPI: {openapi}", openapi))
                {
                    stopwatch.Start();

                    var result = await new OpenApiStreamReader(new OpenApiReaderSettings
                    {
                        RuleSet = ValidationRuleSet.GetDefaultRuleSet()
                    }
                    ).ReadAsync(stream);
                    
                    logger.LogTrace("{timestamp}ms: Completed parsing.", stopwatch.ElapsedMilliseconds);

                    document = result.OpenApiDocument;
                    var context = result.OpenApiDiagnostic;
                    if (context.Errors.Count != 0)
                    {
                        using (logger.BeginScope("Detected errors"))
                        {
                            foreach (var error in context.Errors)
                            {
                                logger.LogError(error.ToString());
                            }
                        }
                    } 
                    stopwatch.Stop();
                }

                using (logger.BeginScope("Calculating statistics"))
                {
                    var statsVisitor = new StatsVisitor();
                    var walker = new OpenApiWalker(statsVisitor);
                    walker.Walk(document);

                    logger.LogTrace("Finished walking through the OpenApi document. Generating a statistics report..");
                    logger.LogInformation(statsVisitor.GetStatisticsReport());
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Could not validate the document, reason: {ex.Message}", ex);
            }
        }

        public static IConfiguration GetConfiguration(string settingsFile)
        {
            settingsFile ??= "appsettings.json";

            IConfiguration config = new ConfigurationBuilder()
            .AddJsonFile(settingsFile, true)
            .Build();

            return config;
        }

        /// <summary>
        /// Converts CSDL to OpenAPI
        /// </summary>
        /// <param name="csdl">The CSDL stream.</param>
        /// <returns>An OpenAPI document.</returns>
        public static async Task<OpenApiDocument> ConvertCsdlToOpenApi(Stream csdl, string settingsFile = null)
        {
            using var reader = new StreamReader(csdl);
            var csdlText = await reader.ReadToEndAsync();
            var edmModel = CsdlReader.Parse(XElement.Parse(csdlText).CreateReader());
            
            var config = GetConfiguration(settingsFile);
            var settings = config.GetSection("OpenApiConvertSettings").Get<OpenApiConvertSettings>();

<<<<<<< HEAD
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
                EnableDiscriminatorValue = true,
                EnableDerivedTypesReferencesForRequestBody = false,
                EnableDerivedTypesReferencesForResponses = false,
                ShowRootPath = false,
                ShowLinks = false,
                ExpandDerivedTypesNavigationProperties = false,
                EnableCount = true,
                UseSuccessStatusCodeRange = true
            };
=======
            settings ??= new OpenApiConvertSettings()
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
                    EnableDiscriminatorValue = true,
                    EnableDerivedTypesReferencesForRequestBody = false,
                    EnableDerivedTypesReferencesForResponses = false,
                    ShowRootPath = false,
                    ShowLinks = false,
                    ExpandDerivedTypesNavigationProperties = false,
                    EnableCount = true,
                    UseSuccessStatusCodeRange = true
                };
            
>>>>>>> e2101373 (Add a settingsFile parameter that allows one to input a path to the settingsfile)
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
                if(item.ValueKind == JsonValueKind.Object)
                {
                   if(item.TryGetProperty("request", out var request))
                   {
                        // Fetch list of methods and urls from collection, store them in a dictionary
                        var path = request.GetProperty("url").GetProperty("raw").ToString();
                        var method = request.GetProperty("method").ToString();
                        if (!paths.ContainsKey(path))
                        {
                            paths.Add(path, new List<string> { method });
                        }
                        else
                        {
                            paths[path].Add(method);
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
        private static async Task<Stream> GetStream(string input, ILogger logger, CancellationToken cancellationToken)
        {
            Stream stream;
            using (logger.BeginScope("Reading input stream"))
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();

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
                    catch (Exception ex) when (ex is FileNotFoundException ||
                        ex is PathTooLongException ||
                        ex is DirectoryNotFoundException ||
                        ex is IOException ||
                        ex is UnauthorizedAccessException ||
                        ex is SecurityException ||
                        ex is NotSupportedException)
                    {
                        throw new InvalidOperationException($"Could not open the file at {input}", ex);
                    }
                }
                stopwatch.Stop();
                logger.LogTrace("{timestamp}ms: Read file {input}", stopwatch.ElapsedMilliseconds, input);
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
            return !input.StartsWith("http") && Path.GetExtension(input) == ".json" ? OpenApiFormat.Json : OpenApiFormat.Yaml;
        }

        private static string GetInputPathExtension(string openapi = null, string csdl = null)
        {
            var extension = String.Empty;
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

        private static ILoggerFactory ConfigureLoggerInstance(LogLevel loglevel)
        {
            // Configure logger options
#if DEBUG
            loglevel = loglevel > LogLevel.Debug ? LogLevel.Debug : loglevel;
#endif

            return Microsoft.Extensions.Logging.LoggerFactory.Create((builder) => {
                builder
                    .AddSimpleConsole(c => {
                        c.IncludeScopes = true;
                    })
#if DEBUG   
                    .AddDebug()
#endif
                    .SetMinimumLevel(loglevel);
            });
        }
    }
}
