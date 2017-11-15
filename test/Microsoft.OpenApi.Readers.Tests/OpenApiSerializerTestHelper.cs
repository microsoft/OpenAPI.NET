// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.IO;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Readers.Tests
{
    public static class OpenApiSerializerTestHelper
    {
        public static string SerializeAsJson<T>(this T element,
               OpenApiSpecVersion version = OpenApiSpecVersion.OpenApi3_0)
               where T : OpenApiElement
        {
            return element.Serialize(version, OpenApiFormat.Json);
        }

        public static string SerializeAsYaml<T>(this T element,
               OpenApiSpecVersion version = OpenApiSpecVersion.OpenApi3_0)
               where T : OpenApiElement
        {
            return element.Serialize(version, OpenApiFormat.Yaml);
        }

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
            string value = new StreamReader(stream).ReadToEnd();

            if (value.IndexOf("\n") == -1)
            {
                return value;
            }
            else
            {
                return "\r\n" + value.Replace("\n", "\r\n");
            }
        }
    }
}
