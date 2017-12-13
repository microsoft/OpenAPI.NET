// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.Diagnostics;
using System.Linq;
using Microsoft.OpenApi.Interfaces;

namespace Microsoft.OpenApi.Validations.Validators
{
    /// <summary>
    /// The base visitor class for Open Api elements.
    /// </summary>
    internal abstract class ValidatorBase<T> : IValidator where T : IOpenApiElement
    {
        /// <summary>
        /// Visit the element itself.
        /// </summary>
        /// <param name="context">The validation context.</param>
        /// <param name="item">The element.</param>
        public void Validate(ValidationContext context, object item)
        {
            Debug.Assert(item is T, "item should be " + typeof(T));

            // validate itself
            var rules = context.RuleSet.Where(r => r.ValidatedType == typeof(T));
            foreach (var rule in rules)
            {
                rule.Evaluate(context, item);
            }

            // validate it's children
            T typedItem = (T)item;
            this.Next(context, typedItem);
        }

        /// <summary>
        /// Visit the children.
        /// </summary>
        /// <param name="context">The validation context.</param>
        /// <param name="element">The element.</param>
        protected virtual void Next(ValidationContext context, T element)
        {
            IOpenApiExtensible extensbile = element as IOpenApiExtensible;
            if (extensbile != null)
            {
                var rules = context.RuleSet.Where(r => r.ValidatedType == typeof(IOpenApiExtensible));
                foreach (var rule in rules)
                {
                    rule.Evaluate(context, extensbile);
                }
            }
        }
    }
}
