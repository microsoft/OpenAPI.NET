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

            var validateCommand = new Command("validate")
            {
                new Option("--input", "Input OpenAPI description file path or URL", typeof(string) )
            };
            validateCommand.Handler = CommandHandler.Create<string>(OpenApiService.ValidateOpenApiDocument);

            var transformCommand = new Command("transform")
            {
                new Option("--input", "Input OpenAPI description file path or URL", typeof(string) ),
                new Option("--output","Output OpenAPI description file", typeof(FileInfo), arity: ArgumentArity.ZeroOrOne),
                new Option("--version", "OpenAPI specification version", typeof(OpenApiSpecVersion)),
                new Option("--format", "File format",typeof(OpenApiFormat) ),
                new Option("--inline", "Inline $ref instances", typeof(bool) ),
                new Option("--resolveExternal","Resolve external $refs", typeof(bool)),
                new Option("--filterByOperationIds", "Filters OpenApiDocument by OperationId(s) provided", typeof(string)),
                new Option("--filterByTags", "Filters OpenApiDocument by Tag(s) provided", typeof(string)),
                new Option("--filterByCollection", "Filters OpenApiDocument by Postman collection provided", typeof(string))
            };
            transformCommand.Handler = CommandHandler.Create<string, FileInfo, OpenApiSpecVersion, OpenApiFormat, string, string, string, bool, bool>(
                OpenApiService.ProcessOpenApiDocument);

            rootCommand.Add(transformCommand);
            rootCommand.Add(validateCommand);

            // Parse the incoming args and invoke the handler
            return await rootCommand.InvokeAsync(args);
        }
    }
}
