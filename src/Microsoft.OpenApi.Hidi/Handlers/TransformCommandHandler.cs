// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Microsoft.OpenApi.Hidi.Handlers
{
    internal class TransformCommandHandler : ICommandHandler
    {
        public Option<string> DescriptionOption { get; set; }
        public Option<string> CsdlOption { get; set; }
        public Option<string> CsdlFilterOption { get; set; }
        public Option<FileInfo> OutputOption { get; set; }
        public Option<bool> CleanOutputOption { get; set; }
        public Option<string?> VersionOption { get; set; }
        public Option<string?> MetadataVersionOption { get; set; }
        public Option<OpenApiFormat?> FormatOption { get; set; }
        public Option<bool> TerseOutputOption { get; set; }
        public Option<string> SettingsFileOption { get; set; }
        public Option<LogLevel> LogLevelOption { get; set; }
        public Option<string> FilterByOperationIdsOption { get; set; }
        public Option<string> FilterByTagsOption { get; set; }
        public Option<string> FilterByCollectionOption { get; set; }
        public Option<bool> InlineLocalOption { get; set; }
        public Option<bool> InlineExternalOption { get; set; }
        public Option<string?> LanguageFormatOption { get; set; }

        public int Invoke(InvocationContext context)
        {
            return InvokeAsync(context).GetAwaiter().GetResult();
        }
        public async Task<int> InvokeAsync(InvocationContext context)
        {
            string openapi = context.ParseResult.GetValueForOption(DescriptionOption);
            string csdlFilter = context.ParseResult.GetValueForOption(CsdlFilterOption);
            string csdl = context.ParseResult.GetValueForOption(CsdlOption);
            FileInfo output = context.ParseResult.GetValueForOption(OutputOption);
            bool cleanOutput = context.ParseResult.GetValueForOption(CleanOutputOption);
            string? version = context.ParseResult.GetValueForOption(VersionOption);
            string metadataVersion = context.ParseResult.GetValueForOption(MetadataVersionOption);
            OpenApiFormat? format = context.ParseResult.GetValueForOption(FormatOption);
            bool terseOutput = context.ParseResult.GetValueForOption(TerseOutputOption);
            string settingsFile = context.ParseResult.GetValueForOption(SettingsFileOption);
            LogLevel logLevel = context.ParseResult.GetValueForOption(LogLevelOption);
            bool inlineLocal = context.ParseResult.GetValueForOption(InlineLocalOption);
            bool inlineExternal = context.ParseResult.GetValueForOption(InlineExternalOption);
            string? languageFormatOption = context.ParseResult.GetValueForOption(LanguageFormatOption);
            string filterbyoperationids = context.ParseResult.GetValueForOption(FilterByOperationIdsOption);
            string filterbytags = context.ParseResult.GetValueForOption(FilterByTagsOption);
            string filterbycollection = context.ParseResult.GetValueForOption(FilterByCollectionOption);

            CancellationToken cancellationToken = (CancellationToken)context.BindingContext.GetService(typeof(CancellationToken));

            using var loggerFactory = Logger.ConfigureLogger(logLevel);
            var logger = loggerFactory.CreateLogger<OpenApiService>();
            try
            {
                await OpenApiService.TransformOpenApiDocument(openapi, csdl, csdlFilter, output, cleanOutput, version, metadataVersion, format, terseOutput, settingsFile, inlineLocal, inlineExternal, languageFormatOption, filterbyoperationids, filterbytags, filterbycollection, logger, cancellationToken);

                return 0;
            }
            catch (Exception ex)
            {
#if DEBUG
                logger.LogCritical(ex, ex.Message);
                throw; // so debug tools go straight to the source of the exception when attached
#else
                logger.LogCritical( ex.Message);
                return 1;
#endif
            }
        }
    }
}
