// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Linq;

namespace Microsoft.OpenApi.Hidi
{
    public static class OpenApiSpecVersionExtension
    {
        public static OpenApiSpecVersion TryParseOpenApiSpecVersion(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new InvalidOperationException("Please provide a version");
            }
            var res = value.Split('.', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
            
            if (int.TryParse(res, out int result))
            {

                if (result >= 2 || result <= 3)
                {
                    return (OpenApiSpecVersion)result;
                }
            }  

            return OpenApiSpecVersion.OpenApi3_0; // default
        }
    }
}
