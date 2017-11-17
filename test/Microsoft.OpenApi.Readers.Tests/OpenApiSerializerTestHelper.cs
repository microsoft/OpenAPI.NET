// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.IO;
using Microsoft.OpenApi.Interfaces;

namespace Microsoft.OpenApi.Readers.Tests
{
    /// <summary>
    /// Serialization helpers.
    /// </summary>
    internal static class OpenApiSerializerTestHelper
    {
        /// <summary>
        /// Serializes as JSON.
        /// </summary>
        public static string SerializeAsJson<T>(
            this T element,
            OpenApiSpecVersion version)
            where T : IOpenApiSerializable
        {
            return element.Serialize(version, OpenApiFormat.Json);
        }

        /// <summary>
        /// Serializes as YAML.
        /// </summary>
        public static string SerializeAsYaml<T>(
            this T element,
            OpenApiSpecVersion version)
            where T : IOpenApiSerializable
        {
            return element.Serialize(version, OpenApiFormat.Yaml);
        }

        /// <summary>
        /// Serializes as the specified format.
        /// </summary>
        public static string Serialize<T>(
            this T element,
            OpenApiSpecVersion version,
            OpenApiFormat format)
            where T : IOpenApiSerializable
        {
            if (element == null)
            {
                throw Error.ArgumentNull(nameof(element));
            }

            using (var stream = new MemoryStream())
            {
                element.Serialize(stream, version, format);
                stream.Position = 0;

                using (var streamReader = new StreamReader(stream))
                {
                    return streamReader.ReadToEnd();
                }
            }
        }
    }
}