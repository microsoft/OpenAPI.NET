// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System;
using Microsoft.OpenApi.Exceptions;
using Microsoft.OpenApi.Properties;

namespace Microsoft.OpenApi.Expressions
{
    /// <summary>
    /// Source expression.
    /// </summary>
    public abstract class SourceExpression : RuntimeExpression
    {
        /// <summary>
        /// Gets the expression string.
        /// </summary>
        protected string Value { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryExpression"/> class.
        /// </summary>
        /// <param name="value">The value string.</param>
        protected SourceExpression(string value)
        {
            Value = value;
        }

        /// <summary>
        /// Build the source expression from input string.
        /// </summary>
        /// <param name="expression">The source expression.</param>
        /// <returns>The built source expression.</returns>
        public new static SourceExpression Build(string expression)
        {
            if (!String.IsNullOrWhiteSpace(expression))
            {
                var expressions = expression.Split('.');
                if (expressions.Length == 2)
                {
                    if (expression.StartsWith(HeaderExpression.Header))
                    {
                        // header.
                        return new HeaderExpression(expressions[1]);
                    }
                    else if (expression.StartsWith(QueryExpression.Query))
                    {
                        // query.
                        return new QueryExpression(expressions[1]);
                    }
                    else if (expression.StartsWith(PathExpression.Path))
                    {
                        // path.
                        return new PathExpression(expressions[1]);
                    }
                }

                // body
                if (expression.StartsWith(BodyExpression.Body))
                {
                    string subString = expression.Substring(BodyExpression.Body.Length);
                    if (String.IsNullOrEmpty(subString))
                    {
                        return new BodyExpression();
                    }
                    else
                    {
                        return new BodyExpression(new JsonPointer(subString));
                    }
                }
            }

            throw new OpenApiException(String.Format(SRResource.SourceExpressionHasInvalidFormat, expression));
        }
    }
}
