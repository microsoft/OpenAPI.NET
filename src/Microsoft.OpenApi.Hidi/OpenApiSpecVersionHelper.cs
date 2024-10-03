// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;

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
            // Split the version string by the dot
            var versionSegments = value.Split('.', StringSplitOptions.RemoveEmptyEntries);

            if (!int.TryParse(versionSegments[0], out var majorVersion)
                || !int.TryParse(versionSegments[1], out var minorVersion))
            {
                throw new InvalidOperationException("Invalid version format. Please provide a valid OpenAPI version (e.g., 2.0, 3.0, 3.1).");
            }

            // Check for specific version matches
            if (majorVersion == 2)
            {
                return OpenApiSpecVersion.OpenApi2_0;
            }
            else if (majorVersion == 3 && minorVersion == 0)
            {
                return OpenApiSpecVersion.OpenApi3_0;
            }
            else if (majorVersion == 3 && minorVersion == 1)
            {
                return OpenApiSpecVersion.OpenApi3_1;
            }

            return OpenApiSpecVersion.OpenApi3_1; // default
        }
    }
}
