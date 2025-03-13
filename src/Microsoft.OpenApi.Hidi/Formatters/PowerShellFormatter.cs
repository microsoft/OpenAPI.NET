﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using Humanizer;
using Humanizer.Inflections;
using Microsoft.OpenApi.Hidi.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Models.Interfaces;
using Microsoft.OpenApi.Services;

namespace Microsoft.OpenApi.Hidi.Formatters
{
    internal class PowerShellFormatter : OpenApiVisitorBase
    {
        private const string DefaultPutPrefix = ".Update";
        private const string PowerShellPutPrefix = ".Set";
        private readonly Stack<IOpenApiSchema> _schemaLoop = new();
        private static readonly Regex s_oDataCastRegex = new("(.*(?<=[a-z]))\\.(As(?=[A-Z]).*)", RegexOptions.Compiled, TimeSpan.FromSeconds(5));
        private static readonly Regex s_hashSuffixRegex = new(@"^[^-]+", RegexOptions.Compiled, TimeSpan.FromSeconds(5));
        private static readonly Regex s_oDataRefRegex = new("(?<=[a-z])Ref(?=[A-Z])", RegexOptions.Compiled, TimeSpan.FromSeconds(5));

        static PowerShellFormatter()
        {
            // Add singularization exclusions.
            // Enhancement: Read exclusions from a user provided file.
            Vocabularies.Default.AddSingular("(drive)s$", "$1"); // drives does not properly singularize to drive.
            Vocabularies.Default.AddSingular("(data)$", "$1"); // exclude the following from singularization.
            Vocabularies.Default.AddSingular("(delta)$", "$1");
            Vocabularies.Default.AddSingular("(quota)$", "$1");
            Vocabularies.Default.AddSingular("(statistics)$", "$1");
        }

        //FHL task for PS
        // Fixes (Order matters):
        // 1. Singularize operationId operationIdSegments.
        // 2. Add '_' to verb in an operationId.
        // 3. Fix odata cast operationIds.
        // 4. Fix hash suffix in operationIds.
        // 5. Fix Put operation id should have -> {xxx}_Set{Yyy}
        // 5. Fix anyOf and oneOf schema.
        // 6. Add AdditionalProperties to object schemas.

        public override void Visit(IOpenApiSchema schema)
        {
            AddAdditionalPropertiesToSchema(schema);
            ResolveAnyOfSchema(schema);
            ResolveOneOfSchema(schema);

            base.Visit(schema);
        }

        public override void Visit(IOpenApiPathItem pathItem)
        {
            if (pathItem.Operations is not null && pathItem.Operations.TryGetValue(HttpMethod.Put, out var value) &&
                value.OperationId != null)
            {
                var operationId = value.OperationId;
                pathItem.Operations[HttpMethod.Put].OperationId = ResolvePutOperationId(operationId);
            }

            base.Visit(pathItem);
        }

        public override void Visit(OpenApiOperation operation)
        {
            if (string.IsNullOrWhiteSpace(operation.OperationId))
                throw new ArgumentException($"OperationId is required {PathString}", nameof(operation));

            var operationId = operation.OperationId;
            var operationTypeExtension = operation.Extensions?.GetExtension("x-ms-docs-operation-type");
            if (operationTypeExtension.IsEquals("function") && operation.Parameters is { Count :> 0})
                ResolveFunctionParameters(operation.Parameters);

            // Order matters. Resolve operationId.
            operationId = RemoveHashSuffix(operationId);
            if (operationTypeExtension.IsEquals("action") || operationTypeExtension.IsEquals("function"))
                operationId = RemoveKeyTypeSegment(operationId, operation.Parameters ?? new List<IOpenApiParameter>());
            operationId = SingularizeAndDeduplicateOperationId(operationId.SplitByChar('.'));
            operationId = ResolveODataCastOperationId(operationId);
            operationId = ResolveByRefOperationId(operationId);
            // Verb segment resolution should always be last. user.get -> user_Get
            operationId = ResolveVerbSegmentInOperationId(operationId);

            operation.OperationId = operationId;
            base.Visit(operation);
        }

        private static string ResolveVerbSegmentInOperationId(string operationId)
        {
            var charPos = operationId.LastIndexOf('.', operationId.Length - 1);
            if (operationId.Contains('_', StringComparison.OrdinalIgnoreCase) || charPos < 0)
                return operationId;
            var newOperationId = new StringBuilder(operationId);
            newOperationId[charPos] = '_';
            operationId = newOperationId.ToString();
            return operationId;
        }

        private static string ResolvePutOperationId(string operationId)
        {
            return operationId.Contains(DefaultPutPrefix, StringComparison.OrdinalIgnoreCase) ?
                operationId.Replace(DefaultPutPrefix, PowerShellPutPrefix, StringComparison.Ordinal) : operationId;
        }

        private static string ResolveByRefOperationId(string operationId)
        {
            // Update $ref path operationId name
            // Ref key word is enclosed between lower-cased and upper-cased letters
            // Ex.: applications_GetRefCreatedOnBehalfOf to applications_GetCreatedOnBehalfOfByRef
            return s_oDataRefRegex.Match(operationId).Success ? $"{s_oDataRefRegex.Replace(operationId, string.Empty)}ByRef" : operationId;
        }

        private static string ResolveODataCastOperationId(string operationId)
        {
            var match = s_oDataCastRegex.Match(operationId);
            return match.Success ? $"{match.Groups[1]}{match.Groups[2]}" : operationId;
        }

