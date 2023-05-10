// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.CommandLine;
using System.CommandLine.Parsing;
using System.Threading.Tasks;
using Microsoft.OpenApi.Hidi.Handlers;
using Microsoft.OpenApi.Hidi.Options;
using Microsoft.OpenApi.Hidi.Extensions;

namespace Microsoft.OpenApi.Hidi
{
    static class Program
    {
        static async Task<int> Main(string[] args)
        {
            var rootCommand = CreateRootCommand();

            // Parse the incoming args and invoke the handler
            return await rootCommand.InvokeAsync(args);
        }

        internal static RootCommand CreateRootCommand()
        {
            var rootCommand = new RootCommand() { };

            var commandOptions = new CommandOptions();

            var validateCommand = new Command("validate");
            validateCommand.AddOptions(commandOptions.GetValidateCommandOptions());
            validateCommand.Handler = new ValidateCommandHandler(commandOptions);

            var transformCommand = new Command("transform");
            transformCommand.AddOptions(commandOptions.GetAllCommandOptions());

            transformCommand.Handler = new TransformCommandHandler(commandOptions);

            var showCommand = new Command("show");
            showCommand.AddOptions(commandOptions.GetShowCommandOptions());
            showCommand.Handler = new ShowCommandHandler(commandOptions);

            rootCommand.Add(showCommand);
            rootCommand.Add(transformCommand);
            rootCommand.Add(validateCommand);
            return rootCommand;
        }
    }
}
