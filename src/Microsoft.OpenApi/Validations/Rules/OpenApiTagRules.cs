// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Properties;

namespace Microsoft.OpenApi.Validations.Rules
{
    /// <summary>
    /// The validation rules for <see cref="OpenApiTag"/>.
    /// </summary>
    [OpenApiRule]
    public static class OpenApiTagRules
    {
        /// <summary>
        /// Validate the field is required.
        /// </summary>
        public static ValidationRule<OpenApiTag> FieldIsRequired =>
            new ValidationRule<OpenApiTag>(
                (context, tag) =>
                {
                    context.Push("name");
                    if (String.IsNullOrEmpty(tag.Name))
                    {
                        ValidationError error = new ValidationError(ErrorReason.Required, context.PathString,
                            String.Format(SRResource.Validation_FieldIsRequired, "name", "Tag"));
                        context.AddError(error);
                    }
                    context.Pop();
                });

        // add more rules
    }
}
