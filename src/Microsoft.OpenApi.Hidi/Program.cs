// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.OpenApi.Hidi
{
    static class Program
    {
        static async Task<int> Main(string[] args)
        {
            var rootCommand = new RootCommand() {
            };
            
            // command option parameters and aliases
            var descriptionOption = new Option("--openapi", "Input OpenAPI description file path or URL", typeof(string));
            descriptionOption.AddAlias("-d");

            var outputOption = new Option("--output", "The output directory path for the generated file.", typeof(FileInfo), () => "./output", arity: ArgumentArity.ZeroOrOne);
            outputOption.AddAlias("-o");

            var versionOption = new Option("--version", "OpenAPI specification version", typeof(OpenApiSpecVersion));
            versionOption.AddAlias("-v");

            var formatOption = new Option("--format", "File format", typeof(OpenApiFormat));
            formatOption.AddAlias("-f");
            
            var inlineOption = new Option("--inline", "Inline $ref instances", typeof(bool));
            inlineOption.AddAlias("-i");

            var resolveExternalOption = new Option("--resolve-external", "Resolve external $refs", typeof(bool));
            resolveExternalOption.AddAlias("-ex");

            var filterByOperationIdsOption = new Option("--filter-by-operationids", "Filters OpenApiDocument by OperationId(s) provided", typeof(string));
            filterByOperationIdsOption.AddAlias("-op");

            var filterByTagsOption = new Option("--filter-by-tags", "Filters OpenApiDocument by Tag(s) provided", typeof(string));
            filterByTagsOption.AddAlias("-t");

            var filterByCollectionOption = new Option("--filter-by-collection", "Filters OpenApiDocument by Postman collection provided", typeof(string));
            filterByCollectionOption.AddAlias("-c");

            var validateCommand = new Command("validate")
            {
                descriptionOption
            };
            validateCommand.Handler = CommandHandler.Create<string>(OpenApiService.ValidateOpenApiDocument);

            var transformCommand = new Command("transform")
            {
                descriptionOption,
                outputOption,
                versionOption,
                formatOption,
                inlineOption,
                resolveExternalOption,
                filterByOperationIdsOption,
                filterByTagsOption,
                filterByCollectionOption
            };
            transformCommand.Handler = CommandHandler.Create<string, FileInfo, OpenApiSpecVersion?, OpenApiFormat?, string, string, string, bool, bool>(
                OpenApiService.ProcessOpenApiDocument);

            rootCommand.Add(transformCommand);
            rootCommand.Add(validateCommand);

            // Parse the incoming args and invoke the handler
            return await rootCommand.InvokeAsync(args);
        }
    }
}
