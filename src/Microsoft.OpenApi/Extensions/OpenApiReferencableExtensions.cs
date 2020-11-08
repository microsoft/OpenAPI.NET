// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Services;
using Microsoft.OpenApi.Validations;

namespace Microsoft.OpenApi.Extensions
{
    /// <summary>
    /// TODO: tmpDbg comment
    /// </summary>
    public static class OpenApiReferencableExtensions
    {
        /// <summary>
        /// TODO: tmpDbg comment
        /// </summary>
        /// <param name="element">Element to validate</param>
        /// <param name="ruleSet">Optional set of rules to use for validation</param>
        /// <returns>An IEnumerable of errors.  This function will never return null.</returns>
        public static IOpenApiReferenceable ResolveReference(this IOpenApiReferenceable element, string jsonPointer)
        {
            return element;
        }
    }
}
