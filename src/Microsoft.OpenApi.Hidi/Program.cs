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
            var parseResult = rootCommand.Parse(args);
            return parseResult.InvokeAsync();
        }

        internal static RootCommand CreateRootCommand()
        {
            var rootCommand = new RootCommand { };

            var commandOptions = new CommandOptions();

            var validateCommand = new Command("validate");
            validateCommand.Description = "Validate an OpenAPI document.";
            validateCommand.AddOptions(commandOptions.GetValidateCommandOptions());
            var validateCommandHandler = new ValidateCommandHandler(commandOptions);
            validateCommand.SetAction(validateCommandHandler.InvokeAsync);

            var transformCommand = new Command("transform");
            transformCommand.Description = "Transform an OpenAPI or CSDL document into a JSON/YAML OpenAPI v2/v3 document.";
            transformCommand.AddOptions(commandOptions.GetAllCommandOptions());
            var transformCommandHandler = new TransformCommandHandler(commandOptions);
            transformCommand.SetAction(transformCommandHandler.InvokeAsync);

            var showCommand = new Command("show");
            showCommand.Description = "Create a visual representation of the paths in an OpenAPI document.";
            showCommand.AddOptions(commandOptions.GetShowCommandOptions());
            var showCommandHandler = new ShowCommandHandler(commandOptions);
            showCommand.SetAction(showCommandHandler.InvokeAsync);

            var pluginCommand = new Command("plugin");
            pluginCommand.Description = "Create manifest files for an OpenAI plugin [preview]";
            pluginCommand.AddOptions(commandOptions.GetPluginCommandOptions());
            var pluginCommandHandler = new PluginCommandHandler(commandOptions);
            pluginCommand.SetAction(pluginCommandHandler.InvokeAsync);

            rootCommand.Add(showCommand);
            rootCommand.Add(transformCommand);
            rootCommand.Add(validateCommand);
            rootCommand.Add(pluginCommand);
            return rootCommand;
        }
    }
}
