// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Services
{
    public class OpenApiValidator : OpenApiVisitorBase
    {
        private List<OpenApiException> openApiException;

        public List<OpenApiException> Exceptions { get { return this.openApiException; } }
        public OpenApiValidator()
        {
            this.openApiException = new List<OpenApiException>();
        }
        public override void Visit(OpenApiResponse response)
        {
            if (string.IsNullOrEmpty(response.Description))
            {
                this.openApiException.Add(new OpenApiException("Response must have a description"));
            }
        }
    }
}
