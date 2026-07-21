// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;

#pragma warning disable CS0618

namespace Microsoft.OpenApi.Reader.V3
{
    /// <summary>
    /// Class containing logic to deserialize Open API V3 document into
    /// runtime Open API object model.
    /// </summary>
    internal static partial class OpenApiV3Deserializer
    {
        private const string EncounteredNullableTrueMetadataKey = "encounteredNullable";

        private static readonly FixedFieldMap<OpenApiSchema> _openApiSchemaFixedFields = new()
        {
            {
                "title",
                (o, n, _, _) => o.Title = n.GetScalarValue()
            },
            {
                "multipleOf",
                (o, n, _, _) =>
                {
                    var multipleOf = n.GetScalarValue();
                    if (multipleOf != null)
                    {
                        o.MultipleOf = decimal.Parse(multipleOf, NumberStyles.Float, CultureInfo.InvariantCulture);
                    }
                }
            },
            {
                "maximum",
                (o, n, _, _) =>
                {
                    var max = n.GetScalarValue();
                    if (!string.IsNullOrEmpty(max))
                    {
                        o.Maximum = max;
                    }
                }
            },
            {
                "exclusiveMaximum",
                (o, n, _, _) => o.IsExclusiveMaximum = n.GetScalarBoolValue()
            },
            {
               "minimum",
                (o, n, _, _) =>
                {
                    var min = n.GetScalarValue();
                    if (!string.IsNullOrEmpty(min))
                    {
                        o.Minimum = min;
                    }
                }
            },
            {
                "exclusiveMinimum",
                (o, n, _, _) => o.IsExclusiveMinimum = n.GetScalarBoolValue()
            },
            {
                "maxLength",
                (o, n, _, _) =>
                {
                    o.MaxLength = n.GetScalarIntValue();
                }
            },
            {
                "minLength",
                (o, n, _, _) =>
                {
                    o.MinLength = n.GetScalarIntValue();
                }
            },
            {
                "pattern",
                (o, n, _, _) => o.Pattern = n.GetScalarValue()
            },
            {
                "maxItems",
                (o, n, _, _) =>
                {
                    o.MaxItems = n.GetScalarIntValue();
                }
            },
            {
                "minItems",
                (o, n, _, _) =>
                {
                    o.MinItems = n.GetScalarIntValue();
                }
            },
            {
                "uniqueItems",
                (o, n, _, _) =>
                {
                    o.UniqueItems = n.GetScalarBoolValue();
                }
            },
            {
                "maxProperties",
                (o, n, _, _) =>
                {
                    o.MaxProperties = n.GetScalarIntValue();
                }
            },
            {
                "minProperties",
                (o, n, _, _) =>
                {
                    o.MinProperties = n.GetScalarIntValue();
                }
            },
            {
                "required",
                (o, n, doc, c) => o.Required = new HashSet<string>(n.CreateSimpleList((n2, _) => n2.GetScalarValue(), doc, c).OfType<string>())
            },
            {
                "enum",
                (o, n, _, c) => o.Enum = n.CreateListOfAny(c)
            },
            {
                "type",
                (o, n, _, c) => {
                    var type = n.GetScalarValue()?.ToJsonSchemaType();
                    // so we don't lose the value from nullable
                    if (o.Type.HasValue)
                        o.Type |= type;
                    else
                        o.Type = type;
                }
            },
            {
                "allOf",
                (o, n, t, c) => o.AllOf = n.CreateList(LoadSchema, t, c)
            },
            {
                "oneOf",
                (o, n, doc, c) => o.OneOf = n.CreateList(LoadSchema, doc, c)
            },
            {
                "anyOf",
                (o, n, t, c) => o.AnyOf = n.CreateList(LoadSchema, t, c)
            },
            {
                "not",
                (o, n, doc, c) => o.Not = LoadSchema(n, doc, c)
            },
            {
                "items",
                (o, n, doc, c) => o.Items = LoadSchema(n, doc, c)
            },
            {
                "properties",
                (o, n, t, c) => o.Properties = n.CreateMap(LoadSchema, t, c)
            },
            {
                "additionalProperties", (o, n, doc, c) =>
                {
                    if (n is JsonValue)
                    {
                        o.AdditionalPropertiesAllowed = n.GetScalarBoolValue();
                    }
                    else
                    {
                        o.AdditionalProperties = LoadSchema(n, doc, c);
                    }
                }
            },
            {
                "description",
                (o, n, _, _) => o.Description = n.GetScalarValue()
            },
            {
                "format",
                (o, n, _, _) => o.Format = n.GetScalarValue()
            },
            {
                "default",
                (o, n, _, _) => o.Default = n
            },
            {
                "nullable",
                (o, n, _, _) =>
                {
                    if (n.GetScalarBoolValue())
                    {
                        if (o.Type is not null && o.Type != 0)
                        {
                            o.Type |= JsonSchemaType.Null;
                        }
                        else
                        {
                            // "nullable" is a type modifier.
                            // It only has effect if there is a "Type" set.
                            // Given that we don't have a Type set yet, we want to keep track of it until the end of deserialization.
                            // If, at a later point during deserialization, Type was set, we apply nullable to it.
                            // Otherwise, we throw it away.
                            o.Metadata ??= new Dictionary<string, object>();
                            o.Metadata[EncounteredNullableTrueMetadataKey] = true;
                        }
                    }
                }
            },
            {
                "discriminator",
                (o, n, doc, c) => o.Discriminator = LoadDiscriminator(n, doc, c)
            },
            {
                "readOnly",
                (o, n, _, _) =>
                {
                    o.ReadOnly = n.GetScalarBoolValue();
                }
            },
            {
                "writeOnly",
                (o, n, _, _) =>
                {
                    o.WriteOnly = n.GetScalarBoolValue();
                }
            },
            {
                "xml",
                (o, n, doc, c) => o.Xml = LoadXml(n, doc, c)
            },
            {
                "externalDocs",
                (o, n, doc, c) => o.ExternalDocs = LoadExternalDocs(n, doc, c)
            },
            {
                "example",
                (o, n, _, _) => o.Example = n
            },
            {
                OpenApiConstants.JsonSchemaExamplesExtension,
                (o, n, _, c) => o.Examples = n.CreateListOfAny(c)
            },
            {
                "deprecated",
                (o, n, _, _) =>
                {
                    o.Deprecated = n.GetScalarBoolValue();
                }
            },
            {
                OpenApiConstants.PatternPropertiesExtension,
                (o, n, t, c) => o.PatternProperties = n.CreateMap(LoadSchema, t, c)
            },
            {
                OpenApiConstants.UnevaluatedPropertiesExtension,
                (o, n, t, c) =>
                {
                    if (n is JsonValue)
                    {
                        o.UnevaluatedProperties = n.GetScalarBoolValue();
                    }
                    else
                    {
                        o.UnevaluatedPropertiesSchema = LoadSchema(n, t, c);
                    }
                }
            },
            {
                OpenApiConstants.AnchorExtension,
                (o, n, _, _) => o.Anchor = n.GetScalarValue()
            },
            {
                OpenApiConstants.ContentEncodingExtension,
                (o, n, _, _) => o.ContentEncoding = n.GetScalarValue()
            },
            {
                OpenApiConstants.ContentMediaTypeExtension,
                (o, n, _, _) => o.ContentMediaType = n.GetScalarValue()
            },
            {
                OpenApiConstants.ContentSchemaExtension,
                (o, n, doc, c) => o.ContentSchema = LoadSchema(n, doc, c)
            },
            {
                OpenApiConstants.ContainsExtension,
                (o, n, doc, c) => o.Contains = LoadSchema(n, doc, c)
            },
            {
                OpenApiConstants.MaxContainsExtension,
                (o, n, _, _) =>
                {
                    o.MaxContains = n.GetScalarUIntValue();
                }
            },
            {
                OpenApiConstants.MinContainsExtension,
                (o, n, _, _) =>
                {
                    o.MinContains = n.GetScalarUIntValue();
                }
            },
            {
                OpenApiConstants.PropertyNamesExtension,
                (o, n, doc, c) => o.PropertyNames = LoadSchema(n, doc, c)
            },
            {
                OpenApiConstants.DependentSchemasExtension,
                (o, n, t, c) => o.DependentSchemas = n.CreateMap(LoadSchema, t, c)
            },
            {
                OpenApiConstants.IfExtension,
                (o, n, doc, c) => o.If = LoadSchema(n, doc, c)
            },
            {
                OpenApiConstants.ThenExtension,
                (o, n, doc, c) => o.Then = LoadSchema(n, doc, c)
            },
            {
                OpenApiConstants.ElseExtension,
                (o, n, doc, c) => o.Else = LoadSchema(n, doc, c)
            },
        };

