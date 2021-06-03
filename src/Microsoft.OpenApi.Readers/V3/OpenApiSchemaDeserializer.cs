﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers.ParseNodes;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.OpenApi.Readers.V3
{
    /// <summary>
    /// Class containing logic to deserialize Open API V3 document into
    /// runtime Open API object model.
    /// </summary>
    internal static partial class OpenApiV3Deserializer
    {
        private static readonly FixedFieldMap<OpenApiSchema> _schemaFixedFields = new FixedFieldMap<OpenApiSchema>
        {
            {
                "title", (o, n) =>
                {
                    o.Title = n.GetScalarValue();
                }
            },
            {
                "multipleOf", (o, n) =>
                {
                    o.MultipleOf = decimal.Parse(n.GetScalarValue(), NumberStyles.Float, CultureInfo.InvariantCulture); 
                }
            },
            {
                "maximum", (o, n) =>
                {
                    o.Maximum = decimal.Parse(n.GetScalarValue(), NumberStyles.Float, CultureInfo.InvariantCulture);
                }
            },
            {
                "exclusiveMaximum", (o, n) =>
                {
                    o.ExclusiveMaximum = bool.Parse(n.GetScalarValue());
                }
            },
            {
                "minimum", (o, n) =>
                {
                    o.Minimum = decimal.Parse(n.GetScalarValue(), NumberStyles.Float, CultureInfo.InvariantCulture);
                }
            },
            {
                "exclusiveMinimum", (o, n) =>
                {
                    o.ExclusiveMinimum = bool.Parse(n.GetScalarValue());
                }
            },
            {
                "maxLength", (o, n) =>
                {
                    o.MaxLength = int.Parse(n.GetScalarValue(), CultureInfo.InvariantCulture);
                }
            },
            {
                "minLength", (o, n) =>
                {
                    o.MinLength = int.Parse(n.GetScalarValue(), CultureInfo.InvariantCulture);
                }
            },
            {
                "pattern", (o, n) =>
                {
                    o.Pattern = n.GetScalarValue();
                }
            },
            {
                "maxItems", (o, n) =>
                {
                    o.MaxItems = int.Parse(n.GetScalarValue(), CultureInfo.InvariantCulture);
                }
            },
            {
                "minItems", (o, n) =>
                {
                    o.MinItems = int.Parse(n.GetScalarValue(), CultureInfo.InvariantCulture);
                }
            },
            {
                "uniqueItems", (o, n) =>
                {
                    o.UniqueItems = bool.Parse(n.GetScalarValue());
                }
            },
            {
                "maxProperties", (o, n) =>
                {
                    o.MaxProperties = int.Parse(n.GetScalarValue(), CultureInfo.InvariantCulture);
                }
            },
            {
                "minProperties", (o, n) =>
                {
                    o.MinProperties = int.Parse(n.GetScalarValue(), CultureInfo.InvariantCulture);
                }
            },
            {
                "required", (o, n) =>
                {
                    o.Required = new HashSet<string>(n.CreateSimpleList(n2 => n2.GetScalarValue()));
                }
            },
            {
                "enum", (o, n) =>
                {
                    o.Enum = n.CreateListOfAny();
                }
            },
            {
                "type", (o, n) =>
                {
                    o.Type = n.GetScalarValue();
                }
            },
            {
                "allOf", (o, n) =>
                {
                    o.AllOf = n.CreateList(LoadSchema);
                }
            },
            {
                "oneOf", (o, n) =>
                {
                    o.OneOf = n.CreateList(LoadSchema);
                }
            },
            {
                "anyOf", (o, n) =>
                {
                    o.AnyOf = n.CreateList(LoadSchema);
                }
            },
            {
                "not", (o, n) =>
                {
                    o.Not = LoadSchema(n);
                }
            },
            {
                "items", (o, n) =>
                {
                    o.Items = LoadSchema(n);
                }
            },
            {
                "properties", (o, n) =>
                {
                    o.Properties = n.CreateMap(LoadSchema);
                }
            },
            {
                "additionalProperties", (o, n) =>
                {
                    if (n is ValueNode)
                    {
                        o.AdditionalPropertiesAllowed = bool.Parse(n.GetScalarValue());
                    }
                    else
                    {
                        o.AdditionalProperties = LoadSchema(n);
                    }
                }
            },
            {
                "description", (o, n) =>
                {
                    o.Description = n.GetScalarValue();
                }
            },
            {
                "format", (o, n) =>
                {
                    o.Format = n.GetScalarValue();
                }
            },
            {
                "default", (o, n) =>
                {
                    o.Default = n.CreateAny();
                }
            },

            {
                "nullable", (o, n) =>
                {
                    o.Nullable = bool.Parse(n.GetScalarValue());
                }
            },
            {
                "discriminator", (o, n) =>
                {
                    o.Discriminator = LoadDiscriminator(n);
                }
            },
            {
                "readOnly", (o, n) =>
                {
                    o.ReadOnly = bool.Parse(n.GetScalarValue());
                }
            },
            {
                "writeOnly", (o, n) =>
                {
                    o.WriteOnly = bool.Parse(n.GetScalarValue());
                }
            },
            {
                "xml", (o, n) =>
                {
                    o.Xml = LoadXml(n);
                }
            },
            {
                "externalDocs", (o, n) =>
                {
                    o.ExternalDocs = LoadExternalDocs(n);
                }
            },
            {
                "example", (o, n) =>
                {
                    o.Example = n.CreateAny();
                }
            },
            {
                "deprecated", (o, n) =>
                {
                    o.Deprecated = bool.Parse(n.GetScalarValue());
                }
            },
        };

        private static readonly PatternFieldMap<OpenApiSchema> _schemaPatternFields = new PatternFieldMap<OpenApiSchema>
        {
            {s => s.StartsWith("x-"), (o, p, n) => o.AddExtension(p, LoadExtension(p,n))}
        };

        private static readonly AnyFieldMap<OpenApiSchema> _schemaAnyFields = new AnyFieldMap<OpenApiSchema>
        {
            {
                OpenApiConstants.Default,
                new AnyFieldMapParameter<OpenApiSchema>(
                    s => s.Default,
                    (s, v) => s.Default = v,
                    s => s)
            },
            {
                 OpenApiConstants.Example,
                new AnyFieldMapParameter<OpenApiSchema>(
                    s => s.Example,
                    (s, v) => s.Example = v,
                    s => s)
            }
        };

        private static readonly AnyListFieldMap<OpenApiSchema> _schemaAnyListFields = new AnyListFieldMap<OpenApiSchema>
        {
            {
                OpenApiConstants.Enum,
                new AnyListFieldMapParameter<OpenApiSchema>(
                    s => s.Enum,
                    (s, v) => s.Enum = v,
                    s => s)
            }
        };

        public static OpenApiSchema LoadSchema(ParseNode node)
        {
            var mapNode = node.CheckMapNode(OpenApiConstants.Schema);

            var pointer = mapNode.GetReferencePointer();

            if (pointer != null)
            {
                return new OpenApiSchema()
                {
                    UnresolvedReference = true,
                    Reference = node.Context.VersionService.ConvertToOpenApiReference(pointer, ReferenceType.Schema)
                };
            }

            var schema = new OpenApiSchema();

            foreach (var propertyNode in mapNode)
            {
                propertyNode.ParseField(schema, _schemaFixedFields, _schemaPatternFields);
            }

            ProcessAnyFields(mapNode, schema, _schemaAnyFields);
            ProcessAnyListFields(mapNode, schema, _schemaAnyListFields);

            return schema;
        }
    }
}
