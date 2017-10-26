// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.IO;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi
{
    /// <summary>
    /// Represents the Open Api serializer.
    /// </summary>
    public class OpenApiSerializer
    {
        /// <summary>
        /// The Open Api serializer settings.
        /// </summary>
        public OpenApiSerializerSettings Settings { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenApiSerializer"/> class.
        /// </summary>
        public OpenApiSerializer()
            : this(new OpenApiSerializerSettings())
        {
        }

        /// <summary>
        ///  Initializes a new instance of the <see cref="OpenApiSerializer"/> class.
        /// </summary>
        /// <param name="settings">The write setting.</param>
        public OpenApiSerializer(OpenApiSerializerSettings settings)
        {
            Settings = settings ?? throw Error.ArgumentNull(nameof(settings));
        }

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

            if (document == null)
            {
                throw Error.ArgumentNull(nameof(document));
            }

            IOpenApiWriter writer;
            switch (Settings.Format)
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

            if (document == null)
            {
                throw Error.ArgumentNull(nameof(document));
            }

            OpenApiSerializerBase serializer;
            switch(Settings.SpecVersion)
            {
                case OpenApiSpecVersion.OpenApi3_0:
                    serializer = new OpenApiV3Serializer(Settings);
                    break;

                case OpenApiSpecVersion.OpenApi2_0:
                    serializer = new OpenApiV2Serializer(Settings);
                    break;

                default:
                    throw new OpenApiException("Unknown Open API specification version.");
            }

            serializer.Write(writer, document);
        }
    }
}