        private static readonly PatternFieldMap<OpenApiSchema> _openApiSchemaPatternFields = new()
        {
            {s => s.StartsWith(OpenApiConstants.ExtensionFieldNamePrefix, StringComparison.OrdinalIgnoreCase), (o, p, n, _, c) => o.AddExtension(p, LoadExtension(p, n, c))}
        };

        public static IOpenApiSchema LoadSchema(JsonNode node, OpenApiDocument hostDocument, ParsingContext context)
        {
            var jsonObject = node.CheckMapNode(OpenApiConstants.Schema, context);

            var pointer = jsonObject.GetReferencePointer();

            if (pointer != null)
            {
                var reference = GetReferenceIdAndExternalResource(pointer);
                return new OpenApiSchemaReference(reference.Item1, hostDocument, reference.Item2);
            }

            var schema = new OpenApiSchema();

            ParseMap(jsonObject, schema, _openApiSchemaFixedFields, _openApiSchemaPatternFields, hostDocument, context);

            if (schema.Metadata?.TryGetValue(EncounteredNullableTrueMetadataKey, out var value) == true)
            {
                schema.Metadata.Remove(EncounteredNullableTrueMetadataKey);
                if (schema.Type is not null && schema.Type != 0 && value is bool isNullable && isNullable)
                {
                    schema.Type |= JsonSchemaType.Null;
                }
            }

            return schema;
        }
    }
}
