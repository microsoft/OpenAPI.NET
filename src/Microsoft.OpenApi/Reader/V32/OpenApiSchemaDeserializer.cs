// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Microsoft.OpenApi.Reader.V32;

internal static partial class OpenApiV32Deserializer
{
    private static readonly FixedFieldMap<OpenApiSchema> _openApiSchemaFixedFields = new()
    {
        {
            "title",
            (o, n, _, _) => o.Title = n.GetScalarValue()
        },
        {
            "$schema",
            (o, n, _, _) => { if (n.GetScalarValue() is string {} sSchema && Uri.TryCreate(sSchema, UriKind.Absolute, out var schema)) {o.Schema = schema;}}
        },
        {
            "$id",
            (o, n, _, _) => o.Id = n.GetScalarValue()
        },
        {
            "$comment",
            (o, n, _, _) => o.Comment = n.GetScalarValue()
        },
        {
            "$vocabulary",
            (o, n, _, c) => o.Vocabulary = n.CreateSimpleMap(LoadBool, c).ToDictionary(kvp => kvp.Key, kvp => kvp.Value)
        },
        {
            "$dynamicRef",
            (o, n, _, _) => o.DynamicRef = n.GetScalarValue()
        },
        {
            "$dynamicAnchor",
            (o, n, _, _) => o.DynamicAnchor = n.GetScalarValue()
        },
        {
            "$defs",
            (o, n, t, c) => o.Definitions = n.CreateMap(LoadSchema, t, c)
        },
        {
            OpenApiConstants.Anchor,
            (o, n, _, _) => o.Anchor = n.GetScalarValue()
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
            (o, n, _, _) => o.ExclusiveMaximum = n.GetScalarValue()
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
            (o, n, _, _) => o.ExclusiveMinimum = n.GetScalarValue()
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
            OpenApiConstants.Contains,
            (o, n, doc, c) => o.Contains = LoadSchema(n, doc, c)
        },
        {
            OpenApiConstants.MaxContains,
            (o, n, _, _) =>
            {
                o.MaxContains = n.GetScalarUIntValue();
            }
        },
        {
            OpenApiConstants.MinContains,
            (o, n, _, _) =>
            {
                o.MinContains = n.GetScalarUIntValue();
            }
        },
        {
            "unevaluatedProperties",
            (o, n, t, c) =>
            {
                // Handle both boolean (false/true) and schema object cases
                if (n is JsonValue)
                {
                    o.UnevaluatedProperties = n.GetScalarBoolValue();
                }
                else
                {
                    // Schema object case: deserialize as schema
                    o.UnevaluatedPropertiesSchema = LoadSchema(n, t, c);
                }
            }
        },
        {
            OpenApiConstants.ContentEncoding,
            (o, n, _, _) => o.ContentEncoding = n.GetScalarValue()
        },
        {
            OpenApiConstants.ContentMediaType,
            (o, n, _, _) => o.ContentMediaType = n.GetScalarValue()
        },
        {
            OpenApiConstants.ContentSchema,
            (o, n, doc, c) => o.ContentSchema = LoadSchema(n, doc, c)
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
            (o, n, doc, c) =>
            {
                // Preserve any Null flag set by a preceding "nullable: true" handler
                var preserveNull = o.Type.HasValue && o.Type.Value.HasFlag(JsonSchemaType.Null);
                if (n is JsonValue)
                {
                    var parsedType = n.GetScalarValue()?.ToJsonSchemaType();
                    o.Type = preserveNull ? parsedType | JsonSchemaType.Null : parsedType;
                }
                else
                {
                    var list = n.CreateSimpleList((n2, _) => n2.GetScalarValue(), doc, c);
                    JsonSchemaType combinedType = preserveNull ? JsonSchemaType.Null : 0;
                    foreach(var type in list.Where(static t => t is not null).Select(static t => t!.ToJsonSchemaType()))
                    {
                        combinedType |= type;
                    }
                    o.Type = combinedType;
                }
            }
        },
        {
            "const",
            (o, n, _, _) => o.Const = n.GetScalarValue()
        },
        {
            "allOf",
            (o, n, t, c) => o.AllOf = n.CreateList(LoadSchema, t, c)
        },
        {
            "oneOf",
            (o, n, t, c) => o.OneOf = n.CreateList(LoadSchema, t, c)
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
            "patternProperties",
            (o, n, t, c) => o.PatternProperties = n.CreateMap(LoadSchema, t, c)
        },
        {
            OpenApiConstants.PropertyNames,
            (o, n, doc, c) => o.PropertyNames = LoadSchema(n, doc, c)
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
            "examples",
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
            "dependentRequired",
            (o, n, doc, c) =>
            {
                o.DependentRequired = n.CreateArrayMap((n2, _) => n2.GetScalarValue()!, doc, c);
            }
        },
        {
            OpenApiConstants.DependentSchemas,
            (o, n, t, c) => o.DependentSchemas = n.CreateMap(LoadSchema, t, c)
        },
        {
            OpenApiConstants.If,
            (o, n, doc, c) => o.If = LoadSchema(n, doc, c)
        },
        {
            OpenApiConstants.Then,
            (o, n, doc, c) => o.Then = LoadSchema(n, doc, c)
        },
        {
            OpenApiConstants.Else,
            (o, n, doc, c) => o.Else = LoadSchema(n, doc, c)
        },
    };

