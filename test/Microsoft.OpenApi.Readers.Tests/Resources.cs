// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

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
            using (Stream stream = GetStream(fileName))
            using (TextReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        /// <summary>
        /// Get the file stream.
        /// </summary>
        /// <param name="fileName">The file name with relative path. For example: "/V3Tests/Samples/OpenApiCallback/..".</param>
        /// <returns>The file stream.</returns>
        public static Stream GetStream(string fileName)
        {
            string path = GetPath(fileName);
            Stream stream = typeof(Resources).Assembly.GetManifestResourceStream(path);

            if (stream == null)
            {
                string message = Error.Format("The embedded resource '{0}' was not found.", path);
                throw new FileNotFoundException(message, path);
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
