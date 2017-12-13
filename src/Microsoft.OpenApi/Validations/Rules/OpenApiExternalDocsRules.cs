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
    /// The validation rules for <see cref="OpenApiExternalDocs"/>.
    /// </summary>
    public static class OpenApiExternalDocsRules
    {
        /// <summary>
        /// REQUIRED. The url of the External Documentation Object.
        /// </summary>
        public static readonly ValidationRule<OpenApiExternalDocs> UrlIsRequired =
            new ValidationRule<OpenApiExternalDocs>(
                (context, docs) =>
                {
                    context.Push("url");
                    if (docs.Url == null)
                    {
                        ValidationError error = new ValidationError(ErrorReason.Required, context.PathString,
                            String.Format(SRResource.Validation_FieldIsRequired, "url", "External Documentation"));
                        context.AddError(error);
                    }
                    context.Pop();
                });
    }
}