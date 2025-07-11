﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace Microsoft.OpenApi
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
        /// <param name="tags">Comma delimited list of tags or a single regex.</param>
        /// <param name="requestUrls">A dictionary of requests from a postman collection.</param>
        /// <param name="source">The input OpenAPI document.</param>
        /// <returns>A predicate.</returns>
        public static Func<string, HttpMethod, OpenApiOperation, bool> CreatePredicate(
                string? operationIds = null,
                string? tags = null,
                Dictionary<string, List<string>>? requestUrls = null,
                OpenApiDocument? source = null)
        {
            Func<string, HttpMethod, OpenApiOperation, bool> predicate;
            ValidateFilters(requestUrls, operationIds, tags);
            if (operationIds != null)
            {
                predicate = GetOperationIdsPredicate(operationIds);
            }
            else if (tags != null)
            {
                predicate = GetTagsPredicate(tags);
            }
            else if (requestUrls != null && source is not null)
            {
                predicate = GetRequestUrlsPredicate(requestUrls, source);
            }
            else
            {
                throw new InvalidOperationException("Either operationId(s),tag(s) or Postman collection need to be specified.");
            }

            return predicate;
        }

        /// <summary>
        /// Create partial OpenAPI document based on the provided predicate.
        /// </summary>
        /// <param name="source">The target <see cref="OpenApiDocument"/>.</param>
        /// <param name="predicate">A predicate function.</param>
        /// <returns>A partial OpenAPI document.</returns>
        public static OpenApiDocument CreateFilteredDocument(OpenApiDocument source, Func<string, HttpMethod, OpenApiOperation, bool> predicate)
        {
            // Fetch and copy title, graphVersion and server info from OpenApiDoc
            var components = source.Components is null 
                ? null 
                : new OpenApiComponents() { SecuritySchemes = source.Components.SecuritySchemes };

            var subset = new OpenApiDocument
            {
                Info = new()
                {
                    Title = source.Info.Title + " - Subset",
                    Description = source.Info.Description,
                    TermsOfService = source.Info.TermsOfService,
                    Contact = source.Info.Contact,
                    License = source.Info.License,
                    Version = source.Info.Version,
                    Extensions = source.Info.Extensions
                },

                Components = components,
                Security = source.Security,
                Servers = source.Servers
            };

            var results = FindOperations(source, predicate);
            foreach (var result in results)
            {
                IOpenApiPathItem? pathItem = null;
                var pathKey = result.CurrentKeys?.Path;

                if (subset.Paths == null)
                {
                    subset.Paths = new();
                    pathItem = new OpenApiPathItem();
                    if (pathKey is not null)
                    {
                        subset.Paths.Add(pathKey, pathItem);
                    }
                }
                else
                {
                    if (pathKey is not null && !subset.Paths.TryGetValue(pathKey, out pathItem))
                    {
                        pathItem = new OpenApiPathItem();
                        subset.Paths.Add(pathKey, pathItem);
                    }
                }

                if (result.CurrentKeys?.Operation != null && result.Operation != null && pathItem is OpenApiPathItem openApiPathItem)
                {
                    openApiPathItem.Operations ??= [];
                    openApiPathItem.Operations?.Add(result.CurrentKeys.Operation, result.Operation);

                    if (result.Parameters?.Any() ?? false)
                    {
                        openApiPathItem.Parameters ??= [];
                        foreach (var parameter in result.Parameters)
                        {
                            if (openApiPathItem?.Parameters is not null && !openApiPathItem.Parameters.Contains(parameter))
                            {
                                openApiPathItem.Parameters.Add(parameter);
                            }
                        }
                    }
                }
            }

            if (subset.Paths == null)
            {
                throw new ArgumentException("No paths found for the supplied parameters.");
            }

            CopyReferences(subset);

            return subset;
        }

        /// <summary>
        /// Creates an <see cref="OpenApiUrlTreeNode"/> from a collection of <see cref="OpenApiDocument"/>.
        /// </summary>
        /// <param name="sources">Dictionary of labels and their corresponding <see cref="OpenApiDocument"/> objects.</param>
        /// <returns>The created <see cref="OpenApiUrlTreeNode"/>.</returns>
        public static OpenApiUrlTreeNode CreateOpenApiUrlTreeNode(Dictionary<string, OpenApiDocument> sources)
        {
            var rootNode = OpenApiUrlTreeNode.Create();
            foreach (var source in sources)
            {
                rootNode.Attach(source.Value, source.Key);
            }
            return rootNode;
        }

        private static Dictionary<HttpMethod, OpenApiOperation>? GetOpenApiOperations(OpenApiUrlTreeNode rootNode, string relativeUrl, string label)
        {
            if (relativeUrl.Equals("/", StringComparison.Ordinal) && rootNode.HasOperations(label))
            {
                return rootNode.PathItems[label].Operations;
            }

            var urlSegments = relativeUrl.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            Dictionary<HttpMethod, OpenApiOperation>? operations = null;

            var targetChild = rootNode;

            /* This will help keep track of whether we've skipped a segment
             * in the target url due to a possible parameter naming mismatch
             * with the corresponding OpenApiUrlTreeNode target child segment.
             */
            var parameterNameOffset = 0;

            for (var i = 0; i < urlSegments?.Length; i++)
            {
                var tempTargetChild = targetChild?.Children?
                                                  .FirstOrDefault(x => x.Key.Equals(urlSegments[i],
                                                                    StringComparison.OrdinalIgnoreCase)).Value;

                // Segment name mismatch
                if (tempTargetChild == null)
                {
                    if (i == 0)
                    {
                        /* If no match and we are at the 1st segment of the relative url,
                         * exit; no need to continue matching subsequent segments.
                         */
                        break;
                    }

                    /* Attempt to get the parameter segment from the children of the current node:
                     * We are assuming a failed match because of different parameter namings
                     * between the relative url segment and the corresponding OpenApiUrlTreeNode segment name
                     * ex.: matching '/users/12345/messages' with '/users/{user-id}/messages'
                     */
                    tempTargetChild = targetChild?.Children?
                                                 .FirstOrDefault(x => x.Value.IsParameter).Value;

                    /* If no parameter segment exists in the children of the
                     * current node or we've already skipped a parameter
                     * segment in the relative url from the last pass,
                     * then exit; there's no match.
                     */
                    if (tempTargetChild == null || parameterNameOffset > 0)
                    {
                        break;
                    }

                    /* To help us know we've skipped a
                     * corresponding segment in the relative url.
                     */
                    parameterNameOffset++;
                }
                else
                {
                    parameterNameOffset = 0;
                }

                // Move to the next segment
                targetChild = tempTargetChild;

                // We want the operations of the last segment of the path.
                if (i == urlSegments.Length - 1 && targetChild.HasOperations(label))
                {
                    operations = targetChild.PathItems[label].Operations;
                }
            }

            return operations;
        }

        private static IList<SearchResult> FindOperations(OpenApiDocument sourceDocument, Func<string, HttpMethod, OpenApiOperation, bool> predicate)
        {
            var search = new OperationSearch(predicate);
            var walker = new OpenApiWalker(search);
            walker.Walk(sourceDocument);
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

        private static bool AddReferences(OpenApiComponents newComponents, OpenApiComponents? target)
        {
            var moreStuff = false;
            if (newComponents.Schemas is not null)
            {
                foreach (var item in newComponents.Schemas)
                {
                    if (target?.Schemas is not null && !target.Schemas.ContainsKey(item.Key))
                    {
                        moreStuff = true;
                        target.Schemas.Add(item.Key, item.Value);
                    }
                }
            }

            if (newComponents.Parameters is not null)
            {
                foreach (var item in newComponents.Parameters)
                {
                    if (target?.Parameters is not null && !target.Parameters.ContainsKey(item.Key))
                    {
                        moreStuff = true;
                        target.Parameters.Add(item.Key, item.Value);
                    }
                }
            }

            if (newComponents.Responses is not null)
            {
                foreach (var item in newComponents.Responses)
                {
                    if (target?.Responses is not null && !target.Responses.ContainsKey(item.Key))
                    {
                        moreStuff = true;
                        target.Responses.Add(item.Key, item.Value);
                    }
                }
            }

            if (newComponents.RequestBodies is not null)
            {
                foreach (var item in newComponents.RequestBodies
                                            .Where(item => target?.RequestBodies is not null && !target.RequestBodies.ContainsKey(item.Key)))
                {
                    moreStuff = true;
                    target?.RequestBodies?.Add(item.Key, item.Value);
                }
            }

            if (newComponents.Headers is not null)
            {
                foreach (var item in newComponents.Headers
                                            .Where(item => target?.Headers is not null && !target.Headers.ContainsKey(item.Key)))
                {
                    moreStuff = true;
                    target?.Headers?.Add(item.Key, item.Value);
                }
            }

            if (newComponents.Links is not null)
            {
                foreach (var item in newComponents.Links
                                            .Where(item => target?.Links is not null && !target.Links.ContainsKey(item.Key)))
                {
                    moreStuff = true;
                    target?.Links?.Add(item.Key, item.Value);
                }
            }

            if (newComponents.Callbacks is not null)
            {
                foreach (var item in newComponents.Callbacks
                                            .Where(item => target?.Callbacks is not null && !target.Callbacks.ContainsKey(item.Key)))
                {
                    moreStuff = true;
                    target?.Callbacks?.Add(item.Key, item.Value);
                }
            }

            if (newComponents.Examples is not null)
            {
                foreach (var item in newComponents.Examples
                                            .Where(item => target?.Examples is not null && !target.Examples.ContainsKey(item.Key)))
                {
                    moreStuff = true;
                    target?.Examples?.Add(item.Key, item.Value);
                }
            }

            if (newComponents.SecuritySchemes is not null)
            {
                foreach (var item in newComponents.SecuritySchemes
                                            .Where(item => target?.SecuritySchemes is not null && !target.SecuritySchemes.ContainsKey(item.Key)))
                {
                    moreStuff = true;
                    target?.SecuritySchemes?.Add(item.Key, item.Value);
                }
            }

            return moreStuff;
        }

        private static string ExtractPath(string url, IList<OpenApiServer>? serverList)
        {
            // if OpenAPI has servers, then see if the url matches one of them
            var baseUrl = serverList?.Select(s => s.Url?.TrimEnd('/'))
                        .FirstOrDefault(c => c != null && url.Contains(c));

            return baseUrl == null ?
                    new Uri(new(SRResource.DefaultBaseUri), url).GetComponents(UriComponents.Path | UriComponents.KeepDelimiter, UriFormat.Unescaped)
                    : url.Split(new[] { baseUrl }, StringSplitOptions.None)[1];
        }

        private static void ValidateFilters(Dictionary<string, List<string>>? requestUrls, string? operationIds, string? tags)
        {
            if (requestUrls != null && (operationIds != null || tags != null))
            {
                throw new InvalidOperationException("Cannot filter by Postman collection and either operationIds and tags at the same time.");
            }
            if (!string.IsNullOrEmpty(operationIds) && !string.IsNullOrEmpty(tags))
            {
                throw new InvalidOperationException("Cannot specify both operationIds and tags at the same time.");
            }
        }

        private static Func<string, HttpMethod, OpenApiOperation, bool> GetOperationIdsPredicate(string operationIds)
        {
            if (operationIds == "*")
            {
                return (_, _, _) => true;  // All operations
            }
            else
            {
                var operationIdsArray = operationIds.Split(',');
                return (_, _, operation) => operationIdsArray.Contains(operation.OperationId);
            }
        }

        private static Func<string, HttpMethod, OpenApiOperation, bool> GetTagsPredicate(string tags)
        {
            var tagsArray = tags.Split(',');
            if (tagsArray.Length == 1)
            {
                var regex = new Regex(tagsArray[0]);
                return (_, _, operation) => operation.Tags?.Any(tag => tag.Name is not null && regex.IsMatch(tag.Name)) ?? false;
            }
            else
            {
                return (_, _, operation) => operation.Tags?.Any(tag => tagsArray.Contains(tag.Name)) ?? false;
            }
        }

        private static Func<string, HttpMethod, OpenApiOperation, bool> GetRequestUrlsPredicate(Dictionary<string, List<string>> requestUrls, OpenApiDocument source)
        {
            var operationTypes = new List<string>();
            if (source != null)
            {
                var apiVersion = source.Info.Version;
                if (apiVersion is not null)
                {
                    var sources = new Dictionary<string, OpenApiDocument> { { apiVersion, source } };
                    var rootNode = CreateOpenApiUrlTreeNode(sources);

                    // Iterate through urls dictionary and fetch operations for each url
                    foreach (var url in requestUrls)
                    {
                        var serverList = source.Servers;
                        var path = ExtractPath(url.Key, serverList);
                        var openApiOperations = GetOpenApiOperations(rootNode, path, apiVersion);
                        if (openApiOperations == null)
                        {
                            Debug.WriteLine($"The url {url.Key} could not be found in the OpenApi description");
                            continue;
                        }
                        operationTypes.AddRange(GetOperationTypes(openApiOperations, url.Value, path));
                    }
                }                
            }

            if (!operationTypes.Any())
            {
                throw new ArgumentException("The urls in the Postman collection supplied could not be found.");
            }

            // predicate for matching url and operationTypes
            return (path, operationType, _) => operationTypes.Contains(operationType + path);
        }

        private static List<string> GetOperationTypes(Dictionary<HttpMethod, OpenApiOperation> openApiOperations, List<string> url, string path)
        {
            // Add the available ops if they are in the postman collection. See path.Value
            return openApiOperations.Where(ops => url.Contains(ops.Key.ToString().ToUpper()))
                            .Select(ops => ops.Key + path)
                            .ToList();
        }
    }
}
