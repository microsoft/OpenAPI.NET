// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
            (o, n, _, c) => o.Vocabulary = n.CreateSimpleMap(LoadBool, c).ToDictionary(kvp => kvp.Key, kvp => kvp.Value ?? false)
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
            OpenApiConstants.Contains,
            (o, n, doc, c) => o.Contains = LoadSchema(n, doc, c)
        },
        {
            OpenApiConstants.MaxContains,
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
            OpenApiConstants.MinContains,
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
            "unevaluatedProperties",
            (o, n, t, c) =>
            {
                // Handle both boolean (false/true) and schema object cases
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
                var value = n.GetScalarValue();
                if (value is not null)
                {
                    var nullable = bool.Parse(value);
                    if (nullable) // if nullable, convert type into an array of type(s) and null
                    {
                        if (o.Type.HasValue)
                            o.Type |= JsonSchemaType.Null;
                        else
                            o.Type = JsonSchemaType.Null;
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
            "examples",
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
        var dynamicPointer = jsonObject.GetDynamicReferencePointer();
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

        if (dynamicPointer != null)
        {
            var nodeLocation = context.GetLocation();
            var anchorName = JsonNodeHelper.ExtractDynamicAnchorName(dynamicPointer);
            var result = new OpenApiSchemaReference(!string.IsNullOrEmpty(anchorName) ? anchorName! : dynamicPointer, hostDocument);
            var referenceMetadata = new OpenApiSchema();
            jsonObject.ParseMap(referenceMetadata, _openApiSchemaFixedFields, _openApiSchemaPatternFields, hostDocument, context,
                static (schema, name, value) =>
                {
                    if (!string.Equals(name, OpenApiConstants.DynamicRef, StringComparison.Ordinal))
                    {
                        schema.UnrecognizedKeywords ??= new Dictionary<string, JsonNode>(StringComparer.Ordinal);
                        schema.UnrecognizedKeywords[name] = value;
                    }
                });
            result.Reference.ApplySchemaMetadata(referenceMetadata, jsonObject);
            result.Reference.SetJsonPointerPath(dynamicPointer, nodeLocation);
            return result;
        }

        var schema = new OpenApiSchema();

        jsonObject.ParseMap(schema, _openApiSchemaFixedFields, _openApiSchemaPatternFields, hostDocument, context,
            static (schema, name, value) =>
            {
                schema.UnrecognizedKeywords ??= new Dictionary<string, JsonNode>(StringComparer.Ordinal);
                schema.UnrecognizedKeywords[name] = value;
            });

        if (schema.Extensions is not null && schema.Extensions.ContainsKey(OpenApiConstants.NullableExtension))
        {
            if (schema.Type.HasValue)
                schema.Type |= JsonSchemaType.Null;
            else
                schema.Type = JsonSchemaType.Null;

            schema.Extensions.Remove(OpenApiConstants.NullableExtension);
        }

        if (!string.IsNullOrEmpty(identifier) && hostDocument.Workspace is not null)
        {
            // register the schema in our registry using the identifier's URL
            hostDocument.Workspace.RegisterComponentForDocument(hostDocument, schema, identifier!);
        }

        return schema;
    }
}
