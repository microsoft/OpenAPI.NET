// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using Microsoft.OpenApi.Writers;
using System.IO;

namespace Microsoft.OpenApi
{
    /// <summary>
    /// Represents the Open Api specification version.
    /// </summary>
    public class OpenApiSerializer
    {
        public OpenApiSpecVersion TargetVersion { get; }

        public OpenApiWriterSettings Settings { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenApiSerializer"/> class.
        /// </summary>
        public OpenApiSerializer()
            : this(OpenApiSpecVersion.OpenApi3_0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenApiSerializer"/> class.
        /// </summary>
        /// <param name="version">The specification version.</param>
        public OpenApiSerializer(OpenApiSpecVersion version)
            : this(version, new OpenApiWriterSettings())
        {
        }

        /// <summary>
        ///  Initializes a new instance of the <see cref="OpenApiSerializer"/> class.
        /// </summary>
        /// <param name="version">The specification version.</param>
        /// <param name="settings">The write setting.</param>
        public OpenApiSerializer(OpenApiSpecVersion version, OpenApiWriterSettings settings)
        {
            TargetVersion = version;
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
            if (Settings.WriterFactory!= null)
            {
                writer = Settings.WriterFactory(stream);
            }
            else
            {
                writer = CreateDefaultWriter(stream);
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

            OpenApiInternalSerializer serializer;
            switch(TargetVersion)
            {
                case OpenApiSpecVersion.OpenApi3_0:
                    serializer = new OpenApiV3Serializer(Settings);
                    break;

                case OpenApiSpecVersion.Swagger2_0:
                    serializer = new OpenApiV2Serializer(Settings);
                    break;

                default:
                    throw new OpenApiException("Unknow Spec version.");
            }

            serializer.Write(writer, document);
        }

        /// <summary>
        /// Create default <see cref="IOpenApiWriter"/>
        /// </summary>
        /// <param name="stream">The output stream.</param>
        /// <returns>The <see cref="IOpenApiWriter"/> object created, or null.</returns>
        private static IOpenApiWriter CreateDefaultWriter(Stream stream)
        {
            return new OpenApiJsonWriter(new StreamWriter(stream));
        }
    }
}
