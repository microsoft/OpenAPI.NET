// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Microsoft.OpenApi.Reader.V2
{
    /// <summary>
    /// Class containing logic to deserialize Open API V2 document into
    /// runtime Open API object model.
    /// </summary>
    internal static partial class OpenApiV2Deserializer
    {
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
                (o, n, doc, c) =>
                {
                    o.Required = new HashSet<string>(
                        n.CreateSimpleList((n2, _) => n2.GetScalarValue(), doc, c)
                            .OfType<string>());
                }
            },
            {
                "enum",
                (o, n, _, c) => o.Enum = n.CreateListOfAny(c)
            },

            {
                "type",
                (o, n, _, _) =>
                {
                    var type = n.GetScalarValue();
                    if (type != null)
                    {
                        o.Type = type.ToJsonSchemaType();
                    }
                }
            },
            {
                "allOf",
                (o, n, t, c) => o.AllOf = n.CreateList(LoadSchema, t, c)
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
                OpenApiConstants.NullableExtension,
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
                            // We mirror the same behavior of OpenAPI 3.0 "nullable" which is a type modifier.
                            // It only has effect if there is a "Type" set.
                            // Given that we don't have a Type set yet, we want to keep track of it until the end of deserialization.
                            // If, at a later point during deserialization, Type was set, we apply nullable to it.
                            // Otherwise, we throw it away.
                            o.Metadata ??= new Dictionary<string, object>();
                            o.Metadata["encounteredNullable"] = true;
                        }
                    }
                }
            },
            {
                "discriminator", (o, n, _, _) =>
                {
                    o.Discriminator = new()
                    {
                        PropertyName = n.GetScalarValue()
                    };
                }
            },
            {
                "readOnly",
                (o, n, _, _) =>
                {
                    var readOnly = n.GetScalarValue();
                    if (readOnly is not null)
                    {
                        o.ReadOnly = bool.Parse(readOnly);
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
                OpenApiConstants.PatternPropertiesExtension,
                (o, n, t, c) => o.PatternProperties = n.CreateMap(LoadSchema, t, c)
            },
        };

        private static readonly PatternFieldMap<OpenApiSchema> _openApiSchemaPatternFields = new PatternFieldMap<OpenApiSchema>
        {
            {s => s.StartsWith(OpenApiConstants.ExtensionFieldNamePrefix, StringComparison.OrdinalIgnoreCase), (o, p, n, _, c) => o.AddExtension(p, LoadExtension(p, n, c))}
        };

        public static IOpenApiSchema LoadSchema(JsonNode node, OpenApiDocument hostDocument, ParsingContext context)
        {
            var jsonObject = node.CheckMapNode("schema", context);

            var pointer = jsonObject.GetReferencePointer();
            if (pointer != null)
            {
                var reference = GetReferenceIdAndExternalResource(pointer);
                return new OpenApiSchemaReference(reference.Item1, hostDocument, reference.Item2);
            }

            var schema = new OpenApiSchema();

            ParseMap(jsonObject, schema, _openApiSchemaFixedFields, _openApiSchemaPatternFields, hostDocument, context);

            if (schema.Metadata?.TryGetValue("encounteredNullable", out var value) == true)
            {
                schema.Metadata.Remove("encounteredNullable");
                if (schema.Type is not null && schema.Type != 0 && value is bool isNullable && isNullable)
                {
                    schema.Type |= JsonSchemaType.Null;
                }
            }

            return schema;
        }
    }
}
