// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Microsoft.OpenApi.Reader.V3
{
    /// <summary>
    /// Class containing logic to deserialize Open API V3 document into
    /// runtime Open API object model.
    /// </summary>
    internal static partial class OpenApiV3Deserializer
    {
        private static readonly IOpenApiExtension _nullableTrueExtension = new JsonNodeExtension(JsonValue.Create(true));

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
                            // While we are dealing with v3 document, we are still using x-nullable extension here.
                            // This is used only as a marker during deserialization to indicate that the schema is nullable.
                            // When we do FinalizeDeserialization, we will modify the OpenApiSchema.Type to
                            // include JsonSchemaType.Null (only if needed), and always remove the extension.
                            // The reason is that "nullable" property should only take effect if "Type" is set.
                            // If we knew Type is set, we add "Null" to it.
                            // Otherwise, it might be that it will be set later.
                            // In this case, we add the marker extension, and check at the end of deserialization, and remove the extension.
                            o.AddExtension(OpenApiConstants.NullableExtension, _nullableTrueExtension);
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

            schema.FinalizeDeserialization(OpenApiSpecVersion.OpenApi3_0);

            return schema;
        }
    }
}
