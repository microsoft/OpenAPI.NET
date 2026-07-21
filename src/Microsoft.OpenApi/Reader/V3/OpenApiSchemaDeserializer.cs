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
                (o, n, _, _) => o.IsExclusiveMaximum = bool.Parse(n.GetScalarValue()!)
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
                (o, n, _, _) => o.IsExclusiveMinimum = bool.Parse(n.GetScalarValue()!)
            },
            {
                "maxLength",
                (o, n, _, _) =>
                {
                    var maxLength = n.GetScalarValue();
                    if (maxLength != null)
                    {
                        o.MaxLength = int.Parse(maxLength, CultureInfo.InvariantCulture);
                    }
                }
            },
            {
                "minLength",
                (o, n, _, _) =>
                {
                    var minLength = n.GetScalarValue();
                    if (minLength != null)
                    {
                        o.MinLength = int.Parse(minLength, CultureInfo.InvariantCulture);
                    }
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
                    var maxItems = n.GetScalarValue();
                    if (maxItems != null)
                    {
                        o.MaxItems = int.Parse(maxItems, CultureInfo.InvariantCulture);
                    }
                }
            },
            {
                "minItems",
                (o, n, _, _) =>
                {
                    var minItems = n.GetScalarValue();
                    if (minItems != null)
                    {
                        o.MinItems = int.Parse(minItems, CultureInfo.InvariantCulture);
                    }
                }
            },
            {
                "uniqueItems",
                (o, n, _, _) =>
                {
                    var uniqueItems = n.GetScalarValue();
                    if (uniqueItems != null)
                    {
                        o.UniqueItems = bool.Parse(uniqueItems);
                    }
                }
            },
            {
                "maxProperties",
                (o, n, _, _) =>
                {
                    var maxProps = n.GetScalarValue();
                    if (maxProps != null)
                    {
                        o.MaxProperties = int.Parse(maxProps, CultureInfo.InvariantCulture);
                    }
                }
            },
            {
                "minProperties",
                (o, n, _, _) =>
                {
                    var minProps = n.GetScalarValue();
                    if (minProps != null)
                    {
                        o.MinProperties = int.Parse(minProps, CultureInfo.InvariantCulture);
                    }
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
                        var value = n.GetScalarValue();
                        if (value is not null)
                        {
                            o.AdditionalPropertiesAllowed = bool.Parse(value);
                        }
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
                    if (bool.TryParse(n.GetScalarValue(), out var parsed) && parsed)
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
                    var readOnly = n.GetScalarValue();
                    if (readOnly != null)
                    {
                        o.ReadOnly = bool.Parse(readOnly);
                    }
                }
            },
            {
                "writeOnly",
                (o, n, _, _) =>
                {
                    var writeOnly = n.GetScalarValue();
                    if (writeOnly != null)
                    {
                        o.WriteOnly = bool.Parse(writeOnly);
                    }
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
                    var deprecated = n.GetScalarValue();
                    if (deprecated != null)
                    {
                        o.Deprecated = bool.Parse(deprecated);
                    }
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
                        var value = n.GetScalarValue();
                        if (value is not null)
                        {
                            o.UnevaluatedProperties = bool.Parse(value);
                        }
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
                    var maxContains = n.GetScalarValue();
                    if (maxContains != null)
                    {
                        o.MaxContains = uint.Parse(maxContains, CultureInfo.InvariantCulture);
                    }
                }
            },
            {
                OpenApiConstants.MinContainsExtension,
                (o, n, _, _) =>
                {
                    var minContains = n.GetScalarValue();
                    if (minContains != null)
                    {
                        o.MinContains = uint.Parse(minContains, CultureInfo.InvariantCulture);
                    }
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

            // The object model represents the latest version of the spec.
            // https://spec.openapis.org/oas/v3.2.0.html#migrating-binary-descriptions-from-oas-3-0
            // When we deserializing from V2, we detect the "old way" of specifying binary descriptions, and
            // transform it in the object model to the latest thing.
            if (schema.Type.HasValue && schema.Type.Value.HasFlag(JsonSchemaType.String) &&
                schema.Format == "byte" &&
                schema.ContentEncoding is null)
            {
                schema.ContentEncoding = "base64";
                schema.Format = null;
            }

            if (schema.Type.HasValue && schema.Type.Value == JsonSchemaType.String &&
                schema.Format == "binary")
            {
                schema.ContentMediaType ??= "application/octet-stream";
                schema.Format = null;
                schema.Type = null;
            }

            return schema;
        }
    }
}
