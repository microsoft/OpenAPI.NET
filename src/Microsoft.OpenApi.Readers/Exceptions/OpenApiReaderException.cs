// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Runtime.Serialization;

namespace Microsoft.OpenApi.Readers.Exceptions
{
    /// <summary>
    /// Defines an exception indicating OpenAPI Reader encountered an issue while reading.
    /// </summary>
    [Serializable]
    public class OpenApiReaderException : Exception
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
        /// Initializes the <see cref="OpenApiReaderException"/> class with a custom message and inner exception.
        /// </summary>
        /// <param name="message">Plain text error message for this exception.</param>
        /// <param name="innerException">Inner exception that caused this exception to be thrown.</param>
        public OpenApiReaderException(string message, Exception innerException) : base(message, innerException) { }

        /// <summary>
        /// Initializes the <see cref="OpenApiReaderException" /> class based on serialization info and context.
        /// </summary>
        /// <param name="info">Info needed to serialize or deserialize this exception.</param>
        /// <param name="context">Context needed to serialize or deserialize this exception.</param>
        protected OpenApiReaderException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}