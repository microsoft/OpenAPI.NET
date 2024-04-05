// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using Json.Schema;
using Microsoft.OpenApi.Exceptions;
using System.Linq;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Extensions;

namespace Microsoft.OpenApi.Services
{
    /// <summary>
    /// This class is used to wallk an OpenApiDocument and sets the host document of OpenApiReferences
    /// and resolves JsonSchema references.
    /// </summary>
    internal class ReferenceResolver : OpenApiVisitorBase
    {
        private readonly OpenApiDocument _currentDocument;
        private readonly List<OpenApiError> _errors = new();

        public ReferenceResolver(OpenApiDocument currentDocument)
        {
            _currentDocument = currentDocument;
        }

        /// <summary>
        /// List of errors related to the OpenApiDocument
        /// </summary>
        public IEnumerable<OpenApiError> Errors => _errors;

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
        /// Resolves schemas in components
        /// </summary>
        /// <param name="components"></param>
        public override void Visit(OpenApiComponents components)
        {
            components.Schemas = ResolveJsonSchemas(components.Schemas);
        }

        /// <summary>
        /// Resolve all JsonSchema references used in mediaType object
        /// </summary>
        /// <param name="mediaType"></param>
        public override void Visit(OpenApiMediaType mediaType)
        {
            ResolveJsonSchema(mediaType.Schema, r => mediaType.Schema = r ?? mediaType.Schema);
        }

        /// <summary>
        /// Resolve all JsonSchema references used in a parameter
        /// </summary>
        public override void Visit(OpenApiParameter parameter)
        {
            ResolveJsonSchema(parameter.Schema, r => parameter.Schema = r);
        }

        /// <summary>
        /// Resolve all references used in a JsonSchema
        /// </summary>
        /// <param name="schema"></param>
        public override void Visit(ref JsonSchema schema)
        {
            var reference = schema.GetRef();
            var description = schema.GetDescription();
            var summary = schema.GetSummary();

            if (schema.Keywords.Count.Equals(1) && reference != null)
            {
                schema = ResolveJsonSchemaReference(reference, description, summary);
            }

            var builder = new JsonSchemaBuilder();
            if (schema?.Keywords is { } keywords)
            {
                foreach (var keyword in keywords)
                {
                    builder.Add(keyword);
                }
            }

            ResolveJsonSchema(schema.GetItems(), r => builder.Items(r));
            ResolveJsonSchemaList((IList<JsonSchema>)schema.GetOneOf(), r => builder.OneOf(r));
            ResolveJsonSchemaList((IList<JsonSchema>)schema.GetAllOf(), r => builder.AllOf(r));
            ResolveJsonSchemaList((IList<JsonSchema>)schema.GetAnyOf(), r => builder.AnyOf(r));
            ResolveJsonSchemaMap((IDictionary<string, JsonSchema>)schema.GetProperties(), r => builder.Properties((IReadOnlyDictionary<string, JsonSchema>)r));
            ResolveJsonSchema(schema.GetAdditionalProperties(), r => builder.AdditionalProperties(r));

            schema = builder.Build();
        }

        /// <summary>
        /// Visits an IBaseDocument instance
        /// </summary>
        /// <param name="document"></param>
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
        /// Resolves the target to a JsonSchema reference by retrieval from Schema registry
        /// </summary>
        /// <param name="reference">The JSON schema reference.</param>
        /// <param name="description">The schema's description.</param>
        /// <param name="summary">The schema's summary.</param>
        /// <returns></returns>
        public JsonSchema ResolveJsonSchemaReference(Uri reference, string description = null, string summary = null)
        {
            var resolvedSchema = _currentDocument.ResolveJsonSchemaReference(reference);

            if (resolvedSchema != null)
            {
                var resolvedSchemaBuilder = new JsonSchemaBuilder();

                foreach (var keyword in resolvedSchema.Keywords)
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
                var referenceId = reference.OriginalString.Split('/').LastOrDefault();
                throw new OpenApiException(string.Format(Properties.SRResource.InvalidReferenceId, referenceId));
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

        private void ResolveJsonSchemaList(IList<JsonSchema> list, Action<List<JsonSchema>> assign)
        {
            if (list == null) return;

            for (int i = 0; i < list.Count; i++)
            {
                var entity = list[i];
                var reference = entity?.GetRef();
                if (reference != null)
                {
                    list[i] = ResolveJsonSchemaReference(reference);
                }
            }

            assign(list.ToList());
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
    }
}
