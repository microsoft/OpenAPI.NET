// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Extensions.Logging;

namespace Microsoft.OpenApi.Hidi
{
    public static class Logger
    {
        public static ILoggerFactory ConfigureLogger(LogLevel logLevel)
        {
            // Configure logger options
#if DEBUG
            logLevel = logLevel > LogLevel.Debug ? LogLevel.Debug : logLevel;
#endif

            return LoggerFactory.Create((builder) =>
            {
                builder
                    .AddSimpleConsole(c => c.IncludeScopes = true)
#if DEBUG
                    .AddDebug()
#endif
                    .SetMinimumLevel(logLevel);
            });
        }
    }
}
