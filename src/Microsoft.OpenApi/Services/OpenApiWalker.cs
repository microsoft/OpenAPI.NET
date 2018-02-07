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
        private readonly Stack<OpenApiSchema> _schemaLoop = new Stack<OpenApiSchema>();
        private readonly Stack<OpenApiPathItem> _pathItemLoop = new Stack<OpenApiPathItem>();

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
            if (doc == null)
            {
                return;
            }

            _visitor.Visit(doc);

            Walk(OpenApiConstants.Info,() => Walk(doc.Info));
            Walk(OpenApiConstants.Servers, () => Walk(doc.Servers));
            Walk(OpenApiConstants.Paths, () => Walk(doc.Paths));
            Walk(OpenApiConstants.Components, () => Walk(doc.Components));
            Walk(OpenApiConstants.ExternalDocs, () => Walk(doc.ExternalDocs));
            Walk(OpenApiConstants.Tags, () => Walk(doc.Tags));
            Walk(doc as IOpenApiExtensible);
        }

        /// <summary>
        /// Visits list of <see cref="OpenApiTag"/> and child objects
        /// </summary>
        internal void Walk(IList<OpenApiTag> tags)
        {
            if (tags == null)
            {
                return;
            }

            _visitor.Visit(tags);

            // Visit tags
            if (tags != null)
            {
                for (int i = 0; i < tags.Count; i++)
                {
                    Walk(i.ToString(), () => Walk(tags[i]));
                }
            }

        }

        /// <summary>
        /// Visits <see cref="OpenApiExternalDocs"/> and child objects
        /// </summary>
        internal void Walk(OpenApiExternalDocs externalDocs)
        {
            if (externalDocs == null)
            {
                return;
            }

            _visitor.Visit(externalDocs);
        }

        /// <summary>
        /// Visits <see cref="OpenApiComponents"/> and child objects
        /// </summary>
        internal void Walk(OpenApiComponents components)
        {
            if (components == null)
            {
                return;
            }

            _visitor.Visit(components);

            if (components == null)
            {
                return;
            }

            Walk(OpenApiConstants.Schemas, () =>
            {
                if (components.Schemas != null)
                {
                    foreach (var item in components.Schemas)
                    {
                        Walk(item.Key, () => Walk(item.Value));
                    }
                }
            });

            Walk(OpenApiConstants.Callbacks, () =>
            {
                if (components.Callbacks != null)
                {
                    foreach (var item in components.Callbacks)
                    {
                        Walk(item.Key, () => Walk(item.Value));
                    }
                }
            });

            Walk(OpenApiConstants.Parameters, () =>
            {
                if (components.Parameters != null)
                {
                    foreach (var item in components.Parameters)
                    {
                        Walk(item.Key, () => Walk(item.Value));
                    }
                }
            });

            Walk(OpenApiConstants.Examples, () =>
            {
                if (components.Examples != null)
                {
                    foreach (var item in components.Examples)
                    {
                        Walk(item.Key, () => Walk(item.Value));
                    }
                }
            });

            Walk(OpenApiConstants.Headers, () =>
            {
                if (components.Headers != null)
                {
                    foreach (var item in components.Headers)
                    {
                        Walk(item.Key, () => Walk(item.Value));
                    }
                }
            });

            Walk(OpenApiConstants.Links, () =>
            {
                if (components.Links != null)
                {
                    foreach (var item in components.Links)
                    {
                        Walk(item.Key, () => Walk(item.Value));
                    }
                }
            });

            Walk(OpenApiConstants.RequestBodies, () =>
            {
                if (components.RequestBodies != null)
                {
                    foreach (var item in components.RequestBodies)
                    {
                        Walk(item.Key, () => Walk(item.Value));
                    }
                }
            });

            Walk(OpenApiConstants.Responses, () =>
            {
                if (components.Responses != null)
                {
                    foreach (var item in components.Responses)
                    {
                        Walk(item.Key, () => Walk(item.Value));
                    }
                }
            });

            Walk(components as IOpenApiExtensible);
        }

        /// <summary>
        /// Visits <see cref="OpenApiPaths"/> and child objects
        /// </summary>
        internal void Walk(OpenApiPaths paths)
        {
            if (paths == null)
            {
                return;
            }

            _visitor.Visit(paths);

            // Visit Paths
            if (paths != null)
            {
                foreach (var pathItem in paths)
                {
                    Walk(pathItem.Key.Replace("/", "~1"), () => Walk(pathItem.Value));// JSON Pointer uses ~1 as an escape character for /
                }
            }
        }

        /// <summary>
        /// Visits list of  <see cref="OpenApiServer"/> and child objects
        /// </summary>
        internal void Walk(IList<OpenApiServer> servers)
        {
            if (servers == null)
            {
                return;
            }

            _visitor.Visit(servers);

            // Visit Servers
            if (servers != null)
            {
                for (int i = 0; i < servers.Count; i++)
                {
                    Walk(i.ToString(),() => Walk(servers[i]));
                }
            }
        }

        /// <summary>
        /// Visits <see cref="OpenApiInfo"/> and child objects
        /// </summary>
        internal void Walk(OpenApiInfo info)
        {
            if (info == null)
            {
                return;
            }

            _visitor.Visit(info);
            if (info != null) {
                Walk(OpenApiConstants.Contact, () => Walk(info.Contact));
                Walk(OpenApiConstants.License, () => Walk(info.License));
            }
            Walk(info as IOpenApiExtensible);
        }

        /// <summary>
        /// Visits dictionary of extensions
        /// </summary>
        internal void Walk(IOpenApiExtensible openApiExtensible)
        {
            if (openApiExtensible == null)
            {
                return;
            }

            _visitor.Visit(openApiExtensible);

            if (openApiExtensible != null)
            {
                foreach (var item in openApiExtensible.Extensions)
                {
                    Walk(item.Key, () => Walk(item.Value));
                }
            }
        }

        /// <summary>
        /// Visits <see cref="IOpenApiExtension"/> 
        /// </summary>
        internal void Walk(IOpenApiExtension extension)
        {
            if (extension == null)
            {
                return;
            }

            _visitor.Visit(extension);
        }

        /// <summary>
        /// Visits <see cref="OpenApiLicense"/> and child objects
        /// </summary>
        internal void Walk(OpenApiLicense license)
        {
            if (license == null)
            {
                return;
            }

            _visitor.Visit(license);
        }

        /// <summary>
        /// Visits <see cref="OpenApiContact"/> and child objects
        /// </summary>
        internal void Walk(OpenApiContact contact)
        {
            if (contact == null)
            {
                return;
            }

            _visitor.Visit(contact);
        }

        /// <summary>
        /// Visits <see cref="OpenApiCallback"/> and child objects
        /// </summary>
        internal void Walk(OpenApiCallback callback)
        {
            if (callback == null)
            {
                return;
            }

            _visitor.Visit(callback);

            if (callback != null)
            {
                foreach (var item in callback.PathItems)
                {
                    var pathItem = item.Value;
                    Walk(item.Key.ToString(), () => Walk(pathItem));
                }
            }
        }

        /// <summary>
        /// Visits <see cref="OpenApiTag"/> and child objects
        /// </summary>
        internal void Walk(OpenApiTag tag)
        {
            if (tag == null)
            {
                return;
            }

            _visitor.Visit(tag);
            _visitor.Visit(tag.ExternalDocs);
            _visitor.Visit(tag as IOpenApiExtensible);
        }

        /// <summary>
        /// Visits <see cref="OpenApiServer"/> and child objects
        /// </summary>
        internal void Walk(OpenApiServer server)
        {
            if (server == null)
            {
                return;
            }

            _visitor.Visit(server);
            Walk(OpenApiConstants.Variables, () => Walk(server.Variables));
            _visitor.Visit(server as IOpenApiExtensible);
        }

        /// <summary>
        /// Visits dictionary of <see cref="OpenApiServerVariable"/>
        /// </summary>
        internal void Walk(IDictionary<string,OpenApiServerVariable> serverVariables)
        {
            if (serverVariables == null)
            {
                return;
            }

            _visitor.Visit(serverVariables);

            if (serverVariables != null)
            {
                foreach (var variable in serverVariables)
                {
                    Walk(variable.Key, () => Walk(variable.Value));
                }
            }
        }

        /// <summary>
        /// Visits <see cref="OpenApiServerVariable"/> and child objects
        /// </summary>
        internal void Walk(OpenApiServerVariable serverVariable)
        {
            if (serverVariable == null)
            {
                return;
            }

            _visitor.Visit(serverVariable);
            _visitor.Visit(serverVariable as IOpenApiExtensible);
        }

        /// <summary>
        /// Visits <see cref="OpenApiPathItem"/> and child objects
        /// </summary>
        internal void Walk(OpenApiPathItem pathItem)
        {
            if (pathItem == null)
            {
                return;
            }

            if (_pathItemLoop.Contains(pathItem))
            {
                return;  // Loop detected, this pathItem has already been walked.
            }
            else
            {
                _pathItemLoop.Push(pathItem);
            }

            _visitor.Visit(pathItem);

            if (pathItem != null)
            {
                Walk(pathItem.Operations);
            }
            _visitor.Visit(pathItem as IOpenApiExtensible);

            _pathItemLoop.Pop();
        }

        /// <summary>
        /// Visits dictionary of <see cref="OpenApiOperation"/>
        /// </summary>
        internal void Walk(IDictionary<OperationType, OpenApiOperation> operations)
        {
            if (operations == null)
            {
                return;
            }

            _visitor.Visit(operations);
            if (operations != null)
            {
                foreach (var operation in operations)
                {
                    Walk(operation.Key.GetDisplayName(), () => Walk(operation.Value));
                }
            }
        }

        /// <summary>
        /// Visits <see cref="OpenApiOperation"/> and child objects
        /// </summary>
        /// <param name="operation"></param>
        internal void Walk(OpenApiOperation operation)
        {
            if (operation == null)
            {
                return;
            }

            _visitor.Visit(operation);

            Walk(OpenApiConstants.Parameters, () => Walk(operation.Parameters));
            Walk(OpenApiConstants.RequestBody, () => Walk(operation.RequestBody));
            Walk(OpenApiConstants.Responses, () => Walk(operation.Responses));
            Walk(operation as IOpenApiExtensible);
        }

        /// <summary>
        /// Visits list of <see cref="OpenApiParameter"/>
        /// </summary>
        internal void Walk(IList<OpenApiParameter> parameters)
        {
            if (parameters == null)
            {
                return;
            }

            _visitor.Visit(parameters);

            if (parameters != null)
            {
                for (int i = 0; i < parameters.Count; i++)
                {
                    Walk(i.ToString(), () => Walk(parameters[i]));
                }
            }
        }

        /// <summary>
        /// Visits <see cref="OpenApiParameter"/> and child objects
        /// </summary>
        internal void Walk(OpenApiParameter parameter)
        {
            if (parameter == null)
            {
                return;
            }

            _visitor.Visit(parameter);
            Walk(OpenApiConstants.Schema, () => Walk(parameter.Schema));
            Walk(OpenApiConstants.Content, () => Walk(parameter.Content));
            Walk(parameter as IOpenApiExtensible);
        }

        /// <summary>
        /// Visits <see cref="OpenApiResponses"/> and child objects
        /// </summary>
        internal void Walk(OpenApiResponses responses)
        {
            if (responses == null)
            {
                return;
            }

            _visitor.Visit(responses);

            if (responses != null)
            {
                foreach (var response in responses)
                {
                    Walk(response.Key, () => Walk(response.Value));
                }
            }
            Walk(responses as IOpenApiExtensible);
        }

        /// <summary>
        /// Visits <see cref="OpenApiResponse"/> and child objects
        /// </summary>
        internal void Walk(OpenApiResponse response)
        {
            if (response == null)
            {
                return;
            }

            _visitor.Visit(response);
            Walk(OpenApiConstants.Content, () => Walk(response.Content));
            Walk(OpenApiConstants.Links, () => Walk(response.Links));
            Walk(OpenApiConstants.Headers, () => Walk(response.Headers));

            Walk(response as IOpenApiExtensible);
        }


        /// <summary>
        /// Visits <see cref="OpenApiRequestBody"/> and child objects
        /// </summary>
        internal void Walk(OpenApiRequestBody requestBody)
        {
            if (requestBody == null)
            {
                return;
            }

            _visitor.Visit(requestBody);

            if (requestBody != null)
            {
                if (requestBody.Content != null)
                {
                    Walk(OpenApiConstants.Content, () => Walk(requestBody.Content));
                }
            }
            Walk(requestBody as IOpenApiExtensible);
        }

        /// <summary>
        /// Visits dictionary of <see cref="OpenApiHeader"/>
        /// </summary>
        internal void Walk(IDictionary<string, OpenApiHeader> headers)
        {
            if (headers == null)
            {
                return;
            }

            _visitor.Visit(headers);
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    Walk(header.Key.Replace("/", "~1"), () => Walk(header.Value));
                }
            }
        }


        /// <summary>
        /// Visits dictionary of <see cref="OpenApiMediaType"/>
        /// </summary>
        internal void Walk(IDictionary<string, OpenApiMediaType> content)
        {
            if (content == null)
            {
                return;
            }

            _visitor.Visit(content);
            if (content != null)
            {
                foreach (var mediaType in content)
                {
                    Walk(mediaType.Key.Replace("/", "~1"), () => Walk(mediaType.Value));
                }
            }
        }

        /// <summary>
        /// Visits <see cref="OpenApiMediaType"/> and child objects
        /// </summary>
        internal void Walk(OpenApiMediaType mediaType)
        {
            if (mediaType == null)
            {
                return;
            }

            _visitor.Visit(mediaType);
            
            Walk(OpenApiConstants.Example, () => Walk(mediaType.Examples));
            Walk(OpenApiConstants.Schema, () => Walk(mediaType.Schema));
            Walk(OpenApiConstants.Encoding, () => Walk(mediaType.Encoding));
            Walk(mediaType as IOpenApiExtensible);
        }

        /// <summary>
        /// Visits dictionary of <see cref="OpenApiEncoding"/>
        /// </summary>
        internal void Walk(IDictionary<string, OpenApiEncoding> encodings)
        {
            if (encodings == null)
            {
                return;
            }

            _visitor.Visit(encodings);

            if (encodings != null)
            {
                foreach (var item in encodings)
                {
                    Walk(item.Key, () => Walk(item.Value));
                }
            }
        }

        /// <summary>
        /// Visits <see cref="OpenApiEncoding"/> and child objects
        /// </summary>
        internal void Walk(OpenApiEncoding encoding)
        {
            if (encoding == null)
            {
                return;
            }

            _visitor.Visit(encoding);
            Walk(encoding as IOpenApiExtensible);
        }

        /// <summary>
        /// Visits <see cref="OpenApiSchema"/> and child objects
        /// </summary>
        internal void Walk(OpenApiSchema schema)
        {
            if (schema == null)
            {
                return;
            }

            if (_schemaLoop.Contains(schema))
            {
                return;  // Loop detected, this schema has already been walked.
            } else
            {
                _schemaLoop.Push(schema);
            }

            _visitor.Visit(schema);

            if (schema.Items != null) {
                Walk("items", () => Walk(schema.Items));
            }

            if (schema.AllOf != null)
            {
                Walk("allOf", () => Walk(schema.AllOf));
            }

            if (schema.AnyOf != null)
            {
                Walk("anyOf", () => Walk(schema.AllOf));
            }

            if (schema.Properties != null) {
                Walk("properties", () =>
                {
                    foreach (var item in schema.Properties)
                    {
                        Walk(item.Key, () => Walk(item.Value));
                    }
                });
            }

            Walk(OpenApiConstants.ExternalDocs, () => Walk(schema.ExternalDocs));

            Walk(schema as IOpenApiExtensible);

            _schemaLoop.Pop();
        }

        /// <summary>
        /// Visits dictionary of <see cref="OpenApiExample"/>
        /// </summary>
        internal void Walk(IDictionary<string,OpenApiExample> examples)
        {
            if (examples == null)
            {
                return;
            }

            _visitor.Visit(examples);

            if (examples != null)
            {
                foreach (var example in examples)
                {
                    Walk(example.Key, () => Walk(example.Value));
                }
            }
        }

        /// <summary>
        /// Visits <see cref="IOpenApiAny"/> and child objects
        /// </summary>
        internal void Walk(IOpenApiAny example)
        {
            if (example == null)
            {
                return;
            }

            _visitor.Visit(example);
        }

        /// <summary>
        /// Visits <see cref="OpenApiExample"/> and child objects
        /// </summary>
        internal void Walk(OpenApiExample example)
        {
            if (example == null)
            {
                return;
            }

            _visitor.Visit(example);
            Walk(example as IOpenApiExtensible);
        }

        /// <summary>
        /// Visits the list of <see cref="OpenApiExample"/> and child objects
        /// </summary>
        internal void Walk(IList<OpenApiExample> examples)
        {
            if (examples == null)
            {
                return;
            }

            _visitor.Visit(examples);

            // Visit Examples
            if (examples != null)
            {
                for (int i = 0; i < examples.Count; i++)
                {
                    Walk(i.ToString(), () => Walk(examples[i]));
                }
            }
        }

        /// <summary>
        /// Visits a list of <see cref="OpenApiSchema"/> and child objects
        /// </summary>
        internal void Walk(IList<OpenApiSchema> schemas)
        {
            if (schemas == null)
            {
                return;
            }

            // Visit Schemass
            if (schemas != null)
            {
                for (int i = 0; i < schemas.Count; i++)
                {
                    Walk(i.ToString(), () => Walk(schemas[i]));
                }
            }
        }

        /// <summary>
        /// Visits <see cref="OpenApiOAuthFlows"/> and child objects
        /// </summary>
        internal void Walk(OpenApiOAuthFlows flows)
        {
            if (flows == null)
            {
                return;
            }
            _visitor.Visit(flows);
            Walk(flows as IOpenApiExtensible);
        }

        /// <summary>
        /// Visits <see cref="OpenApiOAuthFlow"/> and child objects
        /// </summary>
        internal void Walk(OpenApiOAuthFlow oAuthFlow)
        {
            if (oAuthFlow == null)
            {
                return;
            }

            _visitor.Visit(oAuthFlow);
            Walk(oAuthFlow as IOpenApiExtensible);
        }

        /// <summary>
        /// Visits dictionary of <see cref="OpenApiLink"/> and child objects
        /// </summary>
        internal void Walk(IDictionary<string,OpenApiLink> links)
        {
            if (links == null)
            {
                return;
            }

            _visitor.Visit(links);

            if (links != null)
            {
                foreach (var item in links)
                {
                    Walk(item.Key, () => Walk(item.Value));
                }
            }
        }

        /// <summary>
        /// Visits <see cref="OpenApiLink"/> and child objects
        /// </summary>
        internal void Walk(OpenApiLink link)
        {
            if (link == null)
            {
                return;
            }

            _visitor.Visit(link);
            Walk(OpenApiConstants.Server, () => Walk(link.Server));
            Walk(link as IOpenApiExtensible);
        }

        /// <summary>
        /// Visits <see cref="OpenApiHeader"/> and child objects
        /// </summary>
        internal void Walk(OpenApiHeader header)
        {
            if (header == null)
            {
                return;
            }

            _visitor.Visit(header);
            Walk(OpenApiConstants.Content, () => Walk(header.Content));
            Walk(OpenApiConstants.Example, () => Walk(header.Example));
            Walk(OpenApiConstants.Examples, () => Walk(header.Examples));
            Walk(OpenApiConstants.Schema, () => Walk(header.Schema));
            Walk(header as IOpenApiExtensible);
        }

        /// <summary>
        /// Visits <see cref="OpenApiSecurityRequirement"/> and child objects
        /// </summary>
        internal void Walk(OpenApiSecurityRequirement securityRequirement)
        {
            if (securityRequirement == null)
            {
                return;
            }

            _visitor.Visit(securityRequirement);
            Walk(securityRequirement as IOpenApiExtensible);
        }

        /// <summary>
        /// Visits <see cref="OpenApiSecurityScheme"/> and child objects
        /// </summary>
        internal void Walk(OpenApiSecurityScheme securityScheme)
        {
            if (securityScheme == null)
            {
                return;
            }

            _visitor.Visit(securityScheme);
            Walk(securityScheme as IOpenApiExtensible);
        }

        /// <summary>
        /// Dispatcher method that enables using a single method to walk the model
        /// starting from any <see cref="IOpenApiElement"/>
        /// </summary>
        internal void Walk(IOpenApiElement element)
        {
            if (element == null)
            {
                return;
            }

            switch (element)
            {
                case OpenApiDocument e: Walk(e); break;
                case OpenApiLicense e: Walk(e); break;
                case OpenApiInfo e: Walk(e); break;
                case OpenApiComponents e: Walk(e); break;
                case OpenApiContact e: Walk(e); break;
                case OpenApiCallback e: Walk(e); break;
                case OpenApiEncoding e: Walk(e); break;
                case OpenApiExample e: Walk(e); break;
                case IDictionary<string, OpenApiExample> e: Walk(e); break;
                case OpenApiExternalDocs e: Walk(e); break;
                case OpenApiHeader e: Walk(e); break;
                case OpenApiLink e: Walk(e); break;
                case IDictionary<string, OpenApiLink> e: Walk(e); break;
                case OpenApiMediaType e: Walk(e); break;
                case OpenApiOAuthFlows e: Walk(e); break;
                case OpenApiOAuthFlow e: Walk(e); break;
                case OpenApiOperation e: Walk(e); break;
                case OpenApiParameter e: Walk(e); break;
                case OpenApiRequestBody e: Walk(e); break;
                case OpenApiResponse e: Walk(e); break;
                case OpenApiSchema e: Walk(e); break;
                case OpenApiSecurityRequirement e: Walk(e); break;
                case OpenApiSecurityScheme e: Walk(e); break;
                case OpenApiServer e: Walk(e); break;
                case OpenApiServerVariable e: Walk(e); break;
                case OpenApiTag e: Walk(e); break;
                case IList<OpenApiTag> e: Walk(e); break;
                case IOpenApiExtensible e: Walk(e); break;
                case IOpenApiExtension e: Walk(e); break;
            }
        }

        /// <summary>
        /// Adds a segment to the context path to enable pointing to the current location in the document
        /// </summary>
        /// <param name="context">An identifier for the context.</param>
        /// <param name="walk">An action that walks objects within the context.</param>
        private void Walk(string context, Action walk)
        {
            _visitor.Enter(context);
            walk();
            _visitor.Exit();
        }



    }
}