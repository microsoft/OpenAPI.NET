﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Models.Interfaces;

namespace Microsoft.OpenApi.Services
{
    /// <summary>
    /// Visits OpenApi operations and parameters.
    /// </summary>
    public class OperationSearch : OpenApiVisitorBase
    {
        private readonly Func<string, HttpMethod, OpenApiOperation, bool> _predicate;
        private readonly List<SearchResult> _searchResults = new();

        /// <summary>
        /// A list of operations from the operation search.
        /// </summary>
        public IList<SearchResult> SearchResults => _searchResults;

        /// <summary>
        /// The OperationSearch constructor.
        /// </summary>
        /// <param name="predicate">A predicate function.</param>
        public OperationSearch(Func<string, HttpMethod, OpenApiOperation, bool> predicate)
        {
            _predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
        }

        /// <inheritdoc/>
        public override void Visit(IOpenApiPathItem pathItem)
        {
            if (pathItem.Operations is not null)
            {
                foreach (var item in pathItem.Operations)
                {
                    var operation = item.Value;
                    var operationType = item.Key;

                    if (CurrentKeys.Path is not null && _predicate(CurrentKeys.Path, operationType, operation))
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
        }

        /// <summary>
        /// Visits list of <see cref="OpenApiParameter"/>.
        /// </summary>
        /// <param name="parameters">The target list of <see cref="OpenApiParameter"/>.</param>
        public override void Visit(IList<IOpenApiParameter> parameters)
        {
            /* The Parameter.Explode property should be true
             * if Parameter.Style == Form; but OData query params
             * as used in Microsoft Graph implement explode: false
             * ex: $select=id,displayName,givenName
             */
            foreach (var parameter in parameters.OfType<OpenApiParameter>().Where(static x => x.Style == ParameterStyle.Form))
            {
                parameter.Explode = false;
            }

            base.Visit(parameters);
        }

        private static CurrentKeys CopyCurrentKeys(CurrentKeys currentKeys, HttpMethod operationType)
        {
            return new()
            {
                Path = currentKeys.Path,
                Operation = operationType,
            };
        }
    }
}
