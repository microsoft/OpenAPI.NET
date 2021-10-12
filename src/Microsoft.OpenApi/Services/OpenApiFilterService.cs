// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Services
{
    /// <summary>
    ///
    /// </summary>
    public class OpenApiFilterService
    {
        public static readonly string GraphAuthorizationUrl = "https://login.microsoftonline.com/common/oauth2/v2.0/authorize";
        public static readonly string GraphTokenUrl = "https://login.microsoftonline.com/common/oauth2/v2.0/token";
        public static readonly string GraphUrl = "https://graph.microsoft.com/{0}/";
        public const string GraphVersion_V1 = "v1.0";


        public OpenApiDocument CreateSubsetOpenApiDocument(string operationIds, OpenApiDocument source, string title)
        {
            var predicate = CreatePredicate(operationIds);

            var subsetOpenApiDocument = CreateFilteredDocument(source, title, GraphVersion_V1, predicate);

            return subsetOpenApiDocument;
        }

        public Func<OpenApiOperation, bool> CreatePredicate(string operationIds)
        {
            string predicateSource = null;

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

                predicateSource = $"operationIds: {operationIds}";
            }

            else
            {
                throw new InvalidOperationException("OperationId needs to be specified.");
            }

            return predicate;
        }
        /// <summary>
        ///
        /// </summary>
        /// <param name="source"></param>
        /// <param name="title"></param>
        /// <param name="graphVersion"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public OpenApiDocument CreateFilteredDocument(OpenApiDocument source, string title, string graphVersion, Func<OpenApiOperation, bool> predicate)
        {
            var subset = new OpenApiDocument
            {
                Info = new OpenApiInfo()
                {
                    Title = title,
                    Version = graphVersion
                },

                Components = new OpenApiComponents()
            };
            var aadv2Scheme = new OpenApiSecurityScheme()
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows()
                {
                    AuthorizationCode = new OpenApiOAuthFlow()
                    {
                        AuthorizationUrl = new Uri(GraphAuthorizationUrl),
                        TokenUrl = new Uri(GraphTokenUrl)
                    }
                },
                Reference = new OpenApiReference() { Id = "azureaadv2", Type = ReferenceType.SecurityScheme },
                UnresolvedReference = false
            };
            subset.Components.SecuritySchemes.Add("azureaadv2", aadv2Scheme);

            subset.SecurityRequirements.Add(new OpenApiSecurityRequirement() { { aadv2Scheme, Array.Empty<string>() } });

            subset.Servers.Add(new OpenApiServer() { Description = "Core", Url = string.Format(GraphUrl, graphVersion) });

            var results = FindOperations(source, predicate);
            foreach (var result in results)
            {
                OpenApiPathItem pathItem;
                string pathKey = result.CurrentKeys.Path;

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
