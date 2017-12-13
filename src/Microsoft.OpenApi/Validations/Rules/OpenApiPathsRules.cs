// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Properties;

namespace Microsoft.OpenApi.Validations.Rules
{
    /// <summary>
    /// The validation rules for <see cref="OpenApiPaths"/>.
    /// </summary>
    public static class OpenApiPathsRules
    {
        /// <summary>
        /// A relative path to an individual endpoint. The field name MUST begin with a slash. 
        /// </summary>
        public static readonly ValidationRule<OpenApiPaths> PathNameMustBeginWithSlash =
            new ValidationRule<OpenApiPaths>(
                (context, item) =>
                {
                    foreach (var pathName in item.Keys)
                    {
                        context.Push(pathName);

                        if (string.IsNullOrEmpty(pathName))
                        {
                            // Add the error message
                            // context.Add(...);
                        }

                        if (!pathName.StartsWith("/"))
                        {
                            ValidationError error = new ValidationError(ErrorReason.Format, context.PathString,
                                string.Format(SRResource.Validation_PathItemMustBeginWithSlash, pathName));
                            context.AddError(error);
                        }

                        context.Pop();
                    }
                });

        // add more rules
    }
}