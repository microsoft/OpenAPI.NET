// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.OpenApi.Reader;

namespace Microsoft.OpenApi
{
    /// <summary>
    /// Contains a set of OpenApi documents and document fragments that reference each other
    /// </summary>
    public class OpenApiWorkspace
    {
        private readonly Dictionary<string, Uri> _documentsIdRegistry = new();
        private readonly Dictionary<Uri, Stream> _artifactsRegistry = new();        
        private readonly Dictionary<Uri, IOpenApiReferenceable> _IOpenApiReferenceableRegistry = new(new UriWithFragmentEqualityComparer());
        private Dictionary<OpenApiDocument, Dictionary<string, List<IOpenApiSchema>>>? _dynamicAnchorRegistryByDocument;
        private Dictionary<OpenApiDocument, Dictionary<string, List<IOpenApiSchema>>>? _anchorRegistryByDocument;

        private sealed class UriWithFragmentEqualityComparer : IEqualityComparer<Uri>
        {
            public bool Equals(Uri? x, Uri? y)
            {
                if (ReferenceEquals(x, y))
                {
                    return true;
                }

                if (x is null || y is null)
                {
                    return false;
                }

                return x.AbsoluteUri == y.AbsoluteUri;
            }

            public int GetHashCode(Uri obj)
            {
                return obj.AbsoluteUri.GetHashCode();
            }
        }

        /// <summary>
        /// The base location from where all relative references are resolved
        /// </summary>
        public Uri? BaseUrl { get; }
       
        /// <summary>
        /// Initialize workspace pointing to a base URL to allow resolving relative document locations.  Use a file:// url to point to a folder
        /// </summary>
        /// <param name="baseUrl"></param>
        public OpenApiWorkspace(Uri baseUrl)
        {
            BaseUrl = baseUrl;
        }

        /// <summary>
        /// Initialize workspace using current directory as the default location.
        /// </summary>
        public OpenApiWorkspace()
        {
            BaseUrl = new Uri(OpenApiConstants.BaseRegistryUri);
        }

        /// <summary>
        /// Initializes a copy of an <see cref="OpenApiWorkspace"/> object
        /// </summary>
        public OpenApiWorkspace(OpenApiWorkspace workspace) { }

        /// <summary>
        /// Returns the total count of all the components in the workspace registry
        /// </summary>
        /// <returns></returns>
        public int ComponentsCount()
        {
            return _IOpenApiReferenceableRegistry.Count + _artifactsRegistry.Count;
        }
        private const string ComponentSegmentSeparator = "/";

        /// <summary>
        /// Registers a document's components into the workspace
        /// </summary>
        /// <param name="document"></param>
        public void RegisterComponents(OpenApiDocument document)
        {
            if (document?.Components == null) return;

            string baseUri = getBaseUri(document);
            string location;

            // Register Schema
            if (document.Components.Schemas != null)
            {
                foreach (var item in document.Components.Schemas)
                {
                    if (item.Value == null) continue;
                    location = baseUri + ReferenceType.Schema.GetDisplayName() + ComponentSegmentSeparator + item.Key;
                    RegisterComponent(location, item.Value);

                    RegisterSchemaIdentifiers(document, item.Value, document.BaseUri);
                }
            }

            // Register Parameters
            if (document.Components.Parameters != null)
            {
                foreach (var item in document.Components.Parameters)
                {
                    if (item.Value == null) continue;
                    location = baseUri + ReferenceType.Parameter.GetDisplayName() + ComponentSegmentSeparator + item.Key;
                    RegisterComponent(location, item.Value);
                }
            }

            // Register Responses
            if (document.Components.Responses != null)
            {
                foreach (var item in document.Components.Responses)
                {
                    if (item.Value == null) continue;
                    location = baseUri + ReferenceType.Response.GetDisplayName() + ComponentSegmentSeparator + item.Key;
                    RegisterComponent(location, item.Value);
                }
            }

            // Register RequestBodies
            if (document.Components.RequestBodies != null)
            {
                foreach (var item in document.Components.RequestBodies)
                {
                    if (item.Value == null) continue;
                    location = baseUri + ReferenceType.RequestBody.GetDisplayName() + ComponentSegmentSeparator + item.Key;
                    RegisterComponent(location, item.Value);
                }
            }

            // Register Links
            if (document.Components.Links != null)
            {
                foreach (var item in document.Components.Links)
                {
                    if (item.Value == null) continue;
                    location = baseUri + ReferenceType.Link.GetDisplayName() + ComponentSegmentSeparator + item.Key;
                    RegisterComponent(location, item.Value);
                }
            }

            // Register Callbacks
            if (document.Components.Callbacks != null)
            {
                foreach (var item in document.Components.Callbacks)
                {
                    if (item.Value == null) continue;
                    location = baseUri + ReferenceType.Callback.GetDisplayName() + ComponentSegmentSeparator + item.Key;
                    RegisterComponent(location, item.Value);
                }
            }

            // Register PathItems
            if (document.Components.PathItems != null)
            {
                foreach (var item in document.Components.PathItems)
                {
                    if (item.Value == null) continue;
                    location = baseUri + ReferenceType.PathItem.GetDisplayName() + ComponentSegmentSeparator + item.Key;
                    RegisterComponent(location, item.Value);
                }
            }

            // Register Examples
            if (document.Components.Examples != null)
            {
                foreach (var item in document.Components.Examples)
                {
                    if (item.Value == null) continue;
                    location = baseUri + ReferenceType.Example.GetDisplayName() + ComponentSegmentSeparator + item.Key;
                    RegisterComponent(location, item.Value);
                }
            }

            // Register Headers
            if (document.Components.Headers != null)
            {
                foreach (var item in document.Components.Headers)
                {
                    if (item.Value == null) continue;
                    location = baseUri + ReferenceType.Header.GetDisplayName() + ComponentSegmentSeparator + item.Key;
                    RegisterComponent(location, item.Value);
                }
            }

            // Register SecuritySchemes
            if (document.Components.SecuritySchemes != null)
            {
                foreach (var item in document.Components.SecuritySchemes)
                {
                    if (item.Value == null) continue;
                    location = baseUri + ReferenceType.SecurityScheme.GetDisplayName() + ComponentSegmentSeparator + item.Key;
                    RegisterComponent(location, item.Value);
                }
            }

            // Register MediaTypes
            if (document.Components.MediaTypes != null)
            {
                foreach (var item in document.Components.MediaTypes)
                {
                    if (item.Value == null) continue;
                    location = baseUri + ReferenceType.MediaType.GetDisplayName() + ComponentSegmentSeparator + item.Key;
                    RegisterComponent(location, item.Value);
                }
            }

            RegisterComponentAnchors(document);
            RegisterInlineAnchors(document);
        }

