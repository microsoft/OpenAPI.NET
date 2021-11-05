// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Services
{
    /// <summary>
    /// A service that slices an OpenApiDocument into a subset document
    /// </summary>
    public static class OpenApiFilterService
    {
        /// <summary>
        /// Create predicate function based on passed query parameters
        /// </summary>
        /// <param name="operationIds">Comma delimited list of operationIds or * for all operations.</param>
        /// <returns>A predicate.</returns>
        public static Func<OpenApiOperation, bool> CreatePredicate(string operationIds)
        {
            Func<OpenApiOperation, bool> predicate;
            if (operationIds != null)
            {
                if (operationIds == "*")
                {
                    predicate = (o) => true;  // All operations
                }
                else
                {
                    var operationIdsArray = operationIds.Split(',');
                    predicate = (o) => operationIdsArray.Contains(o.OperationId);
                }
            }

            else
            {
                throw new InvalidOperationException("OperationId needs to be specified.");
            }

            return predicate;
        }

        /// <summary>
        /// Create partial OpenAPI document based on the provided predicate.
        /// </summary>
        /// <param name="source">The target <see cref="OpenApiDocument"/>.</param>
        /// <param name="predicate">A predicate function.</param>
        /// <returns>A partial OpenAPI document.</returns>
        public static OpenApiDocument CreateFilteredDocument(OpenApiDocument source, Func<OpenApiOperation, bool> predicate)
        {
            // Fetch and copy title, graphVersion and server info from OpenApiDoc
            var subset = new OpenApiDocument
            {
                Info = new OpenApiInfo()
                {
                    Title = source.Info.Title + " - Subset",
                    Description = source.Info.Description,
                    TermsOfService = source.Info.TermsOfService,
                    Contact = source.Info.Contact,
                    License = source.Info.License,
                    Version = source.Info.Version,
                    Extensions = source.Info.Extensions
                },

                Components = new OpenApiComponents()
            };

            subset.Components.SecuritySchemes = source.Components.SecuritySchemes;
            subset.SecurityRequirements = source.SecurityRequirements;
            subset.Servers = source.Servers;

            var results = FindOperations(source, predicate);
            foreach (var result in results)
            {
                OpenApiPathItem pathItem;
                var pathKey = result.CurrentKeys.Path;

                if (subset.Paths == null)
                {
                    subset.Paths = new OpenApiPaths();
                    pathItem = new OpenApiPathItem();
                    subset.Paths.Add(pathKey, pathItem);
                }
                else
                {
                    if (!subset.Paths.TryGetValue(pathKey, out pathItem))
                    {
                        pathItem = new OpenApiPathItem();
                        subset.Paths.Add(pathKey, pathItem);
                    }
                }

                pathItem.Operations.Add((OperationType)result.CurrentKeys.Operation, result.Operation);
            }

            if (subset.Paths == null)
            {
                throw new ArgumentException("No paths found for the supplied parameters.");
            }

            CopyReferences(subset);

            return subset;
        }

        private static IList<SearchResult> FindOperations(OpenApiDocument graphOpenApi, Func<OpenApiOperation, bool> predicate)
        {
            var search = new OperationSearch(predicate);
            var walker = new OpenApiWalker(search);
            walker.Walk(graphOpenApi);
            return search.SearchResults;
        }

        private static void CopyReferences(OpenApiDocument target)
        {
            bool morestuff;
            do
            {
                var copy = new CopyReferences(target);
                var walker = new OpenApiWalker(copy);
                walker.Walk(target);

                morestuff = AddReferences(copy.Components, target.Components);

            } while (morestuff);
        }

        private static bool AddReferences(OpenApiComponents newComponents, OpenApiComponents target)
        {
            var moreStuff = false;
            foreach (var item in newComponents.Schemas)
            {
                if (!target.Schemas.ContainsKey(item.Key))
                {
                    moreStuff = true;
                    target.Schemas.Add(item);
                }
            }

            foreach (var item in newComponents.Parameters)
            {
                if (!target.Parameters.ContainsKey(item.Key))
                {
                    moreStuff = true;
                    target.Parameters.Add(item);
                }
            }

            foreach (var item in newComponents.Responses)
            {
                if (!target.Responses.ContainsKey(item.Key))
                {
                    moreStuff = true;
                    target.Responses.Add(item);
                }
            }
            return moreStuff;
        }
    }
}
