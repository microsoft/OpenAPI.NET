// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.IO;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Serialization.Tests
{
    public static class OpenApiSerializerTestHelper
    {
        public static string SerializeAsJson<T>(this T element, Action<T, IOpenApiWriter> action)
            where T : IOpenApiElement
        {
            return element.Serialize(OpenApiFormat.Json, action);
        }

        public static string SerializeAsYaml<T>(this T element, Action<T, IOpenApiWriter> action)
            where T : IOpenApiElement
        {
            return element.Serialize(OpenApiFormat.Yaml, action);
        }

        public static string Serialize<T>(this T element, OpenApiFormat format, Action<T, IOpenApiWriter> action)
            where T : IOpenApiElement
        {
            if (element == null)
            {
                throw Error.ArgumentNull(nameof(element));
            }

            if (action == null)
            {
                throw Error.ArgumentNull(nameof(action));
            }

            MemoryStream stream = new MemoryStream();
            IOpenApiWriter writer = null;
            switch (format)
            {
                case OpenApiFormat.Yaml:
                    writer = new OpenApiYamlWriter(new StreamWriter(stream));
                    break;
                case OpenApiFormat.Json:
                default: // default is json
                    writer = new OpenApiJsonWriter(new StreamWriter(stream));
                    break;
            }

            action(element, writer);
            writer.Flush();
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
