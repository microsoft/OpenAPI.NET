// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Linq;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers.ParseNodes;

namespace Microsoft.OpenApi.Readers.V3
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
                    (o, n) => o.Name = n.GetScalarValue()
                },
                {
                    "in", (o, n) =>
                    {
                        var inString = n.GetScalarValue();

                        if ( Enum.GetValues(typeof(ParameterLocation)).Cast<ParameterLocation>()
                            .Select( e => e.GetDisplayName() )
                            .Contains(inString) )
                        {
                            o.In = n.GetScalarValue().GetEnumFromDisplayName<ParameterLocation>();
                        }
                        else
                        {
                            o.In = null;
                        }
                    }
                },
                {
                    "description",
                    (o, n) => o.Description = n.GetScalarValue()
                },
                {
                    "required",
                    (o, n) => o.Required = bool.Parse(n.GetScalarValue())
                },
                {
                    "deprecated",
                    (o, n) => o.Deprecated = bool.Parse(n.GetScalarValue())
                },
                {
                    "allowEmptyValue",
                    (o, n) => o.AllowEmptyValue = bool.Parse(n.GetScalarValue())
                },
                {
                    "allowReserved",
                    (o, n) => o.AllowReserved = bool.Parse(n.GetScalarValue())
                },
                {
                    "style",
                    (o, n) => o.Style = n.GetScalarValue().GetEnumFromDisplayName<ParameterStyle>()
                },
                {
                    "explode",
                    (o, n) => o.Explode = bool.Parse(n.GetScalarValue())
                },
                {
                    "schema",
                    (o, n) => o.Schema = LoadSchema(n)
                },
                {
                    "content",
                    (o, n) => o.Content = n.CreateMap(LoadMediaType)
                },
                {
                    "examples",
                    (o, n) => o.Examples = n.CreateMap(LoadExample)
                },
                {
                    "example",
                    (o, n) => o.Example = n.CreateAny()
                },
            };

        private static readonly PatternFieldMap<OpenApiParameter> _parameterPatternFields =
            new()
            {
                {s => s.StartsWith("x-"), (o, p, n) => o.AddExtension(p, LoadExtension(p,n))}
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

        private static readonly AnyMapFieldMap<OpenApiParameter, OpenApiExample> _parameterAnyMapOpenApiExampleFields =
            new()
            {
            {
                OpenApiConstants.Examples,
                new(
                    m => m.Examples,
                    e => e.Value,
                    (e, v) => e.Value = v,
                    m => m.Schema)
            }
        };

        public static OpenApiParameter LoadParameter(ParseNode node)
        {
            var mapNode = node.CheckMapNode("parameter");

            var pointer = mapNode.GetReferencePointer();
            if (pointer != null)
            {
                return mapNode.GetReferencedObject<OpenApiParameter>(ReferenceType.Parameter, pointer);
            }

            var parameter = new OpenApiParameter();

            ParseMap(mapNode, parameter, _parameterFixedFields, _parameterPatternFields);
            ProcessAnyFields(mapNode, parameter, _parameterAnyFields);
            ProcessAnyMapFields(mapNode, parameter, _parameterAnyMapOpenApiExampleFields);

            return parameter;
        }
    }
}
