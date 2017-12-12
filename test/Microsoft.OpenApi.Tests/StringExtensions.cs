// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;

namespace Microsoft.OpenApi.Tests
{
    public static class StringExtensions
    {
        public static string MakeLineBreaksEnvironmentNeutral(this string input)
        {
            return input.Replace("\r\n", "\n")
                .Replace("\r", "\n")
                .Replace("\n", Environment.NewLine);
        }
    }
}