        /// <summary>
        /// Registers $dynamicAnchor and $anchor declarations from schemas nested inside reusable
        /// components (parameters, responses, requestBodies, headers, callbacks, pathItems, mediaTypes).
        /// Uses the same per-element helpers as <see cref="RegisterInlineAnchors"/> so anchors are
        /// discovered regardless of whether their containing definition is inline or reusable.
        /// </summary>
        private void RegisterComponentAnchors(OpenApiDocument document)
        {
            var components = document.Components;
            if (components is null) return;

            if (components.Parameters is not null)
                foreach (var parameter in components.Parameters.Values.Where(p => p is not null))
                    RegisterParameterAnchors(document, parameter);

            if (components.Responses is not null)
                foreach (var response in components.Responses.Values.Where(r => r is not null))
                    RegisterResponseAnchors(document, response);

            if (components.RequestBodies is not null)
                foreach (var requestBody in components.RequestBodies.Values.Where(r => r is not null))
                    RegisterRequestBodyAnchors(document, requestBody);

            if (components.Headers is not null)
                foreach (var header in components.Headers.Values.Where(h => h is not null))
                    RegisterHeaderAnchors(document, header);

            if (components.Callbacks is not null)
                foreach (var callback in components.Callbacks.Values.Where(c => c is not null))
                    RegisterCallbackAnchors(document, callback, new());

            if (components.PathItems is not null)
                foreach (var pathItem in components.PathItems.Values.Where(p => p is not null))
                    RegisterPathItemAnchors(document, pathItem, new());

            RegisterMediaTypeSchemas(document, components.MediaTypes);
        }

        /// <summary>
        /// Registers $dynamicAnchor and $anchor declarations from inline (non-component) schemas:
        /// paths and webhooks.
        /// </summary>
        private void RegisterInlineAnchors(OpenApiDocument document)
        {
            if (document.Paths is not null)
                foreach (var pathItem in document.Paths.Values.Where(p => p is not null))
                    RegisterPathItemAnchors(document, pathItem, new());

            if (document.Webhooks is not null)
                foreach (var pathItem in document.Webhooks.Values.Where(p => p is not null))
                    RegisterPathItemAnchors(document, pathItem, new());
        }

        // The structural walk (pathItem -> operation -> callback -> pathItem) can cycle through
        // self- or mutually-referential callbacks, so each of these guards against re-entry via the
        // shared `visited` set, mirroring RegisterAnchorsRecursive's schema-cycle guard.
        private void RegisterPathItemAnchors(OpenApiDocument document, IOpenApiPathItem pathItem, HashSet<object> visited)
        {
            if (pathItem is OpenApiPathItemReference) return;
            if (!visited.Add(pathItem)) return;

            if (pathItem.Parameters is not null)
                foreach (var parameter in pathItem.Parameters.Where(p => p is not null))
                    RegisterParameterAnchors(document, parameter);

            if (pathItem.Operations is not null)
                foreach (var op in pathItem.Operations.Values.Where(o => o is not null))
                    RegisterOperationAnchors(document, op, visited);
        }

        private void RegisterCallbackAnchors(OpenApiDocument document, IOpenApiCallback callback, HashSet<object> visited)
        {
            if (callback is OpenApiCallbackReference) return;
            if (!visited.Add(callback)) return;

            if (callback.PathItems is not null)
                foreach (var pathItem in callback.PathItems.Values.Where(p => p is not null))
                    RegisterPathItemAnchors(document, pathItem, visited);
        }

