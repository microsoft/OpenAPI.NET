using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;
using Microsoft.OpenApi.Validations;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Tool
{
    static class OpenApiService
    {
        public static void ProcessOpenApiDocument(
            FileInfo input,
            FileInfo output,
            OpenApiSpecVersion version,
            OpenApiFormat format,
            bool inline,
            bool resolveExternal)
        {
            OpenApiDocument document;
            using (Stream stream = input.OpenRead())
            {

                document = new OpenApiStreamReader(new OpenApiReaderSettings
                {
                    ReferenceResolution = resolveExternal == true ? ReferenceResolutionSetting.ResolveAllReferences : ReferenceResolutionSetting.ResolveLocalReferences,
                    RuleSet = ValidationRuleSet.GetDefaultRuleSet()
                }
                ).Read(stream, out var context);
                if (context.Errors.Count != 0)
                {
                    var errorReport = new StringBuilder();

                    foreach (var error in context.Errors)
                    {
                        errorReport.AppendLine(error.ToString());
                    }

                    throw new ArgumentException(String.Join(Environment.NewLine, context.Errors.Select(e => e.Message).ToArray()));
                }
            }

            using (var outputStream = output?.Create())
            {
                TextWriter textWriter;

                if (outputStream!=null)
                {
                    textWriter = new StreamWriter(outputStream);
                } else
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
                
                document.Serialize(writer,version );

                textWriter.Flush();
            }
        }
}
}
