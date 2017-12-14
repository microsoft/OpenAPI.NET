// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.OpenApi.Validations.Rules;

namespace Microsoft.OpenApi.Validations
{
    /// <summary>
    /// The validation context.
    /// </summary>
    public class ValidationContext
    {
        private readonly IList<ValidationError> _errors = new List<ValidationError>();

        /// <summary>
        /// Initializes the <see cref="ValidationContext"/> class.
        /// </summary>
        /// <param name="ruleSet"></param>
        public ValidationContext(ValidationRuleSet ruleSet)
        {
            RuleSet = ruleSet ?? throw Error.ArgumentNull(nameof(ruleSet));
        }

        /// <summary>
        /// Gets the rule set.
        /// </summary>
        public ValidationRuleSet RuleSet { get; }

        /// <summary>
        /// Gets the validation errors.
        /// </summary>
        public IEnumerable<ValidationError> Errors
        {
            get
            {
                return _errors;
            }
        }

        /// <summary>
        /// Register an error with the validation context.
        /// </summary>
        /// <param name="error">Error to register.</param>
        public void AddError(ValidationError error)
        {
            if (error == null)
            {
                throw Error.ArgumentNull(nameof(error));
            }

            _errors.Add(error);
        }

        #region Visit Path
        private readonly Stack<string> _path = new Stack<string>();

        internal void Push(string segment)
        {
            this._path.Push(segment);
        }

        internal void Pop()
        {
            this._path.Pop();
        }

        internal string PathString
        {
            get
            {
                return String.Join("/", _path);
            }
        }
        #endregion
    }
}
