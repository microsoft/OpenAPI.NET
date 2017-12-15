// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Properties;

namespace Microsoft.OpenApi.Validations.Rules
{
    /// <summary>
    /// The validation rules for <see cref="OpenApiServer"/>.
    /// </summary>
    public static class OpenApiServerRules
    {
        /// <summary>
        /// REQUIRED.
        /// </summary>
        public static readonly ValidationRule<OpenApiServer> FieldIsRequired =
            new ValidationRule<OpenApiServer>(
                (context, server) =>
                {
                    context.Push("url");
                    if (String.IsNullOrEmpty(server.Url))
                    {
                        ValidationError error = new ValidationError(ErrorReason.Required, context.PathString,
                            String.Format(SRResource.Validation_FieldIsRequired, "url", "server"));
                        context.AddError(error);
                    }
                    context.Pop();
                });

        // add more rules
    }
}
