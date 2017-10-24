//---------------------------------------------------------------------
// <copyright file="OpenApiValidator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;

namespace Microsoft.OpenApi.Sevices
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
