// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Collections.Generic;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Services;
using Microsoft.OpenApi.Validations;

namespace Microsoft.OpenApi.Extensions
{
    /// <summary>
    /// Extension methods that apply across all OpenAPIElements
    /// </summary>
    public static class OpenApiElementExtensions
    {
        /// <summary>
        /// Validate element and all child elements
        /// </summary>
        public static IEnumerable<ValidationError> Validate(this IOpenApiElement element)
        {
            var validator = new OpenApiValidator();
            var walker = new OpenApiWalker(validator);
            walker.Walk(element);
            return validator.Errors;
        }
    }
}