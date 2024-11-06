// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;

namespace Microsoft.OpenApi.Reader
{
    /// <summary>
    /// Generates custom extension methods for the version string type
    /// </summary>
    public static class OpenApiVersionExtensionMethods
    {
        /// <summary>
        /// Extension method for Spec version 2.0
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        public static bool is2_0(this string version)
        {
            bool result = false;
            if (version.Equals("2.0", StringComparison.OrdinalIgnoreCase))
            {
                result = true;
            }

            return result;
        }

        /// <summary>
        /// Extension method for Spec version 3.0
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        public static bool is3_0(this string version)
        {
            bool result = false;
            if (version.StartsWith("3.0", StringComparison.OrdinalIgnoreCase))
            {
                result = true;
            }

            return result;
        }

        /// <summary>
        /// Extension method for Spec version 3.1
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        public static bool is3_1(this string version)
        {
            bool result = false;
            if (version.StartsWith("3.1", StringComparison.OrdinalIgnoreCase))
            {
                result = true;
            }

            return result;
        }
    }
}
