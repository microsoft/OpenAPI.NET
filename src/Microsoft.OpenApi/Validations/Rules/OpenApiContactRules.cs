// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

namespace Microsoft.OpenApi
{
    /// <summary>
    /// The validation rules for <see cref="OpenApiContact"/>.
    /// </summary>
    [OpenApiRule]
    public static class OpenApiContactRules
    {
        /// <summary>
        /// Email field MUST be email address.
        /// </summary>
        public static ValidationRule<OpenApiContact> EmailMustBeEmailFormat =>
            new(nameof(EmailMustBeEmailFormat),
                (context, item) =>
                {
                    if (item is {Email: not null} && !item.Email.IsEmailAddress())
                    {
                        context.Enter("email");
                        context.CreateError(nameof(EmailMustBeEmailFormat),
                            string.Format(SRResource.Validation_StringMustBeEmailAddress, item.Email));
                        context.Exit();
                    }
                });
    }
}
