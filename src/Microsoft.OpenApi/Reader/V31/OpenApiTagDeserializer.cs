// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Reader.ParseNodes;

namespace Microsoft.OpenApi.Reader.V31
{
    /// <summary>
    /// Class containing logic to deserialize Open API V31 document into
    /// runtime Open API object model.
    /// </summary>
    internal static partial class OpenApiV31Deserializer
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
            {s => s.StartsWith("x-"), (o, p, n, _) => o.AddExtension(p, LoadExtension(p,n))}
        };

        public static OpenApiTag LoadTag(ParseNode n, OpenApiDocument hostDocument = null)
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
