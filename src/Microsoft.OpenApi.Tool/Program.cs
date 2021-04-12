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
        static async Task<int> Main(string[] args)
        {
            var command = new RootCommand
            {
               new Option("--input") { Argument = new Argument<FileInfo>() },
                new Option("--output") { Argument = new Argument<FileInfo>() },
                new Option("--version") { Argument = new Argument<OpenApiSpecVersion>() },
                new Option("--format") { Argument = new Argument<OpenApiFormat>() },
                new Option("--inline") { Argument = new Argument<bool>() },
                new Option("--resolveExternal") { Argument = new Argument<bool>() }
            };

            command.Handler = CommandHandler.Create<FileInfo,FileInfo,OpenApiSpecVersion,OpenApiFormat,bool, bool>(
                OpenApiService.ProcessOpenApiDocument);

            // Parse the incoming args and invoke the handler
            return await command.InvokeAsync(args);
        }
    }
}
