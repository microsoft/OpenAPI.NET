// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Properties;

namespace Microsoft.OpenApi.Validations.Rules
{
    /// <summary>
    /// The validation rules for <see cref="OpenApiContact"/>.
    /// </summary>
    internal static class OpenApiContactRules
    {
        /// <summary>
        /// Email field MUST be email address.
        /// </summary>
        public static readonly ValidationRule<OpenApiContact> EmailMustBeEmailFormat =
            new ValidationRule<OpenApiContact>(
                (context, item) =>
                {
                    context.Push("email");
                    if (item != null && item.Email != null)
                    {
                        if (!item.Email.IsEmailAddress())
                        {
                            ValidationError error = new ValidationError(ErrorReason.Format, context.PathString,
                                String.Format(SRResource.Validation_StringMustBeEmailAddress, item.Email));
                            context.AddError(error);
                        }
                    }
                    context.Pop();
                });

        /// <summary>
        /// Url field MUST be url format.
        /// </summary>
        public static readonly ValidationRule<OpenApiContact> UrlMustBeUrlFormat =
            new ValidationRule<OpenApiContact>(
                (context, item) =>
                {
                    context.Push("url");
                    if (item != null && item.Url != null)
                    {
                        // TODO:
                    }
                    context.Pop();
                });
    }
}
