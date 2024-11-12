// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.CommandLine;
using System.Threading.Tasks;
using Microsoft.OpenApi.Hidi.Handlers;
using Microsoft.OpenApi.Hidi.Options;
using Microsoft.OpenApi.Hidi.Extensions;

namespace Microsoft.OpenApi.Hidi
{
    static class Program
    {
        static Task<int> Main(string[] args)
        {
            var rootCommand = CreateRootCommand();

            // Parse the incoming args and invoke the handler
            return rootCommand.InvokeAsync(args);
        }

        internal static RootCommand CreateRootCommand()
        {
            var rootCommand = new RootCommand { };

            var commandOptions = new CommandOptions();

            var validateCommand = new Command("validate");
            validateCommand.Description = "Validate an OpenAPI document.";
            validateCommand.AddOptions(commandOptions.GetValidateCommandOptions());
            validateCommand.Handler = new ValidateCommandHandler(commandOptions);

            var transformCommand = new Command("transform");
            transformCommand.Description = "Transform an OpenAPI or CSDL document into a JSON/YAML OpenAPI v2/v3 document.";
            transformCommand.AddOptions(commandOptions.GetAllCommandOptions());
            transformCommand.Handler = new TransformCommandHandler(commandOptions);

            var showCommand = new Command("show");
            showCommand.Description = "Create a visual representation of the paths in an OpenAPI document.";
            showCommand.AddOptions(commandOptions.GetShowCommandOptions());
            showCommand.Handler = new ShowCommandHandler(commandOptions);

            var pluginCommand = new Command("plugin");
            pluginCommand.Description = "Create manifest files for an OpenAI plugin [preview]";
            pluginCommand.AddOptions(commandOptions.GetPluginCommandOptions());
            pluginCommand.Handler = new PluginCommandHandler(commandOptions);

            rootCommand.Add(showCommand);
            rootCommand.Add(transformCommand);
            rootCommand.Add(validateCommand);
            rootCommand.Add(pluginCommand);
            return rootCommand;
        }
    }
}
