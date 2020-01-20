using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Readers;
using Microsoft.OpenApi.Validations;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Tool
{
    static class OpenApiService
    {
        public static void ProcessOpenApiDocument(
            FileInfo fileOption,
            string outputPath,
            OpenApiSpecVersion version,
            OpenApiFormat format,
            bool inline = false)
        {
                Stream stream = fileOption.OpenRead();

                var document = new OpenApiStreamReader(new OpenApiReaderSettings
                {
                    ReferenceResolution = ReferenceResolutionSetting.ResolveLocalReferences,
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

            using (var outputStream = new FileStream(outputPath, FileMode.Create))
            {
                document.Serialize(
                   outputStream,
                   version,
                   format,
                   new OpenApiWriterSettings()
                   {
                       ReferenceInline = inline == true ? ReferenceInlineSetting.InlineLocalReferences : ReferenceInlineSetting.DoNotInlineReferences
                   });

                outputStream.Position = 0;
                outputStream.Flush();
            }
        }
}
}
