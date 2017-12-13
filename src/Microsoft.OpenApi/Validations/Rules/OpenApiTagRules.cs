// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Properties;

namespace Microsoft.OpenApi.Validations.Rules
{
    /// <summary>
    /// The validation rules for <see cref="OpenApiTag"/>.
    /// </summary>
    public static class OpenApiTagRules
    {
        /// <summary>
        /// REQUIRED. The name of the tag.
        /// </summary>
        public static readonly ValidationRule<OpenApiTag> NameIsRequired =
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
    }
}