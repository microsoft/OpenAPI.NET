﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

namespace Microsoft.OpenApi
{
    /// <summary>
    /// 
    /// </summary>
    public class OpenApiReferenceError : OpenApiError
    {
        /// <summary>
        /// The reference that caused the error.
        /// </summary>
        public readonly BaseOpenApiReference? Reference;
        /// <summary>
        /// Initializes the <see cref="OpenApiError"/> class using the message and pointer from the given exception.
        /// </summary>
        public OpenApiReferenceError(OpenApiException exception) : base(exception.Pointer, exception.Message)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reference"></param>
        /// <param name="message"></param>
        public OpenApiReferenceError(BaseOpenApiReference reference, string message) : base("", message)
        {
            Reference = reference;
        }
    }
}
