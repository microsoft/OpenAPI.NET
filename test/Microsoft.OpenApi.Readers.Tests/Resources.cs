// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.IO;

namespace Microsoft.OpenApi.Readers.Tests
{
    internal static class Resources
    {
        /// <summary>
        /// Get the file contents.
        /// </summary>
        /// <param name="fileName">The file name with relative path. For example: "/V3Tests/Samples/OpenApiCallback/..".</param>
        /// <returns>The file contents.</returns>
        public static string GetString(string fileName)
        {
            using var stream = GetStream(fileName);
            using TextReader reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }

        /// <summary>
        /// Get the file stream.
        /// </summary>
        /// <param name="fileName">The file name with relative path. For example: "/V3Tests/Samples/OpenApiCallback/..".</param>
        /// <returns>The file stream.</returns>
        public static Stream GetStream(string fileName)
        {
            var path = GetPath(fileName);
            var stream = typeof(Resources).Assembly.GetManifestResourceStream(path);

            if (stream == null)
            {
                throw new FileNotFoundException($"The embedded resource '{path}' was not found.", path);
            }

            return stream;
        }

        private static string GetPath(string fileName)
        {
            const string pathSeparator = ".";
            return typeof(Resources).Namespace + pathSeparator + fileName.Replace('/', '.');
        }
    }
}
