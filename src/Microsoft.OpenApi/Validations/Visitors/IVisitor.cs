// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

namespace Microsoft.OpenApi.Validations.Visitors
{
    /// <summary>
    /// The interface for visitor.
    /// </summary>
    internal interface IVisitor
    {
        /// <summary>
        /// Visit the object.
        /// </summary>
        /// <param name="context">The validation context.</param>
        /// <param name="item">The validation object.</param>
        void Visit(ValidationContext context, object item);
    }
}
