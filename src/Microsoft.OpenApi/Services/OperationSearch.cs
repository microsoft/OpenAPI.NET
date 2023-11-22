// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Services
{
    /// <summary>
    /// Visits OpenApi operations and parameters.
    /// </summary>
    public class OperationSearch : OpenApiVisitorBase
    {
        private readonly Func<string, OperationType?, OpenApiOperation, bool> _predicate;
        private readonly List<SearchResult> _searchResults = new();

        /// <summary>
        /// A list of operations from the operation search.
        /// </summary>
        public IList<SearchResult> SearchResults => _searchResults;

        /// <summary>
        /// The OperationSearch constructor.
        /// </summary>
        /// <param name="predicate">A predicate function.</param>
        public OperationSearch(Func<string, OperationType?, OpenApiOperation, bool> predicate)
        {
            _predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
        }

        /// <summary>
        /// Visits <see cref="OpenApiPathItem"/>
        /// </summary>
        /// <param name="pathItem"> The target <see cref="OpenApiPathItem"/>.</param>
        public override void Visit(OpenApiPathItem pathItem)
        {
            foreach (var item in pathItem.Operations)
            {
                var operation = item.Value;
                var operationType = item.Key;

                if (_predicate(CurrentKeys.Path, operationType, operation))
                {
                    _searchResults.Add(new()
                    {
                        Operation = operation,
                        Parameters = pathItem.Parameters,
                        CurrentKeys = CopyCurrentKeys(CurrentKeys, operationType)
                    });
                }
            }
        }

        /// <summary>
        /// Visits list of <see cref="OpenApiParameter"/>.
        /// </summary>
        /// <param name="parameters">The target list of <see cref="OpenApiParameter"/>.</param>
        public override void Visit(IList<OpenApiParameter> parameters)
        {
            /* The Parameter.Explode property should be true
             * if Parameter.Style == Form; but OData query params
             * as used in Microsoft Graph implement explode: false
             * ex: $select=id,displayName,givenName
             */
            foreach (var parameter in parameters.Where(x => x.Style == ParameterStyle.Form))
            {
                parameter.Explode = false;
            }

            base.Visit(parameters);
        }

        private static CurrentKeys CopyCurrentKeys(CurrentKeys currentKeys, OperationType operationType)
        {
            return new()
            {
                Path = currentKeys.Path,
                Operation = operationType,
            };
        }
    }
}
