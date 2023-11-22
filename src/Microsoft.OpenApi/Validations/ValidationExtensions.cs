// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

namespace Microsoft.OpenApi.Validations
{
    /// <summary>
    /// Helper methods to simplify creating validation rules
    /// </summary>
    public static class ValidationContextExtensions
    {
        /// <summary>
        /// Helper method to simplify validation rules
        /// </summary>
        public static void CreateError(this IValidationContext context, string ruleName, string message)
        {
            var error = new OpenApiValidatorError(ruleName, context.PathString, message);
            context.AddError(error);
        }

        /// <summary>
        /// Helper method to simplify validation rules
        /// </summary>
        public static void CreateWarning(this IValidationContext context, string ruleName, string message)
        {
            var warning = new OpenApiValidatorWarning(ruleName, context.PathString, message);
            context.AddWarning(warning);
        }
    }
}
