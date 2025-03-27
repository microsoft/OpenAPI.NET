﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
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
        private static readonly FixedFieldMap<OpenApiOperation> _operationFixedFields =
            new()
            {
                {
                    "tags", (o, n, doc) => {
                        if (n.CreateSimpleList(
                            (valueNode, doc) =>
                            {
                                var val = valueNode.GetScalarValue();
                                if (string.IsNullOrEmpty(val))
                                    return null;   // Avoid exception on empty tag, we'll remove these from the list further on
                                return LoadTagByReference(val , doc);
                                },
                            doc)
                        // Filter out empty tags instead of excepting on them
                        .OfType<OpenApiTagReference>().ToList() is {Count: > 0} tags)
                        {
                            o.Tags = new HashSet<OpenApiTagReference>(tags, OpenApiTagComparer.Instance);
                        }
                    }
                },
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
                    "externalDocs", (o, n, t) =>
                    {
                        o.ExternalDocs = LoadExternalDocs(n, t);
                    }
                },
                {
                    "operationId", (o, n, _) =>
                    {
                        o.OperationId = n.GetScalarValue();
                    }
                },
                {
                    "parameters", (o, n, t) =>
                    {
                        o.Parameters = n.CreateList(LoadParameter, t);
                    }
                },
                {
                    "requestBody", (o, n, t) =>
                    {
                        o.RequestBody = LoadRequestBody(n, t);
                    }
                },
                {
                    "responses", (o, n, t) =>
                    {
                        o.Responses = LoadResponses(n, t);
                    }
                },
                {
                    "callbacks", (o, n, t) =>
                    {
                        o.Callbacks = n.CreateMap(LoadCallback, t);
                    }
                },
                {
                    "deprecated",
                    (o, n, _) =>
                    {
                        var deprecated = n.GetScalarValue();
                        if (deprecated != null)
                        {
                            o.Deprecated = bool.Parse(deprecated);
                        }
                    }
                },
                {
                    "security", (o, n, t) =>
                    {
                        o.Security = n.CreateList(LoadSecurityRequirement, t);
                    }
                },
                {
                    "servers", (o, n, t) =>
                    {
                        o.Servers = n.CreateList(LoadServer, t);
                    }
                },
            };

        private static readonly PatternFieldMap<OpenApiOperation> _operationPatternFields =
            new()
            {
                {s => s.StartsWith(OpenApiConstants.ExtensionFieldNamePrefix, StringComparison.OrdinalIgnoreCase), (o, p, n, _) => o.AddExtension(p, LoadExtension(p,n))},
            };

        internal static OpenApiOperation LoadOperation(ParseNode node, OpenApiDocument hostDocument)
        {
            var mapNode = node.CheckMapNode("Operation");

            var operation = new OpenApiOperation();

            ParseMap(mapNode, operation, _operationFixedFields, _operationPatternFields, hostDocument);

            return operation;
        }

        private static OpenApiTagReference LoadTagByReference(string tagName, OpenApiDocument? hostDocument)
        {
            var tagObject = new OpenApiTagReference(tagName, hostDocument);
            return tagObject;
        }
    }
}
