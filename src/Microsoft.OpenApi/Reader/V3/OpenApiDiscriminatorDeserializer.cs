// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;

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
                    (o, n, doc) => o.Mapping = n.CreateSimpleMap((node) => LoadMapping(node, doc))
                }
            };

        private static readonly PatternFieldMap<OpenApiDiscriminator> _discriminatorPatternFields = new() {};

        public static OpenApiDiscriminator LoadDiscriminator(ParseNode node, OpenApiDocument hostDocument)
        {
            var mapNode = node.CheckMapNode("discriminator");

            var discriminator = new OpenApiDiscriminator();
            foreach (var property in mapNode)
            {
                property.ParseField(discriminator, _discriminatorFixedFields, _discriminatorPatternFields, hostDocument);
            }

            return discriminator;
        }
        public static OpenApiSchemaReference LoadMapping(ParseNode node, OpenApiDocument hostDocument)
        {
            var pointer = node.GetScalarValue() ?? throw new InvalidOperationException("Could not get a pointer reference");
            var reference = GetReferenceIdAndExternalResource(pointer);
            return new OpenApiSchemaReference(reference.Item1, hostDocument, reference.Item2);
        }
    }
}
