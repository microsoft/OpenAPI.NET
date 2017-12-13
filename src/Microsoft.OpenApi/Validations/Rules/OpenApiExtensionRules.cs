﻿// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Properties;

namespace Microsoft.OpenApi.Validations.Rules
{
    /// <summary>
    /// The validation rules for <see cref="IOpenApiExtensible"/>.
    /// </summary>
    public static class OpenApiExtensibleRules
    {
        /// <summary>
        /// Extension name MUST start with "x-".
        /// </summary>
        public static readonly ValidationRule<IOpenApiExtensible> ExtensionNameMustStartWithXDash =
            new ValidationRule<IOpenApiExtensible>(
                (context, item) =>
                {
                    context.Push("extensions");
                    foreach (var extensible in item.Extensions)
                    {
                        if (!extensible.Key.StartsWith("x-"))
                        {
                            ValidationError error = new ValidationError(ErrorReason.Format, context.PathString,
                                String.Format(SRResource.Validation_ExtensionNameMustBeginWithXDash, extensible.Key, context.PathString));
                            context.AddError(error);
                        }
                    }
                    context.Pop();
                });
    }
}