// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.CommandLine;

namespace Microsoft.OpenApi.Hidi.Extensions
{
    internal static class CommandExtensions
    {
        public static void AddOptions(this Command command, IReadOnlyList<Option> options)
        {
            foreach (var option in options)
            {
                command.AddOption(option);
            }
        }
    }
}
