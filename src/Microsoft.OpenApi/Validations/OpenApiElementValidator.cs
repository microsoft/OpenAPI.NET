// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Validations.Rules;

namespace Microsoft.OpenApi.Validations
{
    /// <summary>
    /// The public APIs to validate the Open API element.
    /// </summary>
    public static class OpenApiElementValidator
    {
        /// <summary>
        /// Validate the Open API element.
        /// </summary>
        /// <typeparam name="T">The Open API element type.</typeparam>
        /// <param name="element">The Open API element.</param>
        /// <returns>True means no errors, otherwise with errors.</returns>
        public static bool Validate<T>(this T element) where T : IOpenApiElement
        {
            IEnumerable<ValidationError> errors;
            return element.Validate(out errors);
        }

        /// <summary>
        /// Validate the Open API element.
        /// </summary>
        /// <typeparam name="T">The Open API element type.</typeparam>
        /// <param name="element">The Open API element.</param>
        /// <param name="errors">The output errors.</param>
        /// <returns>True means no errors, otherwise with errors.</returns>
        public static bool Validate<T>(this T element, out IEnumerable<ValidationError> errors)
            where T : IOpenApiElement
        {
            ValidationRuleSet ruleSet = ValidationRuleSet.DefaultRuleSet;
            return element.Validate(ruleSet, out errors);
        }

        /// <summary>
        ///  Validate the Open API element.
        /// </summary>
        /// <typeparam name="T">The Open API element type.</typeparam>
        /// <param name="element">The Open API element.</param>
        /// <param name="ruleSet">The input rule set.</param>
        /// <param name="errors">The output errors.</param>
        /// <returns>True means no errors, otherwise with errors.</returns>
        public static bool Validate<T>(this T element, ValidationRuleSet ruleSet, out IEnumerable<ValidationError> errors)
            where T : IOpenApiElement
        {
            errors = null;
            if (element == null)
            {
                return true;
            }

            if (ruleSet == null)
            {
                ruleSet = ValidationRuleSet.DefaultRuleSet;
            }

            ValidationContext context = new ValidationContext(ruleSet);

            context.Validate(element);

            errors = context.Errors;

            return !errors.Any();
        }
    }
}
