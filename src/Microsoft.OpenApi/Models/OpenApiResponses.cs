// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;

namespace Microsoft.OpenApi
{
    /// <summary>
    /// Responses object.
    /// </summary>
    public class OpenApiResponses : OpenApiExtensibleDictionary<IOpenApiResponse>
    {
        /// <summary>
        /// Parameterless constructor
        /// </summary>
        public OpenApiResponses() { }

        /// <summary>
        /// Initializes a copy of <see cref="OpenApiResponses"/> object.
        /// </summary>
        /// <param name="openApiResponses">The <see cref="OpenApiResponses"/>.</param>
        /// <remarks>
        /// This creates a shallow copy; the responses are the same reference as in the provided parameter,
        /// and specification extensions are copied into a new dictionary.
        /// </remarks>
        public OpenApiResponses(OpenApiResponses openApiResponses) : base(dictionary: openApiResponses, extensions: openApiResponses?.Extensions != null ? new Dictionary<string, IOpenApiExtension>(openApiResponses.Extensions) : null) { }
    }
}