        private void RegisterOperationAnchors(OpenApiDocument document, OpenApiOperation op, HashSet<object> visited)
        {
            if (!visited.Add(op)) return;

            if (op.Parameters is not null)
                foreach (var parameter in op.Parameters.Where(p => p is not null))
                    RegisterParameterAnchors(document, parameter);

            if (op.RequestBody is not null)
                RegisterRequestBodyAnchors(document, op.RequestBody);

            if (op.Responses is not null)
                foreach (var response in op.Responses.Values.Where(r => r is not null))
                    RegisterResponseAnchors(document, response);

            if (op.Callbacks is not null)
                foreach (var callback in op.Callbacks.Values.Where(c => c is not null))
                    RegisterCallbackAnchors(document, callback, visited);
        }

        private void RegisterParameterAnchors(OpenApiDocument document, IOpenApiParameter parameter)
        {
            if (parameter is OpenApiParameterReference) return;
            if (parameter.Schema is not null)
                RegisterAnchors(document, parameter.Schema);
            RegisterMediaTypeSchemas(document, parameter.Content);
        }

        private void RegisterRequestBodyAnchors(OpenApiDocument document, IOpenApiRequestBody requestBody)
        {
            if (requestBody is OpenApiRequestBodyReference) return;
            RegisterMediaTypeSchemas(document, requestBody.Content);
        }

        private void RegisterResponseAnchors(OpenApiDocument document, IOpenApiResponse response)
        {
            if (response is OpenApiResponseReference) return;
            RegisterMediaTypeSchemas(document, response.Content);
            if (response.Headers is not null)
                foreach (var header in response.Headers.Values.Where(h => h is not null))
                    RegisterHeaderAnchors(document, header);
        }

        private void RegisterHeaderAnchors(OpenApiDocument document, IOpenApiHeader header)
        {
            if (header is OpenApiHeaderReference) return;
            if (header.Schema is not null)
                RegisterAnchors(document, header.Schema);
            RegisterMediaTypeSchemas(document, header.Content);
        }

        private void RegisterMediaTypeSchemas(OpenApiDocument document, IDictionary<string, IOpenApiMediaType>? content)
        {
            if (content is null) return;
            foreach (var mediaType in content.Values.Where(m => m is not null and not OpenApiMediaTypeReference))
            {
                if (mediaType.Schema is not null)
                    RegisterAnchors(document, mediaType.Schema);
                if (mediaType.ItemSchema is not null)
                    RegisterAnchors(document, mediaType.ItemSchema);
            }
        }

        private static string getBaseUri(OpenApiDocument openApiDocument)
        {
            return openApiDocument.BaseUri + "#" + OpenApiConstants.ComponentsSegment;
        }

        /// <summary>
        /// Registers a component for a document in the workspace
        /// </summary>
        /// <param name="openApiDocument">The document to register the component for.</param>
        /// <param name="componentToRegister">The component to register.</param>
        /// <param name="id">The id of the component.</param>
        /// <typeparam name="T">The type of the component to register.</typeparam>
        /// <returns>true if the component is successfully registered; otherwise false.</returns>
        /// <exception cref="ArgumentNullException">openApiDocument is null</exception>
        /// <exception cref="ArgumentNullException">componentToRegister is null</exception>
        /// <exception cref="ArgumentNullException">id is null or empty</exception>
        public bool RegisterComponentForDocument<T>(OpenApiDocument openApiDocument, T componentToRegister, string id)
        {
            Utils.CheckArgumentNull(openApiDocument);
            Utils.CheckArgumentNull(componentToRegister);
            Utils.CheckArgumentNullOrEmpty(id);

            var baseUri = getBaseUri(openApiDocument);

            var location = componentToRegister switch
            {
                IOpenApiSchema => baseUri + ReferenceType.Schema.GetDisplayName() + ComponentSegmentSeparator + id,
                IOpenApiParameter => baseUri + ReferenceType.Parameter.GetDisplayName() + ComponentSegmentSeparator + id,
                IOpenApiResponse => baseUri + ReferenceType.Response.GetDisplayName() + ComponentSegmentSeparator + id,
                IOpenApiRequestBody => baseUri + ReferenceType.RequestBody.GetDisplayName() + ComponentSegmentSeparator + id,
                IOpenApiLink => baseUri + ReferenceType.Link.GetDisplayName() + ComponentSegmentSeparator + id,
                IOpenApiCallback => baseUri + ReferenceType.Callback.GetDisplayName() + ComponentSegmentSeparator + id,
                IOpenApiPathItem => baseUri + ReferenceType.PathItem.GetDisplayName() + ComponentSegmentSeparator + id,
                IOpenApiExample => baseUri + ReferenceType.Example.GetDisplayName() + ComponentSegmentSeparator + id,
                IOpenApiHeader => baseUri + ReferenceType.Header.GetDisplayName() + ComponentSegmentSeparator + id,
                IOpenApiSecurityScheme => baseUri + ReferenceType.SecurityScheme.GetDisplayName() + ComponentSegmentSeparator + id,
                IOpenApiMediaType => baseUri + ReferenceType.MediaType.GetDisplayName() + ComponentSegmentSeparator + id,
                _ => throw new ArgumentException($"Invalid component type {componentToRegister!.GetType().Name}"),
            };

            return RegisterComponent(location, componentToRegister);
        }

