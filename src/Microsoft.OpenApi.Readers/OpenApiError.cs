// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.OpenApi.Readers
{
    public class OpenApiError
    {
        private readonly string message;
        private readonly string pointer;

        public OpenApiError(OpenApiException exception)
        {
            message = exception.Message;
            pointer = exception.Pointer;
        }

        public OpenApiError(string pointer, string message)
        {
            this.pointer = pointer;
            this.message = message;
        }

        public override string ToString()
        {
            return message + (!string.IsNullOrEmpty(pointer) ? " at " + pointer : "");
        }
    }
}