// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Validations
{
    /// <summary>
    /// Error reason.
    /// </summary>
    public enum ErrorReason
    {
        /// <summary>
        /// Field is required.
        /// </summary>
        Required,

        /// <summary>
        /// Format error.
        /// </summary>
        Format,

        /// <summary>
        /// Duplicate Key error.
        /// </summary>
        DuplicateKey

    }

    /// <summary>
    /// Errors detected when validating a OpenAPI Element
    /// </summary>
    public class OpenApiValidatorError : OpenApiError
    {
        /// <summary>
        /// Initializes the <see cref="OpenApiError"/> class.
        /// </summary>
        public OpenApiValidatorError(ErrorReason reason, string pointer, string message) : base(pointer, message)
        {
            Pointer = pointer;
            Message = message;
            ReasonClass = reason;
        }

        /// <summary>
        /// Initializes the <see cref="OpenApiError"/> class.
        /// </summary>
        public OpenApiValidatorError(string ruleName, string pointer, string message) : base(pointer, message)
        {
            Pointer = pointer;
            Message = message;
            RuleName = ruleName;
        }

        /// <summary>
        /// Classified Reason for the error.
        /// </summary>
        public ErrorReason ReasonClass { get; set; }

        /// <summary>
        /// Name of rule that detected the error.
        /// </summary>
        public string RuleName { get; set; }

    }
}
