// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Models.Interfaces;
using Microsoft.OpenApi.Models.References;

namespace Microsoft.OpenApi.Services
{
    /// <summary>
    /// The walker to visit multiple Open API elements.
    /// </summary>
    public class OpenApiWalker
    {
        private readonly OpenApiVisitorBase _visitor;
        private readonly Stack<IOpenApiSchema> _schemaLoop = new();
        private readonly Stack<IOpenApiPathItem> _pathItemLoop = new();

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

            _schemaLoop.Clear();
            _pathItemLoop.Clear();

            _visitor.Visit(doc);

            Walk(OpenApiConstants.Info, () => Walk(doc.Info));
            Walk(OpenApiConstants.Servers, () => Walk(doc.Servers));
            Walk(OpenApiConstants.Paths, () => Walk(doc.Paths));
            Walk(OpenApiConstants.Webhooks, () => Walk(doc.Webhooks));
            Walk(OpenApiConstants.Components, () => Walk(doc.Components));
            Walk(OpenApiConstants.Security, () => Walk(doc.Security));
            Walk(OpenApiConstants.ExternalDocs, () => Walk(doc.ExternalDocs));
            Walk(OpenApiConstants.Tags, () => Walk(doc.Tags));
            Walk(doc as IOpenApiExtensible);
        }

        /// <summary>
        /// Visits list of <see cref="OpenApiTag"/> and child objects
        /// </summary>
        internal void Walk(ISet<OpenApiTag> tags)
        {
            if (tags == null)
            {
                return;
            }

            _visitor.Visit(tags);

            // Visit tags
            if (tags != null)
            {
                var tagsAsArray = tags.ToArray();
                for (var i = 0; i < tagsAsArray.Length; i++)
                {
                    Walk(i.ToString(), () => Walk(tagsAsArray[i]));
                }
            }
        }

        /// <summary>
        /// Visits list of <see cref="OpenApiTagReference"/> and child objects
        /// </summary>
        internal void Walk(ISet<OpenApiTagReference> tags)
        {
            if (tags == null)
            {
                return;
            }

            _visitor.Visit(tags);

            // Visit tags
            if (tags != null)
            {
                var referencesAsArray = tags.ToArray();
                for (var i = 0; i < referencesAsArray.Length; i++)
                {
                    Walk(i.ToString(), () => Walk(referencesAsArray[i]));
                }
            }
        }

