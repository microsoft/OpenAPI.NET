// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Microsoft.OpenApi
{
    /// <summary>
    /// String literal with embedded expressions
    /// </summary>
    public partial class CompositeExpression : RuntimeExpression
    {
        private readonly string template;
        private const string ExpressionPattern = @"{(?<exp>\$[^}]*)";

#if NET8_0_OR_GREATER
        [GeneratedRegex(ExpressionPattern, RegexOptions.None, matchTimeoutMilliseconds: 100)]
        private static partial Regex ExpressionRegex();
#else
        private static readonly Regex ExpressionRegex = new(ExpressionPattern, RegexOptions.None, TimeSpan.FromMilliseconds(100));
#endif

        /// <summary>
        /// Expressions embedded into string literal
        /// </summary>
        public List<RuntimeExpression> ContainedExpressions { get; } = new();

        /// <summary>
        /// Create a composite expression from a string literal with an embedded expression
        /// </summary>
        /// <param name="expression"></param>
        /// <exception cref="RegexMatchTimeoutException">Extracting embedded expressions exceeds the regex match timeout.</exception>
        public CompositeExpression(string expression)
        {
            template = expression;

            // Extract subexpressions and convert to RuntimeExpressions
#if NET8_0_OR_GREATER
            var matches = ExpressionRegex().Matches(expression);
#else
            var matches = ExpressionRegex.Matches(expression);
#endif

            foreach (var item in matches.Cast<Match>())
            {
                var value = item.Groups["exp"].Captures.Cast<Capture>().First().Value;
                ContainedExpressions.Add(Build(value));
            }
        }

        /// <summary>
        /// Return original string literal with embedded expression
        /// </summary>
        public override string Expression => template;
    }
}
