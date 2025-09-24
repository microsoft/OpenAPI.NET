// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;

namespace Microsoft.OpenApi.Reader.V32
{
    /// <summary>
    /// Class containing logic to deserialize Open API V32 document into
    /// runtime Open API object model.
    /// </summary>
    internal static partial class OpenApiV32Deserializer
    {
        private static readonly FixedFieldMap<OpenApiTag> _tagFixedFields = new()
        {
            {
                OpenApiConstants.Name, (o, n, _) =>
                {
                    o.Name = n.GetScalarValue();
                }
            },
            {
                OpenApiConstants.Description, (o, n, _) =>
                {
                    o.Description = n.GetScalarValue();
                }
            },
            {
                OpenApiConstants.ExternalDocs, (o, n, t) =>
                {
                    o.ExternalDocs = LoadExternalDocs(n, t);
                }
            }
        };

        private static readonly PatternFieldMap<OpenApiTag> _tagPatternFields = new()
        {
            {s => s.StartsWith(OpenApiConstants.ExtensionFieldNamePrefix, StringComparison.OrdinalIgnoreCase), (o, p, n, _) => o.AddExtension(p, LoadExtension(p,n))}
        };

        public static OpenApiTag LoadTag(ParseNode n, OpenApiDocument hostDocument)
        {
            var mapNode = n.CheckMapNode("tag");

            var domainObject = new OpenApiTag();

            foreach (var propertyNode in mapNode)
            {
                propertyNode.ParseField(domainObject, _tagFixedFields, _tagPatternFields, hostDocument);
            }

            return domainObject;
        }
    }
}

