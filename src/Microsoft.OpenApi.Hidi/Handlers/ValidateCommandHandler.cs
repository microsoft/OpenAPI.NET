// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Microsoft.OpenApi.Hidi.Handlers
{
    internal class ValidateCommandHandler : ICommandHandler
    {
        public Option<string> DescriptionOption { get; set; }
        public Option<LogLevel> LogLevelOption { get; set; }

        public int Invoke(InvocationContext context)
        {
            return InvokeAsync(context).GetAwaiter().GetResult();
        }
        public async Task<int> InvokeAsync(InvocationContext context)
        {
            string openapi = context.ParseResult.GetValueForOption(DescriptionOption);
            LogLevel logLevel = context.ParseResult.GetValueForOption(LogLevelOption);
            CancellationToken cancellationToken = (CancellationToken)context.BindingContext.GetService(typeof(CancellationToken));


            var logger = Logger.ConfigureLogger(logLevel);

            try
            {
                await OpenApiService.ValidateOpenApiDocument(openapi, logLevel, cancellationToken);
                return 0;
            }
            catch (Exception ex)
            {
#if DEBUG
                logger.LogCritical(ex, ex.Message);
                throw; // so debug tools go straight to the source of the exception when attached
#else
                logger.LogCritical( ex.Message);
                Environment.Exit(1);
                return 1;
#endif
            }
        }
    }
}
