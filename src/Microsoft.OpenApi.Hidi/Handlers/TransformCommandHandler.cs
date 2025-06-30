// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Hidi.Options;

namespace Microsoft.OpenApi.Hidi.Handlers
{
    internal class TransformCommandHandler : AsynchronousCommandLineAction
    {
        public CommandOptions CommandOptions { get; }
        public TransformCommandHandler(CommandOptions commandOptions)
        {
            CommandOptions = commandOptions;
        }
        public override async Task<int> InvokeAsync(ParseResult parseResult, CancellationToken cancellationToken = default)
        {
            var hidiOptions = new HidiOptions(parseResult, CommandOptions);

            using var loggerFactory = Logger.ConfigureLogger(hidiOptions.LogLevel);
            var logger = loggerFactory.CreateLogger<TransformCommandHandler>();
            try
            {
                await OpenApiService.TransformOpenApiDocumentAsync(hidiOptions, logger, cancellationToken).ConfigureAwait(false);

                return 0;
            }
#if RELEASE
#pragma warning disable CA1031 // Do not catch general exception types
#endif
            catch (Exception ex)
            {
#if DEBUG
                logger.LogCritical(ex, "Command failed");
                throw; // so debug tools go straight to the source of the exception when attached
#else
#pragma warning disable CA2254
                logger.LogCritical( ex.Message);
#pragma warning restore CA2254
                return 1;
#endif
            }
#if RELEASE
#pragma warning restore CA1031 // Do not catch general exception types
#endif
        }
    }
}
