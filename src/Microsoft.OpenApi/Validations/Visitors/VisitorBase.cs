// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Linq;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Properties;

namespace Microsoft.OpenApi.Validations.Visitors
{
    /// <summary>
    /// The base visitor class for Open Api elements.
    /// </summary>
    internal abstract class VisitorBase<T> : IVisitor where T : IOpenApiElement
    {
        /// <summary>
        /// Visit the element itself.
        /// </summary>
        /// <param name="context">The validation context.</param>
        /// <param name="item">The element.</param>
        public void Visit(ValidationContext context, object item)
        {
            if (context == null)
            {
                throw Error.ArgumentNull(nameof(context));
            }

            if (item == null)
            {
                return; // uplevel should verify
            }

            if (!(item is T))
            {
                throw Error.Argument(string.Format(SRResource.InputItemShouldBeType, typeof(T).FullName));
            }

            var rules = context.RuleSet.Where(r => r.ElementType == typeof(T));
            foreach (var rule in rules)
            {
                rule.Evaluate(context, item);
            }

            // verify its extension if the input item is an extensible element.
            IOpenApiExtensible extensible = item as IOpenApiExtensible;
            if (extensible != null)
            {
                rules = context.RuleSet.Where(r => r.ElementType == typeof(IOpenApiExtensible));
                foreach (var rule in rules)
                {
                    rule.Evaluate(context, extensible);
                }
            }

            T typedItem = (T)item;
            Next(context, typedItem);
        }

        /// <summary>
        /// Visit the children.
        /// </summary>
        /// <param name="context">The validation context.</param>
        /// <param name="element">The element.</param>
        protected virtual void Next(ValidationContext context, T element)
        {
        }
    }
}
