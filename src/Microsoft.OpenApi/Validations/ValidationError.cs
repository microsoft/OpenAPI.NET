// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

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
    }

    /// <summary>
    /// The validation error class.
    /// </summary>
    public sealed class ValidationError
    {
        /// <summary>
        /// Initializes the <see cref="ValidationError"/> class.
        /// </summary>
        /// <param name="reason">The error reason.</param>
        /// <param name="path">The visit path.</param>
        /// <param name="message">The error message.</param>
        public ValidationError(ErrorReason reason, string path, string message)
        {
            ErrorCode = reason;
            ErrorPath = path;
            ErrorMessage = message;
        }

        /// <summary>
        /// Gets the path of the error in the Open API in which it occurred.
        /// </summary>
        public string ErrorPath { get; private set; }

        /// <summary>
        /// Gets an integer code representing the error.
        /// </summary>
        public ErrorReason ErrorCode { get; private set; }

        /// <summary>
        /// Gets a human readable string describing the error.
        /// </summary>
        public string ErrorMessage { get; private set; }

        /// <summary>
        /// Returns the whole error message.
        /// </summary>
        /// <returns>The error string.</returns>
        public override string ToString()
        {
            return "ErroCode: " + ErrorCode + ", " + ErrorPath + " | " + ErrorMessage;
        }
    }
}
