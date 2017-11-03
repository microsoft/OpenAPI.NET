// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.OpenApi.Expressions
{
    /// <summary>
    /// $request. expression.
    /// </summary>
    public sealed class RequestExpression : RuntimeExpression
    {
        public const string Request = "$request.";

        private SourceExpression _source;

        /// <summary>
        /// Gets the expression string.
        /// </summary>
        public override string Expression
        {
            get
            {
                return Request + _source.Expression;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestExpression"/> class.
        /// </summary>
        /// <param name="source">The source of the request.</param>
        public RequestExpression(SourceExpression source)
        {
            _source = source ?? throw Error.ArgumentNull(nameof(source));
        }

        public RequestExpression(string expression)
        {

        }
    }
}
