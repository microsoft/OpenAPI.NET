using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Threading.Tasks;
using Microsoft.OpenApi;

namespace Microsoft.OpenApi.Tool
{
    class Program
    {
        static async Task<int> OldMain(string[] args)
        {

            var command = new RootCommand
            {
               new Option("--input", "Input OpenAPI description file path or URL", typeof(string) ),
                new Option("--output","Output OpenAPI description file", typeof(FileInfo), arity: ArgumentArity.ZeroOrOne),
                new Option("--version", "OpenAPI specification version", typeof(OpenApiSpecVersion)),
                new Option("--format", "File format",typeof(OpenApiFormat) ),
                new Option("--inline", "Inline $ref instances", typeof(bool) ),
                new Option("--resolveExternal","Resolve external $refs", typeof(bool)) 
            };

            command.Handler = CommandHandler.Create<string,FileInfo,OpenApiSpecVersion,OpenApiFormat,bool, bool>(
                OpenApiService.ProcessOpenApiDocument);

            // Parse the incoming args and invoke the handler
            return await command.InvokeAsync(args);
        }

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
                new Option("--resolveExternal","Resolve external $refs", typeof(bool))
            };
            transformCommand.Handler = CommandHandler.Create<string, FileInfo, OpenApiSpecVersion, OpenApiFormat, bool, bool>(
                OpenApiService.ProcessOpenApiDocument);

            rootCommand.Add(transformCommand);
            rootCommand.Add(validateCommand);

            // Parse the incoming args and invoke the handler
            return await rootCommand.InvokeAsync(args);
        }
    }
}
