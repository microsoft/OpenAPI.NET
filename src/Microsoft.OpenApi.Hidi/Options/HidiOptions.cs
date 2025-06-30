// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.CommandLine;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Hidi.Utilities;

namespace Microsoft.OpenApi.Hidi.Options
{
    internal class HidiOptions
    {
        private const string defaultOutputFolderValue = "./";
        public string? OpenApi { get; set; }
        public string? Csdl { get; set; }
        public string? CsdlFilter { get; set; }
        public FileInfo? Output { get; set; }
        public string OutputFolder { get; set; } = defaultOutputFolderValue;
        public bool CleanOutput { get; set; }
        public string? Version { get; set; }
        public string? MetadataVersion { get; set; }
        public string? OpenApiFormat { get; set; }
        public bool TerseOutput { get; set; }
        public IConfiguration? SettingsConfig { get; set; }
        public LogLevel LogLevel { get; set; }
        public bool InlineLocal { get; set; }
        public bool InlineExternal { get; set; }
        public FilterOptions FilterOptions { get; set; } = new();

        public HidiOptions(ParseResult parseResult, CommandOptions options)
        {
            ParseHidiOptions(parseResult, options);
        }

        public HidiOptions()
        {
        }

        private void ParseHidiOptions(ParseResult parseResult, CommandOptions options)
        {
            OpenApi = parseResult.GetValue(options.OpenApiDescriptionOption);
            CsdlFilter = parseResult.GetValue(options.CsdlFilterOption);
            Csdl = parseResult.GetValue(options.CsdlOption);
            Output = parseResult.GetValue(options.OutputOption);
            OutputFolder = parseResult.GetValue(options.OutputFolderOption) is string outputFolderOptionValue && !string.IsNullOrEmpty(outputFolderOptionValue) ? outputFolderOptionValue : defaultOutputFolderValue;
            CleanOutput = parseResult.GetValue(options.CleanOutputOption);
            Version = parseResult.GetValue(options.VersionOption);
            MetadataVersion = parseResult.GetValue(options.MetadataVersionOption);
            OpenApiFormat = parseResult.GetValue(options.FormatOption);
            TerseOutput = parseResult.GetValue(options.TerseOutputOption);
            SettingsConfig = parseResult.GetValue(options.SettingsFileOption) is string configOptionValue && !string.IsNullOrEmpty(configOptionValue) ? SettingsUtilities.GetConfiguration(configOptionValue) : null;
            LogLevel = parseResult.GetValue(options.LogLevelOption);
            InlineLocal = parseResult.GetValue(options.InlineLocalOption);
            InlineExternal = parseResult.GetValue(options.InlineExternalOption);
            FilterOptions = new()
            {
                FilterByOperationIds = parseResult.GetValue(options.FilterByOperationIdsOption),
                FilterByTags = parseResult.GetValue(options.FilterByTagsOption),
                FilterByCollection = parseResult.GetValue(options.FilterByCollectionOption),
                FilterByApiManifest = parseResult.GetValue(options.ManifestOption)
            };
        }
    }
}
