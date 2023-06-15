// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.CommandLine.Invocation;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Hidi.Options;

namespace Microsoft.OpenApi.Hidi.Handlers
{
    internal class ShowCommandHandler : ICommandHandler
    {
        public CommandOptions CommandOptions { get; set; }
        public ShowCommandHandler(CommandOptions commandOptions)
        {
            CommandOptions = commandOptions;
        }
        public int Invoke(InvocationContext context)
        {
            return InvokeAsync(context).GetAwaiter().GetResult();
        }
        public async Task<int> InvokeAsync(InvocationContext context)
        {
            HidiOptions hidiOptions = new HidiOptions(context.ParseResult, CommandOptions);
            CancellationToken cancellationToken = (CancellationToken)context.BindingContext.GetService(typeof(CancellationToken));

            using var loggerFactory = Logger.ConfigureLogger(hidiOptions.LogLevel);
            var logger = loggerFactory.CreateLogger<ShowCommandHandler>();
            try
            {
                await OpenApiService.ShowOpenApiDocument(hidiOptions, logger, cancellationToken);

                return 0;
            }
            catch (Exception ex)
            {
#if DEBUG
                logger.LogCritical(ex, "Command failed");
                throw; // so debug tools go straight to the source of the exception when attached
#else
                logger.LogCritical( ex.Message);
                return 1;
#endif
            }
        }
    }
}
