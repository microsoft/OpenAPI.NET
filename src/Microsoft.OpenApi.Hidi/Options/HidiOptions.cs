// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.CommandLine.Parsing;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Hidi.Utilities;

namespace Microsoft.OpenApi.Hidi.Options
{
    internal class HidiOptions
    {
        public string OpenApi { get; set; }
        public string Csdl { get; set; }
        public string CsdlFilter { get; set; }
        public FileInfo Output { get; set; }
        public bool CleanOutput { get; set; }
        public string Version { get; set; }
        public string MetadataVersion { get; set; }
        public OpenApiFormat? OpenApiFormat { get; set; }
        public bool TerseOutput { get; set; }
        public IConfiguration SettingsConfig { get; set; }
        public LogLevel LogLevel { get; set; }
        public bool InlineLocal { get; set; }
        public bool InlineExternal { get; set; }
        public FilterOptions FilterOptions { get; set; }

        public HidiOptions(ParseResult parseResult, CommandOptions options)
        {
            ParseHidiOptions(parseResult, options);
        }

        public HidiOptions()
        {

        }

        private void ParseHidiOptions(ParseResult parseResult, CommandOptions options)
        {
            OpenApi = parseResult.GetValueForOption(options.OpenApiDescriptionOption);
            CsdlFilter = parseResult.GetValueForOption(options.CsdlFilterOption);
            Csdl = parseResult.GetValueForOption(options.CsdlOption);
            Output = parseResult.GetValueForOption(options.OutputOption);
            CleanOutput = parseResult.GetValueForOption(options.CleanOutputOption);
            Version = parseResult.GetValueForOption(options.VersionOption);
            MetadataVersion = parseResult.GetValueForOption(options.MetadataVersionOption);
            OpenApiFormat = parseResult.GetValueForOption(options.FormatOption);
            TerseOutput = parseResult.GetValueForOption(options.TerseOutputOption);
            SettingsConfig = SettingsUtilities.GetConfiguration(parseResult.GetValueForOption(options.SettingsFileOption));
            LogLevel = parseResult.GetValueForOption(options.LogLevelOption);
            InlineLocal = parseResult.GetValueForOption(options.InlineLocalOption);
            InlineExternal = parseResult.GetValueForOption(options.InlineExternalOption);
            FilterOptions = new FilterOptions
            {
                FilterByOperationIds = parseResult.GetValueForOption(options.FilterByOperationIdsOption),
                FilterByTags = parseResult.GetValueForOption(options.FilterByTagsOption),
                FilterByCollection = parseResult.GetValueForOption(options.FilterByCollectionOption),
                FilterByApiManifest = parseResult.GetValueForOption(options.FilterByApiManifestOption)
            };
        }
    }
}
