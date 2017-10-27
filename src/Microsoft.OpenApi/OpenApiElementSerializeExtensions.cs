// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.IO;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi
{
    /// <summary>
    /// Represents the Open Api serializer.
    /// </summary>
    public static class OpenApiElementSerializeExtensions
    {
        /// <summary>
        /// Serialize the <see cref="IOpenApiElement"/> to the given stream as JSON.
        /// </summary>
        /// <typeparam name="T">the <see cref="IOpenApiElement"/></typeparam>
        /// <param name="element">The Open Api element.</param>
        /// <param name="stream">The output stream.</param>
        /// <param name="specVersion">The Open Api specification version, default is 3.0.</param>
        public static void SerializeAsJson<T>(this T element, Stream stream,
            OpenApiSpecVersion specVersion = OpenApiSpecVersion.OpenApi3_0)
            where T : IOpenApiElement
        {
            element.Serialize(stream, specVersion, OpenApiFormat.Json);
        }

        /// <summary>
        /// Serialize the <see cref="IOpenApiElement"/> to the given stream as YAML.
        /// </summary>
        /// <typeparam name="T">the <see cref="IOpenApiElement"/></typeparam>
        /// <param name="element">The Open Api element.</param>
        /// <param name="stream">The output stream.</param>
        /// <param name="specVersion">The Open Api specification version, default is 3.0.</param>
        public static void SerializeAsYaml<T>(this T element, Stream stream,
            OpenApiSpecVersion specVersion = OpenApiSpecVersion.OpenApi3_0)
            where T : IOpenApiElement
        {
            element.Serialize(stream, specVersion, OpenApiFormat.Yaml);
        }

        /// <summary>
        /// Serialize the <see cref="IOpenApiElement"/> to the given stream.
        /// </summary>
        /// <typeparam name="T">the <see cref="IOpenApiElement"/></typeparam>
        /// <param name="element">The Open Api element.</param>
        /// <param name="stream">The given stream.</param>
        /// <param name="specVersion">The Open Api specification version.</param>
        /// <param name="format">The output format (JSON or YAML).</param>
        public static void Serialize<T>(this T element, Stream stream, OpenApiSpecVersion specVersion, OpenApiFormat format)
            where T : IOpenApiElement
        {
            if (element == null)
            {
                throw Error.ArgumentNull(nameof(element));
            }

            if (stream == null)
            {
                throw Error.ArgumentNull(nameof(stream));
            }

            IOpenApiWriter writer;
            switch (format)
            {
                case OpenApiFormat.Json:
                    writer = new OpenApiJsonWriter(new StreamWriter(stream));
                    break;
                case OpenApiFormat.Yaml:
                    writer = new OpenApiYamlWriter(new StreamWriter(stream));
                    break;
                default:
                    throw new OpenApiException("Not supported Open Api document format!");
            }

            switch (specVersion)
            {
                case OpenApiSpecVersion.OpenApi3_0:
                    element.WriteAsV3(writer);
                    break;

                case OpenApiSpecVersion.OpenApi2_0:
                    element.WriteAsV2(writer);
                    break;

                default:
                    throw new OpenApiException("Unknown Open API specification version.");
            }

            writer.Flush();
        }
    }
}
