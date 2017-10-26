// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.IO;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Serialization;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi
{
    /// <summary>
    /// Represents the Open Api serializer.
    /// </summary>
    public class OpenApiSerializer
    {
        /// <summary>
        /// Open Api specification version
        /// </summary>
        public OpenApiSpecVersion SpecVersion { get; set; } = OpenApiSpecVersion.OpenApi3_0;

        /// <summary>
        /// Open Api document format.
        /// </summary>
        public OpenApiFormat Format { get; set; } = OpenApiFormat.Json;

        /// <summary>
        /// Serialize the <see cref="OpenApiDocument"/> to the given stream.
        /// </summary>
        /// <param name="stream">The given stream.</param>
        /// <param name="document">The given <see cref="OpenApiDocument"/></param>
        public virtual void Serialize(Stream stream, OpenApiDocument document)
        {
            if (stream == null)
            {
                throw Error.ArgumentNull(nameof(stream));
            }

            IOpenApiWriter writer;
            switch (Format)
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

            Serialize(writer, document);
        }

        /// <summary>
        /// Serialize the <see cref="OpenApiDocument"/> using the given writer <see cref="IOpenApiWriter"/>.
        /// </summary>
        /// <param name="writer">The given writer.</param>
        /// <param name="document">The given <see cref="OpenApiDocument"/></param>
        public virtual void Serialize(IOpenApiWriter writer, OpenApiDocument document)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            switch(SpecVersion)
            {
                case OpenApiSpecVersion.OpenApi3_0:
                    document.SerializeV3(writer);
                    break;

                case OpenApiSpecVersion.OpenApi2_0:
                    document.SerializeV2(writer);
                    break;

                default:
                    throw new OpenApiException("Unknown Open API specification version.");
            }

            writer.Flush();
        }
    }
}