        /// <summary>
        /// Visits <see cref="OpenApiExternalDocs"/> and child objects
        /// </summary>
        internal void Walk(string externalDocs)
        {
            if (externalDocs == null)
            {
                return;
            }

            _visitor.Visit(externalDocs);
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
#nullable enable
        /// <summary>
        /// Visits <see cref="OpenApiComponents"/> and child objects
        /// </summary>
        internal void Walk(OpenApiComponents? components)
        {
            if (components == null)
            {
                return;
            }

            _visitor.Visit(components);

            Walk(OpenApiConstants.Schemas, () =>
            {
                if (components.Schemas != null)
                {
                    foreach (var item in components.Schemas)
                    {
                        Walk(item.Key, () => Walk(item.Value, isComponent: true));
                    }
                }
            });

            Walk(OpenApiConstants.SecuritySchemes, () =>
            {
                if (components.SecuritySchemes != null)
                {
                    foreach (var item in components.SecuritySchemes)
                    {
                        Walk(item.Key, () => Walk(item.Value, isComponent: true));
                    }
                }
            });

            Walk(OpenApiConstants.Callbacks, () =>
            {
                if (components.Callbacks != null)
                {
                    foreach (var item in components.Callbacks)
                    {
                        Walk(item.Key, () => Walk(item.Value, isComponent: true));
                    }
                }
            });

            Walk(OpenApiConstants.PathItems, () =>
            {
                if (components.PathItems != null)
                {
                    foreach (var path in components.PathItems)
                    {
                        Walk(path.Key, () => Walk(path.Value, isComponent: true));
                    }
                }
            });

            Walk(OpenApiConstants.Parameters, () =>
            {
                if (components.Parameters != null)
                {
                    foreach (var item in components.Parameters)
                    {
                        Walk(item.Key, () => Walk(item.Value, isComponent: true));
                    }
                }
            });

            Walk(OpenApiConstants.Examples, () =>
            {
                if (components.Examples != null)
                {
                    foreach (var item in components.Examples)
                    {
                        Walk(item.Key, () => Walk(item.Value, isComponent: true));
                    }
                }
            });

            Walk(OpenApiConstants.Headers, () =>
            {
                if (components.Headers != null)
                {
                    foreach (var item in components.Headers)
                    {
                        Walk(item.Key, () => Walk(item.Value, isComponent: true));
                    }
                }
            });

            Walk(OpenApiConstants.Links, () =>
            {
                if (components.Links != null)
                {
                    foreach (var item in components.Links)
                    {
                        Walk(item.Key, () => Walk(item.Value, isComponent: true));
                    }
                }
            });

            Walk(OpenApiConstants.RequestBodies, () =>
            {
                if (components.RequestBodies != null)
                {
                    foreach (var item in components.RequestBodies)
                    {
                        Walk(item.Key, () => Walk(item.Value, isComponent: true));
                    }
                }
            });

            Walk(OpenApiConstants.Responses, () =>
            {
                if (components.Responses != null)
                {
                    foreach (var item in components.Responses)
                    {
                        Walk(item.Key, () => Walk(item.Value, isComponent: true));
                    }
                }
            });

            Walk(components as IOpenApiExtensible);
        }

#nullable restore
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
                    _visitor.CurrentKeys.Path = pathItem.Key;
                    Walk(pathItem.Key, () => Walk(pathItem.Value));// JSON Pointer uses ~1 as an escape character for /
                    _visitor.CurrentKeys.Path = null;
                }
            }

        }

        /// <summary>
        /// Visits Webhooks and child objects
        /// </summary>
        internal void Walk(IDictionary<string, IOpenApiPathItem> webhooks)
        {
            if (webhooks == null)
            {
                return;
            }

            _visitor.Visit(webhooks);

            // Visit Webhooks
            if (webhooks != null)
            {
                foreach (var pathItem in webhooks)
                {
                    _visitor.CurrentKeys.Path = pathItem.Key;
                    Walk(pathItem.Key, () => Walk(pathItem.Value));// JSON Pointer uses ~1 as an escape character for /
                    _visitor.CurrentKeys.Path = null;
                }
            };
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
                for (var i = 0; i < servers.Count; i++)
                {
                    Walk(i.ToString(), () => Walk(servers[i]));
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
            if (info != null)
            {
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

            if (openApiExtensible.Extensions != null)
            {
                foreach (var item in openApiExtensible.Extensions)
                {
                    _visitor.CurrentKeys.Extension = item.Key;
                    Walk(item.Key, () => Walk(item.Value));
                    _visitor.CurrentKeys.Extension = null;
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
        /// Visits <see cref="IOpenApiCallback"/> and child objects
        /// </summary>
        internal void Walk(IOpenApiCallback callback, bool isComponent = false)
        {
            if (callback == null)
            {
                return;
            }

            if (callback is IOpenApiReferenceHolder openApiReferenceHolder)
            {
                Walk(openApiReferenceHolder);
                return;
            }

            _visitor.Visit(callback);

            if (callback != null)
            {
                foreach (var item in callback.PathItems)
                {
                    _visitor.CurrentKeys.Callback = item.Key.ToString();
                    var pathItem = item.Value;
                    Walk(item.Key.ToString(), () => Walk(pathItem));
                    _visitor.CurrentKeys.Callback = null;
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
        /// Visits <see cref="OpenApiTagReference"/> and child objects
        /// </summary>
        internal void Walk(OpenApiTagReference tag)
        {
            if (tag == null)
            {
                return;
            }

            if (tag is IOpenApiReferenceHolder openApiReferenceHolder)
            {
                Walk(openApiReferenceHolder);
            }
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
        internal void Walk(IDictionary<string, OpenApiServerVariable> serverVariables)
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
                    _visitor.CurrentKeys.ServerVariable = variable.Key;
                    Walk(variable.Key, () => Walk(variable.Value));
                    _visitor.CurrentKeys.ServerVariable = null;
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
        internal void Walk(IOpenApiPathItem pathItem, bool isComponent = false)
        {
            if (pathItem == null)
            {
                return;
            }

            if (pathItem is IOpenApiReferenceHolder openApiReferenceHolder)
            {
                Walk(openApiReferenceHolder);
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
                Walk(OpenApiConstants.Parameters, () => Walk(pathItem.Parameters));
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
                    _visitor.CurrentKeys.Operation = operation.Key;
                    Walk(operation.Key.GetDisplayName(), () => Walk(operation.Value));
                    _visitor.CurrentKeys.Operation = null;
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
            Walk(OpenApiConstants.Callbacks, () => Walk(operation.Callbacks));
            Walk(OpenApiConstants.Tags, () => Walk(operation.Tags));
            Walk(OpenApiConstants.Security, () => Walk(operation.Security));
            Walk(operation as IOpenApiExtensible);
        }

        /// <summary>
        /// Visits list of <see cref="OpenApiSecurityRequirement"/>
        /// </summary>
        internal void Walk(IList<OpenApiSecurityRequirement> securityRequirements)
        {
            if (securityRequirements == null)
            {
                return;
            }

            _visitor.Visit(securityRequirements);

            if (securityRequirements != null)
            {
                for (var i = 0; i < securityRequirements.Count; i++)
                {
                    Walk(i.ToString(), () => Walk(securityRequirements[i]));
                }
            }
        }

        /// <summary>
        /// Visits list of <see cref="OpenApiParameter"/>
        /// </summary>
        internal void Walk(IList<IOpenApiParameter> parameters)
        {
            if (parameters == null)
            {
                return;
            }

            _visitor.Visit(parameters);

            if (parameters != null)
            {
                for (var i = 0; i < parameters.Count; i++)
                {
                    Walk(i.ToString(), () => Walk(parameters[i]));
                }
            }
        }

        /// <summary>
        /// Visits <see cref="OpenApiParameter"/> and child objects
        /// </summary>
        internal void Walk(IOpenApiParameter parameter, bool isComponent = false)
        {
            if (parameter == null)
            {
                return;
            }

            if (parameter is IOpenApiReferenceHolder openApiReferenceHolder)
            {
                Walk(openApiReferenceHolder);
                return;
            }

            _visitor.Visit(parameter);
            Walk(OpenApiConstants.Schema, () => Walk(parameter.Schema));
            Walk(OpenApiConstants.Content, () => Walk(parameter.Content));
            Walk(OpenApiConstants.Examples, () => Walk(parameter.Examples));

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
                    _visitor.CurrentKeys.Response = response.Key;
                    Walk(response.Key, () => Walk(response.Value));
                    _visitor.CurrentKeys.Response = null;
                }
            }
            Walk(responses as IOpenApiExtensible);
        }

        /// <summary>
        /// Visits <see cref="OpenApiResponse"/> and child objects
        /// </summary>
        internal void Walk(IOpenApiResponse response, bool isComponent = false)
        {
            if (response == null)
            {
                return;
            }

            if (response is IOpenApiReferenceHolder openApiReferenceHolder)
            {
                Walk(openApiReferenceHolder);
                return;
            }

            _visitor.Visit(response);
            Walk(OpenApiConstants.Content, () => Walk(response.Content));
            Walk(OpenApiConstants.Links, () => Walk(response.Links));
            Walk(OpenApiConstants.Headers, () => Walk(response.Headers));
            Walk(response as IOpenApiExtensible);
        }

        /// <summary>
        /// Visits <see cref="IOpenApiRequestBody"/> and child objects
        /// </summary>
        internal void Walk(IOpenApiRequestBody requestBody, bool isComponent = false)
        {
            if (requestBody == null)
            {
                return;
            }

            if (requestBody is IOpenApiReferenceHolder openApiReferenceHolder)
            {
                Walk(openApiReferenceHolder);
                return;
            }

            _visitor.Visit(requestBody);

            if (requestBody is {Content: not null})
            {
                Walk(OpenApiConstants.Content, () => Walk(requestBody.Content));
            }
            Walk(requestBody as IOpenApiExtensible);
        }

        /// <summary>
        /// Visits dictionary of <see cref="OpenApiHeader"/>
        /// </summary>
        internal void Walk(IDictionary<string, IOpenApiHeader> headers)
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
                    _visitor.CurrentKeys.Header = header.Key;
                    Walk(header.Key, () => Walk(header.Value));
                    _visitor.CurrentKeys.Header = null;
                }
            }
        }

        /// <summary>
        /// Visits dictionary of <see cref="IOpenApiCallback"/>
        /// </summary>
        internal void Walk(IDictionary<string, IOpenApiCallback> callbacks)
        {
            if (callbacks == null)
            {
                return;
            }

            _visitor.Visit(callbacks);
            if (callbacks != null)
            {
                foreach (var callback in callbacks)
                {
                    _visitor.CurrentKeys.Callback = callback.Key;
                    Walk(callback.Key, () => Walk(callback.Value));
                    _visitor.CurrentKeys.Callback = null;
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
                    _visitor.CurrentKeys.Content = mediaType.Key;
                    Walk(mediaType.Key, () => Walk(mediaType.Value));
                    _visitor.CurrentKeys.Content = null;
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
                    _visitor.CurrentKeys.Encoding = item.Key;
                    Walk(item.Key, () => Walk(item.Value));
                    _visitor.CurrentKeys.Encoding = null;
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

            if (encoding.Headers != null)
            {
                Walk(encoding.Headers);
            }
            Walk(encoding as IOpenApiExtensible);
        }

        /// <summary>
        /// Visits <see cref="IOpenApiSchema"/> and child objects
        /// </summary>
        internal void Walk(IOpenApiSchema schema, bool isComponent = false)
        {
            if (schema == null || schema is IOpenApiReferenceHolder holder && ProcessAsReference(holder, isComponent))
            {
                return;
            }

            if (_schemaLoop.Contains(schema))
            {
                return;  // Loop detected, this schema has already been walked.
            }
            else
            {
                _schemaLoop.Push(schema);
            }

            _visitor.Visit(schema);

            if (schema.Items != null)
            {
                Walk("items", () => Walk(schema.Items));
            }

            if (schema.Not != null)
            {
                Walk("not", () => Walk(schema.Not));
            }

            if (schema.AllOf != null)
            {
                Walk("allOf", () => Walk(schema.AllOf));
            }

            if (schema.AnyOf != null)
            {
                Walk("anyOf", () => Walk(schema.AnyOf));
            }

            if (schema.OneOf != null)
            {
                Walk("oneOf", () => Walk(schema.OneOf));
            }

            if (schema.Properties != null)
            {
                Walk("properties", () =>
                {
                    foreach (var item in schema.Properties)
                    {
                        Walk(item.Key, () => Walk(item.Value));
                    }
                });
            }

            if (schema.AdditionalProperties != null)
            {
                Walk("additionalProperties", () => Walk(schema.AdditionalProperties));
            }

            Walk(OpenApiConstants.ExternalDocs, () => Walk(schema.ExternalDocs));

            Walk(schema as IOpenApiExtensible);

            _schemaLoop.Pop();
        }


        /// <summary>
        /// Visits dictionary of <see cref="IOpenApiExample"/>
        /// </summary>
        internal void Walk(IDictionary<string, IOpenApiExample> examples)
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
                    _visitor.CurrentKeys.Example = example.Key;
                    Walk(example.Key, () => Walk(example.Value));
                    _visitor.CurrentKeys.Example = null;
                }
            }
        }

        /// <summary>
        /// Visits <see cref="OpenApiAny"/> and child objects
        /// </summary>
        internal void Walk(JsonNode example)
        {
            if (example == null)
            {
                return;
            }

            _visitor.Visit(example);
        }

        /// <summary>
        /// Visits <see cref="IOpenApiExample"/> and child objects
        /// </summary>
        internal void Walk(IOpenApiExample example, bool isComponent = false)
        {
            if (example == null)
            {
                return;
            }

            if (example is OpenApiExampleReference reference)
            {
                Walk(reference);
                return;
            }

            _visitor.Visit(example);
            Walk(example as IOpenApiExtensible);
        }

        /// <summary>
        /// Visits the list of <see cref="IOpenApiExample"/> and child objects
        /// </summary>
        internal void Walk(IList<IOpenApiExample> examples)
        {
            if (examples == null)
            {
                return;
            }

            _visitor.Visit(examples);

            // Visit Examples
            if (examples != null)
            {
                for (var i = 0; i < examples.Count; i++)
                {
                    Walk(i.ToString(), () => Walk(examples[i]));
                }
            }
        }

        /// <summary>
        /// Visits a list of <see cref="IOpenApiSchema"/> and child objects
        /// </summary>
        internal void Walk(IList<IOpenApiSchema> schemas)
        {
            if (schemas == null)
            {
                return;
            }

            // Visit Schemas
            if (schemas != null)
            {
                for (var i = 0; i < schemas.Count; i++)
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
        /// Visits dictionary of <see cref="IOpenApiLink"/> and child objects
        /// </summary>
        internal void Walk(IDictionary<string, IOpenApiLink> links)
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
                    _visitor.CurrentKeys.Link = item.Key;
                    Walk(item.Key, () => Walk(item.Value));
                    _visitor.CurrentKeys.Link = null;
                }
            }
        }

        /// <summary>
        /// Visits <see cref="OpenApiLink"/> and child objects
        /// </summary>
        internal void Walk(IOpenApiLink link, bool isComponent = false)
        {
            if (link == null)
            {
                return;
            }

            if (link is IOpenApiReferenceHolder openApiReferenceHolder)
            {
                Walk(openApiReferenceHolder);
                return;
            }

            _visitor.Visit(link);
            Walk(OpenApiConstants.Server, () => Walk(link.Server));
            Walk(link as IOpenApiExtensible);
        }

        /// <summary>
        /// Visits <see cref="OpenApiHeader"/> and child objects
        /// </summary>
        internal void Walk(IOpenApiHeader header, bool isComponent = false)
        {
            if (header == null)
            {
                return;
            }

            if (header is IOpenApiReferenceHolder openApiReferenceHolder)
            {
                Walk(openApiReferenceHolder);
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

            foreach(var securityScheme in securityRequirement.Keys)
            {
                Walk(securityScheme);
            }

            _visitor.Visit(securityRequirement);
            Walk(securityRequirement as IOpenApiExtensible);
        }

        /// <summary>
        /// Visits <see cref="IOpenApiSecurityScheme"/> and child objects
        /// </summary>
        internal void Walk(IOpenApiSecurityScheme securityScheme, bool isComponent = false)
        {
            if (securityScheme == null)
            {
                return;
            }

            if (securityScheme is IOpenApiReferenceHolder openApiReferenceHolder)
            {
                Walk(openApiReferenceHolder);
                return;
            }

            _visitor.Visit(securityScheme);
            Walk(securityScheme as IOpenApiExtensible);
        }

        /// <summary>
        /// Visits <see cref="OpenApiSecurityScheme"/> and child objects
        /// </summary>
        internal void Walk(IOpenApiReferenceHolder referenceableHolder)
        {
            _visitor.Visit(referenceableHolder);
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
                case IOpenApiCallback e: Walk(e); break;
                case OpenApiEncoding e: Walk(e); break;
                case IOpenApiExample e: Walk(e); break;
                case IDictionary<string, IOpenApiExample> e: Walk(e); break;
                case OpenApiExternalDocs e: Walk(e); break;
                case OpenApiHeader e: Walk(e); break;
                case OpenApiLink e: Walk(e); break;
                case IDictionary<string, IOpenApiLink> e: Walk(e); break;
                case OpenApiMediaType e: Walk(e); break;
                case OpenApiOAuthFlows e: Walk(e); break;
                case OpenApiOAuthFlow e: Walk(e); break;
                case OpenApiOperation e: Walk(e); break;
                case IOpenApiParameter e: Walk(e); break;
                case OpenApiPaths e: Walk(e); break;
                case OpenApiRequestBody e: Walk(e); break;
                case OpenApiResponse e: Walk(e); break;
                case OpenApiSchema e: Walk(e); break;
                case OpenApiSecurityRequirement e: Walk(e); break;
                case OpenApiSecurityScheme e: Walk(e); break;
                case OpenApiServer e: Walk(e); break;
                case OpenApiServerVariable e: Walk(e); break;
                case OpenApiTag e: Walk(e); break;
                case ISet<OpenApiTag> e: Walk(e); break;
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
            _visitor.Enter(context.Replace("/", "~1"));
            walk();
            _visitor.Exit();
        }

        /// <summary>
        /// Identify if an element is just a reference to a component, or an actual component
        /// </summary>
        private bool ProcessAsReference(IOpenApiReferenceHolder referenceableHolder, bool isComponent = false)
        {
            var isReference = referenceableHolder.Reference != null &&
                              (!isComponent || referenceableHolder.UnresolvedReference);
            if (isReference)
            {
                Walk(referenceableHolder);
            }
            return isReference;
        }
    }

    /// <summary>
    /// Object containing contextual information based on where the walker is currently referencing in an OpenApiDocument
    /// </summary>
    public class CurrentKeys
    {
        /// <summary>
        /// Current Path key
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Current Operation Type
        /// </summary>
        public OperationType? Operation { get; set; }

        /// <summary>
        /// Current Response Status Code
        /// </summary>
        public string Response { get; set; }

        /// <summary>
        /// Current Content Media Type
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Current Callback Key
        /// </summary>
        public string Callback { get; set; }

        /// <summary>
        /// Current Link Key
        /// </summary>
        public string Link { get; set; }

        /// <summary>
        /// Current Header Key
        /// </summary>
        public string Header { get; internal set; }

        /// <summary>
        /// Current Encoding Key
        /// </summary>
        public string Encoding { get; internal set; }

        /// <summary>
        /// Current Example Key
        /// </summary>
        public string Example { get; internal set; }

        /// <summary>
        /// Current Extension Key
        /// </summary>
        public string Extension { get; internal set; }

        /// <summary>
        /// Current ServerVariable
        /// </summary>
        public string ServerVariable { get; internal set; }
    }
}
