using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;
using Microsoft.OpenApi.Services;
using Microsoft.OpenApi.Validations;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Tool
{
    static class OpenApiService
    {
        public static void ProcessOpenApiDocument(
            string input,
            FileInfo output,
            OpenApiSpecVersion version,
            OpenApiFormat format,
            bool inline,
            bool resolveExternal)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
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
            var context = result.OpenApiDiagnostic;

            if (context.Errors.Count != 0)
            {
                var errorReport = new StringBuilder();

                foreach (var error in context.Errors)
                {
                    errorReport.AppendLine(error.ToString());
                }

                throw new ArgumentException(String.Join(Environment.NewLine, context.Errors.Select(e => e.Message).ToArray()));
            }

            using (var outputStream = output?.Create())
            {
                TextWriter textWriter;

                if (outputStream != null)
                {
                    textWriter = new StreamWriter(outputStream);
                }
                else
                {
                    textWriter = Console.Out;
                }

                var settings = new OpenApiWriterSettings()
                {
                    ReferenceInline = inline == true ? ReferenceInlineSetting.InlineLocalReferences : ReferenceInlineSetting.DoNotInlineReferences
                };
                IOpenApiWriter writer;
                switch (format)
                {
                    case OpenApiFormat.Json:
                        writer = new OpenApiJsonWriter(textWriter, settings);
                        break;
                    case OpenApiFormat.Yaml:
                        writer = new OpenApiYamlWriter(textWriter, settings);
                        break;
                    default:
                        throw new ArgumentException("Unknown format");
                }

                document.Serialize(writer, version);

                textWriter.Flush();
            }
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
    }
}
