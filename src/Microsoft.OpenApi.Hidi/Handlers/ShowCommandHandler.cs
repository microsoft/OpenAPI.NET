// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Microsoft.OpenApi.Hidi.Handlers
{
    internal class ShowCommandHandler : ICommandHandler
    {
        public Option<string> DescriptionOption { get; set; }
        public Option<FileInfo> OutputOption { get; set; }
        public Option<LogLevel> LogLevelOption { get; set; }
        public Option<string> CsdlOption { get; set; }
        public Option<string> CsdlFilterOption { get; set; }


        public int Invoke(InvocationContext context)
        {
            return InvokeAsync(context).GetAwaiter().GetResult();
        }
        public async Task<int> InvokeAsync(InvocationContext context)
        {
            string openapi = context.ParseResult.GetValueForOption(DescriptionOption);
            FileInfo output = context.ParseResult.GetValueForOption(OutputOption);
            LogLevel logLevel = context.ParseResult.GetValueForOption(LogLevelOption);
            string csdlFilter = context.ParseResult.GetValueForOption(CsdlFilterOption);
            string csdl = context.ParseResult.GetValueForOption(CsdlOption);
            CancellationToken cancellationToken = (CancellationToken)context.BindingContext.GetService(typeof(CancellationToken));

            using var loggerFactory = Logger.ConfigureLogger(logLevel);
            var logger = loggerFactory.CreateLogger<OpenApiService>();
            try
            {
                await OpenApiService.ShowOpenApiDocument(openapi, csdl, csdlFilter, output, logger, cancellationToken);

                return 0;
            }
            catch (Exception ex)
            {
#if DEBUG
                logger.LogCritical(ex, ex.Message);
                throw; // so debug tools go straight to the source of the exception when attached
#else
                logger.LogCritical( ex.Message);
                return 1;
#endif
            }
        }
    }
}
