// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

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
        private static readonly FixedFieldMap<OpenApiTag> _tagFixedFields = new FixedFieldMap<OpenApiTag>
        {
            {
                OpenApiConstants.Name, (o, n) =>
                {
                    o.Name = n.GetScalarValue();
                }
            },
            {
                OpenApiConstants.Description, (o, n) =>
                {
                    o.Description = n.GetScalarValue();
                }
            },
            {
                OpenApiConstants.ExternalDocs, (o, n) =>
                {
                    o.ExternalDocs = LoadExternalDocs(n);
                }
            }
        };

        private static readonly PatternFieldMap<OpenApiTag> _tagPatternFields = new PatternFieldMap<OpenApiTag>
        {
            {s => s.StartsWith("x-"), (o, p, n) => o.AddExtension(p, LoadExtension(p,n))}
        };

        public static OpenApiTag LoadTag(ParseNode n)
        {
            var mapNode = n.CheckMapNode("tag");

            var domainObject = new OpenApiTag();

            foreach (var propertyNode in mapNode)
            {
                propertyNode.ParseField(domainObject, _tagFixedFields, _tagPatternFields);
            }

            return domainObject;
        }
    }
}