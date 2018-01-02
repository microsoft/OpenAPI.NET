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

            Walk(doc.Info);
            Walk(doc.Servers);
            Walk(doc.Paths);
            Walk(doc.Components);
            Walk(doc.ExternalDocs);
            Walk(doc.Tags);
            Walk(doc as IOpenApiExtensible);
        }

        /// <summary>
        /// Walk Tags and child objects
        /// </summary>
        /// <param name="tags"></param>
        internal void Walk(IList<OpenApiTag> tags)
        {
            _visitor.Visit(tags);

            foreach (var tag in tags)
            {
                Walk(tag);
            }
        }

        /// <summary>
        /// Walk External Docs
        /// </summary>
        /// <param name="externalDocs"></param>
        internal void Walk(OpenApiExternalDocs externalDocs)
        {
            _visitor.Visit(externalDocs);
        }

        /// <summary>
        /// Walk Components
        /// </summary>
        /// <param name="components"></param>
        internal void Walk(OpenApiComponents components)
        {
            _visitor.Visit(components);
        }

        /// <summary>
        /// Walk Paths and all child objects
        /// </summary>
        /// <param name="paths"></param>
        internal void Walk(OpenApiPaths paths)
        {
            _visitor.Visit(paths);
            foreach (var pathItem in paths.Values)
            {
                Walk(pathItem);
            }
        }

        /// <summary>
        /// Walk Servers object and all child objects
        /// </summary>
        /// <param name="servers"></param>
        internal void Walk(IList<OpenApiServer> servers)
        {
            _visitor.Visit(servers);

            // Visit Servers
            if (servers != null)
            {
                foreach (var server in servers)
                {
                    Walk(server);
                }
            }
        }

        /// <summary>
        ///  Walk the info object and all child objects
        /// </summary>
        /// <param name="info"></param>
        internal void Walk(OpenApiInfo info)
        {
            _visitor.Visit(info);
            Walk(info.Contact);
            Walk(info.License);
            Walk(info as IOpenApiExtensible);
        }

        /// <summary>
        /// Walk Extensions
        /// </summary>
        /// <param name="openApiExtensible"></param>
        internal void Walk(IOpenApiExtensible openApiExtensible)
        {
            _visitor.Visit(openApiExtensible);
        }

        /// <summary>
        /// Walk License object
        /// </summary>
        /// <param name="license"></param>
        internal void Walk(OpenApiLicense license)
        {
            _visitor.Visit(license);
        }

        /// <summary>
        /// Walk Contact object
        /// </summary>
        /// <param name="contact"></param>
        internal void Walk(OpenApiContact contact)
        {
            _visitor.Visit(contact);
        }

        /// <summary>
        /// Walk Tag Object
        /// </summary>
        /// <param name="tag"></param>
        internal void Walk(OpenApiTag tag)
        {
            _visitor.Visit(tag);
            _visitor.Visit(tag.ExternalDocs);
            _visitor.Visit(tag as IOpenApiExtensible);
        }

        /// <summary>
        /// Walk Server Object
        /// </summary>
        /// <param name="server"></param>
        internal void Walk(OpenApiServer server)
        {
            _visitor.Visit(server);
            foreach (var variable in server.Variables.Values)
            {
                _visitor.Visit(variable);
            }
            _visitor.Visit(server as IOpenApiExtensible);
        }

        /// <summary>
        /// Walk Path Item
        /// </summary>
        /// <param name="pathItem"></param>
        internal void Walk(OpenApiPathItem pathItem)
        {
            _visitor.Visit(pathItem);

            Walk(pathItem.Operations);
            _visitor.Visit(pathItem as IOpenApiExtensible);
        }

        /// <summary>
        /// Walk Operations
        /// </summary>
        /// <param name="operations"></param>
        internal void Walk(IDictionary<OperationType, OpenApiOperation> operations)
        {
            _visitor.Visit(operations);
            foreach (var operation in operations.Values)
            {
                Walk(operation);
            }
        }

        /// <summary>
        /// Walk Operation
        /// </summary>
        /// <param name="operation"></param>
        internal void Walk(OpenApiOperation operation)
        {
            _visitor.Visit(operation);

            Walk(operation.Parameters);
            Walk(operation.RequestBody);
            Walk(operation.Responses);
            Walk(operation as IOpenApiExtensible);
        }

        /// <summary>
        /// Walk Parameters
        /// </summary>
        /// <param name="parameters"></param>
        internal void Walk(IList<OpenApiParameter> parameters)
        {
            if (parameters != null)
            {
                _visitor.Visit(parameters);
                foreach (var parameter in parameters)
                {
                    Walk(parameter);
                }
            }
        }

        /// <summary>
        /// Walk Parameter
        /// </summary>
        /// <param name="parameter"></param>
        internal void Walk(OpenApiParameter parameter)
        {
            _visitor.Visit(parameter);
        }

        /// <summary>
        /// Walk Responses
        /// </summary>
        /// <param name="responses"></param>
        internal void Walk(OpenApiResponses responses)
        {
            if (responses != null)
            {
                _visitor.Visit(responses);

                foreach (var response in responses.Values)
                {
                    Walk(response);
                }

                Walk(responses as IOpenApiExtensible);
            }
        }

        /// <summary>
        /// Walk Response
        /// </summary>
        /// <param name="response"></param>
        internal void Walk(OpenApiResponse response)
        {
            _visitor.Visit(response);
            Walk(response.Content);

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

        /// <summary>
        /// Walk RequestBody
        /// </summary>
        /// <param name="requestBody"></param>
        internal void Walk(OpenApiRequestBody requestBody)
        {
            if (requestBody != null)
            {
                _visitor.Visit(requestBody);

                if (requestBody.Content != null)
                {
                    Walk(requestBody.Content);
                }

                Walk(requestBody as IOpenApiExtensible);
            }
        }

        /// <summary>
        /// Walk ContentMap
        /// </summary>
        /// <param name="content"></param>
        internal void Walk(IDictionary<string, OpenApiMediaType> content)
        {
            if (content == null)
            {
                return;
            }

            _visitor.Visit(content);
            foreach (var mediaType in content.Values)
            {
                Walk(mediaType);
            }
        }

        /// <summary>
        /// Walk MediaType
        /// </summary>
        /// <param name="mediaType"></param>
        internal void Walk(OpenApiMediaType mediaType)
        {
            _visitor.Visit(mediaType);
            
            Walk(mediaType.Examples);
            Walk(mediaType.Schema);
            Walk(mediaType.Encoding);
            Walk(mediaType as IOpenApiExtensible);
        }

        /// <summary>
        /// Walk Encodings
        /// </summary>
        /// <param name="encoding"></param>
        internal void Walk(IDictionary<string, OpenApiEncoding> encoding)
        {
            foreach (var item in encoding.Values)
            {
                _visitor.Visit(item);
            }
        }

        /// <summary>
        /// Walk Encoding
        /// </summary>
        /// <param name="encoding"></param>
        internal void Walk(OpenApiEncoding encoding)
        {
            _visitor.Visit(encoding);
            Walk(encoding as IOpenApiExtensible);
        }

        /// <summary>
        /// Walk Schema
        /// </summary>
        /// <param name="schema"></param>
        internal void Walk(OpenApiSchema schema)
        {
            _visitor.Visit(schema);
            Walk(schema as IOpenApiExtensible);
        }

        /// <summary>
        /// Walk Examples
        /// </summary>
        /// <param name="examples"></param>
        internal void Walk(IDictionary<string,OpenApiExample> examples)
        {
            _visitor.Visit(examples);
            foreach (var example in examples.Values)
            {
                Walk(example);
            }
        }

        /// <summary>
        /// Walk Example
        /// </summary>
        /// <param name="example"></param>
        internal void Walk(OpenApiExample example)
        {
            _visitor.Visit(example);
            Walk(example as IOpenApiExtensible);
        }

        /// <summary>
        /// Walk OAuthFlow
        /// </summary>
        /// <param name="oAuthFlow"></param>
        internal void Walk(OpenApiOAuthFlow oAuthFlow)
        {
            _visitor.Visit(oAuthFlow);
            Walk(oAuthFlow as IOpenApiExtensible);
        }

    }
}