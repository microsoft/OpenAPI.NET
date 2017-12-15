// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.OpenApi.Validations.Validators
{
    /// <summary>
    /// The interface for visitor.
    /// </summary>
    internal interface IValidator
    {
        /// <summary>
        /// Visit the object.
        /// </summary>
        /// <param name="context">The validation context.</param>
        /// <param name="item">The validation object.</param>
        void Validate(ValidationContext context, object item);
    }
}
