// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

namespace Microsoft.OpenApi.Expressions
{
    /// <summary>
    /// StatusCode expression.
    /// </summary>
    public sealed class StatusCodeExpression : RuntimeExpression
    {
        /// <summary>
        /// $statusCode string.
        /// </summary>
        public const string StatusCode = "$statusCode";

        /// <summary>
        /// Gets the expression string.
        /// </summary>
        public override string Expression { get; } = StatusCode;

        /// <summary>
        /// Gets the singleton.
        /// </summary>
        public static StatusCodeExpression Instance = new StatusCodeExpression();

        /// <summary>
        /// Private constructor.
        /// </summary>
        private StatusCodeExpression()
        {
        }
    }
}
