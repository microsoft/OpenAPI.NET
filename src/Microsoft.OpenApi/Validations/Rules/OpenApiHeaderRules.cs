﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Validations.Rules
{
    /// <summary>
    /// The validation rules for <see cref="OpenApiHeader"/>.
    /// </summary>
    //Removed from Default Rules as this is not a MUST in OpenAPI
    [OpenApiRule]
    public static class OpenApiHeaderRules
    {
        /// <summary>
        /// Validate the data matches with the given data type.
        /// </summary>
        public static ValidationRule<OpenApiHeader> HeaderMismatchedDataType =>
            new(
                (context, header) =>
                {
                    // example
                    context.Enter("example");

                    if (header.Example != null)
                    {
                        RuleHelpers.ValidateDataTypeMismatch(context, nameof(HeaderMismatchedDataType), header.Example, header.Schema);
                    }

                    context.Exit();

                    // examples
                    context.Enter("examples");

                    if (header.Examples != null)
                    {
                        foreach (var key in header.Examples.Keys)
                        {
                            if (header.Examples[key] != null)
                            {
                                context.Enter(key);
                                context.Enter("value");
                                RuleHelpers.ValidateDataTypeMismatch(context, nameof(HeaderMismatchedDataType), header.Examples[key]?.Value, header.Schema);
                                context.Exit();
                                context.Exit();
                            }
                        }
                    }

                    context.Exit();
                });

        // add more rule.
    }
}
