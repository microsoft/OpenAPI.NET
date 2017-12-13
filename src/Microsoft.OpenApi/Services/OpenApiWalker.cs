// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Collections.Generic;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Services
{
    /// <summary>
    /// The walker to visit multiple Open API elements.
    /// </summary>
    public class OpenApiWalker
    {
        private readonly OpenApiVisitorBase _visitor;

        /// <summary>
        /// Initializes the <see cref="OpenApiWalker"/> class.
        /// </summary>
        public OpenApiWalker(OpenApiVisitorBase visitor)
        {
            _visitor = visitor;
        }

        /// <summary>
        /// Walks through the <see cref="OpenApiDocument"/> and validates each element.
        /// </summary>
        /// <param name="doc"></param>
        public void Walk(OpenApiDocument doc)
        {
            _visitor.Visit(doc);
            _visitor.Visit(doc.Info);
            _visitor.Visit(doc.Servers);

            if (doc.Servers != null)
            {
                foreach (var server in doc.Servers)
                {
                    _visitor.Visit(server);
                    foreach (var variable in server.Variables.Values)
                    {
                        _visitor.Visit(variable);
                    }
                }
            }

            _visitor.Visit(doc.Paths);
            foreach (var pathItem in doc.Paths.Values)
            {
                _visitor.Visit(pathItem);
                _visitor.Visit(pathItem.Operations);
                foreach (var operation in pathItem.Operations.Values)
                {
                    _visitor.Visit(operation);
                    if (operation.Parameters != null)
                    {
                        _visitor.Visit(operation.Parameters);
                        foreach (var parameter in operation.Parameters)
                        {
                            _visitor.Visit(parameter);
                        }
                    }

                    if (operation.RequestBody != null)
                    {
                        _visitor.Visit(operation.RequestBody);

                        if (operation.RequestBody.Content != null)
                        {
                            WalkContent(operation.RequestBody.Content);
                        }
                    }

                    if (operation.Responses != null)
                    {
                        _visitor.Visit(operation.Responses);

                        foreach (var response in operation.Responses.Values)
                        {
                            _visitor.Visit(response);
                            WalkContent(response.Content);

                            if (response.Links != null)
                            {
                                _visitor.Visit(response.Links);
                                foreach (var link in response.Links.Values)
                                {
                                    _visitor.Visit(link);
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Walks through each media type in content and validates.
        /// </summary>
        private void WalkContent(IDictionary<string, OpenApiMediaType> content)
        {
            if (content == null)
            {
                return;
            }

            _visitor.Visit(content);
            foreach (var mediaType in content.Values)
            {
                _visitor.Visit(mediaType);
                _visitor.Visit(mediaType.Examples);
                _visitor.Visit(mediaType.Schema);
            }
        }
    }
}