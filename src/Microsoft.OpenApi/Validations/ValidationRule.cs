// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using Microsoft.OpenApi.Exceptions;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Properties;

namespace Microsoft.OpenApi.Validations
{
    /// <summary>
    /// Class containing validation rule logic.
    /// </summary>
    public abstract class ValidationRule
    {
        /// <summary>
        /// Element Type.
        /// </summary>
        internal abstract Type ElementType { get; }

        /// <summary>
        /// Validation rule Name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="item">The object item.</param>
        internal abstract void Evaluate(IValidationContext context, object item);

        internal ValidationRule(string name)
        {
            Name = !string.IsNullOrEmpty(name) ? name : throw new ArgumentNullException(nameof(name));
        }
    }

    /// <summary>
    /// Class containing validation rule logic for <see cref="IOpenApiElement"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ValidationRule<T> : ValidationRule
    {
        private readonly Action<IValidationContext, T> _validate;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationRule"/> class.
        /// </summary>        
        /// <param name="validate">Action to perform the validation.</param>
        [Obsolete("Please use the other constructor and specify a name")]
        public ValidationRule(Action<IValidationContext, T> validate)
            : this (Guid.NewGuid().ToString("D"), validate)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationRule"/> class.
        /// </summary>
        /// <param name="name">Validation rule name.</param>
        /// <param name="validate">Action to perform the validation.</param>
        public ValidationRule(string name, Action<IValidationContext, T> validate)
            : base(name) 
        {
            _validate = Utils.CheckArgumentNull(validate);            
        }

        internal override Type ElementType
        {
            get { return typeof(T); }
        }

        internal override void Evaluate(IValidationContext context, object item)
        {
            if (item == null)
            {
                return;
            }

            if (item is not T)
            {
                throw new ArgumentException(string.Format(SRResource.InputItemShouldBeType, typeof(T).FullName));
            }

            var typedItem = (T)item;
            this._validate(context, typedItem);
        }
    }
}
