// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.OpenApi.Exceptions;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Services
{
    /// <summary>
    /// Class containing logic to validate an Open API document object.
    /// </summary>
    public class OpenApiValidator : OpenApiVisitorBase
    {
        /// <summary>
        /// Exceptions related to this validation.
        /// </summary>
        public List<OpenApiException> Exceptions { get; } = new List<OpenApiException>();

        /// <summary>
        /// Visit Open API Response element.
        /// </summary>
        /// <param name="response">Response element.</param>
        public override void Visit(OpenApiResponse response)
        {
            if (string.IsNullOrEmpty(response.Description))
            {
                Exceptions.Add(new OpenApiException("Response must have a description"));
            }
        }
    }
}