        /// <summary>
        /// Registers a component in the component registry.
        /// </summary>
        /// <param name="location"></param>
        /// <param name="component"></param>
        /// <returns>true if the component is successfully registered; otherwise false.</returns>
        internal bool RegisterComponent<T>(string location, T component)
        {
            var uri = ToLocationUrl(location);
            if (uri is not null)
            {
                if (component is IOpenApiReferenceable referenceable)
                {
                    if (!_IOpenApiReferenceableRegistry.ContainsKey(uri))
                    {
                        _IOpenApiReferenceableRegistry[uri] = referenceable;
                        return true;
                    }
                }
                else if (component is Stream stream && !_artifactsRegistry.ContainsKey(uri))
                {
                    _artifactsRegistry[uri] = stream;
                    return true;
                }
                return false;
            }            

            return false;
        }

        private void RegisterSchemaIdentifiers(OpenApiDocument document, IOpenApiSchema schema, Uri baseUri)
        {
            RegisterSchemaIdentifiers(document, schema, baseUri, new HashSet<IOpenApiSchema>());
        }

        private void RegisterSchemaIdentifiers(OpenApiDocument document, IOpenApiSchema schema, Uri baseUri, ISet<IOpenApiSchema> visitedSchemas)
        {
            if (schema is OpenApiSchemaReference osr)
            {
                if (osr.Reference.DynamicAnchor is string dynAnchor && dynAnchor.Length > 0)
                    RegisterAnchor(document, dynAnchor, schema, isDynamic: true);
                if (osr.Reference.Anchor is string plainAnchor && plainAnchor.Length > 0)
                    RegisterAnchor(document, plainAnchor, schema, isDynamic: false);
                foreach (var child in EnumerateChildren(osr.Reference))
                    RegisterSchemaIdentifiers(document, child, baseUri, visitedSchemas);
                return;
            }

            if (!visitedSchemas.Add(schema))
            {
                return;
            }

            var schemaBaseUri = baseUri;
            if (!string.IsNullOrEmpty(schema.Id) &&
                Uri.TryCreate(schema.Id, UriKind.RelativeOrAbsolute, out var schemaIdUri))
            {
                schemaBaseUri = schemaIdUri.IsAbsoluteUri ? schemaIdUri : new Uri(baseUri, schemaIdUri);
                RegisterComponent(schemaBaseUri.AbsoluteUri, schema);
            }

            if (schema is IOpenApiSchemaMissingProperties { Anchor.Length: > 0 } schemaWithAnchor)
            {
                var anchorUriBuilder = new UriBuilder(schemaBaseUri)
                {
                    Fragment = schemaWithAnchor.Anchor
                };
                RegisterComponent(anchorUriBuilder.Uri.AbsoluteUri, schema);
            }

            if (schema.DynamicAnchor is string dynAnchorName && dynAnchorName.Length > 0)
                RegisterAnchor(document, dynAnchorName, schema, isDynamic: true);
            if (schema is IOpenApiSchemaMissingProperties { Anchor.Length: > 0 } schemaWithPlainAnchor)
                RegisterAnchor(document, schemaWithPlainAnchor.Anchor, schema, isDynamic: false);

            RegisterSchemaIdentifier(document, schema.Items, schemaBaseUri, visitedSchemas);
            RegisterSchemaIdentifier(document, schema.AdditionalProperties, schemaBaseUri, visitedSchemas);
            RegisterSchemaIdentifier(document, schema.Not, schemaBaseUri, visitedSchemas);

            RegisterSchemaIdentifiers(document, schema.Properties?.Values, schemaBaseUri, visitedSchemas);
            RegisterSchemaIdentifiers(document, schema.PatternProperties?.Values, schemaBaseUri, visitedSchemas);
            RegisterSchemaIdentifiers(document, schema.Definitions?.Values, schemaBaseUri, visitedSchemas);
            RegisterSchemaIdentifiers(document, schema.AllOf, schemaBaseUri, visitedSchemas);
            RegisterSchemaIdentifiers(document, schema.AnyOf, schemaBaseUri, visitedSchemas);
            RegisterSchemaIdentifiers(document, schema.OneOf, schemaBaseUri, visitedSchemas);

            if (schema is IOpenApiSchemaMissingProperties missingProperties)
            {
                RegisterSchemaIdentifier(document, missingProperties.Contains, schemaBaseUri, visitedSchemas);
                RegisterSchemaIdentifier(document, missingProperties.PropertyNames, schemaBaseUri, visitedSchemas);
                RegisterSchemaIdentifier(document, missingProperties.UnevaluatedPropertiesSchema, schemaBaseUri, visitedSchemas);
                RegisterSchemaIdentifier(document, missingProperties.ContentSchema, schemaBaseUri, visitedSchemas);
                RegisterSchemaIdentifier(document, missingProperties.If, schemaBaseUri, visitedSchemas);
                RegisterSchemaIdentifier(document, missingProperties.Then, schemaBaseUri, visitedSchemas);
                RegisterSchemaIdentifier(document, missingProperties.Else, schemaBaseUri, visitedSchemas);
                RegisterSchemaIdentifiers(document, missingProperties.DependentSchemas?.Values, schemaBaseUri, visitedSchemas);
            }
        }

