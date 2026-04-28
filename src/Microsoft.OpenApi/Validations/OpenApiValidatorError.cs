// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

#pragma warning disable OAI020 // Internal implementation uses experimental APIs
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.OpenApi
{
    /// <summary>
    /// Errors detected when validating an OpenAPI Element
    /// </summary>
    public class OpenApiValidatorError : OpenApiError
    {
        /// <summary>
        /// Initializes the <see cref="OpenApiError"/> class.
        /// </summary>
        public OpenApiValidatorError(string ruleName, string pointer, string message) : base(pointer, message)
        {
            RuleName = ruleName;
        }

        /// <summary>
        /// Name of rule that detected the error.
        /// </summary>
        public string RuleName { get; set; }

        /// <summary>
        /// Gets the error pointer translated to the equivalent path for the specified OpenAPI version.
        /// </summary>
        /// <param name="targetVersion">The target OpenAPI specification version.</param>
        /// <returns>
        /// The equivalent pointer in the target version, the original pointer if no transformation is needed,
        /// or <c>null</c> if the pointer has no equivalent in the target version.
        /// </returns>
        [Experimental("OAI020", UrlFormat = "https://aka.ms/openapi/net/experimental/{0}")]
        public string? GetVersionedPointer(OpenApiSpecVersion targetVersion)
        {
            if (Pointer is null)
            {
                return null;
            }

            return OpenApiPathHelper.GetVersionedPath(Pointer, targetVersion);
        }
    }
}
