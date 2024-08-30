// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.CommandLine.Invocation;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Hidi.Options;

namespace Microsoft.OpenApi.Hidi.Handlers
{
    internal class ValidateCommandHandler : AsyncCommandHandler
    {
        public CommandOptions CommandOptions { get; }

        public ValidateCommandHandler(CommandOptions commandOptions)
        {
            CommandOptions = commandOptions;
        }
        public override async Task<int> InvokeAsync(InvocationContext context)
        {
            var hidiOptions = new HidiOptions(context.ParseResult, CommandOptions);
            var cancellationToken = (CancellationToken)context.BindingContext.GetRequiredService(typeof(CancellationToken));
            using var loggerFactory = Logger.ConfigureLogger(hidiOptions.LogLevel);
            var logger = loggerFactory.CreateLogger<ValidateCommandHandler>();
            try
            {
                if (hidiOptions.OpenApi is null) throw new InvalidOperationException("OpenApi file is required");
                var isValid = await OpenApiService.ValidateOpenApiDocumentAsync(hidiOptions.OpenApi, logger, cancellationToken).ConfigureAwait(false);
                return isValid is not false ? 0 : -1;
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
                logger.LogCritical(ex.Message);
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