        private void RegisterSchemaIdentifiers(OpenApiDocument document, IEnumerable<IOpenApiSchema>? schemas, Uri baseUri, ISet<IOpenApiSchema> visitedSchemas)
        {
            if (schemas is null)
            {
                return;
            }

            foreach (var schema in schemas)
            {
                RegisterSchemaIdentifiers(document, schema, baseUri, visitedSchemas);
            }
        }

        private void RegisterSchemaIdentifier(OpenApiDocument document, IOpenApiSchema? schema, Uri baseUri, ISet<IOpenApiSchema> visitedSchemas)
        {
            if (schema is not null)
            {
                RegisterSchemaIdentifiers(document, schema, baseUri, visitedSchemas);
            }
        }

        /// <summary>
        /// Registers all $dynamicAnchor and $anchor declarations found anywhere within a schema, including
        /// nested locations ($defs, properties, items, allOf/anyOf/oneOf, if/then/else, etc.).
        /// Anchors are scoped to <paramref name="document"/> so that two documents in the same
        /// workspace can each declare the same anchor name without interfering.
        /// $ref targets are not followed; referenced components are registered independently.
        /// </summary>
        private void RegisterAnchors(OpenApiDocument document, IOpenApiSchema schema)
            => RegisterAnchorsRecursive(document, schema, new HashSet<IOpenApiSchema>());

        private void RegisterAnchorsRecursive(OpenApiDocument document, IOpenApiSchema? schema, HashSet<IOpenApiSchema> visited)
        {
            if (schema is null || !visited.Add(schema)) return;

            // For reference holders, read authored $dynamicAnchor and $anchor siblings from the
            // reference object itself — never from the resolved target. Reading IOpenApiSchema.DynamicAnchor
            // or .Anchor on a reference falls through to Target, which would duplicate the entry under a
            // different object and make single-candidate resolution look ambiguous.
            if (schema is OpenApiSchemaReference osr)
            {
                if (osr.Reference.DynamicAnchor is string dynAnchor && dynAnchor.Length > 0)
                    RegisterAnchor(document, dynAnchor, schema, isDynamic: true);
                if (osr.Reference.Anchor is string plainAnchor && plainAnchor.Length > 0)
                    RegisterAnchor(document, plainAnchor, schema, isDynamic: false);
            }
            else
            {
                if (schema.DynamicAnchor is string dynAnchor && dynAnchor.Length > 0)
                    RegisterAnchor(document, dynAnchor, schema, isDynamic: true);
                if (schema is IOpenApiSchemaMissingProperties mp && mp.Anchor is string plainAnchor && plainAnchor.Length > 0)
                    RegisterAnchor(document, plainAnchor, schema, isDynamic: false);
            }

            // Walk child schemas. For reference holders, read siblings from the reference object
            // itself (JsonSchemaReference carries authored siblings like $defs via ApplySchemaMetadata),
            // NOT from the resolved target — the target is registered independently as its own
            // component and following it would cross document boundaries and duplicate entries.
            var children = schema is OpenApiSchemaReference r ? EnumerateChildren(r.Reference) : EnumerateChildren(schema);
            foreach (var child in children)
                RegisterAnchorsRecursive(document, child, visited);
        }

        /// <summary>
        /// Enumerates all child schemas from an IOpenApiSchema, including properties from
        /// IOpenApiSchemaMissingProperties (Contains, If/Then/Else, etc.).
        /// </summary>
        private static IEnumerable<IOpenApiSchema> EnumerateChildren(IOpenApiSchema s)
        {
            if (s.Definitions is not null)
                foreach (var c in s.Definitions.Values) yield return c;
            if (s.AllOf is not null)
                foreach (var c in s.AllOf) yield return c;
            if (s.OneOf is not null)
                foreach (var c in s.OneOf) yield return c;
            if (s.AnyOf is not null)
                foreach (var c in s.AnyOf) yield return c;
            if (s.Not is not null) yield return s.Not;
            if (s.Items is not null) yield return s.Items;
            if (s.AdditionalProperties is not null) yield return s.AdditionalProperties;
            if (s.Properties is not null)
                foreach (var c in s.Properties.Values) yield return c;
            if (s.PatternProperties is not null)
                foreach (var c in s.PatternProperties.Values) yield return c;
            if (s is IOpenApiSchemaMissingProperties mp)
                foreach (var c in EnumerateMissingPropertiesChildren(mp))
                    yield return c;
        }

