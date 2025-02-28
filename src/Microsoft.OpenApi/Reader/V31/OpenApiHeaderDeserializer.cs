﻿using System;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Models.Interfaces;
using Microsoft.OpenApi.Models.References;
using Microsoft.OpenApi.Reader.ParseNodes;

namespace Microsoft.OpenApi.Reader.V31
{
    /// <summary>
    /// Class containing logic to deserialize Open API V31 document into
    /// runtime Open API object model.
    /// </summary>
    internal static partial class OpenApiV31Deserializer
    {
        private static readonly FixedFieldMap<OpenApiHeader> _headerFixedFields = new()
        {
            {
                "description", (o, n, _) =>
                {
                    o.Description = n.GetScalarValue();
                }
            },
            {
                "required", (o, n, _) =>
                {
                    o.Required = bool.Parse(n.GetScalarValue());
                }
            },
            {
                "deprecated", (o, n, _) =>
                {
                    o.Deprecated = bool.Parse(n.GetScalarValue());
                }
            },
            {
                "allowEmptyValue", (o, n, _) =>
                {
                    o.AllowEmptyValue = bool.Parse(n.GetScalarValue());
                }
            },
            {
                "allowReserved", (o, n, _) =>
                {
                    o.AllowReserved = bool.Parse(n.GetScalarValue());
                }
            },
            {
                "style", (o, n, _) =>
                {
                    if(!n.GetScalarValue().TryGetEnumFromDisplayName<ParameterStyle>(n.Context, out var style))
                    {
                        return;
                    }
                    o.Style = style;
                }
            },
            {
                "explode", (o, n, _) =>
                {
                    o.Explode = bool.Parse(n.GetScalarValue());
                }
            },
            {
                "schema", (o, n, t) =>
                {
                    o.Schema = LoadSchema(n, t);
                }
            },
            {
                "examples", (o, n, t) =>
                {
                    o.Examples = n.CreateMap(LoadExample, t);
                }
            },
            {
                "example", (o, n, _) =>
                {
                    o.Example = n.CreateAny();
                }
            },
        };

        private static readonly PatternFieldMap<OpenApiHeader> _headerPatternFields = new()
        {
            {s => s.StartsWith(OpenApiConstants.ExtensionFieldNamePrefix, StringComparison.OrdinalIgnoreCase), (o, p, n, _) => o.AddExtension(p, LoadExtension(p,n))}
        };

        public static IOpenApiHeader LoadHeader(ParseNode node, OpenApiDocument hostDocument)
        {
            var mapNode = node.CheckMapNode("header");

            var pointer = mapNode.GetReferencePointer();
            if (pointer != null)
            {
                var reference = GetReferenceIdAndExternalResource(pointer);
                return new OpenApiHeaderReference(reference.Item1, hostDocument, reference.Item2);
            }

            var header = new OpenApiHeader();
            foreach (var property in mapNode)
            {
                property.ParseField(header, _headerFixedFields, _headerPatternFields, hostDocument);
            }

            return header;
        }
    }
}
