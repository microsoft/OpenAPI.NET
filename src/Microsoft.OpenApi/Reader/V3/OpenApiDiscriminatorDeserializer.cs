// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Reader.ParseNodes;

namespace Microsoft.OpenApi.Reader.V3
{
    /// <summary>
    /// Class containing logic to deserialize Open API V3 document into
    /// runtime Open API object model.
    /// </summary>
    internal static partial class OpenApiV3Deserializer
    {
        private static readonly FixedFieldMap<OpenApiDiscriminator> _discriminatorFixedFields =
            new()
            {
                {
                    "propertyName",
                    (o, n, _) => o.PropertyName = n.GetScalarValue()
                },
                {
                    "mapping",
                    (o, n, _) => o.Mapping = n.CreateSimpleMap(LoadString)
                }
            };

        private static readonly PatternFieldMap<OpenApiDiscriminator> _discriminatorPatternFields = new();

        public static OpenApiDiscriminator LoadDiscriminator(ParseNode node, OpenApiDocument hostDocument = null)
        {
            var mapNode = node.CheckMapNode("discriminator");

            var discriminator = new OpenApiDiscriminator();
            foreach (var property in mapNode)
            {
                property.ParseField(discriminator, _discriminatorFixedFields, _discriminatorPatternFields);
            }

            return discriminator;
        }
    }
}