        /// <summary>
        /// Enumerates child schemas from a JsonSchemaReference's own properties (not the resolved Target).
        /// Needed because JsonSchemaReference is not an IOpenApiSchema — reading via the interface
        /// on OpenApiSchemaReference would delegate to Target and cross document boundaries.
        /// </summary>
        private static IEnumerable<IOpenApiSchema> EnumerateChildren(JsonSchemaReference r)
        {
            if (r.Definitions is not null)
                foreach (var c in r.Definitions.Values) yield return c;
            if (r.AllOf is not null)
                foreach (var c in r.AllOf) yield return c;
            if (r.OneOf is not null)
                foreach (var c in r.OneOf) yield return c;
            if (r.AnyOf is not null)
                foreach (var c in r.AnyOf) yield return c;
            if (r.Not is not null) yield return r.Not;
            if (r.Items is not null) yield return r.Items;
            if (r.AdditionalProperties is not null) yield return r.AdditionalProperties;
            if (r.Properties is not null)
                foreach (var c in r.Properties.Values) yield return c;
            if (r.PatternProperties is not null)
                foreach (var c in r.PatternProperties.Values) yield return c;
            if (r.Contains is not null) yield return r.Contains;
            if (r.PropertyNames is not null) yield return r.PropertyNames;
            if (r.ContentSchema is not null) yield return r.ContentSchema;
            if (r.UnevaluatedPropertiesSchema is not null) yield return r.UnevaluatedPropertiesSchema;
            if (r.If is not null) yield return r.If;
            if (r.Then is not null) yield return r.Then;
            if (r.Else is not null) yield return r.Else;
            if (r.DependentSchemas is not null)
                foreach (var c in r.DependentSchemas.Values) yield return c;
        }

        private static IEnumerable<IOpenApiSchema> EnumerateMissingPropertiesChildren(IOpenApiSchemaMissingProperties mp)
        {
            if (mp.Contains is not null) yield return mp.Contains;
            if (mp.PropertyNames is not null) yield return mp.PropertyNames;
            if (mp.ContentSchema is not null) yield return mp.ContentSchema;
            if (mp.UnevaluatedPropertiesSchema is not null) yield return mp.UnevaluatedPropertiesSchema;
            if (mp.If is not null) yield return mp.If;
            if (mp.Then is not null) yield return mp.Then;
            if (mp.Else is not null) yield return mp.Else;
            if (mp.DependentSchemas is not null)
                foreach (var c in mp.DependentSchemas.Values) yield return c;
        }

        private void RegisterAnchor(OpenApiDocument document, string anchorName, IOpenApiSchema schema, bool isDynamic)
        {
            ref var registry = ref (isDynamic ? ref _dynamicAnchorRegistryByDocument : ref _anchorRegistryByDocument);
            registry ??= new();
            if (!registry.TryGetValue(document, out var anchors))
            {
                anchors = new(StringComparer.Ordinal);
                registry[document] = anchors;
            }
            if (!anchors.TryGetValue(anchorName, out var list))
            {
                list = [];
                anchors[anchorName] = list;
            }
            if (!list.Contains(schema))
                list.Add(schema);
        }

        /// <summary>
        /// Resolves a plain $anchor by name within the scope of <paramref name="hostDocument"/>.
        /// Used as the fallback when $dynamicAnchor resolution finds zero candidates, per JSON Schema 2020-12 §8.2.3.2.
        /// Returns the schema when exactly one candidate exists; returns null for zero or multiple.
        /// </summary>
        internal IOpenApiSchema? ResolveAnchor(OpenApiDocument hostDocument, string anchorName)
        {
            if (_anchorRegistryByDocument is not null &&
                _anchorRegistryByDocument.TryGetValue(hostDocument, out var anchors) &&
                anchors.TryGetValue(anchorName, out var candidates))
                return candidates.Count == 1 ? candidates[0] : null;
            return null;
        }

        /// <summary>
        /// Finds a registered document by its base URI. Used to resolve cross-document
        /// $dynamicRef values that target an external resource.
        /// </summary>
        internal OpenApiDocument? FindDocumentByBaseUri(string documentUri)
        {
            if (!Uri.TryCreate(documentUri, UriKind.Absolute, out var uri)) return null;
            if (_dynamicAnchorRegistryByDocument?.Keys.FirstOrDefault(doc => doc.BaseUri == uri) is { } dynMatch)
                return dynMatch;
            if (_anchorRegistryByDocument?.Keys.FirstOrDefault(doc => doc.BaseUri == uri) is { } anchorMatch)
                return anchorMatch;
            return null;
        }

        /// <summary>
        /// Resolves a $dynamicRef value against the workspace's anchor registries.
        /// For fragment-only refs (#node), resolves against the host document.
        /// For URI refs (absolute), finds the external document and resolves there.
        /// </summary>
        /// <summary>
        /// Resolves a $dynamicRef value against the workspace's anchor registries.
        /// For fragment-only refs (#node), resolves against the host document.
        /// For URI refs (absolute or relative), finds the external document and resolves there.
        /// Relative URIs are resolved against the host document's BaseUri.
        /// </summary>
        internal IOpenApiSchema? ResolveDynamicRef(OpenApiDocument hostDocument, string dynamicRef)
        {
            var anchorName = JsonNodeHelper.ExtractDynamicAnchorName(dynamicRef);
            if (string.IsNullOrEmpty(anchorName)) return null;

            OpenApiDocument? targetDoc = hostDocument;
            if (!JsonNodeHelper.IsFragmentOnlyDynamicRef(dynamicRef))
            {
                var docUri = JsonNodeHelper.ExtractDocumentUri(dynamicRef);
                if (docUri is not null)
                {
                    if (!Uri.IsWellFormedUriString(docUri, UriKind.Absolute) && hostDocument.BaseUri is not null)
                        docUri = new Uri(hostDocument.BaseUri, docUri).AbsoluteUri;
                    targetDoc = FindDocumentByBaseUri(docUri);
                }
                else
                {
                    targetDoc = null;
                }
            }

            if (targetDoc is null) return null;

            var candidates = GetDynamicAnchorCandidates(targetDoc, anchorName!);
            if (candidates.Count == 1) return candidates[0];
            if (candidates.Count == 0 && ResolveAnchor(targetDoc, anchorName!) is { } anchorTarget)
                return anchorTarget;
            return null;
        }

