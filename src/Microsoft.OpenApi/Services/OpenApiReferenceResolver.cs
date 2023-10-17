// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;
using System.Linq;
using Json.Schema;
using Microsoft.OpenApi.Exceptions;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Services
{
    /// <summary>
    /// This class is used to walk an OpenApiDocument and convert unresolved references to references to populated objects
    /// </summary>
    public class OpenApiReferenceResolver : OpenApiVisitorBase
    {
        private OpenApiDocument _currentDocument;
        private readonly bool _resolveRemoteReferences;
        private List<OpenApiError> _errors = new List<OpenApiError>();

        /// <summary>
        /// Initializes the <see cref="OpenApiReferenceResolver"/> class.
        /// </summary>
        public OpenApiReferenceResolver(OpenApiDocument currentDocument, bool resolveRemoteReferences = true)
        {
            _currentDocument = currentDocument;
            _resolveRemoteReferences = resolveRemoteReferences;
        }

        /// <summary>
        /// List of errors related to the OpenApiDocument
        /// </summary>
        public IEnumerable<OpenApiError> Errors => _errors;

        /// <summary>
        /// Resolves tags in OpenApiDocument
        /// </summary>
        /// <param name="doc"></param>
        public override void Visit(OpenApiDocument doc)
        {
            if (doc.Tags != null)
            {
                ResolveTags(doc.Tags);
            }
        }

        /// <summary>
        /// Visits the referenceable element in the host document
        /// </summary>
        /// <param name="referenceable">The referenceable element in the doc.</param>
        public override void Visit(IOpenApiReferenceable referenceable)
        {
            if (referenceable.Reference != null)
            {
                referenceable.Reference.HostDocument = _currentDocument;
            }
        }

        /// <summary>
        /// Resolves references in components
        /// </summary>
        /// <param name="components"></param>
        public override void Visit(OpenApiComponents components)
        {
            ResolveMap(components.Parameters);
            ResolveMap(components.RequestBodies);
            ResolveMap(components.Responses);
            ResolveMap(components.Links);
            ResolveMap(components.Callbacks);
            ResolveMap(components.Examples);
            components.Schemas = ResolveJsonSchemas(components.Schemas);
            ResolveMap(components.PathItems);
            ResolveMap(components.SecuritySchemes);
            ResolveMap(components.Headers);
        }

        /// <summary>
        /// Resolves all references used in callbacks
        /// </summary>
        /// <param name="callbacks"></param>
        public override void Visit(IDictionary<string, OpenApiCallback> callbacks)
        {
            ResolveMap(callbacks);
        }

        /// <summary>
        /// Resolves all references used in webhooks
        /// </summary>
        /// <param name="webhooks"></param>
        public override void Visit(IDictionary<string, OpenApiPathItem> webhooks)
        {
            ResolveMap(webhooks);
        }

        /// <summary>
        /// Resolve all references used in an operation
        /// </summary>
        public override void Visit(OpenApiOperation operation)
        {
            ResolveObject(operation.RequestBody, r => operation.RequestBody = r);
            ResolveList(operation.Parameters);

            if (operation.Tags != null)
            {
                ResolveTags(operation.Tags);
            }
        }

        /// <summary>
        /// Resolve all references used in mediaType object
        /// </summary>
        /// <param name="mediaType"></param>
        public override void Visit(OpenApiMediaType mediaType)
        {
            ResolveJsonSchema(mediaType.Schema, r => mediaType.Schema = r ?? mediaType.Schema);
        }

        /// <summary>
        /// Resolve all references to examples
        /// </summary>
        /// <param name="examples"></param>
        public override void Visit(IDictionary<string, OpenApiExample> examples)
        {
            ResolveMap(examples);
        }

        /// <summary>
        /// Resolve all references to responses
        /// </summary>
        public override void Visit(OpenApiResponses responses)
        {
            ResolveMap(responses);
        }

        /// <summary>
        /// Resolve all references to headers
        /// </summary>
        /// <param name="headers"></param>
        public override void Visit(IDictionary<string, OpenApiHeader> headers)
        {
            ResolveMap(headers);
        }

        /// <summary>
        /// Resolve all references to SecuritySchemes
        /// </summary>
        public override void Visit(OpenApiSecurityRequirement securityRequirement)
        {
            foreach (var scheme in securityRequirement.Keys.ToList())
            {
                ResolveObject(scheme, (resolvedScheme) =>
                {
                    if (resolvedScheme != null)
                    {
                        // If scheme was unresolved
                        // copy Scopes and remove old unresolved scheme
                        var scopes = securityRequirement[scheme];
                        securityRequirement.Remove(scheme);
                        securityRequirement.Add(resolvedScheme, scopes);
                    }
                });
            }
        }

        /// <summary>
        /// Resolve all references to parameters
        /// </summary>
        public override void Visit(IList<OpenApiParameter> parameters)
        {
            ResolveList(parameters);
        }

        /// <summary>
        /// Resolve all references used in a parameter
        /// </summary>
        public override void Visit(OpenApiParameter parameter)
        {
            ResolveJsonSchema(parameter.Schema, r => parameter.Schema = r);
            ResolveMap(parameter.Examples);
        }

        /// <summary>
        /// Resolve all references to links
        /// </summary>
        public override void Visit(IDictionary<string, OpenApiLink> links)
        {
            ResolveMap(links);
        }
        
        /// <summary>
        /// Resolve all references used in a schem
        /// </summary>
        /// <param name="schema"></param>
        public override void Visit(ref JsonSchema schema)
        {
            var reference = schema.GetRef();
            var description = schema.GetDescription();
            var summary = schema.GetSummary();

            if (reference != null)
            {
                schema = ResolveJsonSchemaReference(reference, description, summary);
            }
            
            var builder = new JsonSchemaBuilder();
            foreach (var keyword in schema.Keywords)
            {
                builder.Add(keyword);
            }
            
            ResolveJsonSchema(schema.GetItems(), r => builder.Items(r));
            ResolveJsonSchemaList((IList<JsonSchema>)schema.GetOneOf(), r => builder.OneOf(r));
            ResolveJsonSchemaList((IList<JsonSchema>)schema.GetAllOf(), r => builder.AllOf(r));
            ResolveJsonSchemaList((IList<JsonSchema>)schema.GetAnyOf(), r => builder.AnyOf(r));
            ResolveJsonSchemaMap((IDictionary<string, JsonSchema>)schema.GetProperties(), r => builder.Properties((IReadOnlyDictionary<string, JsonSchema>)r));
            ResolveJsonSchema(schema.GetAdditionalProperties(), r => builder.AdditionalProperties(r));

            schema = builder.Build();
        }

        public override void Visit(IBaseDocument document) { }

        private Dictionary<string, JsonSchema> ResolveJsonSchemas(IDictionary<string, JsonSchema> schemas)
        {
            var resolvedSchemas = new Dictionary<string, JsonSchema>();
            foreach (var schema in schemas)
            {
                var schemaValue = schema.Value;
                Visit(ref schemaValue);
                resolvedSchemas[schema.Key] = schemaValue;
            }

            return resolvedSchemas;
        }

        /// <summary>
        /// Resolves the target to a JSON schema reference by retrieval from Schema registry
        /// </summary>
        /// <param name="reference">The JSON schema reference.</param>
        /// <param name="description">The schema's description.</param>
        /// <param name="summary">The schema's summary.</param>
        /// <returns></returns>
        public JsonSchema ResolveJsonSchemaReference(Uri reference, string description = null, string summary = null)
        {
            var refUri = $"https://everything.json{reference.OriginalString.Split('#').LastOrDefault()}";
            var resolvedSchema = (JsonSchema)SchemaRegistry.Global.Get(new Uri(refUri));

            if (resolvedSchema != null)
            {
                var resolvedSchemaBuilder = new JsonSchemaBuilder();

                foreach (var keyword in resolvedSchema?.Keywords)
                {
                    resolvedSchemaBuilder.Add(keyword);

                    // Replace the resolved schema's description with that of the schema reference
                    if (!string.IsNullOrEmpty(description))
                    {
                        resolvedSchemaBuilder.Description(description);
                    }

                    // Replace the resolved schema's summary with that of the schema reference
                    if (!string.IsNullOrEmpty(summary))
                    {
                        resolvedSchemaBuilder.Summary(summary);
                    }
                }

                return resolvedSchemaBuilder.Build();
            }
            else 
            {
                return null;
            }
        }

        /// <summary>
        /// Replace references to tags with either tag objects declared in components, or inline tag object
        /// </summary>
        private void ResolveTags(IList<OpenApiTag> tags)
        {
            for (int i = 0; i < tags.Count; i++)
            {
                var tag = tags[i];
                if (IsUnresolvedReference(tag))
                {
                    var resolvedTag = ResolveReference<OpenApiTag>(tag.Reference);

                    if (resolvedTag == null)
                    {
                        resolvedTag = new OpenApiTag()
                        {
                            Name = tag.Reference.Id
                        };
                    }
                    tags[i] = resolvedTag;
                }
            }
        }

        private void ResolveObject<T>(T entity, Action<T> assign) where T : class, IOpenApiReferenceable, new()
        {
            if (entity == null) return;

            if (IsUnresolvedReference(entity))
            {
                assign(ResolveReference<T>(entity.Reference));
            }
        }

        private void ResolveJsonSchema(JsonSchema schema, Action<JsonSchema> assign)
        {
            if (schema == null) return;
            var reference = schema.GetRef();
            var description = schema.GetDescription();
            var summary = schema.GetSummary();

            if (reference != null)
            {
                assign(ResolveJsonSchemaReference(reference, description, summary));
            }
        }

        private void ResolveList<T>(IList<T> list) where T : class, IOpenApiReferenceable, new()
        {
            if (list == null) return;

            for (int i = 0; i < list.Count; i++)
            {
                var entity = list[i];
                if (IsUnresolvedReference(entity))
                {
                    list[i] = ResolveReference<T>(entity.Reference);
                }
            }
        }

        private void ResolveJsonSchemaList(IList<JsonSchema> list, Action<List<JsonSchema>> assign)
        {
            if (list == null) return;

            for (int i = 0; i < list.Count; i++)
            {
                var entity = list[i];
                var reference = entity.GetRef();
                if (reference != null)
                {
                    list[i] = ResolveJsonSchemaReference(reference);
                }
            }

            assign(list.ToList());
        }

        private void ResolveMap<T>(IDictionary<string, T> map) where T : class, IOpenApiReferenceable, new()
        {
            if (map == null) return;

            foreach (var key in map.Keys.ToList())
            {
                var entity = map[key];
                if (IsUnresolvedReference(entity))
                {
                    map[key] = ResolveReference<T>(entity.Reference);
                }
            }
        }

        private void ResolveJsonSchemaMap(IDictionary<string, JsonSchema> map, Action<IDictionary<string, JsonSchema>> assign)
        {
            if (map == null) return;

            foreach (var key in map.Keys.ToList())
            {
                var entity = map[key];
                var reference = entity.GetRef();
                if (reference != null)
                {
                    map[key] = ResolveJsonSchemaReference(reference);
                }
            }

            assign(map.ToDictionary(e => e.Key, e => e.Value));
        }

        private T ResolveReference<T>(OpenApiReference reference) where T : class, IOpenApiReferenceable, new()
        {
            if (string.IsNullOrEmpty(reference.ExternalResource))
            {
                try
                {
                    return _currentDocument.ResolveReference(reference, false) as T;
                }
                catch (OpenApiException ex)
                {
                    _errors.Add(new OpenApiReferenceError(ex));
                    return null;
                }
            }
            // The concept of merging references with their target at load time is going away in the next major version
            // External references will not support this approach.
            //else if (_resolveRemoteReferences == true)
            //{
            //    if (_currentDocument.Workspace == null)
            //    {
            //        _errors.Add(new OpenApiReferenceError(reference,"Cannot resolve external references for documents not in workspaces."));
            //        // Leave as unresolved reference
            //        return new T()
            //        {
            //            UnresolvedReference = true,
            //            Reference = reference
            //        };
            //    }
            //    var target = _currentDocument.Workspace.ResolveReference(reference);

            //    // TODO:  If it is a document fragment, then we should resolve it within the current context

            //    return target as T;
            //}
            else
            {
                // Leave as unresolved reference
                return new T()
                {
                    UnresolvedReference = true,
                    Reference = reference
                };
            }
        }

        private bool IsUnresolvedReference(IOpenApiReferenceable possibleReference)
        {
            return possibleReference != null && possibleReference.UnresolvedReference;
        }
    }
}
