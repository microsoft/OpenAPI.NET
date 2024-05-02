using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Humanizer;
using Json.Schema;
using Json.Schema.OpenApi;
using Microsoft.OpenApi.Hidi.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Services;
using Microsoft.OpenApi.Extensions;

namespace Microsoft.OpenApi.Hidi.Formatters
{
    internal class PowerShellFormatter : OpenApiVisitorBase
    {
        private const string DefaultPutPrefix = ".Update";
        private const string PowerShellPutPrefix = ".Set";
        private readonly Stack<JsonSchema> _schemaLoop = new();
        private static readonly Regex s_oDataCastRegex = new("(.*(?<=[a-z]))\\.(As(?=[A-Z]).*)", RegexOptions.Compiled, TimeSpan.FromSeconds(5));
        private static readonly Regex s_hashSuffixRegex = new(@"^[^-]+", RegexOptions.Compiled, TimeSpan.FromSeconds(5));
        private static readonly Regex s_oDataRefRegex = new("(?<=[a-z])Ref(?=[A-Z])", RegexOptions.Compiled, TimeSpan.FromSeconds(5));

        static PowerShellFormatter()
        {
            // Add singularization exclusions.
            // Enhancement: Read exclusions from a user provided file.
            Humanizer.Inflections.Vocabularies.Default.AddSingular("(drive)s$", "$1"); // drives does not properly singularize to drive.
            Humanizer.Inflections.Vocabularies.Default.AddSingular("(data)$", "$1"); // exclude the following from singularization.
            Humanizer.Inflections.Vocabularies.Default.AddSingular("(delta)$", "$1");
            Humanizer.Inflections.Vocabularies.Default.AddSingular("(quota)$", "$1");
            Humanizer.Inflections.Vocabularies.Default.AddSingular("(statistics)$", "$1");
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

        public override void Visit(ref JsonSchema schema)
         {
            AddAdditionalPropertiesToSchema(ref schema);
            schema = ResolveAnyOfSchema(ref schema);
            schema = ResolveOneOfSchema(ref schema);

            base.Visit(ref schema);
        }

        public override void Visit(OpenApiPathItem pathItem)
        {
            if (pathItem.Operations.TryGetValue(OperationType.Put, out var value))
            {
                var operationId = value.OperationId;
                pathItem.Operations[OperationType.Put].OperationId = ResolvePutOperationId(operationId);
            }

            base.Visit(pathItem);
        }

        public override void Visit(OpenApiOperation operation)
        {
            if (string.IsNullOrWhiteSpace(operation.OperationId))
                throw new ArgumentException($"OperationId is required {PathString}", nameof(operation));

            var operationId = operation.OperationId;
            var operationTypeExtension = operation.Extensions.GetExtension("x-ms-docs-operation-type");
            if (operationTypeExtension.IsEquals("function"))
                operation.Parameters = ResolveFunctionParameters(operation.Parameters);

            // Order matters. Resolve operationId.
            operationId = RemoveHashSuffix(operationId);
            if (operationTypeExtension.IsEquals("action") || operationTypeExtension.IsEquals("function"))
                operationId = RemoveKeyTypeSegment(operationId, operation.Parameters);
            operationId = SingularizeAndDeduplicateOperationId(operationId.SplitByChar('.'));
            operationId = ResolveODataCastOperationId(operationId);
            operationId = ResolveByRefOperationId(operationId);
            // Verb segment resolution should always be last. user.get -> user_Get
            operationId = ResolveVerbSegmentInOpertationId(operationId);

            operation.OperationId = operationId;
            base.Visit(operation);
        }

        private static string ResolveVerbSegmentInOpertationId(string operationId)
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

        private static string RemoveKeyTypeSegment(string operationId, IList<OpenApiParameter> parameters)
        {
            var segments = operationId.SplitByChar('.');
            foreach (var parameter in parameters)
            {
                var keyTypeExtension = parameter.Extensions.GetExtension("x-ms-docs-key-type");
                if (keyTypeExtension != null && operationId.Contains(keyTypeExtension, StringComparison.OrdinalIgnoreCase))
                {
                    segments.Remove(keyTypeExtension);
                }
            }
            return string.Join('.', segments);
        }

        private static IList<OpenApiParameter> ResolveFunctionParameters(IList<OpenApiParameter> parameters)
        {
            foreach (var parameter in parameters.Where(static p => p.Content?.Any() ?? false))
            {
                // Replace content with a schema object of type array
                // for structured or collection-valued function parameters
                parameter.Content = null;
                parameter.Schema = new JsonSchemaBuilder()
                    .Type(SchemaValueType.Array)
                    .Items(new JsonSchemaBuilder()
                        .Type(SchemaValueType.String))
                ;
            }
            return parameters;
        }

        private void AddAdditionalPropertiesToSchema(ref JsonSchema schema)
        {
            if (schema != null && !_schemaLoop.Contains(schema) && schema.GetJsonType().Equals(SchemaValueType.Object))
            {
                var schemaBuilder = new JsonSchemaBuilder();
                if (schema.Keywords != null)
                {
                    foreach (var keyword in schema.Keywords)
                    {
                        schemaBuilder.Add(keyword);
                    }
                }

                schema = schemaBuilder.AdditionalProperties(new JsonSchemaBuilder().Type(SchemaValueType.Object));

                /* Because 'additionalProperties' are now being walked,
                 * we need a way to keep track of visited schemas to avoid
                 * endlessly creating and walking them in an infinite recursion.
                 */
                var additionalProps = schema.GetAdditionalProperties();

                if (additionalProps != null)
                {
                    _schemaLoop.Push(additionalProps);
                }
            }
            
        }

        private static JsonSchema ResolveOneOfSchema(ref JsonSchema schema)
        {
            if (schema.GetOneOf()?[0] is {} newSchema)
            {
                var schemaBuilder = BuildSchema(schema);
                schemaBuilder = schemaBuilder.Remove("oneOf");
                schema = schemaBuilder.Build();

                schema = FlattenSchema(schema, newSchema);
            }

            return schema;
        }

        private static JsonSchema ResolveAnyOfSchema(ref JsonSchema schema)
        {
            if (schema.GetAnyOf()?[0] is {} newSchema)
            {
                var schemaBuilder = BuildSchema(schema);
                schemaBuilder = schemaBuilder.Remove("anyOf");
                schema = schemaBuilder.Build();

                schema = FlattenSchema(schema, newSchema);
            }

            return schema;
        }

        private static JsonSchema FlattenSchema(JsonSchema schema, JsonSchema newSchema)
        {
            if (newSchema != null)
            {
                var newSchemaRef = newSchema.GetRef();

                if (newSchemaRef != null)
                {
                    var schemaBuilder = BuildSchema(schema);
                    schema = schemaBuilder.Ref(newSchemaRef);
                }
                else
                {
                    // Copies schema properties based on https://github.com/microsoft/OpenAPI.NET.OData/pull/264.
                    schema = CopySchema(schema, newSchema);
                }
            }

            return schema;
        }

        private static JsonSchema CopySchema(JsonSchema schema, JsonSchema newSchema)
        {
            var schemaBuilder = new JsonSchemaBuilder();
            var keywords = schema.Keywords;
            if (keywords != null)
            {
                foreach (var keyword in keywords)
                {
                    schemaBuilder.Add(keyword);
                }
            }

            if (schema.GetTitle() == null && newSchema.GetTitle() is { } title)
            {
                schemaBuilder.Title(title);
            }
            if (schema.GetJsonType() == null && newSchema.GetJsonType() is { } type)
            {
                schemaBuilder.Type(type);
            }
            if (schema.GetFormat() == null && newSchema.GetFormat() is { } format)
            {
                schemaBuilder.Format(format);
            }
            if (schema.GetDescription() == null && newSchema.GetDescription() is { } description)
            {
                schemaBuilder.Description(description);
            }
            if (schema.GetMaximum() == null && newSchema.GetMaximum() is { } max)
            {
                schemaBuilder.Maximum(max);
            }
            if (schema.GetExclusiveMaximum() == null && newSchema.GetExclusiveMaximum() is { } exclusiveMaximum)
            {
                schemaBuilder.ExclusiveMaximum(exclusiveMaximum);
            }
            if (schema.GetMinimum() == null && newSchema.GetMinimum() is { } min)
            {
                schemaBuilder.Minimum(min);
            }
            if (schema.GetExclusiveMinimum() == null && newSchema.GetExclusiveMinimum() is { } exclusiveMinimum)
            {
                schemaBuilder.ExclusiveMinimum(exclusiveMinimum);
            }
            if (schema.GetMaxLength() == null && newSchema.GetMaxLength() is { } maxLength)
            {
                schemaBuilder.MaxLength(maxLength);
            }
            if (schema.GetMinLength() == null && newSchema.GetMinLength() is { } minLength)
            {
                schemaBuilder.MinLength(minLength);
            }
            if (schema.GetPattern() == null && newSchema.GetPattern() is { } pattern)
            {
                schemaBuilder.Pattern(pattern);
            }
            if (schema.GetMultipleOf() == null && newSchema.GetMultipleOf() is { } multipleOf)
            {
                schemaBuilder.MultipleOf(multipleOf);
            }
            if (schema.GetNot() == null && newSchema.GetNot() is { } not)
            {
                schemaBuilder.Not(not);
            }
            if (schema.GetRequired() == null && newSchema.GetRequired() is { } required)
            {
                schemaBuilder.Required(required);
            }
            if (schema.GetItems() == null && newSchema.GetItems() is { } items)
            {
                schemaBuilder.Items(items);
            }
            if (schema.GetMaxItems() == null && newSchema.GetMaxItems() is { } maxItems)
            {
                schemaBuilder.MaxItems(maxItems);
            }
            if (schema.GetMinItems() == null && newSchema.GetMinItems() is { } minItems)
            {
                schemaBuilder.MinItems(minItems);
            }
            if (schema.GetUniqueItems() == null && newSchema.GetUniqueItems() is { } uniqueItems)
            {
                schemaBuilder.UniqueItems(uniqueItems);
            }
            if (schema.GetProperties() == null && newSchema.GetProperties() is { } properties)
            {
                schemaBuilder.Properties(properties);
            }
            if (schema.GetMaxProperties() == null && newSchema.GetMaxProperties() is { } maxProperties)
            {
                schemaBuilder.MaxProperties(maxProperties);
            }
            if (schema.GetMinProperties() == null && newSchema.GetMinProperties() is { } minProperties)
            {
                schemaBuilder.MinProperties(minProperties);
            }
            if (schema.GetDiscriminator() == null && newSchema.GetDiscriminator() is { } discriminator)
            {
                schemaBuilder.Discriminator(discriminator.PropertyName, discriminator.Mapping, discriminator.Extensions);
            }
            if (schema.GetOpenApiExternalDocs() == null && newSchema.GetOpenApiExternalDocs() is { } externalDocs)
            {
                schemaBuilder.OpenApiExternalDocs(externalDocs);
            }
            if (schema.GetEnum() == null && newSchema.GetEnum() is { } enumCollection)
            {
                schemaBuilder.Enum(enumCollection);
            }

            if (!schema.GetReadOnly() is { } && newSchema.GetReadOnly() is { } newValue)
            {
                schemaBuilder.ReadOnly(newValue);
            }

            if (!schema.GetWriteOnly() is { } && newSchema.GetWriteOnly() is { } newWriteOnlyValue)
            {
                schemaBuilder.WriteOnly(newWriteOnlyValue);
            }

            if (!schema.GetNullable() is { } && newSchema.GetNullable() is { } newNullableValue)
            {
                schemaBuilder.Nullable(newNullableValue);
            }

            if (!schema.GetDeprecated() is { } && newSchema.GetDeprecated() is { } newDepracatedValue)
            {
                schemaBuilder.Deprecated(newDepracatedValue);
            }

            return schemaBuilder;            
        }

        private static JsonSchemaBuilder BuildSchema(JsonSchema schema)
        {
            var schemaBuilder = new JsonSchemaBuilder();
            if (schema.Keywords != null)
            {
                foreach (var keyword in schema.Keywords)
                {
                    schemaBuilder.Add(keyword);
                }
            }

            return schemaBuilder;
        }
    }
}
