using System;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Models.Interfaces;
using Microsoft.OpenApi.Models.References;
using Microsoft.OpenApi.Reader.ParseNodes;

namespace Microsoft.OpenApi.Reader.V31
{
    /// <summary>
    /// Class containing logic to deserialize Open API V31 document into
    /// runtime Open API object model.
    /// </summary>
    internal static partial class OpenApiV31Deserializer
    {
        private static readonly FixedFieldMap<OpenApiExample> _exampleFixedFields = new()
        {
            {
                "summary", (o, n, _) =>
                {
                    o.Summary = n.GetScalarValue();
                }
            },
            {
                "description", (o, n, _) =>
                {
                    o.Description = n.GetScalarValue();
                }
            },
            {
                "value", (o, n, _) =>
                {
                    o.Value = n.CreateAny();
                }
            },
            {
                "externalValue", (o, n, _) =>
                {
                    o.ExternalValue = n.GetScalarValue();
                }
            },

        };

        private static readonly PatternFieldMap<OpenApiExample> _examplePatternFields =
            new()
            {
                {s => s.StartsWith(OpenApiConstants.ExtensionFieldNamePrefix, StringComparison.OrdinalIgnoreCase), (o, p, n, _) => o.AddExtension(p, LoadExtension(p,n))}
            };

        public static IOpenApiExample LoadExample(ParseNode node, OpenApiDocument? hostDocument)
        {
            var mapNode = node.CheckMapNode("example");

            var pointer = mapNode.GetReferencePointer();
            if (pointer != null)
            {
                var reference = GetReferenceIdAndExternalResource(pointer);
                return new OpenApiExampleReference(reference.Item1, hostDocument, reference.Item2);
            }

            var example = new OpenApiExample();
            foreach (var property in mapNode)
            {
                property.ParseField(example, _exampleFixedFields, _examplePatternFields, hostDocument);
            }

            return example;
        }
    }
}
