// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

namespace Microsoft.OpenApi.Expressions
{
    /// <summary>
    /// Method expression.
    /// </summary>
    public sealed class MethodExpression : RuntimeExpression
    {
        /// <summary>
        /// $method. string
        /// </summary>
        public const string Method = "$method";

        /// <summary>
        /// Gets the expression string.
        /// </summary>
        public override string Expression { get; } = Method;

        /// <summary>
        /// Gets the singleton.
        /// </summary>
        public static MethodExpression Instance = new MethodExpression();

        /// <summary>
        /// Private constructor.
        /// </summary>
        private MethodExpression()
        {
        }
    }
}
