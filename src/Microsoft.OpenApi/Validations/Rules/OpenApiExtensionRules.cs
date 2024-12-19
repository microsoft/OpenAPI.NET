// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Linq;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Properties;

namespace Microsoft.OpenApi.Validations.Rules
{
    /// <summary>
    /// The validation rules for <see cref="IOpenApiExtensible"/>.
    /// </summary>
    [OpenApiRule]
    public static class OpenApiExtensibleRules
    {
        /// <summary>
        /// Extension name MUST start with "x-".
        /// </summary>
        public static ValidationRule<IOpenApiExtensible> ExtensionNameMustStartWithXDash =>
            new(nameof(ExtensionNameMustStartWithXDash),
                (context, item) =>
                {
                    context.Enter("extensions");
                    foreach (var extensible in item.Extensions.Keys.Where(static x => !x.StartsWith("x-", StringComparison.OrdinalIgnoreCase)))
                    {
                        context.CreateError(nameof(ExtensionNameMustStartWithXDash),
                            string.Format(SRResource.Validation_ExtensionNameMustBeginWithXDash, extensible, context.PathString));
                    }
                    context.Exit();
                });
    }
}
