using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using Microsoft.OpenApi;

namespace Microsoft.OpenApi.Tool
{
    class Program
    {
        static int Main(string[] args)
        {
            var rootCommand = new RootCommand
            {
                new Option(
                    "--input",
                    "Input OpenAPI description")
                {
                    Argument = new Argument<FileInfo>()
                },
                new Option(
                    "--output",
                    "Output path for OpenAPI Description")
                {
                    Argument = new Argument<string>()
                },
                new Option(
                    "--output-version",
                    "OpenAPI Version")
                {
                    Argument = new Argument<OpenApiSpecVersion>(() => OpenApiSpecVersion.OpenApi3_0)
                },
                new Option(
                    "--output-format",
                    "OpenAPI format [Json | Yaml")
                {
                    Argument = new Argument<OpenApiFormat>(() => OpenApiFormat.Yaml )
                }
            };

            rootCommand.Description = "OpenAPI";

            rootCommand.Handler = CommandHandler.Create<FileInfo,string,OpenApiSpecVersion,OpenApiFormat, bool>(
               OpenApiService.ProcessOpenApiDocument);

            // Parse the incoming args and invoke the handler
            return rootCommand.InvokeAsync(args).Result;
        }
    }
}
