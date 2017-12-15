// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System;

namespace Microsoft.OpenApi.Validations
{
    /// <summary>
    /// The Validator attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class OpenApiValidatorAttribute : Attribute
    {
        /// <summary>
        /// Initializes the <see cref="OpenApiValidatorAttribute"/> class.
        /// </summary>
        /// <param name="validatorType">The validator type.</param>
        public OpenApiValidatorAttribute(Type validatorType)
        {
            ValidatorType = validatorType ?? throw Error.ArgumentNull(nameof(validatorType));
        }

        /// <summary>
        /// Gets the validator type.
        /// </summary>
        public Type ValidatorType { get; }
    }
}
