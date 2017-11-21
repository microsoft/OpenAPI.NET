﻿// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using Microsoft.OpenApi.Any;
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
        private static readonly FixedFieldMap<OpenApiServerVariable> ServerVariableFixedFields =
            new FixedFieldMap<OpenApiServerVariable>
            {
                {
                    "enum", (o, n) =>
                    {
                        o.Enum = n.CreateSimpleList(s => s.GetScalarValue());
                    }
                },
                {
                    "default", (o, n) =>
                    {
                        o.Default = n.GetScalarValue();
                    }
                },
                {
                    "description", (o, n) =>
                    {
                        o.Description = n.GetScalarValue();
                    }
                },
            };

        private static readonly PatternFieldMap<OpenApiServerVariable> ServerVariablePatternFields =
            new PatternFieldMap<OpenApiServerVariable>
            {
                {s => s.StartsWith("x-"), (o, p, n) => o.AddExtension(p, n.CreateAny())}
            };

        public static OpenApiServerVariable LoadServerVariable(ParseNode node)
        {
            var mapNode = node.CheckMapNode("serverVariable");

            var serverVariable = new OpenApiServerVariable();

            ParseMap(mapNode, serverVariable, ServerVariableFixedFields, ServerVariablePatternFields);

            return serverVariable;
        }
    }
}