// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Linq;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Properties;

namespace Microsoft.OpenApi.Validations.Rules
{
    /// <summary>
    /// The validation rules for <see cref="OpenApiResponses"/>.
    /// </summary>
    [OpenApiRule]
    public static class OpenApiResponsesRules
    {
        /// <summary>
        /// An OpenAPI operation must contain at least one successful response
        /// </summary>
        public static ValidationRule<OpenApiResponses> ResponsesMustContainSuccessResponse =>
            new ValidationRule<OpenApiResponses>(
                (context, item) =>
                {
                    if (!item.Keys.Any(k => k.StartsWith("2"))) {
                        context.AddError(new ValidationError(ErrorReason.Required,context.PathString,"Responses must contain success response"));
                    }
                });

    }
}