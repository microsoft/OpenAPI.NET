// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.OpenApi.Writers
{
    /// <summary>
    /// Class to serialize Open API v2.0 document.
    /// </summary>
    internal class OpenApiV2Serializer : OpenApiInternalSerializer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OpenApiV2Serializer"/> class.
        /// </summary>
        public OpenApiV2Serializer(OpenApiWriterSettings settings)
            : base(settings)
        {
        }

        /// <summary>
        /// Write the Open Api document to v2.0.
        /// </summary>
        /// <param name="document">The Open API document.</param>
        /// <param name="writer">The Open Api writer.</param>
        /// <returns>True for successful, false for errors.</returns>
        public override void Write(IOpenApiWriter writer, OpenApiDocument document)
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
        }
    }
}
