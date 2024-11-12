// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Text.Json.Nodes;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Expressions;
using Microsoft.OpenApi.Helpers;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Models
{
    /// <summary>
    /// The wrapper either for <see cref="JsonNode"/> or <see cref="RuntimeExpression"/>
    /// </summary>
    public class RuntimeExpressionAnyWrapper : IOpenApiElement
    {
        private JsonNode _any;
        private RuntimeExpression _expression;

        /// <summary>
        /// Parameterless constructor
        /// </summary>
        public RuntimeExpressionAnyWrapper() { }

        /// <summary>
        /// Initializes a copy of an <see cref="RuntimeExpressionAnyWrapper"/> object
        /// </summary>
        public RuntimeExpressionAnyWrapper(RuntimeExpressionAnyWrapper runtimeExpressionAnyWrapper)
        {
            Any = JsonNodeCloneHelper.Clone(runtimeExpressionAnyWrapper?.Any);
            Expression = runtimeExpressionAnyWrapper?.Expression;
        }

        /// <summary>
        /// Gets/Sets the <see cref="JsonNode"/>
        /// </summary>
        public JsonNode Any
        {
            get
            {
                return _any;
            }
            set
            {
                _expression = null;
                _any = value;
            }
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
                _any = null;
                _expression = value;
            }
        }

        /// <summary>
        /// Write <see cref="RuntimeExpressionAnyWrapper"/>
        /// </summary>
        public void WriteValue(IOpenApiWriter writer)
        {
            Utils.CheckArgumentNull(writer);

            if (_any != null)
            {
                writer.WriteAny(_any);
            }
            else if (_expression != null)
            {
                writer.WriteValue(_expression.Expression);
            }
        }
    }
}
