// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using Microsoft.OpenApi.Interfaces;

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
        private static readonly ValidationRule<IOpenApiExtensible> ExtensionNameMustStartWithXDollar =
            new ValidationRule<IOpenApiExtensible>(
                (context, item) =>
                {
                    foreach (var extensible in item.Extensions)
                    {

                    }
                });
    }
}