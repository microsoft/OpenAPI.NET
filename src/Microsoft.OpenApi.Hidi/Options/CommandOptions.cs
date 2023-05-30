using System.Collections.Generic;
using System.CommandLine;
using System.IO;
using Microsoft.Extensions.Logging;

namespace Microsoft.OpenApi.Hidi.Options
{
    internal class CommandOptions
    {
        // command option parameters and aliases
        public Option<string> OpenApiDescriptionOption = new("--openapi", "Input OpenAPI description file path or URL");
        public Option<string> CsdlOption = new("--csdl", "Input CSDL file path or URL");
        public Option<string> CsdlFilterOption = new("--csdl-filter", "Comma delimited list of EntitySets or Singletons to filter CSDL on. e.g. tasks,accounts");
        public Option<FileInfo> OutputOption = new("--output", "The output file path for the generated file.") { Arity = ArgumentArity.ZeroOrOne };
        public Option<string> OutputFolderOption = new("--output-folder", "The output directory path for the generated files.") { Arity = ArgumentArity.ZeroOrOne };
        public Option<bool> CleanOutputOption = new("--clean-output", "Overwrite an existing file");
        public Option<string> VersionOption = new("--version", "OpenAPI specification version");
        public Option<string> MetadataVersionOption = new("--metadata-version", "Graph metadata version to use.");
        public Option<OpenApiFormat?> FormatOption = new("--format", "File format");
        public Option<bool> TerseOutputOption = new("--terse-output", "Produce terse json output");
        public Option<string> SettingsFileOption = new("--settings-path", "The configuration file with CSDL conversion settings.");
        public Option<LogLevel> LogLevelOption = new("--log-level", () => LogLevel.Information, "The log level to use when logging messages to the main output.");
        public Option<string> FilterByOperationIdsOption = new("--filter-by-operationids", "Filters OpenApiDocument by comma delimited list of OperationId(s) provided");
        public Option<string> FilterByTagsOption = new("--filter-by-tags", "Filters OpenApiDocument by comma delimited list of Tag(s) provided. Also accepts a single regex.");
        public Option<string> FilterByCollectionOption = new("--filter-by-collection", "Filters OpenApiDocument by Postman collection provided. Provide path to collection file.");
        public Option<string> ManifestOption = new("--manifest", "Path to API manifest file to locate and filter an OpenApiDocument");
        public Option<bool> InlineLocalOption = new("--inline-local", "Inline local $ref instances");
        public Option<bool> InlineExternalOption = new("--inline-external", "Inline external $ref instances");

        public CommandOptions()
        {
            OpenApiDescriptionOption.AddAlias("-d");
            CsdlOption.AddAlias("--cs");
            CsdlFilterOption.AddAlias("--csf");
            OutputOption.AddAlias("-o");
            OutputFolderOption.AddAlias("--of");
            CleanOutputOption.AddAlias("--co");
            VersionOption.AddAlias("-v");
            MetadataVersionOption.AddAlias("--mv");
            FormatOption.AddAlias("-f");
            TerseOutputOption.AddAlias("--to");
            SettingsFileOption.AddAlias("--sp");
            LogLevelOption.AddAlias("--ll");
            FilterByOperationIdsOption.AddAlias("--op");
            FilterByTagsOption.AddAlias("--t");
            FilterByCollectionOption.AddAlias("-c");
            ManifestOption.AddAlias("-m");
            InlineLocalOption.AddAlias("--il");
            InlineExternalOption.AddAlias("--ie");
        }

        public IReadOnlyList<Option> GetAllCommandOptions()
        {
            return new List<Option>
            {
                OpenApiDescriptionOption,
                CsdlOption,
                CsdlFilterOption,
                OutputOption,
                CleanOutputOption,
                VersionOption,
                MetadataVersionOption,
                FormatOption,
                TerseOutputOption,
                SettingsFileOption,
                LogLevelOption,
                FilterByOperationIdsOption,
                FilterByTagsOption,
                FilterByCollectionOption,
                InlineLocalOption,
                InlineExternalOption
            };
        }

        public IReadOnlyList<Option> GetValidateCommandOptions()
        {
            return new List<Option>
            {
                OpenApiDescriptionOption,
                LogLevelOption,
            };
        }

        public IReadOnlyList<Option> GetShowCommandOptions()
        {
            return new List<Option>
            {
                OpenApiDescriptionOption,
                CsdlOption,
                CsdlFilterOption,
                OutputOption,
                CleanOutputOption,
                LogLevelOption
            };
        }

        public IReadOnlyList<Option> GetPluginCommandOptions()
        {
            return new List<Option>
            {

                ManifestOption,
                OutputFolderOption,
                CleanOutputOption,
                LogLevelOption
            };
        }

    }
}
