// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Interfaces;

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
        /// Walks through the <see cref="OpenApiDocument"/> and visits each element.
        /// </summary>
        /// <param name="doc">OpenApiDocument to be walked</param>
        public void Walk(OpenApiDocument doc)
        {
            _visitor.Visit(doc);
            _visitor.Visit(doc.Info);
            _visitor.Visit(doc.Servers);

            // Visit Servers
            if (doc.Servers != null)
            {
                foreach (var server in doc.Servers)
                {
                    WalkServer(server);
                }
            }

            // Visit Paths
            _visitor.Visit(doc.Paths);
            foreach (var pathItem in doc.Paths.Values)
            {
                WalkPathItem(pathItem);
            }

            _visitor.Visit(doc.Components);

            _visitor.Visit(doc.ExternalDocs);

            _visitor.Visit(doc.Tags);

            foreach (var tag in doc.Tags)
            {
                WalkTag(tag);
            }

            _visitor.Visit(doc as IOpenApiExtensible);

        }

        private void WalkTag(OpenApiTag tag)
        {
            _visitor.Visit(tag);
            _visitor.Visit(tag.ExternalDocs);
            _visitor.Visit(tag as IOpenApiExtensible);
        }

        private void WalkServer(OpenApiServer server)
        {
            _visitor.Visit(server);
            foreach (var variable in server.Variables.Values)
            {
                _visitor.Visit(variable);
            }
            _visitor.Visit(server as IOpenApiExtensible);
        }

        private void WalkPathItem(OpenApiPathItem pathItem)
        {
            _visitor.Visit(pathItem);
            _visitor.Visit(pathItem.Operations);
            foreach (var operation in pathItem.Operations.Values)
            {
                WalkOperation(operation);
            }
            _visitor.Visit(pathItem as IOpenApiExtensible);
        }

        private void WalkOperation(OpenApiOperation operation)
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

            WalkRequestBody(operation.RequestBody);

            if (operation.Responses != null)
            {
                _visitor.Visit(operation.Responses);

                foreach (var response in operation.Responses.Values)
                {
                    WalkResponse(response);
                }

                _visitor.Visit(operation.Responses as IOpenApiExtensible);
            }
        }

        private void WalkResponse(OpenApiResponse response)
        {
            _visitor.Visit(response);
            WalkContentMap(response.Content);

            if (response.Links != null)
            {
                _visitor.Visit(response.Links);
                foreach (var link in response.Links.Values)
                {
                    _visitor.Visit(link);
                }
            }

            _visitor.Visit(response as IOpenApiExtensible);
        }

        private void WalkRequestBody(OpenApiRequestBody requestBody)
        {
            if (requestBody != null)
            {
                _visitor.Visit(requestBody);

                if (requestBody.Content != null)
                {
                    WalkContentMap(requestBody.Content);
                }

                _visitor.Visit(requestBody as IOpenApiExtensible);
            }
        }
 
        private void WalkContentMap(IDictionary<string, OpenApiMediaType> content)
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