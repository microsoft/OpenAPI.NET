// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using Microsoft.OpenApi.Exceptions;

namespace Microsoft.OpenApi.Models
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
    /// Error related to the Open API Document.
    /// </summary>
    public class OpenApiError
    {

        /// <summary>
        /// Initializes the <see cref="OpenApiError"/> class using the message and pointer from the given exception.
        /// </summary>
        public OpenApiError(OpenApiException exception)
        {
            Message = exception.Message;
            Pointer = exception.Pointer;
        }

        /// <summary>
        /// Initializes the <see cref="OpenApiError"/> class.
        /// </summary>
        public OpenApiError(string pointer, string message)
        {
            Pointer = pointer;
            Message = message;
        }

        /// <summary>
        /// Initializes the <see cref="OpenApiError"/> class.
        /// </summary>
        public OpenApiError(ErrorReason reason, string pointer, string message)
        {
            Pointer = pointer;
            Message = message;
        }

        /// <summary>
        /// Classified Reason for the error.
        /// </summary>
        public ErrorReason ReasonClass { get; set; }

        /// <summary>
        /// Message explaining the error.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Pointer to the location of the error.
        /// </summary>
        public string Pointer { get; set; }

        /// <summary>
        /// Gets the string representation of <see cref="OpenApiError"/>.
        /// </summary>
        public override string ToString()
        {
            return Message + (!string.IsNullOrEmpty(Pointer) ? " at " + Pointer : "");
        }
    }
}