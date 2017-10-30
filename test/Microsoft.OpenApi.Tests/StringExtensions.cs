// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System;

namespace Microsoft.OpenApi.Tests
{
    public static class StringExtensions
    {
        public static string MakeLineBreaksEnvironmentNeutral(this string input)
        {
            if (input.IndexOf("\n") == -1)
            {
                return input;
            }
            else
            {
                input = "\r\n" + input;
            }

            return input.Replace("\r\n", "\n")
                .Replace("\r", "\n")
                .Replace("\n", Environment.NewLine);
        }
    }
}