        /// <summary>
        /// Returns all schemas in the document that declare a <c>$dynamicAnchor</c> matching
        /// <paramref name="anchorName"/>. A single candidate resolves directly; zero candidates
        /// means no dynamic anchor exists (callers may fall back to plain <c>$anchor</c>); multiple
        /// candidates indicate an ambiguous anchor whose resolution requires dynamic-scope evaluation
        /// that this library does not perform.
        /// </summary>
        /// <param name="hostDocument">The document whose anchors to search.</param>
        /// <param name="anchorName">The bare anchor name (without leading <c>#</c>).</param>
        /// <returns>All candidate schemas, or an empty list if none.</returns>
        public IReadOnlyList<IOpenApiSchema> GetDynamicAnchorCandidates(OpenApiDocument hostDocument, string anchorName)
        {
            if (_dynamicAnchorRegistryByDocument is not null &&
                _dynamicAnchorRegistryByDocument.TryGetValue(hostDocument, out var anchors) &&
                anchors.TryGetValue(anchorName, out var candidates))
                return candidates.AsReadOnly();
            return [];
        }

        /// <summary>
        /// Resolves a <c>$dynamicAnchor</c> within the context of a specific schema's <c>$defs</c>.
        /// This is a context-aware lookup: given the schema that serves as the dynamic-scope entry
        /// point (e.g. a response body schema), checks whether it or its <c>$defs</c> entries declare
        /// a matching <c>$dynamicAnchor</c>.
        /// Consumers that track dynamic scope (e.g. code generators processing an endpoint) call this
        /// with their entry-point schema to resolve context-dependent <c>$dynamicRef</c> values.
        /// </summary>
        /// <param name="contextSchema">The schema to search (e.g. the response body schema that
        /// provides the <c>$defs</c> binding).</param>
        /// <param name="anchorName">The bare anchor name (without leading <c>#</c>).</param>
        /// <returns>The matching schema, or null if not declared in this context.</returns>
        public static IOpenApiSchema? ResolveDynamicAnchorInContext(IOpenApiSchema? contextSchema, string anchorName)
        {
            if (contextSchema is null || string.IsNullOrEmpty(anchorName)) return null;

            if (contextSchema is OpenApiSchemaReference osr)
            {
                if (osr.Reference.DynamicAnchor is string a && a.Equals(anchorName, StringComparison.Ordinal))
                    return contextSchema;
                return osr.Reference.Definitions?.Values.FirstOrDefault(def =>
                    AuthoredDynamicAnchor(def) is string da && da.Equals(anchorName, StringComparison.Ordinal));
            }

            if (contextSchema.DynamicAnchor is string b && b.Equals(anchorName, StringComparison.Ordinal))
                return contextSchema;

            return contextSchema.Definitions?.Values.FirstOrDefault(def =>
                AuthoredDynamicAnchor(def) is string da && da.Equals(anchorName, StringComparison.Ordinal));
        }

        // Reads only the authored $dynamicAnchor on the schema itself. For OpenApiSchemaReference,
        // IOpenApiSchema.DynamicAnchor falls through to Target when the reference's authored sibling
        // is empty, which would resolve an anchor declared on the referenced target rather than on
        // the $defs entry. Context-bound resolution must consider only the authored sibling.
        private static string? AuthoredDynamicAnchor(IOpenApiSchema schema)
            => schema is OpenApiSchemaReference r ? r.Reference.DynamicAnchor : schema.DynamicAnchor;

        /// <summary>
        /// Adds a document id to the dictionaries of document locations and their ids.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void AddDocumentId(string? key, Uri? value)
        {
            if (!string.IsNullOrEmpty(key) && key is not null && value is not null && !_documentsIdRegistry.ContainsKey(key))
            {
                _documentsIdRegistry[key] = value;
            }
        }

        /// <summary>
        /// Retrieves the document id given a key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>The document id of the given key.</returns>
        public Uri? GetDocumentId(string? key)
        {
            if (key is not null && _documentsIdRegistry.TryGetValue(key, out var id))
            {
                return id;
            }

            return null;
        }

        /// <summary>
        /// Verify if workspace contains a component based on its URL.
        /// </summary>
        /// <param name="location">A relative or absolute URL of the file.  Use file:// for folder locations.</param>
        /// <returns>Returns true if a matching document is found.</returns>
        public bool Contains(string location)
        {
            var key = ToLocationUrl(location);
            if (key is null) return false;
            return _IOpenApiReferenceableRegistry.ContainsKey(key) || _artifactsRegistry.ContainsKey(key);
        }

