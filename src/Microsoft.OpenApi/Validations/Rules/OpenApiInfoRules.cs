// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Validations.Rules
{
    /// <summary>
    /// The validation rules for <see cref="OpenApiInfo"/>.
    /// </summary>
    internal static class OpenApiInfoRules
    {
        /// <summary>
        /// The title of the application is required.
        /// </summary>
        public static readonly ValidationRule<OpenApiInfo> TitleIsRequired =
            new ValidationRule<OpenApiInfo>(
                (context, item) =>
                {
                    if (String.IsNullOrEmpty(item.Title))
                    {
                        // add error.
                    }
                });

        // add more rule.
    }
}