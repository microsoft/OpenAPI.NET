// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Services
{
    /// <summary>
    /// A class encapsulating the comarision context.
    /// </summary>
    public class ComparisonContext
    {
        private readonly IList<OpenApiDifference> _openApiDifferences = new List<OpenApiDifference>();
        private readonly Stack<string> _path = new Stack<string>();
        internal OpenApiComparerFactory OpenApiComparerFactory;

        /// <summary>
        /// Creates instance of <see cref="ComparisonContext"/>.
        /// </summary>
        /// <param name="openApiComparerFactory"></param>
        public ComparisonContext(OpenApiComparerFactory openApiComparerFactory)
        {
            OpenApiComparerFactory = openApiComparerFactory;
        }

        /// <summary>
        /// Gets the list of open api differences.
        /// </summary>
        public IEnumerable<OpenApiDifference> OpenApiDifferences => _openApiDifferences;

        /// <summary>
        /// Pointer to the source of difference in the document.
        /// </summary>
        public string PathString => "#/" + string.Join("/", _path.Reverse());

        /// <summary>
        /// Adds an open api difference.
        /// </summary>
        /// <param name="openApiDifference">The open api difference to add.</param>
        public void AddOpenApiDifference(OpenApiDifference openApiDifference)
        {
            if (openApiDifference == null)
            {
                throw Error.ArgumentNull(nameof(openApiDifference));
            }

            _openApiDifferences.Add(openApiDifference);
        }

        /// <summary>
        /// Allow Rule to indicate difference occured at a deeper context level.
        /// </summary>
        /// <param name="segment">Identifier for the context.</param>
        public void Enter(string segment)
        {
            _path.Push(segment);
        }

        /// <summary>
        /// Exit from path context level.  Enter and Exit calls should be matched.
        /// </summary>
        public void Exit()
        {
            _path.Pop();
        }

        /// <summary>
        /// Gets the comparer instance for the requested type.
        /// </summary>
        /// <typeparam name="T">Type of requested comparer.</typeparam>
        /// <returns>Comparer instance to use when comparing requested type.</returns>
        internal OpenApiComparerBase<T> GetComparer<T>()
        {
            return OpenApiComparerFactory.GetComparer<T>();
        }
    }
}