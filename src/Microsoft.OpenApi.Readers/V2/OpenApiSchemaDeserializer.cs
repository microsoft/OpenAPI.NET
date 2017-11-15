﻿// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers.ParseNodes;

namespace Microsoft.OpenApi.Readers.V2
{
    /// <summary>
    /// Class containing logic to deserialize Open API V2 document into
    /// runtime Open API object model.
    /// </summary>
    internal static partial class OpenApiV2Deserializer
    {
        private static readonly FixedFieldMap<OpenApiSchema> SchemaFixedFields = new FixedFieldMap<OpenApiSchema>
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
                    o.MultipleOf = decimal.Parse(n.GetScalarValue());
                }
            },
            {
                "maximum", (o, n) =>
                {
                    o.Maximum = decimal.Parse(n.GetScalarValue());
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
                    o.Minimum = decimal.Parse(n.GetScalarValue());
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
                    o.MaxLength = int.Parse(n.GetScalarValue());
                }
            },
            {
                "minLength", (o, n) =>
                {
                    o.MinLength = int.Parse(n.GetScalarValue());
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
                    o.MaxItems = int.Parse(n.GetScalarValue());
                }
            },
            {
                "minItems", (o, n) =>
                {
                    o.MinItems = int.Parse(n.GetScalarValue());
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
                    o.MaxProperties = int.Parse(n.GetScalarValue());
                }
            },
            {
                "minProperties", (o, n) =>
                {
                    o.MinProperties = int.Parse(n.GetScalarValue());
                }
            },
            {
                "required", (o, n) =>
                {
                    o.Required = n.CreateSimpleList(n2 => n2.GetScalarValue()).ToArray();
                }
            },
            {
                "enum", (o, n) =>
                {
                    o.Enum = n.CreateSimpleList<IOpenApiAny>(s => new OpenApiString(s.GetScalarValue()));
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
                    o.AdditionalProperties = LoadSchema(n);
                }
            },
            {
                "description", (o, n) =>
                {
                    o.Type = n.GetScalarValue();
                }
            },
            {
                "format", (o, n) =>
                {
                    o.Description = n.GetScalarValue();
                }
            },
            {
                "default", (o, n) =>
                {
                    o.Default = new OpenApiString(n.GetScalarValue());
                }
            },

            // discriminator
            {
                "readOnly", (o, n) =>
                {
                    o.ReadOnly = bool.Parse(n.GetScalarValue());
                }
            },
            // xml
            {
                "externalDocs", (o, n) =>
                {
                    o.ExternalDocs = LoadExternalDocs(n);
                }
            },
            {
                "example", (o, n) =>
                {
                    o.Example = new OpenApiString(n.GetScalarValue());
                }
            },
        };

        private static readonly PatternFieldMap<OpenApiSchema> SchemaPatternFields = new PatternFieldMap<OpenApiSchema>
        {
            {s => s.StartsWith("x-"), (o, p, n) => o.AddExtension(p, n.CreateAny())}
        };

        public static OpenApiSchema LoadSchema(ParseNode node)
        {
            var mapNode = node.CheckMapNode("schema");

            var pointer = mapNode.GetReferencePointer();
            if (pointer != null)
            {
                return mapNode.GetReferencedObject<OpenApiSchema>(ReferenceType.Schema, pointer);
            }

            var domainObject = new OpenApiSchema();

            foreach (var propertyNode in mapNode)
            {
                propertyNode.ParseField(domainObject, SchemaFixedFields, SchemaPatternFields);
            }

            return domainObject;
        }
    }
}