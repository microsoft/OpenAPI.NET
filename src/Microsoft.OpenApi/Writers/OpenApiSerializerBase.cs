// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using Microsoft.OpenApi.Models;
using System;

namespace Microsoft.OpenApi.Writers
{
    /// <summary>
    /// Base class for Open API document internal serializer.
    /// </summary>
    internal abstract class OpenApiSerializerBase
    {
        public OpenApiSerializerSettings Settings { get; }

        public OpenApiSerializerBase(OpenApiSerializerSettings settings)
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

        /// <summary>
        /// Write the Open Api document.
        /// </summary>
        /// <param name="writer">The Open Api writer.</param>
        /// <param name="action">The Open Api writing action.</param>
        protected static void Write(IOpenApiWriter writer, Action<IOpenApiWriter> action)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            if (action == null)
            {
                throw Error.ArgumentNull(nameof(action));
            }

            writer.WriteStartObject();
            action(writer);
            writer.WriteEndObject();
        }
    }
}
