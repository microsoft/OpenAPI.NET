// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Linq;

namespace Microsoft.OpenApi.Hidi
{
    public static class OpenApiSpecVersionHelper
    {
        public static OpenApiSpecVersion TryParseOpenApiSpecVersion(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new InvalidOperationException("Please provide a version");
            }
            var res = value.Split('.', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();

            if (int.TryParse(res, out var result))
            {
                if (result is >= 2 and < 3)
                {
                    return OpenApiSpecVersion.OpenApi2_0;
                }
            }

            return OpenApiSpecVersion.OpenApi3_0; // default
        }
    }
}
