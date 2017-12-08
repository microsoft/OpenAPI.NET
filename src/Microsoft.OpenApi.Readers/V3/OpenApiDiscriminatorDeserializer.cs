// ------------------------------------------------------------
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
        private static readonly FixedFieldMap<OpenApiDiscriminator> DiscriminatorFixedFields = 
            new FixedFieldMap<OpenApiDiscriminator>
        {
            {
                "propertyName", (o, n) =>
                {
                    o.PropertyName = n.GetScalarValue();
                }
            },
            {
                "mapping", (o, n) =>
                {
                    o.Mapping = n.CreateMap(LoadString);
                }
            }
        };

        private static readonly PatternFieldMap<OpenApiDiscriminator> DiscriminatorPatternFields =
            new PatternFieldMap<OpenApiDiscriminator>
            {
            };

        public static OpenApiDiscriminator LoadDiscriminator(ParseNode node)
        {
            var mapNode = node.CheckMapNode("discriminator");

            var discriminator = new OpenApiDiscriminator();
            foreach (var property in mapNode)
            {
                property.ParseField(discriminator, DiscriminatorFixedFields, DiscriminatorPatternFields);
            }

            return discriminator;
        }
    }
}