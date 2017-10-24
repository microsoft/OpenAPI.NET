//---------------------------------------------------------------------
// <copyright file="OpenApiV2Serializer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.IO;

namespace Microsoft.OpenApi.Writers
{
    /// <summary>
    /// Class to serialize Open API v2.0 document.
    /// </summary>
    internal class OpenApiV2Serializer : OpenApiDocumentSerializer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OpenApiV2Serializer"/> class.
        /// </summary>
        public OpenApiV2Serializer()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenApiV2Serializer"/> class.
        /// </summary>
        /// <param name="writerFactory">The writer factory.</param>
        public OpenApiV2Serializer(Func<Stream, IOpenApiWriter> writerFactory)
            : base(writerFactory)
        {
        }

        /// <summary>
        /// Write the Open Api document to v2.0.
        /// </summary>
        /// <param name="document">The Open API document.</param>
        /// <param name="writer">The Open Api writer.</param>
        /// <returns>True for successful, false for errors.</returns>
        protected override bool WriteOpenApiDocument(OpenApiDocument document, IOpenApiWriter writer)
        {
            if (document == null)
            {
                throw Error.ArgumentNull(nameof(document));
            }

            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            // add the logic to write v2.0 document.

            return true;
        }
    }
}
