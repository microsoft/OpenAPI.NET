// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.OpenApi.Expressions
{
    /// <summary>
    /// $response. expression.
    /// </summary>
    public sealed class ResponseExpression : RuntimeExpression
    {
        public const string Response = "$response.";

        private SourceExpression _source;

        /// <summary>
        /// Gets the expression string.
        /// </summary>
        public override string Expression
        {
            get
            {
                return Response + _source.Expression;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResponseExpression"/> class.
        /// </summary>
        /// <param name="source">The source of the response.</param>
        public ResponseExpression(SourceExpression source)
        {
            _source = source ?? throw Error.ArgumentNull(nameof(source));
        }
    }
}
