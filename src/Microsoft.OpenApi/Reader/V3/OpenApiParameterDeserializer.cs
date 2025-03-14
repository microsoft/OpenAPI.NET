// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Models.Interfaces;
using Microsoft.OpenApi.Models.References;
using Microsoft.OpenApi.Reader.ParseNodes;

namespace Microsoft.OpenApi.Reader.V3
{
    /// <summary>
    /// Class containing logic to deserialize Open API V3 document into
    /// runtime Open API object model.
    /// </summary>
    internal static partial class OpenApiV3Deserializer
    {
        private static readonly FixedFieldMap<OpenApiParameter> _parameterFixedFields =
            new()
            {
                {
                    "name",
                    (o, n,_) => o.Name = n.GetScalarValue()
                },
                {
                    "in", (o, n, _) =>
                    {
                        if (!n.GetScalarValue().TryGetEnumFromDisplayName<ParameterLocation>(n.Context, out var _in))
                        {
                            return;
                        }
                        o.In = _in;
                    }
                },
                {
                    "description",
                    (o, n, _) => o.Description = n.GetScalarValue()
                },
                {
                    "required",
                    (o, n, t) =>
                    {
                        var required = n.GetScalarValue();
                        if (required != null)
                        {
                            o.Required = bool.Parse(required);
                        }
                    }
                },
                {
                    "deprecated",
                    (o, n, t) =>
                    {
                        var deprecated = n.GetScalarValue();
                        if (deprecated != null)
                        {
                            o.Deprecated = bool.Parse(deprecated);
                        }
                    }
                },
                {
                    "allowEmptyValue",
                    (o, n, t) =>
                    {
                        var allowEmptyValue = n.GetScalarValue();
                        if (allowEmptyValue != null)
                        {
                            o.AllowEmptyValue = bool.Parse(allowEmptyValue);
                        }
                    }
                },
                {
                    "allowReserved",
                    (o, n, _) =>
                    {
                        var allowReserved = n.GetScalarValue();
                        if (allowReserved != null)
                        {
                            o.AllowReserved = bool.Parse(allowReserved);
                        }
                    }
                },
                {
                    "style",
                    (o, n, _) => 
                    {
                        if (!n.GetScalarValue().TryGetEnumFromDisplayName<ParameterStyle>(n.Context, out var style))
                        {
                            return;
                        }
                        o.Style = style;
                    }
                },
                {
                    "explode",
                    (o, n, _) =>
                    {
                        var explode = n.GetScalarValue();
                        if (explode != null)
                        {
                            o.Explode = bool.Parse(explode);
                        }
                    }
                },
                {
                    "schema",
                    (o, n, t) => o.Schema = LoadSchema(n, t)
                },
                {
                    "content",
                    (o, n, t) => o.Content = n.CreateMap(LoadMediaType, t)
                },
                {
                    "examples",
                    (o, n, t) => o.Examples = n.CreateMap(LoadExample, t)
                },
                {
                    "example",
                    (o, n, _) => o.Example = n.CreateAny()
                },
            };

        private static readonly PatternFieldMap<OpenApiParameter> _parameterPatternFields =
            new()
            {
                {s => s.StartsWith(OpenApiConstants.ExtensionFieldNamePrefix, StringComparison.OrdinalIgnoreCase), (o, p, n, _) => o.AddExtension(p, LoadExtension(p,n))}
            };

        private static readonly AnyFieldMap<OpenApiParameter> _parameterAnyFields = new()
        {
            {
                OpenApiConstants.Example,
                new(
                    s => s.Example,
                    (s, v) => s.Example = v,
                    s => s.Schema)
            }
        };

        private static readonly AnyMapFieldMap<OpenApiParameter, IOpenApiExample> _parameterAnyMapOpenApiExampleFields =
            new()
            {
            {
                OpenApiConstants.Examples,
                new(
                    m => m.Examples,
                    e => e.Value,
                    (e, v) => {if (e is OpenApiExample ex) {ex.Value = v;}},
                    m => m.Schema)
            }
        };

        public static IOpenApiParameter LoadParameter(ParseNode node, OpenApiDocument hostDocument)
        {
            var mapNode = node.CheckMapNode("parameter");

            var pointer = mapNode.GetReferencePointer();
            if (pointer != null)
            {
                var reference = GetReferenceIdAndExternalResource(pointer);
                return new OpenApiParameterReference(reference.Item1, hostDocument, reference.Item2);
            }

            var parameter = new OpenApiParameter();

            ParseMap(mapNode, parameter, _parameterFixedFields, _parameterPatternFields, hostDocument);
            ProcessAnyFields(mapNode, parameter, _parameterAnyFields);
            ProcessAnyMapFields(mapNode, parameter, _parameterAnyMapOpenApiExampleFields);

            return parameter;
        }
    }
}
