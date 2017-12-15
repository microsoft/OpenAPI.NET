// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Properties;

namespace Microsoft.OpenApi.Validations.Rules
{
    /// <summary>
    /// The validation rules for <see cref="OpenApiResponse"/>.
    /// </summary>
    internal static class OpenApiResponseRules
    {
        /// <summary>
        /// Validate the field is required.
        /// </summary>
        public static readonly ValidationRule<OpenApiResponse> FieldIsRequired =
            new ValidationRule<OpenApiResponse>(
                (context, response) =>
                {
                    // title
                    context.Push("description");
                    if (String.IsNullOrEmpty(response.Description))
                    {
                        ValidationError error = new ValidationError(ErrorReason.Required, context.PathString,
                            String.Format(SRResource.Validation_FieldIsRequired, "description", "response"));
                        context.AddError(error);
                    }
                    context.Pop();
                });

        // add more rule.
    }
}
