
using System;
using System.Diagnostics;
using Microsoft.OpenApi.Interfaces;

namespace Microsoft.OpenApi.Validations.Rules
{
    /// <summary>
    /// Class containing validation rule logic.
    /// </summary>
    public abstract class ValidationRule
    {
        internal abstract Type ValidatedType { get; }

        internal abstract void Evaluate(ValidationContext context, object item);
    }

    /// <summary>
    /// Class containing validation rule logic for <see cref="IOpenApiElement"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ValidationRule<T> : ValidationRule
        where T: IOpenApiElement
    {
        private readonly Action<ValidationContext, T> _validate;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationRule"/> class.
        /// </summary>
        /// <param name="validate">Action to perform the validation.</param>
        public ValidationRule(Action<ValidationContext, T> validate)
        {
            this._validate = validate;
        }

        internal override Type ValidatedType
        {
            get { return typeof(T); }
        }

        internal override void Evaluate(ValidationContext context, object item)
        {
            Debug.Assert(item is T, "item should be " + typeof(T));
            T typedItem = (T)item;
            this._validate(context, typedItem);
        }
    }
}
