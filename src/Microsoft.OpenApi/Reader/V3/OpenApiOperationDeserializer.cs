// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Models.References;
using Microsoft.OpenApi.Reader.ParseNodes;

namespace Microsoft.OpenApi.Reader.V3
{
    /// <summary>
    /// Class containing logic to deserialize Open API V3 document into
    /// runtime Open API object model.
    /// </summary>
    internal static partial class OpenApiV3Deserializer
    {
        private static readonly FixedFieldMap<OpenApiOperation> _operationFixedFields =
            new()
            {
                {
                    "tags", (o, n, doc) => o.Tags = n.CreateSimpleList(
                        (valueNode, doc) =>
                            LoadTagByReference(
                                valueNode.Context,
                                valueNode.GetScalarValue(), doc))
                },
                {
                    "summary",
                    (o, n, _) => o.Summary = n.GetScalarValue()
                },
                {
                    "description",
                    (o, n, _) => o.Description = n.GetScalarValue()
                },
                {
                    "externalDocs",
                    (o, n, _) => o.ExternalDocs = LoadExternalDocs(n)
                },
                {
                    "operationId",
                    (o, n, _) => o.OperationId = n.GetScalarValue()
                },
                {
                    "parameters",
                    (o, n, t) => o.Parameters = n.CreateList(LoadParameter, t)
                },
                {
                    "requestBody",
                    (o, n, t) => o.RequestBody = LoadRequestBody(n, t)
                },
                {
                    "responses",
                    (o, n, t) => o.Responses = LoadResponses(n, t)
                },
                {
                    "callbacks",
                    (o, n, t) => o.Callbacks = n.CreateMap(LoadCallback, t)
                },
                {
                    "deprecated",
                    (o, n, _) => o.Deprecated = bool.Parse(n.GetScalarValue())
                },
                {
                    "security",
                    (o, n, t) => o.Security = n.CreateList(LoadSecurityRequirement, t)
                },
                {
                    "servers",
                    (o, n, t) => o.Servers = n.CreateList(LoadServer, t)
                },
            };

        private static readonly PatternFieldMap<OpenApiOperation> _operationPatternFields =
            new()
            {
                {s => s.StartsWith("x-"), (o, p, n, _) => o.AddExtension(p, LoadExtension(p,n))},
            };

        internal static OpenApiOperation LoadOperation(ParseNode node, OpenApiDocument hostDocument = null)
        {
            var mapNode = node.CheckMapNode("Operation");

            var operation = new OpenApiOperation();

            ParseMap(mapNode, operation, _operationFixedFields, _operationPatternFields, hostDocument);

            return operation;
        }

        private static OpenApiTag LoadTagByReference(
            ParsingContext context,
            string tagName, OpenApiDocument hostDocument)
        {
            return new OpenApiTagReference(tagName, hostDocument);
        }
    }
}
