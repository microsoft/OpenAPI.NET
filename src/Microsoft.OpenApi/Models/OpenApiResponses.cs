// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

namespace Microsoft.OpenApi
{
    /// <summary>
    /// Responses object.
    /// </summary>
    public class OpenApiResponses : OpenApiExtensibleDictionary<IOpenApiResponse>, IDeepCopyable<OpenApiResponses>
    {
        /// <summary>
        /// Parameterless constructor
        /// </summary>
        public OpenApiResponses() { }

        /// <summary>
        /// Initializes a copy of <see cref="OpenApiResponses"/> object
        /// </summary>
        /// <param name="openApiResponses">The <see cref="OpenApiResponses"/></param>
        public OpenApiResponses(OpenApiResponses openApiResponses) : base(dictionary: openApiResponses) { }

        /// <inheritdoc/>
        public OpenApiResponses CreateDeepCopy()
        {
            return new OpenApiDeepCopyContext().Copy(this);
        }
    }
}