        /// <summary>
        /// Resolves a reference given a key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="location"></param>
        /// <returns>The resolved reference.</returns>
        public T? ResolveReference<T>(string location)
        {
            if (string.IsNullOrEmpty(location)) return default;

            var uri = ToLocationUrl(location);
            if (uri is not null)
            {
                if (_IOpenApiReferenceableRegistry.TryGetValue(uri, out var referenceableValue) && referenceableValue is T referenceable)
                {
                    return referenceable;
                }
                else if (_artifactsRegistry.TryGetValue(uri, out var artifact) && artifact is T artifactValue)
                {
                    return artifactValue;
                }
            }            

            return default;
        }

        /// <summary>
        /// Recursively resolves a schema from a URI fragment.
        /// </summary>
        /// <param name="location"></param>
        /// <param name="parentSchema">The parent schema to resolve against.</param>
        /// <returns></returns>
        internal IOpenApiSchema? ResolveJsonSchemaReference(string location, IOpenApiSchema parentSchema)
        {
            /* Enables resolving references for nested subschemas
             * Examples:
             * #/components/schemas/person/properties/address"
             * #/components/schemas/human/allOf/0
             */

            if (string.IsNullOrEmpty(location) || ToLocationUrl(location) is not Uri uri) return default;

            if (!string.IsNullOrEmpty(uri.Fragment) && !uri.Fragment.StartsWith("#/", StringComparison.Ordinal))
            {
                return ResolveReference<IOpenApiSchema>(location);
            }

#if NETSTANDARD2_1 || NETCOREAPP || NET5_0_OR_GREATER
            if (!location.Contains("#/components/schemas/", StringComparison.OrdinalIgnoreCase))
#else
            if (!location.Contains("#/components/schemas/"))
#endif
                throw new ArgumentException($"Invalid schema reference location: {location}. It should contain '#/components/schemas/'");

            var pathSegments = uri.Fragment.Split(['/'], StringSplitOptions.RemoveEmptyEntries);

            // Build the base path for the root schema: "#/components/schemas/person"
            var fragment = OpenApiConstants.ComponentsSegment + ReferenceType.Schema.GetDisplayName() + ComponentSegmentSeparator + pathSegments[3];
            var uriBuilder = new UriBuilder(uri)
            {
                Fragment = fragment
            }; // to avoid escaping the # character in the resulting Uri

            if (_IOpenApiReferenceableRegistry.TryGetValue(uriBuilder.Uri, out var schema) && schema is IOpenApiSchema targetSchema)
            {
                // traverse remaining segments after fetching the base schema
                var remainingSegments = pathSegments.Skip(4).ToArray();
                var stack = new Stack<IOpenApiSchema>();
                stack.Push(parentSchema);
                return ResolveSubSchema(targetSchema, remainingSegments, stack);
            }

            return default;          
        }
        
        internal static IOpenApiSchema? ResolveSubSchema(IOpenApiSchema schema, string[] pathSegments, Stack<IOpenApiSchema> visitedSchemas)
        {
            // Prevent infinite recursion in case of circular references
            if (visitedSchemas.Contains(schema))
            {
                if (schema is OpenApiSchemaReference openApiSchemaReference)
                    throw new InvalidOperationException($"Circular reference detected while resolving schema: {openApiSchemaReference.Reference.ReferenceV3}");
                else
                    throw new InvalidOperationException($"Circular reference detected while resolving schema");
            }
            visitedSchemas.Push(schema);
            // Traverse schema object to resolve subschemas
            if (pathSegments.Length == 0)
            {
                return schema;
            }
            var currentSegment = pathSegments[0];
            pathSegments = [.. pathSegments.Skip(1)]; // skip one segment for the next recursive call

            switch (currentSegment)
            {
                case OpenApiConstants.Properties:
                    var propName = pathSegments[0];
                    if (schema.Properties != null && schema.Properties.TryGetValue(propName, out var propSchema))
                        return ResolveSubSchema(propSchema, [.. pathSegments.Skip(1)], visitedSchemas);
                    break;
                case OpenApiConstants.Items:
                    return schema.Items is OpenApiSchema itemsSchema ? ResolveSubSchema(itemsSchema, pathSegments, visitedSchemas) : null;

                case OpenApiConstants.AdditionalProperties:
                    return schema.AdditionalProperties is OpenApiSchema additionalSchema ? ResolveSubSchema(additionalSchema, pathSegments, visitedSchemas) : null;
                case OpenApiConstants.AllOf:
                case OpenApiConstants.AnyOf:
                case OpenApiConstants.OneOf:
                    if (!int.TryParse(pathSegments[0], out var index)) return null;

                    var list = currentSegment switch
                    {
                        OpenApiConstants.AllOf => schema.AllOf,
                        OpenApiConstants.AnyOf => schema.AnyOf,
                        OpenApiConstants.OneOf => schema.OneOf,
                        _ => null
                    };

                    // recurse into the indexed subschema if valid
                    if (list != null && index < list.Count)
                        return ResolveSubSchema(list[index], [.. pathSegments.Skip(1)], visitedSchemas);
                    break;
            }

            return null;
        }

        private Uri? ToLocationUrl(string location)
        {
            if (BaseUrl is not null)
            {
                return new(BaseUrl, location);
            }
            return null;
        }
    }
}
