// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.IO;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Tests.Serialization
{
    /// <summary>
    /// Serialiation Helpers
    /// </summary>
    public static class OpenApiSerializerTestHelper
    {
        /// <summary>
        /// Serializes as JSON.
        /// </summary>
        public static string SerializeAsJson<T>(this T element,
               OpenApiSpecVersion version = OpenApiSpecVersion.OpenApi3_0)
               where T : OpenApiElement
        {
            return element.Serialize(version, OpenApiFormat.Json);
        }

        /// <summary>
        /// Serializes as YAML.
        /// </summary>
        public static string SerializeAsYaml<T>(this T element,
               OpenApiSpecVersion version = OpenApiSpecVersion.OpenApi3_0)
               where T : OpenApiElement
        {
            return element.Serialize(version, OpenApiFormat.Yaml);
        }

        /// <summary>
        /// Serializes as the specified format.
        /// </summary>
        public static string Serialize<T>(this T element,
            OpenApiSpecVersion version = OpenApiSpecVersion.OpenApi3_0,
            OpenApiFormat format = OpenApiFormat.Json)
            where T : OpenApiElement
        {
            if (element == null)
            {
                throw Error.ArgumentNull(nameof(element));
            }

            MemoryStream stream = new MemoryStream();
            element.Serialize(stream, version, format);
            stream.Position = 0;
            return new StreamReader(stream).ReadToEnd();
        }
    }
}
