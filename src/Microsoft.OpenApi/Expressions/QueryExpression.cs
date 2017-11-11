// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.OpenApi.Expressions
{
    /// <summary>
    /// Query expression, the name in query is case-sensitive.
    /// </summary>
    public sealed class QueryExpression : SourceExpression
    {
        /// <summary>
        /// query. string
        /// </summary>
        public const string Query = "query.";

        /// <summary>
        /// Gets the expression string.
        /// </summary>
        public override string Expression
        {
            get
            {
                return Query + Value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryExpression"/> class.
        /// </summary>
        /// <param name="name">The name string, it's case-insensitive.</param>
        public QueryExpression(string name)
            : base(name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw Error.ArgumentNullOrWhiteSpace(nameof(name));
            }
        }
    }
}
