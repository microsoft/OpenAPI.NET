// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Extensions;

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
        /// Visits list of <see cref="OpenApiDocument"/> and child objects
        /// </summary>
        /// <param name="doc">OpenApiDocument to be walked</param>
        public void Walk(OpenApiDocument doc)
        {
            _visitor.Visit(doc);

            Walk(doc.Info, OpenApiConstants.Info);
            Walk(doc.Servers, OpenApiConstants.Servers);
            Walk(doc.Paths,OpenApiConstants.Paths);
            Walk(doc.Components, OpenApiConstants.Components);
            Walk(doc.ExternalDocs, OpenApiConstants.ExternalDocs);
            Walk(doc.Tags, OpenApiConstants.Tags);
            Walk(doc as IOpenApiExtensible);
        }

        /// <summary>
        /// Visits list of <see cref="OpenApiTag"/> and child objects
        /// </summary>
        /// <param name="tags"></param>
        internal void Walk(IList<OpenApiTag> tags, string context)
        {
            _visitor.Enter(context);
            _visitor.Visit(tags);

            // Visit tags
            if (tags != null)
            {
                for (int i = 0; i < tags.Count; i++)
                {
                    _visitor.Enter(i.ToString());
                    Walk(tags[i], OpenApiConstants.Tags);
                    _visitor.Exit();
                }
            }
            _visitor.Exit();

        }

        /// <summary>
        /// Visits <see cref="OpenApiExternalDocs"/> and child objects
        /// </summary>
        /// <param name="externalDocs"></param>
        internal void Walk(OpenApiExternalDocs externalDocs, string context)
        {
            _visitor.Enter(context);
            _visitor.Visit(externalDocs);
            _visitor.Exit();
        }

        /// <summary>
        /// Visits <see cref="OpenApiComponents"/> and child objects
        /// </summary>
        /// <param name="components"></param>
        internal void Walk(OpenApiComponents components, string context)
        {
            _visitor.Enter(context);
            _visitor.Visit(components);
            _visitor.Exit();
        }

        /// <summary>
        /// Visits <see cref="OpenApiPaths"/> and child objects
        /// </summary>
        /// <param name="paths"></param>
        internal void Walk(OpenApiPaths paths, string context)
        {
            _visitor.Enter(context);
            _visitor.Visit(paths);
            foreach (var pathItem in paths)
            {
                Walk(pathItem.Value, pathItem.Key.Replace("/", "~1"));  // JSON Pointer uses ~1 as an escape character for /
            }
            _visitor.Exit();
        }

        /// <summary>
        /// Visits list of  <see cref="OpenApiServer"/> and child objects
        /// </summary>
        /// <param name="servers"></param>
        internal void Walk(IList<OpenApiServer> servers, string context)
        {
            _visitor.Enter(context);
            _visitor.Visit(servers);

            // Visit Servers
            if (servers != null)
            {
                for (int i = 0; i < servers.Count; i++)
                {
                    Walk(servers[i], i.ToString());
                }
            }
            _visitor.Exit();
        }

        /// <summary>
        /// Visits <see cref="OpenApiInfo"/> and child objects
        /// </summary>
        /// <param name="info"></param>
        internal void Walk(OpenApiInfo info, string context)
        {
            _visitor.Enter(context);
            _visitor.Visit(info);
            Walk(info.Contact, OpenApiConstants.Contact);
            Walk(info.License, OpenApiConstants.License);
            Walk(info as IOpenApiExtensible);
            _visitor.Exit();
        }

        /// <summary>
        /// Visits dictionary of extensions
        /// </summary>
        /// <param name="openApiExtensible"></param>
        internal void Walk(IOpenApiExtensible openApiExtensible)
        {
            _visitor.Visit(openApiExtensible);
            foreach (var item in openApiExtensible.Extensions)
            {
                Walk(item.Value, item.Key);
            }
        }

        /// <summary>
        /// Visits <see cref="IOpenApiExtension"/> 
        /// </summary>
        /// <param name="extension"></param>
        internal void Walk(IOpenApiExtension extension, string context)
        {
            _visitor.Enter(context);
            _visitor.Visit(extension);
            _visitor.Exit();
        }

        /// <summary>
        /// Visits <see cref="OpenApiLicense"/> and child objects
        /// </summary>
        /// <param name="license"></param>
        internal void Walk(OpenApiLicense license, string context)
        {
            _visitor.Enter(context);
            _visitor.Visit(license);
            _visitor.Exit();
        }

        /// <summary>
        /// Visits <see cref="OpenApiContact"/> and child objects
        /// </summary>
        /// <param name="contact"></param>
        internal void Walk(OpenApiContact contact, string context)
        {
            _visitor.Enter(context);
            _visitor.Visit(contact);
            _visitor.Exit();
        }

        /// <summary>
        /// Visits <see cref="OpenApiCallback"/> and child objects
        /// </summary>
        /// <param name="callback"></param>
        internal void Walk(OpenApiCallback callback, string context)
        {
            _visitor.Enter(context);
            _visitor.Visit(callback);
            foreach (var item in callback.PathItems)
            {
                var pathItem = item.Value;
                Walk(pathItem, item.Key.ToString());
            }
            _visitor.Exit();
        }

        /// <summary>
        /// Visits <see cref="OpenApiTag"/> and child objects
        /// </summary>
        /// <param name="tag"></param>
        internal void Walk(OpenApiTag tag, string context)
        {
            _visitor.Enter(context);
            _visitor.Visit(tag);
            _visitor.Visit(tag.ExternalDocs);
            _visitor.Visit(tag as IOpenApiExtensible);
            _visitor.Exit();
        }

        /// <summary>
        /// Visits <see cref="OpenApiServer"/> and child objects
        /// </summary>
        /// <param name="server"></param>
        internal void Walk(OpenApiServer server, string context)
        {
            _visitor.Enter(context);
            _visitor.Visit(server);
            Walk(server.Variables,OpenApiConstants.Variables);
            _visitor.Visit(server as IOpenApiExtensible);
            _visitor.Exit();
        }

        /// <summary>
        /// Visits dictionary of <see cref="OpenApiServerVariable"/>
        /// </summary>
        /// <param name="serverVariables"></param>
        internal void Walk(IDictionary<string,OpenApiServerVariable> serverVariables, string context)
        {
            _visitor.Enter(context);
            foreach (var variable in serverVariables)
            {
                Walk(variable.Value, variable.Key);
            }
            _visitor.Exit();
        }

        /// <summary>
        /// Visits <see cref="OpenApiServerVariable"/> and child objects
        /// </summary>
        /// <param name="serverVariable"></param>
        internal void Walk(OpenApiServerVariable serverVariable, string context)
        {
            _visitor.Enter(context);
            _visitor.Visit(serverVariable);
            _visitor.Visit(serverVariable as IOpenApiExtensible);
            _visitor.Exit();
        }

        /// <summary>
        /// Visits <see cref="OpenApiPathItem"/> and child objects
        /// </summary>
        /// <param name="pathItem"></param>
        internal void Walk(OpenApiPathItem pathItem, string context)
        {
            _visitor.Enter(context);
            _visitor.Visit(pathItem);
            Walk(pathItem.Operations);
            _visitor.Visit(pathItem as IOpenApiExtensible);
            _visitor.Exit();
        }

        /// <summary>
        /// Visits dictionary of <see cref="OpenApiOperation"/>
        /// </summary>
        /// <param name="operations"></param>
        internal void Walk(IDictionary<OperationType, OpenApiOperation> operations)
        {
            _visitor.Visit(operations);
            foreach (var operation in operations)
            {
                Walk(operation.Value, operation.Key.GetDisplayName());
            }
        }

        /// <summary>
        /// Visits <see cref="OpenApiOperation"/> and child objects
        /// </summary>
        /// <param name="operation"></param>
        internal void Walk(OpenApiOperation operation, string context)
        {
            _visitor.Enter(context);
            _visitor.Visit(operation);

            Walk(operation.Parameters, OpenApiConstants.Parameters);
            Walk(operation.RequestBody, OpenApiConstants.RequestBody);
            Walk(operation.Responses, OpenApiConstants.Responses);
            Walk(operation as IOpenApiExtensible);
            _visitor.Exit();
        }

        /// <summary>
        /// Visits list of <see cref="OpenApiParameter"/>
        /// </summary>
        /// <param name="parameters"></param>
        internal void Walk(IList<OpenApiParameter> parameters, string context)
        {
            _visitor.Enter(context);
            _visitor.Visit(parameters);

            if (parameters != null)
            {
                for (int i = 0; i < parameters.Count; i++)
                {
                    Walk(parameters[i], i.ToString());
                }
            }
            _visitor.Exit();
        }

        /// <summary>
        /// Visits <see cref="OpenApiParameter"/> and child objects
        /// </summary>
        /// <param name="parameter"></param>
        internal void Walk(OpenApiParameter parameter, string context)
        {
            _visitor.Enter(context);
            _visitor.Visit(parameter);
            Walk(parameter.Schema, OpenApiConstants.Schema);
            Walk(parameter.Content, OpenApiConstants.Content);
            Walk(parameter as IOpenApiExtensible);
            _visitor.Exit();
        }

        /// <summary>
        /// Visits <see cref="OpenApiResponses"/> and child objects
        /// </summary>
        /// <param name="responses"></param>
        internal void Walk(OpenApiResponses responses, string context)
        {
            if (responses != null)
            {
                _visitor.Enter(context);
                _visitor.Visit(responses);

                foreach (var response in responses)
                {
                    Walk(response.Value, response.Key);
                }

                Walk(responses as IOpenApiExtensible);
                _visitor.Exit();
            }
        }

        /// <summary>
        /// Visits <see cref="OpenApiResponse"/> and child objects
        /// </summary>
        /// <param name="response"></param>
        internal void Walk(OpenApiResponse response, string context)
        {
            _visitor.Enter(context);
            _visitor.Visit(response);
            Walk(response.Content, OpenApiConstants.Content);

            if (response.Links != null)
            {
                _visitor.Visit(response.Links);
                foreach (var link in response.Links.Values)
                {
                    _visitor.Visit(link);
                }
            }

            _visitor.Visit(response as IOpenApiExtensible);
            _visitor.Exit();
        }

        /// <summary>
        /// Visits <see cref="OpenApiRequestBody"/> and child objects
        /// </summary>
        /// <param name="requestBody"></param>
        internal void Walk(OpenApiRequestBody requestBody, string context)
        {
            _visitor.Enter(context);
            if (requestBody != null)
            {
                _visitor.Visit(requestBody);

                if (requestBody.Content != null)
                {
                    Walk(requestBody.Content, OpenApiConstants.Content);
                }

                Walk(requestBody as IOpenApiExtensible);
            }
            _visitor.Exit();
        }

        /// <summary>
        /// Visits dictionary of <see cref="OpenApiMediaType"/>
        /// </summary>
        /// <param name="content"></param>
        internal void Walk(IDictionary<string, OpenApiMediaType> content, string context)
        {
            if (content == null)
            {
                return;
            }

            _visitor.Enter(context);
            _visitor.Visit(content);
            foreach (var mediaType in content)
            {
                Walk(mediaType.Value, mediaType.Key.Replace("/", "~1"));
            }
            _visitor.Exit();
        }

        /// <summary>
        /// Visits <see cref="OpenApiMediaType"/> and child objects
        /// </summary>
        /// <param name="mediaType"></param>
        internal void Walk(OpenApiMediaType mediaType, string context)
        {
            _visitor.Enter(context);
            _visitor.Visit(mediaType);
            
            Walk(mediaType.Examples, OpenApiConstants.Example);
            Walk(mediaType.Schema, OpenApiConstants.Schema);
            Walk(mediaType.Encoding,OpenApiConstants.Encoding);
            Walk(mediaType as IOpenApiExtensible);
            _visitor.Exit();
        }

        /// <summary>
        /// Visits dictionary of <see cref="OpenApiEncoding"/>
        /// </summary>
        /// <param name="encoding"></param>
        internal void Walk(IDictionary<string, OpenApiEncoding> encoding, string context)
        {
            _visitor.Enter(context);
            foreach (var item in encoding.Values)
            {
                _visitor.Visit(item);
            }
            _visitor.Exit();
        }

        /// <summary>
        /// Visits <see cref="OpenApiEncoding"/> and child objects
        /// </summary>
        /// <param name="encoding"></param>
        internal void Walk(OpenApiEncoding encoding, string context)
        {
            _visitor.Enter(context);
            _visitor.Visit(encoding);
            Walk(encoding as IOpenApiExtensible);
            _visitor.Exit();
        }

        /// <summary>
        /// Visits <see cref="OpenApiSchema"/> and child objects
        /// </summary>
        /// <param name="schema"></param>
        internal void Walk(OpenApiSchema schema, string context)
        {
            _visitor.Enter(context);
            _visitor.Visit(schema);
            Walk(schema.ExternalDocs, OpenApiConstants.ExternalDocs);
            Walk(schema as IOpenApiExtensible);
            _visitor.Exit();
        }

        /// <summary>
        /// Visits dictionary of <see cref="OpenApiExample"/>
        /// </summary>
        /// <param name="examples"></param>
        internal void Walk(IDictionary<string,OpenApiExample> examples, string context)
        {
            _visitor.Enter(context);
            _visitor.Visit(examples);
            foreach (var example in examples.Values)
            {
                Walk(example);
            }
            _visitor.Exit();
        }

        /// <summary>
        /// Visits <see cref="IOpenApiAny"/> and child objects
        /// </summary>
        /// <param name="example"></param>
        internal void Walk(IOpenApiAny example)
        {
            _visitor.Visit(example);
        }

        /// <summary>
        /// Visits <see cref="OpenApiExample"/> and child objects
        /// </summary>
        /// <param name="example"></param>
        internal void Walk(OpenApiExample example)
        {
            _visitor.Visit(example);
            Walk(example as IOpenApiExtensible);
        }

        /// <summary>
        /// Visits the list of <see cref="OpenApiExample"/> and child objects
        /// </summary>
        /// <param name="examples"></param>
        internal void Walk(IList<OpenApiExample> examples, string context)
        {
            _visitor.Enter(context);
            _visitor.Visit(examples);

            // Visit Examples
            if (examples != null)
            {
                for (int i = 0; i < examples.Count; i++)
                {
                    _visitor.Enter(i.ToString());
                    Walk(examples[i]);
                    _visitor.Exit();
                }
            }
            _visitor.Exit();
        }

        /// <summary>
        /// Visits <see cref="OpenApiOAuthFlows"/> and child objects
        /// </summary>
        /// <param name="flows"></param>
        internal void Walk(OpenApiOAuthFlows flows, string context)
        {
            _visitor.Enter(context);
            _visitor.Visit(flows);
            Walk(flows as IOpenApiExtensible);
            _visitor.Exit();

        }

        /// <summary>
        /// Visits <see cref="OpenApiOAuthFlow"/> and child objects
        /// </summary>
        /// <param name="oAuthFlow"></param>
        internal void Walk(OpenApiOAuthFlow oAuthFlow, string context)
        {
            _visitor.Enter(context);
            _visitor.Visit(oAuthFlow);
            Walk(oAuthFlow as IOpenApiExtensible);
            _visitor.Exit();
        }

        /// <summary>
        /// Visits dictionary of <see cref="OpenApiLink"/> and child objects
        /// </summary>
        /// <param name="links"></param>
        internal void Walk(IDictionary<string,OpenApiLink> links, string context)
        {
            _visitor.Enter(context);
            foreach (var item in links)
            {
                Walk(item.Value, item.Key);
            }
            _visitor.Exit();
        }

        /// <summary>
        /// Visits <see cref="OpenApiLink"/> and child objects
        /// </summary>
        /// <param name="link"></param>
        internal void Walk(OpenApiLink link, string context)
        {
            _visitor.Enter(context);
            _visitor.Visit(link);
            Walk(link.Server, OpenApiConstants.Server);
            Walk(link as IOpenApiExtensible);
            _visitor.Exit();
        }

        /// <summary>
        /// Visits <see cref="OpenApiHeader"/> and child objects
        /// </summary>
        /// <param name="header"></param>
        internal void Walk(OpenApiHeader header, string context)
        {
            _visitor.Enter(context);
            _visitor.Visit(header);
            Walk(header.Content, OpenApiConstants.Content);
            Walk(header.Example);
            Walk(header.Examples, OpenApiConstants.Examples);
            Walk(header.Schema, OpenApiConstants.Schema);
            Walk(header as IOpenApiExtensible);
            _visitor.Exit();
        }

        /// <summary>
        /// Visits <see cref="OpenApiSecurityRequirement"/> and child objects
        /// </summary>
        /// <param name="securityRequirement"></param>
        internal void Walk(OpenApiSecurityRequirement securityRequirement, string context)
        {
            _visitor.Enter(context);
            _visitor.Visit(securityRequirement);
            Walk(securityRequirement as IOpenApiExtensible);
            _visitor.Exit();
        }

        /// <summary>
        /// Visits <see cref="OpenApiSecurityScheme"/> and child objects
        /// </summary>
        /// <param name="securityScheme"></param>
        internal void Walk(OpenApiSecurityScheme securityScheme, string context)
        {
            _visitor.Enter(context);
            _visitor.Visit(securityScheme);
            Walk(securityScheme as IOpenApiExtensible);
            _visitor.Exit();
        }

        /// <summary>
        /// Walk IOpenApiElement
        /// </summary>
        /// <param name="element"></param>
        internal void Walk(IOpenApiElement element)
        {
            switch(element)
            {
                case OpenApiDocument e: Walk(e); break;
                case OpenApiLicense e: Walk(e, OpenApiConstants.License); break;
                case OpenApiInfo e: Walk(e, OpenApiConstants.Info); break;
                case OpenApiComponents e: Walk(e, OpenApiConstants.Components); break;
                case OpenApiContact e: Walk(e, OpenApiConstants.Contact); break;
                case OpenApiCallback e: Walk(e,""); break;
                case OpenApiEncoding e: Walk(e,""); break;
                case OpenApiExample e: Walk(e); break;
                case IDictionary<string, OpenApiExample> e: Walk(e, OpenApiConstants.Examples); break;
                case OpenApiExternalDocs e: Walk(e, OpenApiConstants.ExternalDocs); break;
                case OpenApiHeader e: Walk(e, OpenApiConstants.Headers); break;
                case OpenApiLink e: Walk(e, ""); break;
                case IDictionary<string, OpenApiLink> e: Walk(e, OpenApiConstants.Links); break;
                case OpenApiMediaType e: Walk(e, ""); break;
                case OpenApiOAuthFlows e: Walk(e, OpenApiConstants.Flows); break;
                case OpenApiOAuthFlow e: Walk(e, ""); break;
                case OpenApiOperation e: Walk(e, ""); break;
                case OpenApiParameter e: Walk(e,""); break;
                case OpenApiRequestBody e: Walk(e, OpenApiConstants.RequestBody); break;
                case OpenApiResponse e: Walk(e,""); break;
                case OpenApiSchema e: Walk(e, OpenApiConstants.Schema); break;
                case OpenApiSecurityRequirement e: Walk(e,""); break;
                case OpenApiSecurityScheme e: Walk(e,""); break;
                case OpenApiServer e: Walk(e,""); break;
                case OpenApiServerVariable e: Walk(e,""); break;
                case OpenApiTag e: Walk(e,""); break;
                case IList<OpenApiTag> e: Walk(e, OpenApiConstants.Tags); break;
                case IOpenApiExtensible e: Walk(e); break;
                case IOpenApiExtension e: Walk(e,""); break;

            }
        }

    }
}