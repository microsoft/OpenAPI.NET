// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Validations
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
    }
}
