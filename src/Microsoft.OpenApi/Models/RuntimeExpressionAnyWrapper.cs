// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using Microsoft.OpenApi.Expressions;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Models
{
    /// <summary>
    /// The wrapper for <see cref="RuntimeExpression"/>
    /// </summary>
    public class RuntimeExpressionAnyWrapper : IOpenApiElement
    {
        //private IOpenApiAny _any;
        private RuntimeExpression _expression;

        /// <summary>
        /// Parameterless constructor
        /// </summary>
        public RuntimeExpressionAnyWrapper() {}

        /// <summary>
        /// Initializes a copy of an <see cref="RuntimeExpressionAnyWrapper"/> object
        /// </summary>
        public RuntimeExpressionAnyWrapper(RuntimeExpressionAnyWrapper runtimeExpressionAnyWrapper)
        {
            Expression = runtimeExpressionAnyWrapper?.Expression;
        }

        /// <summary>
        /// Gets/Set the <see cref="RuntimeExpression"/>
        /// </summary>
        public RuntimeExpression Expression
        {
            get
            {
                return _expression;
            }
            set
            {
                _expression = value;
            }
        }

        /// <summary>
        /// Write <see cref="RuntimeExpressionAnyWrapper"/>
        /// </summary>
        public void WriteValue(IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            if (_expression != null)
            {
                writer.WriteValue(_expression.Expression);
            }
        }
    }
}
