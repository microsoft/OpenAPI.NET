// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.CommandLine;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Hidi.Handlers;

namespace Microsoft.OpenApi.Hidi
{
    static class Program
    {
        static async Task Main(string[] args)
        {
            var rootCommand = new RootCommand()
            {
            };

            // command option parameters and aliases
            var descriptionOption = new Option<string>("--openapi", "Input OpenAPI description file path or URL");
            descriptionOption.AddAlias("-d");

            var csdlOption = new Option<string>("--csdl", "Input CSDL file path or URL");
            csdlOption.AddAlias("--cs");

            var csdlFilterOption = new Option<string>("--csdl-filter", "Comma delimited list of EntitySets or Singletons to filter CSDL on. e.g. tasks,accounts");
            csdlFilterOption.AddAlias("--csf");

            var outputOption = new Option<FileInfo>("--output", "The output directory path for the generated file.") { Arity = ArgumentArity.ZeroOrOne };
            outputOption.AddAlias("-o");

            var cleanOutputOption = new Option<bool>("--clean-output", "Overwrite an existing file");
            cleanOutputOption.AddAlias("--co");

            var versionOption = new Option<string?>("--version", "OpenAPI specification version");
            versionOption.AddAlias("-v");

            var formatOption = new Option<OpenApiFormat?>("--format", "File format");
            formatOption.AddAlias("-f");

            var terseOutputOption = new Option<bool>("--terse-output", "Produce terse json output");
            terseOutputOption.AddAlias("--to");

            var logLevelOption = new Option<LogLevel>("--log-level", () => LogLevel.Information, "The log level to use when logging messages to the main output.");
            logLevelOption.AddAlias("--ll");

            var filterByOperationIdsOption = new Option<string>("--filter-by-operationids", "Filters OpenApiDocument by comma delimited list of OperationId(s) provided");
            filterByOperationIdsOption.AddAlias("--op");

            var filterByTagsOption = new Option<string>("--filter-by-tags", "Filters OpenApiDocument by comma delimited list of Tag(s) provided. Also accepts a single regex.");
            filterByTagsOption.AddAlias("--t");

            var filterByCollectionOption = new Option<string>("--filter-by-collection", "Filters OpenApiDocument by Postman collection provided. Provide path to collection file.");
            filterByCollectionOption.AddAlias("-c");

            var inlineLocalOption = new Option<bool>("--inline-local", "Inline local $ref instances");
            inlineLocalOption.AddAlias("--il");

            var inlineExternalOption = new Option<bool>("--inline-external", "Inline external $ref instances");
            inlineExternalOption.AddAlias("--ie");

            var validateCommand = new Command("validate")
            {
                descriptionOption,
                logLevelOption
            };

            validateCommand.Handler = new ValidateCommandHandler
            {
                DescriptionOption = descriptionOption,
                LogLevelOption = logLevelOption
            };

            var transformCommand = new Command("transform")
            {
                descriptionOption,
                csdlOption,
                csdlFilterOption,
                outputOption,
                cleanOutputOption,
                versionOption,
                formatOption,
                terseOutputOption,
                logLevelOption,
                filterByOperationIdsOption,
                filterByTagsOption,
                filterByCollectionOption,
                inlineLocalOption,
                inlineExternalOption
            };

            transformCommand.Handler = new TransformCommandHandler
            {
                DescriptionOption = descriptionOption,
                CsdlOption = csdlOption,
                CsdlFilterOption = csdlFilterOption,
                OutputOption = outputOption,
                CleanOutputOption = cleanOutputOption,
                VersionOption = versionOption,
                FormatOption = formatOption,
                TerseOutputOption = terseOutputOption,
                LogLevelOption = logLevelOption,
                FilterByOperationIdsOption = filterByOperationIdsOption,
                FilterByTagsOption = filterByTagsOption,
                FilterByCollectionOption = filterByCollectionOption,
                InlineLocalOption = inlineLocalOption,
                InlineExternalOption = inlineExternalOption
            };

            rootCommand.Add(transformCommand);
            rootCommand.Add(validateCommand);

            // Parse the incoming args and invoke the handler
            await rootCommand.InvokeAsync(args);

            //// Wait for logger to write messages to the console before exiting
            await Task.Delay(10);
        }
    }
}
