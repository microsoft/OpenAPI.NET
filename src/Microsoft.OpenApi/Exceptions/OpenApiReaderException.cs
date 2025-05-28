// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using Microsoft.OpenApi.Reader;

namespace Microsoft.OpenApi
{
    /// <summary>
    /// Defines an exception indicating OpenAPI Reader encountered an issue while reading.
    /// </summary>
    [Serializable]
    public class OpenApiReaderException : OpenApiException
    {
        /// <summary>
        /// Initializes the <see cref="OpenApiReaderException"/> class.
        /// </summary>
        public OpenApiReaderException() { }

        /// <summary>
        /// Initializes the <see cref="OpenApiReaderException"/> class with a custom message.
        /// </summary>
        /// <param name="message">Plain text error message for this exception.</param>
        public OpenApiReaderException(string message) : base(message) { }

        /// <summary>
        /// Initializes the <see cref="OpenApiReaderException"/> class with a custom message.
        /// </summary>
        /// <param name="message">Plain text error message for this exception.</param>
        /// <param name="context">Context of current parsing process.</param>
        public OpenApiReaderException(string message, ParsingContext context) : base(message)
        {
            Pointer = context.GetLocation();
        }

        /// <summary>
        /// Initializes the <see cref="OpenApiReaderException"/> class with a custom message and inner exception.
        /// </summary>
        /// <param name="message">Plain text error message for this exception.</param>
        /// <param name="innerException">Inner exception that caused this exception to be thrown.</param>
        public OpenApiReaderException(string message, Exception innerException) : base(message, innerException) { }
    }
}
