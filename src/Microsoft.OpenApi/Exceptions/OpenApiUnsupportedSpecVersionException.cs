// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Globalization;

namespace Microsoft.OpenApi.Exceptions
{
    /// <summary>
    /// Defines an exception indicating OpenAPI Reader encountered an unsupported specification version while reading.
    /// </summary>
    [Serializable]
    public class OpenApiUnsupportedSpecVersionException : Exception
    {
        const string messagePattern = "OpenAPI specification version '{0}' is not supported.";

        /// <summary>
        /// Initializes the <see cref="OpenApiUnsupportedSpecVersionException"/> class with a specification version.
        /// </summary>
        /// <param name="specificationVersion">Version that caused this exception to be thrown.</param>
        public OpenApiUnsupportedSpecVersionException(string specificationVersion)
            : base(string.Format(CultureInfo.InvariantCulture, messagePattern, specificationVersion))
        {
            this.SpecificationVersion = specificationVersion;
        }

        /// <summary>
        /// Initializes the <see cref="OpenApiUnsupportedSpecVersionException"/> class with a specification version and
        /// inner exception.
        /// </summary>
        /// <param name="specificationVersion">Version that caused this exception to be thrown.</param>
        /// <param name="innerException">Inner exception that caused this exception to be thrown.</param>
        public OpenApiUnsupportedSpecVersionException(string specificationVersion, Exception innerException)
            : base(string.Format(CultureInfo.InvariantCulture, messagePattern, specificationVersion), innerException)
        {
            this.SpecificationVersion = specificationVersion;
        }

        /// <summary>
        /// The unsupported specification version.
        /// </summary>
        public string SpecificationVersion { get; }
    }
}
