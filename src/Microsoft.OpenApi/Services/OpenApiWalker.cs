// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json.Nodes;

namespace Microsoft.OpenApi
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
        public void Walk(OpenApiDocument? doc)
        {
            if (doc == null)
            {
                return;
            }

            _schemaLoop.Clear();
            _pathItemLoop.Clear();

            _visitor.Visit(doc);

            if (doc.Info is { } info)
            {
                WalkItem(OpenApiConstants.Info, doc.Info, Walk);
            }

            if (doc.Servers is { } servers)
            {
                WalkItem(OpenApiConstants.Servers, servers, Walk);
            }

            if (doc.Paths is { } paths)
            {
                WalkItem(OpenApiConstants.Paths, doc.Paths, Walk);
            }
            
            WalkDictionary(OpenApiConstants.Webhooks, doc.Webhooks, Walk);

            if (doc.Components is { } components)
            {
                WalkItem(OpenApiConstants.Components, components, Walk);
            }

            if (doc.Security is { } security)
            {
                WalkItem(OpenApiConstants.Security, security, Walk);
            }

            if (doc.ExternalDocs is { } externalDocs)
            {
                WalkItem(OpenApiConstants.ExternalDocs, externalDocs, Walk);
            }

            if (doc.Tags is { } tags)
            {
                WalkItem(OpenApiConstants.Tags, tags, Walk);
            }

            Walk(doc as IOpenApiExtensible);
        }

        /// <summary>
        /// Visits list of <see cref="OpenApiTag"/> and child objects
        /// </summary>
        internal void Walk(ISet<OpenApiTag>? tags)
        {
            if (tags == null)
            {
                return;
            }

            _visitor.Visit(tags);

            // Visit tags
            if (tags is HashSet<OpenApiTag> { Count: 1 } hashSet && hashSet.First() is { } only)
            {
                WalkItem("0", only, Walk);
            }
            else
            {
                int index = 0;
                foreach (var tag in tags)
                {
                    if (tag is null)
                    {
                        continue;
                    }

                    var context = index.ToString();
                    WalkItem(context, tag, Walk);
                    index++;
                }
            }
        }

        /// <summary>
        /// Visits list of <see cref="OpenApiTagReference"/> and child objects
        /// </summary>
        internal void Walk(ISet<OpenApiTagReference>? tags)
        {
            if (tags == null)
            {
                return;
            }

            _visitor.Visit(tags);

            // Visit tags
            if (tags is HashSet<OpenApiTagReference> { Count: 1 } hashSet && hashSet.First() is { } only)
            {
                WalkItem("0", only, Walk);
            }
            else
            {
                int index = 0;
                foreach (var tag in tags)
                {
                    if (tag is null)
                    {
                        continue;
                    }

                    var context = index.ToString();
                    WalkItem(context, tag, Walk);
                    index++;
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
        internal void Walk(OpenApiExternalDocs? externalDocs)
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
        internal void Walk(OpenApiComponents? components)
        {
            if (components == null)
            {
                return;
            }

            _visitor.Visit(components);

            var isComponent = true;
            WalkDictionary(OpenApiConstants.Schemas, components.Schemas, Walk, isComponent);
            WalkDictionary(OpenApiConstants.SecuritySchemes, components.SecuritySchemes, Walk, isComponent);
            WalkDictionary(OpenApiConstants.Callbacks, components.Callbacks, Walk, isComponent);
            WalkDictionary(OpenApiConstants.PathItems, components.PathItems, Walk, isComponent);
            WalkDictionary(OpenApiConstants.Parameters, components.Parameters, Walk, isComponent);
            WalkDictionary(OpenApiConstants.Examples, components.Examples, Walk, isComponent);
            WalkDictionary(OpenApiConstants.Headers, components.Headers, Walk, isComponent);
            WalkDictionary(OpenApiConstants.Links, components.Links, Walk, isComponent);
            WalkDictionary(OpenApiConstants.RequestBodies, components.RequestBodies, Walk, isComponent);
            WalkDictionary(OpenApiConstants.Responses, components.Responses, Walk, isComponent);

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
            foreach (var pathItem in paths)
            {
                if (pathItem.Value is null)
                {
                    continue;
                }

                _visitor.CurrentKeys.Path = pathItem.Key;
                WalkItem(pathItem.Key, pathItem.Value, Walk, isComponent: false);// JSON Pointer uses ~1 as an escape character for /
                _visitor.CurrentKeys.Path = null;
            }
        }

        /// <summary>
        /// Visits Webhooks and child objects
        /// </summary>
        internal void Walk(IDictionary<string, IOpenApiPathItem>? webhooks)
        {
            if (webhooks == null)
            {
                return;
            }

            _visitor.Visit(webhooks);

            // Visit Webhooks
            foreach (var pathItem in webhooks)
            {
                if (pathItem.Value is null)
                {
                    continue;
                }

                _visitor.CurrentKeys.Path = pathItem.Key;
                WalkItem(pathItem.Key, pathItem.Value, Walk, isComponent: false);// JSON Pointer uses ~1 as an escape character for /
                _visitor.CurrentKeys.Path = null;
            }
        }

        /// <summary>
        /// Visits list of  <see cref="OpenApiServer"/> and child objects
        /// </summary>
        internal void Walk(IList<OpenApiServer>? servers)
        {
            if (servers == null)
            {
                return;
            }

            _visitor.Visit(servers);

            // Visit Servers
            for (var i = 0; i < servers.Count; i++)
            {
                if (servers[i] is not { } server)
                {
                    continue;
                }

                WalkItem(i.ToString(), server, Walk);
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

            if (info.Contact is { } contact)
            {
                WalkItem(OpenApiConstants.Contact, contact, Walk);
            }

            if (info.License is { } license)
            {
                WalkItem(OpenApiConstants.License, license, Walk);
            }

            Walk(info as IOpenApiExtensible);
        }

        /// <summary>
        /// Visits dictionary of extensions
        /// </summary>
        internal void Walk(IOpenApiExtensible? openApiExtensible)
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
                    if (item.Value == null)
                    {
                        continue;
                    }

                    _visitor.CurrentKeys.Extension = item.Key;
                    WalkItem(item.Key, item.Value, Walk);
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
        internal void Walk(OpenApiLicense? license)
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
        internal void Walk(OpenApiContact? contact)
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

            if (callback.PathItems != null)
            {
                foreach (var item in callback.PathItems)
                {
                    if (item.Value is null)
                    {
                        continue;
                    }

                    var context = item.Key.ToString();

                    _visitor.CurrentKeys.Callback = context;
                    WalkItem(context, item.Value, Walk);
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
            if (tag.ExternalDocs is { } externalDocs)
            {
                _visitor.Visit(externalDocs);
            }
            _visitor.Visit(tag as IOpenApiExtensible);
        }

        /// <summary>
        /// Visits <see cref="OpenApiTagReference"/> and child objects
        /// </summary>
        internal void Walk(OpenApiTagReference tag)
        {
            if (tag is IOpenApiReferenceHolder openApiReferenceHolder)
            {
                Walk(openApiReferenceHolder);
            }
        }

        /// <summary>
        /// Visits <see cref="OpenApiServer"/> and child objects
        /// </summary>
        internal void Walk(OpenApiServer? server)
        {
            if (server == null)
            {
                return;
            }

            _visitor.Visit(server);

            if (server.Variables is { } variables)
            {
                WalkItem(OpenApiConstants.Variables, variables, Walk);
            }

            _visitor.Visit(server as IOpenApiExtensible);
        }

        /// <summary>
        /// Visits dictionary of <see cref="OpenApiServerVariable"/>
        /// </summary>
        internal void Walk(IDictionary<string, OpenApiServerVariable>? serverVariables)
        {
            if (serverVariables == null)
            {
                return;
            }

            _visitor.Visit(serverVariables);

            foreach (var variable in serverVariables)
            {
                if (variable.Value == null)
                {
                    continue;
                }

                _visitor.CurrentKeys.ServerVariable = variable.Key;
                WalkItem(variable.Key, variable.Value, Walk);
                _visitor.CurrentKeys.ServerVariable = null;
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
                if (pathItem.Parameters is { } parameters)
                {
                    WalkItem(OpenApiConstants.Parameters, parameters, Walk);
                }

                Walk(pathItem.Operations);
            }

            if (pathItem is IOpenApiExtensible extensiblePathItem)
            {
                _visitor.Visit(extensiblePathItem);
            }
            _pathItemLoop.Pop();
         }

        /// <summary>
        /// Visits dictionary of <see cref="OpenApiOperation"/>
        /// </summary>
        internal void Walk(IDictionary<HttpMethod, OpenApiOperation>? operations)
        {
            if (operations == null)
            {
                return;
            }

            _visitor.Visit(operations);

            foreach (var operation in operations)
            {
                if (operation.Value is null)
                {
                    continue;
                }

                _visitor.CurrentKeys.Operation = operation.Key;
                WalkItem(operation.Key.Method.ToLowerInvariant(), operation.Value, Walk);
                _visitor.CurrentKeys.Operation = null;
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

            if (operation.Parameters is { } parameters)
            {
                WalkItem(OpenApiConstants.Parameters, parameters, Walk);
            }

            Walk(OpenApiConstants.RequestBody, () => Walk(operation.RequestBody));

            if (operation.Responses is { } responses)
            {
                WalkItem(OpenApiConstants.Responses, responses, Walk);
            }

            WalkDictionary(OpenApiConstants.Callbacks, operation.Callbacks, Walk);

            if (operation.Tags is { } tags)
            {
                WalkItem(OpenApiConstants.Tags, tags, Walk);
            }

            if (operation.Security is { } security)
            {
                WalkItem(OpenApiConstants.Security, security, Walk);
            }

            Walk(operation as IOpenApiExtensible);
        }

        /// <summary>
        /// Visits list of <see cref="OpenApiSecurityRequirement"/>
        /// </summary>
        internal void Walk(IList<OpenApiSecurityRequirement>? securityRequirements)
        {
            if (securityRequirements == null)
            {
                return;
            }

            _visitor.Visit(securityRequirements);

            for (var i = 0; i < securityRequirements.Count; i++)
            {
                if (securityRequirements[i] is not { } requirement)
                {
                    continue;
                }

                WalkItem(i.ToString(), requirement, Walk);
            }
        }

        /// <summary>
        /// Visits list of <see cref="OpenApiParameter"/>
        /// </summary>
        internal void Walk(IList<IOpenApiParameter>? parameters)
        {
            if (parameters == null)
            {
                return;
            }

            _visitor.Visit(parameters);

            for (var i = 0; i < parameters.Count; i++)
            {
                if (parameters[i] is not { } parameter)
                {
                    continue;
                }

                WalkItem(i.ToString(), parameter, Walk);
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

            if (parameter.Schema is { } schema)
            {
                WalkItem(OpenApiConstants.Schema, schema, Walk, isComponent: false);
            }

            if (parameter.Content is { } content)
            {
                WalkItem(OpenApiConstants.Content, content, Walk);
            }

            WalkDictionary(OpenApiConstants.Examples, parameter.Examples, Walk);

            Walk(parameter as IOpenApiExtensible);
        }

        /// <summary>
        /// Visits <see cref="OpenApiResponses"/> and child objects
        /// </summary>
        internal void Walk(OpenApiResponses? responses)
        {
            if (responses == null)
            {
                return;
            }

            _visitor.Visit(responses);

            foreach (var response in responses)
            {
                if (response.Value is null)
                {
                    continue;
                }

                _visitor.CurrentKeys.Response = response.Key;
                WalkItem(response.Key, response.Value, Walk, isComponent: false);
                _visitor.CurrentKeys.Response = null;
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

            if (response.Content is { } content)
            {
                WalkItem(OpenApiConstants.Content, content, Walk);
            }

            WalkDictionary(OpenApiConstants.Links, response.Links, Walk);
            WalkDictionary(OpenApiConstants.Headers, response.Headers, Walk);
            Walk(response as IOpenApiExtensible);
        }

        /// <summary>
        /// Visits <see cref="IOpenApiRequestBody"/> and child objects
        /// </summary>
        internal void Walk(IOpenApiRequestBody? requestBody, bool isComponent = false)
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

            if (requestBody.Content is { } content)
            {
                WalkItem(OpenApiConstants.Content, content, Walk);
            }

            Walk(requestBody as IOpenApiExtensible);
        }

        /// <summary>
        /// Visits dictionary of <see cref="OpenApiHeader"/>
        /// </summary>
        internal void Walk(IDictionary<string, IOpenApiHeader>? headers)
        {
            if (headers == null)
            {
                return;
            }

            _visitor.Visit(headers);

            foreach (var header in headers)
            {
                if (header.Value is null)
                {
                    continue;
                }

                _visitor.CurrentKeys.Header = header.Key;
                WalkItem(header.Key, header.Value, Walk, isComponent: false);
                _visitor.CurrentKeys.Header = null;
            }
        }

        /// <summary>
        /// Visits dictionary of <see cref="IOpenApiCallback"/>
        /// </summary>
        internal void Walk(IDictionary<string, IOpenApiCallback>? callbacks)
        {
            if (callbacks == null)
            {
                return;
            }

            _visitor.Visit(callbacks);

            foreach (var callback in callbacks)
            {
                if (callback.Value is null)
                {
                    continue;
                }

                _visitor.CurrentKeys.Callback = callback.Key;
                WalkItem(callback.Key, callback.Value, Walk, isComponent: false);
                _visitor.CurrentKeys.Callback = null;
            }
        }

        /// <summary>
        /// Visits dictionary of <see cref="OpenApiMediaType"/>
        /// </summary>
        internal void Walk(IDictionary<string, OpenApiMediaType>? content)
        {
            if (content == null)
            {
                return;
            }

            _visitor.Visit(content);

            foreach (var mediaType in content)
            {
                if (mediaType.Value is null)
                {
                    continue;
                }

                _visitor.CurrentKeys.Content = mediaType.Key;
                WalkItem(mediaType.Key, mediaType.Value, Walk);
                _visitor.CurrentKeys.Content = null;
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

            WalkDictionary(OpenApiConstants.Example, mediaType.Examples, Walk);

            if (mediaType.Schema is { } schema)
            {
                WalkItem(OpenApiConstants.Schema, schema, Walk, isComponent: false);
            }

            if (mediaType.Encoding is { } encoding)
            {
                WalkItem(OpenApiConstants.Encoding, encoding, Walk);
            }

            Walk(mediaType as IOpenApiExtensible);
        }

        /// <summary>
        /// Visits dictionary of <see cref="OpenApiEncoding"/>
        /// </summary>
        internal void Walk(IDictionary<string, OpenApiEncoding>? encodings)
        {
            if (encodings == null)
            {
                return;
            }

            _visitor.Visit(encodings);

            foreach (var item in encodings)
            {
                if (item.Value is null)
                {
                    continue;
                }

                _visitor.CurrentKeys.Encoding = item.Key;
                WalkItem(item.Key, item.Value, Walk);
                _visitor.CurrentKeys.Encoding = null;
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

            if (encoding.Headers is { } headers)
            {
                Walk(headers);
            }

            Walk(encoding as IOpenApiExtensible);
        }

        /// <summary>
        /// Visits <see cref="IOpenApiSchema"/> and child objects
        /// </summary>
        internal void Walk(IOpenApiSchema? schema, bool isComponent = false)
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

            if (schema.Items is { } items)
            {
                WalkItem("items", items, Walk, isComponent: false);
            }

            if (schema.Not is { } not)
            {
                WalkItem("not", not, Walk, isComponent: false);
            }

            if (schema.AllOf is { Count: > 0 } allOf)
            {
                WalkItem("allOf", allOf, Walk);
            }

            if (schema.AnyOf is { Count: > 0 } anyOf)
            {
                WalkItem("anyOf", anyOf, Walk);
            }

            if (schema.OneOf is { Count: > 0 } oneOf)
            {
                WalkItem("oneOf", oneOf, Walk);
            }

            if (schema.Properties is { } properties)
            {
                WalkDictionary("properties", properties, Walk);
            }

            if (schema.AdditionalProperties is { } additionalProperties)
            {
                WalkItem("additionalProperties", additionalProperties, Walk, isComponent: false);
            }

            if (schema.Discriminator is { } discriminator)
            {
                WalkItem("discriminator", discriminator, Walk);
            }

            if (schema.ExternalDocs is { } externalDocs)
            {
                WalkItem(OpenApiConstants.ExternalDocs, externalDocs, Walk);
            }

            Walk(schema as IOpenApiExtensible);

            _schemaLoop.Pop();
        }

        internal void Walk(OpenApiDiscriminator? openApiDiscriminator)
        {
            if (openApiDiscriminator == null)
            {
                return;
            }

            _visitor.Visit(openApiDiscriminator);

            if (openApiDiscriminator.Mapping is { Count: > 0 } mapping)
            {
                WalkDictionary("mapping", mapping, (item, _) => Walk((IOpenApiSchema)item));
            }
        }

        /// <summary>
        /// Visits dictionary of <see cref="IOpenApiExample"/>
        /// </summary>
        internal void Walk(IDictionary<string, IOpenApiExample>? examples)
        {
            if (examples == null)
            {
                return;
            }

            _visitor.Visit(examples);

            foreach (var example in examples)
            {
                if (example.Value is null)
                {
                    continue;
                }

                _visitor.CurrentKeys.Example = example.Key;
                WalkItem(example.Key, example.Value, Walk, isComponent: false);
                _visitor.CurrentKeys.Example = null;
            }
        }

        /// <summary>
        /// Visits <see cref="JsonNodeExtension"/> and child objects
        /// </summary>
        internal void Walk(JsonNode? example)
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
        internal void Walk(List<IOpenApiExample> examples)
        {
            if (examples == null)
            {
                return;
            }

            _visitor.Visit(examples);

            // Visit Examples
            for (var i = 0; i < examples.Count; i++)
            {
                if (examples[i] is not { } example)
                {
                    continue;
                }

                WalkItem(i.ToString(), example, Walk, isComponent: false);
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
            for (var i = 0; i < schemas.Count; i++)
            {
                if (schemas[i] is not { } schema)
                {
                    continue;
                }

                WalkItem(i.ToString(), schema, Walk, isComponent: false);
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
        internal void Walk(IDictionary<string, IOpenApiLink>? links)
        {
            if (links == null)
            {
                return;
            }

            _visitor.Visit(links);

            foreach (var item in links)
            {
                if (item.Value is null)
                {
                    continue;
                }

                _visitor.CurrentKeys.Link = item.Key;
                WalkItem(item.Key, item.Value, Walk, isComponent: false);
                _visitor.CurrentKeys.Link = null;
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

            if (link.Server is { } server)
            {
                WalkItem(OpenApiConstants.Server, server, Walk);
            }

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

            if (header.Content is { } content)
            {
                WalkItem(OpenApiConstants.Content, content, Walk);
            }

            if (header.Example is { } example)
            {
                WalkItem(OpenApiConstants.Example, example, Walk);
            }

            WalkDictionary(OpenApiConstants.Examples, header.Examples, Walk);

            if (header.Schema is { } schema)
            {
                WalkItem(OpenApiConstants.Schema, schema, Walk, isComponent: false);
            }

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
                case Dictionary<string, IOpenApiExample> e: Walk(e); break;
                case OpenApiExternalDocs e: Walk(e); break;
                case OpenApiHeader e: Walk(e); break;
                case OpenApiLink e: Walk(e); break;
                case Dictionary<string, IOpenApiLink> e: Walk(e); break;
                case OpenApiMediaType e: Walk(e); break;
                case OpenApiOAuthFlows e: Walk(e); break;
                case OpenApiOAuthFlow e: Walk(e); break;
                case OpenApiOperation e: Walk(e); break;
                case IOpenApiParameter e: Walk(e); break;
                case OpenApiPaths e: Walk(e); break;
                case OpenApiRequestBody e: Walk(e); break;
                case OpenApiResponse e: Walk(e); break;
                case OpenApiSchema e: Walk(e); break;
                case OpenApiDiscriminator e: Walk(e); break;
                case OpenApiSecurityRequirement e: Walk(e); break;
                case OpenApiSecurityScheme e: Walk(e); break;
                case OpenApiServer e: Walk(e); break;
                case OpenApiServerVariable e: Walk(e); break;
                case OpenApiTag e: Walk(e); break;
                case HashSet<OpenApiTag> e: Walk(e); break;
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
        /// Adds a segment to the context path to enable pointing to the current location in the document
        /// </summary>
        /// <typeparam name="T">The type of the state.</typeparam>
        /// <param name="context">An identifier for the context.</param>
        /// <param name="state">The state to pass to the walk action.</param>
        /// <param name="walk">An action that walks objects within the context.</param>
        private void WalkItem<T>(string context, T state, Action<T> walk)
        {
            _visitor.Enter(context.Replace("/", "~1"));
            walk(state);
            _visitor.Exit();
        }

        /// <summary>
        /// Adds a segment to the context path to enable pointing to the current location in the document
        /// </summary>
        /// <typeparam name="T">The type of the state.</typeparam>
        /// <param name="context">An identifier for the context.</param>
        /// <param name="state">The state to pass to the walk action.</param>
        /// <param name="isComponent">Whether the state is a component.</param>
        /// <param name="walk">An action that walks objects within the context.</param>
        private void WalkItem<T>(string context, T state, Action<T, bool> walk, bool isComponent)
        {
            _visitor.Enter(context.Replace("/", "~1"));
            walk(state, isComponent);
            _visitor.Exit();
        }

        /// <summary>
        /// Adds a segment to the context path to enable pointing to the current location in the document
        /// </summary>
        /// <typeparam name="T">The type of the state.</typeparam>
        /// <param name="context">An identifier for the context.</param>
        /// <param name="state">The state to pass to the walk action.</param>
        /// <param name="isComponent">Whether the state is a component.</param>
        /// <param name="walk">An action that walks objects within the context.</param>
        private void WalkDictionary<T>(
            string context,
            IDictionary<string, T>? state,
            Action<T, bool> walk,
            bool isComponent = false)
        {
            if (state != null && state.Count > 0)
            {
                _visitor.Enter(context.Replace("/", "~1"));

                foreach (var item in state)
                {
                    WalkItem(item.Key, (item.Value, isComponent), (state) => walk(state.Value, state.isComponent));
                }

                _visitor.Exit();
            }
        }

        /// <summary>
        /// Identify if an element is just a reference to a component, or an actual component
        /// </summary>
        private bool ProcessAsReference(IOpenApiReferenceHolder referenceableHolder, bool isComponent = false)
        {
            var isReference = !isComponent || referenceableHolder.UnresolvedReference;
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
        public string? Path { get; internal set; }

        /// <summary>
        /// Current Operation Type
        /// </summary>
        public HttpMethod? Operation { get; internal set; }

        /// <summary>
        /// Current Response Status Code
        /// </summary>
        public string? Response { get; internal set; }

        /// <summary>
        /// Current Content Media Type
        /// </summary>
        public string? Content { get; internal set; }

        /// <summary>
        /// Current Callback Key
        /// </summary>
        public string? Callback { get; internal set; }

        /// <summary>
        /// Current Link Key
        /// </summary>
        public string? Link { get; internal set; }

        /// <summary>
        /// Current Header Key
        /// </summary>
        public string? Header { get; internal set; }

        /// <summary>
        /// Current Encoding Key
        /// </summary>
        public string? Encoding { get; internal set; }

        /// <summary>
        /// Current Example Key
        /// </summary>
        public string? Example { get; internal set; }

        /// <summary>
        /// Current Extension Key
        /// </summary>
        public string? Extension { get; internal set; }

        /// <summary>
        /// Current ServerVariable
        /// </summary>
        public string? ServerVariable { get; internal set; }
    }
}
