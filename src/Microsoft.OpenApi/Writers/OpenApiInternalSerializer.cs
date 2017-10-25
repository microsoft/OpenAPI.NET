// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.OpenApi.Writers
{
    /// <summary>
    /// Base class for Open API document internal serializer.
    /// </summary>
    internal abstract class OpenApiInternalSerializer
    {
        public OpenApiWriterSettings Settings { get; }

        public OpenApiInternalSerializer(OpenApiWriterSettings settings)
        {
            Settings = settings;
        }

        /// <summary>
        /// Abstract method to write the Open Api document.
        /// </summary>
        /// <param name="document">The Open API document.</param>
        /// <param name="writer">The Open Api writer.</param>
        /// <returns>True for successful, false for errors.</returns>
        public abstract void Write(IOpenApiWriter writer, OpenApiDocument document);
    }
}
