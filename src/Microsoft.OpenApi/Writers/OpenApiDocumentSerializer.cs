//---------------------------------------------------------------------
// <copyright file="OpenApiDocumentSerializer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.IO;

namespace Microsoft.OpenApi.Writers
{
    /// <summary>
    /// Base class for Open API document serializer.
    /// </summary>
    internal abstract class OpenApiDocumentSerializer
    {
        private Func<Stream, IOpenApiWriter> _writerFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenApiDocumentSerializer"/> class.
        /// </summary>
        /// <param name="writerFactory">The writer factory.</param>
        public OpenApiDocumentSerializer(Func<Stream, IOpenApiWriter> writerFactory)
        {
            _writerFactory = writerFactory;
        }

        /// <summary>
        /// Serialize an <see cref="OpenApiDocument"/> into <see cref="Stream"/>.
        /// </summary>
        /// <param name="stream">The output stream.</param>
        /// <param name="document">The Open API document.</param>
        /// <returns>True for successful, false for errors.</returns>
        public virtual bool Save(Stream stream, OpenApiDocument document)
        {
            if (stream == null)
            {
                throw Error.ArgumentNull(nameof(stream));
            }

            if (document == null)
            {
                throw Error.ArgumentNull(nameof(document));
            }

            IOpenApiWriter writer = _writerFactory != null ? _writerFactory(stream) : CreateDefaultWriter(stream);
            if (writer == null)
            {
                throw new OpenApiException("TODO: Create create the writer.");
            }

            return WriteOpenApiDocument(document, writer);
        }

        /// <summary>
        /// Abstract method to write the Open Api document.
        /// </summary>
        /// <param name="document">The Open API document.</param>
        /// <param name="writer">The Open Api writer.</param>
        /// <returns>True for successful, false for errors.</returns>
        protected abstract bool WriteOpenApiDocument(OpenApiDocument document, IOpenApiWriter writer);

        /// <summary>
        /// Create default <see cref="IOpenApiWriter"/>
        /// </summary>
        /// <param name="stream">The output stream.</param>
        /// <returns>The <see cref="IOpenApiWriter"/> object created, or null.</returns>
        private static IOpenApiWriter CreateDefaultWriter(Stream stream)
        {
            return new OpenApiYamlWriter(new StreamWriter(stream));
        }
    }
}
