using System.Collections.Generic;
using System.CommandLine;
using System.IO;
using Microsoft.Extensions.Logging;

namespace Microsoft.OpenApi.Hidi.Options
{
    internal class CommandOptions
    {
        // command option parameters and aliases
        public readonly Option<string> OpenApiDescriptionOption = new("--openapi", "-d")
        {
            Description = "Input OpenAPI description file path or URL",
        };
        public readonly Option<string> CsdlOption = new("--csdl", "--cs")
        {
            Description = "Input CSDL file path or URL",
        };
        public readonly Option<string> CsdlFilterOption = new("--csdl-filter", "--csf")
        {
            Description = "Comma delimited list of EntitySets or Singletons to filter CSDL on. e.g. tasks,accounts",
        };
        public readonly Option<FileInfo> OutputOption = new("--output", "-o")
        {
            Description = "The output file path for the generated file.",
            Arity = ArgumentArity.ZeroOrOne
        };
        public readonly Option<string> OutputFolderOption = new("--output-folder", "--of")
        {
            Description = "The output directory path for the generated files.",
            Arity = ArgumentArity.ZeroOrOne
        };
        public readonly Option<bool> CleanOutputOption = new("--clean-output", "--co")
        {
            Description = "Overwrite an existing file",
        };
        public readonly Option<string> VersionOption = new("--version", "-v")
        {
            Description = "OpenAPI specification version",
        };
        public readonly Option<string> MetadataVersionOption = new("--metadata-version", "--mv")
        {
            Description = "Graph metadata version to use.",
        };
        public readonly Option<string> FormatOption = new("--format", "-f")
        {
            Description = "File format",
        };
        public readonly Option<bool> TerseOutputOption = new("--terse-output", "--to")
        {
            Description = "Produce terse json output",
        };
        public readonly Option<string> SettingsFileOption = new("--settings-path", "--sp")
        {
            Description = "The configuration file with CSDL conversion settings.",
        };
        public readonly Option<LogLevel> LogLevelOption = new("--log-level", "--ll")
        {
            Description = "The log level to use when logging messages to the main output.",
            DefaultValueFactory = (_) => LogLevel.Information,
        };
        public readonly Option<string> FilterByOperationIdsOption = new("--filter-by-operationids", "--op")
        {
            Description = "Filters OpenApiDocument by comma delimited list of OperationId(s) provided",
        };
        public readonly Option<string> FilterByTagsOption = new("--filter-by-tags", "-t")
        {
            Description = "Filters OpenApiDocument by comma delimited list of Tag(s) provided. Also accepts a single regex.",
        };
        public readonly Option<string> FilterByCollectionOption = new("--filter-by-collection", "-c")
        {
             Description = "Filters OpenApiDocument by Postman collection provided. Provide path to collection file.",
        };
        public readonly Option<string> ManifestOption = new("--manifest", "-m")
        {
            Description = "Path to API manifest file to locate and filter an OpenApiDocument",
        };
        public readonly Option<bool> InlineLocalOption = new("--inline-local", "--il")
        {
            Description = "Inline local $ref instances",
        };
        public readonly Option<bool> InlineExternalOption = new("--inline-external", "--ie")
        {
            Description = "Inline external $ref instances",
        };

        public IReadOnlyList<Option> GetAllCommandOptions()
        {
            return
            [
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
            ];
        }

        public IReadOnlyList<Option> GetValidateCommandOptions()
        {
            return
            [
                OpenApiDescriptionOption,
                LogLevelOption,
            ];
        }

        public IReadOnlyList<Option> GetShowCommandOptions()
        {
            return
            [
                OpenApiDescriptionOption,
                CsdlOption,
                CsdlFilterOption,
                OutputOption,
                CleanOutputOption,
                LogLevelOption
            ];
        }

        public IReadOnlyList<Option> GetPluginCommandOptions()
        {
            return
            [
                ManifestOption,
                OutputFolderOption,
                CleanOutputOption,
                LogLevelOption
            ];
        }
    }

}
