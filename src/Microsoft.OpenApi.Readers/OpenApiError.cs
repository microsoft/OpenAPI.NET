// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using Microsoft.OpenApi.Exceptions;

namespace Microsoft.OpenApi.Readers
{
    /// <summary>
    /// Error related to reading Open API YAML/JSON.
    /// </summary>
    public class OpenApiError
    {
        private readonly string _message;
        private readonly string _pointer;

        /// <summary>
        /// Initializes the <see cref="OpenApiError"/> class using the message and pointer from the given exception.
        /// </summary>
        public OpenApiError(OpenApiException exception)
        {
            _message = exception.Message;
            _pointer = exception.Pointer;
        }

        /// <summary>
        /// Initializes the <see cref="OpenApiError"/> class.
        /// </summary>
        public OpenApiError(string pointer, string message)
        {
            this._pointer = pointer;
            this._message = message;
        }

        /// <summary>
        /// Gets the string representation of <see cref="OpenApiError"/>.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return _message + (!string.IsNullOrEmpty(_pointer) ? " at " + _pointer : "");
        }
    }
}