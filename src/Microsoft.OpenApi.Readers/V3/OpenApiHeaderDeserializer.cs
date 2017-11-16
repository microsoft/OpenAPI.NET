// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

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
        private static readonly FixedFieldMap<OpenApiHeader> HeaderFixedFields = new FixedFieldMap<OpenApiHeader>
        {
            {
                "description", (o, n) =>
                {
                    o.Description = n.GetScalarValue();
                }
            },
            {
                "required", (o, n) =>
                {
                    o.Required = bool.Parse(n.GetScalarValue());
                }
            },
            {
                "deprecated", (o, n) =>
                {
                    o.Deprecated = bool.Parse(n.GetScalarValue());
                }
            },
            {
                "allowReserved", (o, n) =>
                {
                    o.AllowReserved = bool.Parse(n.GetScalarValue());
                }
            },
            {
                "style", (o, n) =>
                {
                    o.Style = n.GetScalarValue().GetEnumFromDisplayName<ParameterStyle>();
                }
            },
            {
                "schema", (o, n) =>
                {
                    o.Schema = LoadSchema(n);
                }
            }
        };

        private static readonly PatternFieldMap<OpenApiHeader> HeaderPatternFields = new PatternFieldMap<OpenApiHeader>
        {
            {s => s.StartsWith("x-"), (o, p, n) => o.AddExtension(p, n.CreateAny())}
        };

        public static OpenApiHeader LoadHeader(ParseNode node)
        {
            var mapNode = node.CheckMapNode("header");

            var pointer = mapNode.GetReferencePointer();
            if (pointer != null)
            {
                return mapNode.GetReferencedObject<OpenApiHeader>(ReferenceType.Header, pointer);
            }

            var header = new OpenApiHeader();
            foreach (var property in mapNode)
            {
                property.ParseField(header, HeaderFixedFields, HeaderPatternFields);
            }

            return header;
        }
    }
}