        private static string SingularizeAndDeduplicateOperationId(IList<string> operationIdSegments)
        {
            var segmentsCount = operationIdSegments.Count;
            var lastSegmentIndex = segmentsCount - 1;
            var singularizedSegments = new List<string>();

            for (var x = 0; x < segmentsCount; x++)
            {
                var segment = operationIdSegments[x].Singularize(inputIsKnownToBePlural: false);

                // If a segment name is contained in the previous segment, the latter is considered a duplicate.
                // The last segment is ignored as a rule.
                if ((x > 0 && x < lastSegmentIndex) && singularizedSegments[singularizedSegments.Count - 1].Equals(segment, StringComparison.OrdinalIgnoreCase))
                    continue;

                singularizedSegments.Add(segment);
            }
            return string.Join('.', singularizedSegments);
        }

        private static string RemoveHashSuffix(string operationId)
        {
            // Remove hash suffix values from OperationIds.
            return s_hashSuffixRegex.Match(operationId).Value;
        }

        private static string RemoveKeyTypeSegment(string operationId, IList<IOpenApiParameter> parameters)
        {
            var segments = operationId.SplitByChar('.');
            foreach (var parameter in parameters)
            {
                var keyTypeExtension = parameter.Extensions?.GetExtension("x-ms-docs-key-type");
                if (keyTypeExtension != null && operationId.Contains(keyTypeExtension, StringComparison.OrdinalIgnoreCase))
                {
                    segments.Remove(keyTypeExtension);
                }
            }
            return string.Join('.', segments);
        }

        private static void ResolveFunctionParameters(IList<IOpenApiParameter> parameters)
        {
            foreach (var parameter in parameters.OfType<OpenApiParameter>().Where(static p => p.Content?.Any() ?? false))
            {
                // Replace content with a schema object of type array
                // for structured or collection-valued function parameters
                parameter.Content = null;
                parameter.Schema = new OpenApiSchema()
                {
                    Type = JsonSchemaType.Array,
                    Items = new OpenApiSchema()
                    {
                        Type = JsonSchemaType.String
                    }
                };
            }
        }

        private void AddAdditionalPropertiesToSchema(IOpenApiSchema schema)
        {
            if (schema is OpenApiSchema openApiSchema 
                && !_schemaLoop.Contains(schema) 
                && schema.Type.Equals(JsonSchemaType.Object))
            {
                openApiSchema.AdditionalProperties = new OpenApiSchema() { Type = JsonSchemaType.Object };

                /* Because 'additionalProperties' are now being walked,
                 * we need a way to keep track of visited schemas to avoid
                 * endlessly creating and walking them in an infinite recursion.
                 */
                if (schema.AdditionalProperties is not null)
                {
                    _schemaLoop.Push(schema.AdditionalProperties);
                }
            }
        }

        private static void ResolveOneOfSchema(IOpenApiSchema schema)
        {
            if (schema is OpenApiSchema openApiSchema && schema.OneOf?.FirstOrDefault() is OpenApiSchema newSchema)
            {
                openApiSchema.OneOf = null;
                FlattenSchema(openApiSchema, newSchema);
            }
        }

        private static void ResolveAnyOfSchema(IOpenApiSchema schema)
        {
            if (schema is OpenApiSchema openApiSchema && schema.AnyOf?.FirstOrDefault() is OpenApiSchema newSchema)
            {
                openApiSchema.AnyOf = null;
                FlattenSchema(openApiSchema, newSchema);
            }
        }

        private static void FlattenSchema(OpenApiSchema schema, OpenApiSchema newSchema)
        {
            if (newSchema is null) return;
            // Copies schema properties based on https://github.com/microsoft/OpenAPI.NET.OData/pull/264.
            CopySchema(schema, newSchema);
        }

        private static void CopySchema(OpenApiSchema schema, OpenApiSchema newSchema)
        {
            schema.Title ??= newSchema.Title;
            schema.Type ??= newSchema.Type;
            schema.Format ??= newSchema.Format;
            schema.Description ??= newSchema.Description;
            schema.Maximum ??= newSchema.Maximum;
            schema.ExclusiveMaximum ??= newSchema.ExclusiveMaximum;
            schema.Minimum ??= newSchema.Minimum;
            schema.ExclusiveMinimum ??= newSchema.ExclusiveMinimum;
            schema.MaxLength ??= newSchema.MaxLength;
            schema.MinLength ??= newSchema.MinLength;
            schema.Pattern ??= newSchema.Pattern;
            schema.MultipleOf ??= newSchema.MultipleOf;
            schema.Not ??= newSchema.Not;
            schema.Required ??= newSchema.Required;
            schema.Items ??= newSchema.Items;
            schema.MaxItems ??= newSchema.MaxItems;
            schema.MinItems ??= newSchema.MinItems;
            schema.UniqueItems ??= newSchema.UniqueItems;
            schema.Properties ??= newSchema.Properties;
            schema.MaxProperties ??= newSchema.MaxProperties;
            schema.MinProperties ??= newSchema.MinProperties;
            schema.Discriminator ??= newSchema.Discriminator;
            schema.ExternalDocs ??= newSchema.ExternalDocs;
            schema.Enum ??= newSchema.Enum;
            schema.ReadOnly = !schema.ReadOnly ? newSchema.ReadOnly : schema.ReadOnly;
            schema.WriteOnly = !schema.WriteOnly ? newSchema.WriteOnly : schema.WriteOnly;
            schema.Deprecated = !schema.Deprecated ? newSchema.Deprecated : schema.Deprecated;
        }
    }
}