    private static readonly PatternFieldMap<OpenApiSchema> _openApiSchemaPatternFields = new()
    {
        {s => s.StartsWith(OpenApiConstants.ExtensionFieldNamePrefix, StringComparison.OrdinalIgnoreCase), (o, p, n, _, c) => o.AddExtension(p, LoadExtension(p, n, c))}
    };

    public static IOpenApiSchema LoadSchema(JsonNode node, OpenApiDocument hostDocument, ParsingContext context)
    {
        // Handle boolean schemas (true/false) for JSON Schema 2020-12 compatibility
        if (node is JsonValue jsonValue && jsonValue.TryGetValue<bool>(out var boolValue))
        {
            var boolSchema = new OpenApiSchema();
            if (!boolValue)
            {
                // false schema: represents "not valid" -> convert to "not: {}"
                boolSchema.Not = new OpenApiSchema();
            }
            // true schema: represents "always valid" -> return empty schema (default)
            return boolSchema;
        }

        var jsonObject = node.CheckMapNode(OpenApiConstants.Schema, context);

        var pointer = jsonObject.GetReferencePointer();
        var identifier = jsonObject.GetJsonSchemaIdentifier();

        if (pointer != null)
        {
            var nodeLocation = context.GetLocation();
            var reference = GetReferenceIdAndExternalResource(pointer);
            var result = new OpenApiSchemaReference(reference.Item1, hostDocument, reference.Item2);
            var referenceMetadata = new OpenApiSchema();
            jsonObject.ParseMap(referenceMetadata, _openApiSchemaFixedFields, _openApiSchemaPatternFields, hostDocument, context,
                static (schema, name, value) =>
                {
                    if (!string.Equals(name, OpenApiConstants.DollarRef, StringComparison.Ordinal))
                    {
                        schema.UnrecognizedKeywords ??= new Dictionary<string, JsonNode>(StringComparer.Ordinal);
                        schema.UnrecognizedKeywords[name] = value;
                    }
                });
            result.Reference.ApplySchemaMetadata(referenceMetadata, jsonObject);
            result.Reference.SetJsonPointerPath(pointer, nodeLocation);

            return result;
        }

        var schema = new OpenApiSchema();

        jsonObject.ParseMap(schema, _openApiSchemaFixedFields, _openApiSchemaPatternFields, hostDocument, context,
            static (schema, name, value) =>
            {
                schema.UnrecognizedKeywords ??= new Dictionary<string, JsonNode>(StringComparer.Ordinal);
                schema.UnrecognizedKeywords[name] = value;
            });

        if (!string.IsNullOrEmpty(identifier) && hostDocument.Workspace is not null)
        {
            // register the schema in our registry using the identifier's URL
            hostDocument.Workspace.RegisterComponentForDocument(hostDocument, schema, identifier!);
        }

        return schema;
    }
}
