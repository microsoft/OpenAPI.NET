// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Services
{
    /// <summary>
    /// The walker to visit multiple Open API elements.
    /// </summary>
    public class OpenApiWalker
    {
        readonly OpenApiVisitorBase _visitor;

        /// <summary>
        /// Initializes the <see cref="OpenApiWalker"/> class.
        /// </summary>
        public OpenApiWalker(OpenApiVisitorBase visitor)
        {
            this._visitor = visitor;
        }

        /// <summary>
        /// Walks through the <see cref="OpenApiDocument"/> and validates each element.
        /// </summary>
        /// <param name="doc"></param>
        public void Walk(OpenApiDocument doc)
        {
            this._visitor.Visit(doc);
            this._visitor.Visit(doc.Info);
            this._visitor.Visit(doc.Servers);

            if (doc.Servers != null)
            {
                foreach (var server in doc.Servers)
                {
                    this._visitor.Visit(server);
                    foreach (var variable in server.Variables.Values)
                    {
                        this._visitor.Visit(variable);
                    }
                }
            }

            this._visitor.Visit(doc.Paths);
            foreach (var pathItem in doc.Paths.Values)
            {
                this._visitor.Visit(pathItem);
                this._visitor.Visit(pathItem.Operations);
                foreach (var operation in pathItem.Operations.Values)
                {
                    this._visitor.Visit(operation);
                    if (operation.Parameters != null)
                    {
                        this._visitor.Visit(operation.Parameters);
                        foreach (var parameter in operation.Parameters)
                        {
                            this._visitor.Visit(parameter);
                        }
                    }

                    if (operation.RequestBody != null)
                    {
                        this._visitor.Visit(operation.RequestBody);

                        if (operation.RequestBody.Content != null)
                        {
                            WalkContent(operation.RequestBody.Content);
                        }
                    }

                    if (operation.Responses != null)
                    {
                        this._visitor.Visit(operation.Responses);

                        foreach (var response in operation.Responses.Values)
                        {
                            this._visitor.Visit(response);
                            WalkContent(response.Content);

                            if (response.Links != null)
                            {
                                this._visitor.Visit(response.Links);
                                foreach (var link in response.Links.Values)
                                {
                                    this._visitor.Visit(link);
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
            if (content == null) return;

            this._visitor.Visit(content);
            foreach (var mediaType in content.Values)
            {
                this._visitor.Visit(mediaType);
                this._visitor.Visit(mediaType.Examples);
                this._visitor.Visit(mediaType.Schema);
            }
        }
    }
}
