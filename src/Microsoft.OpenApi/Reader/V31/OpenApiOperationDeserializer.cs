using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;

namespace Microsoft.OpenApi.Reader.V31
{
    /// <summary>
    /// Class containing logic to deserialize Open API V31 document into
    /// runtime Open API object model.
    /// </summary>
    internal static partial class OpenApiV31Deserializer
    {
        private static readonly FixedFieldMap<OpenApiOperation> _operationFixedFields =
            new()
            {
                {
                    "tags", (o, n, doc, c) => {
                        if (n.CreateSimpleList(
                            (jsonNode, document) =>
                            {
                                var val = jsonNode.GetScalarValue();
                                if (string.IsNullOrEmpty(val))
                                    return null;   // Avoid exception on empty tag, we'll remove these from the list further on
                                return LoadTagByReference(val!, document);
                            },
                            doc,
                            c)
                        // Filter out empty tags instead of excepting on them
                        .OfType<OpenApiTagReference>().ToList() is {Count: > 0} tags)
                        {
                            o.Tags = new HashSet<OpenApiTagReference>(tags, OpenApiTagComparer.Instance);
                        }
                    }
                },
                {
                    "summary", (o, n, _, _) =>
                    {
                        o.Summary = n.GetScalarValue();
                    }
                },
                {
                    "description", (o, n, _, _) =>
                    {
                        o.Description = n.GetScalarValue();
                    }
                },
                {
                    "externalDocs", (o, n, t, c) =>
                    {
                        o.ExternalDocs = LoadExternalDocs(n, t, c);
                    }
                },
                {
                    "operationId", (o, n, _, _) =>
                    {
                        o.OperationId = n.GetScalarValue();
                    }
                },
                {
                    "parameters", (o, n, t, c) =>
                    {
                        o.Parameters = n.CreateList(LoadParameter, t, c);
                    }
                },
                {
                    "requestBody", (o, n, t, c) =>
                    {
                        o.RequestBody = LoadRequestBody(n, t, c);
                    }
                },
                {
                    "responses", (o, n, t, c) =>
                    {
                        o.Responses = LoadResponses(n, t, c);
                    }
                },
                {
                    "callbacks", (o, n, t, c) =>
                    {
                        o.Callbacks = n.CreateMap(LoadCallback, t, c);
                    }
                },
                {
                    "deprecated",
                    (o, n, _, _) =>
                    {
                        o.Deprecated = n.GetScalarBoolValue();
                    }
                },
                {
                    "security", (o, n, t, c) =>
                    { 
                        if (n is JsonArray)
                        {
                            o.Security = n.CreateList(LoadSecurityRequirement, t, c); 
                        } 
                    }
                },
                {
                    "servers", (o, n, t, c) =>
                    {
                        o.Servers = n.CreateList(LoadServer, t, c);
                    }
                },
            };

        private static readonly PatternFieldMap<OpenApiOperation> _operationPatternFields =
            new()
            {
                {s => s.StartsWith(OpenApiConstants.ExtensionFieldNamePrefix, StringComparison.OrdinalIgnoreCase), (o, p, n, _, c) => o.AddExtension(p, LoadExtension(p, n, c))},
            };

        internal static OpenApiOperation LoadOperation(JsonNode node, OpenApiDocument hostDocument, ParsingContext context)
        {
            var jsonObject = node.CheckMapNode("Operation", context);

            var operation = new OpenApiOperation();

            ParseMap(jsonObject, operation, _operationFixedFields, _operationPatternFields, hostDocument, context);

            return operation;
        }

        private static OpenApiTagReference LoadTagByReference(string tagName, OpenApiDocument? hostDocument)
        {
            var tagObject = new OpenApiTagReference(tagName, hostDocument);
            return tagObject;
        }
    }
}
