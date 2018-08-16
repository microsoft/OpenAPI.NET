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
        private static readonly FixedFieldMap<OpenApiOperation> _operationFixedFields =
            new FixedFieldMap<OpenApiOperation>
            {
                {
                    "tags", (o, n) => o.Tags = n.CreateSimpleList(
                        valueNode =>
                            LoadTagByReference(
                                valueNode.Context,
                                valueNode.Diagnostic,
                                valueNode.GetScalarValue()))
                },
                {
                    "summary", (o, n) =>
                    {
                        o.Summary = n.GetScalarValue();
                    }
                },
                {
                    "description", (o, n) =>
                    {
                        o.Description = n.GetScalarValue();
                    }
                },
                {
                    "externalDocs", (o, n) =>
                    {
                        o.ExternalDocs = LoadExternalDocs(n);
                    }
                },
                {
                    "operationId", (o, n) =>
                    {
                        o.OperationId = n.GetScalarValue();
                    }
                },
                {
                    "parameters", (o, n) =>
                    {
                        o.Parameters = n.CreateList(LoadParameter);
                    }
                },
                {
                    "requestBody", (o, n) =>
                    {
                        o.RequestBody = LoadRequestBody(n);
                    }
                },
                {
                    "responses", (o, n) =>
                    {
                        o.Responses = LoadResponses(n);
                    }
                },
                {
                    "callbacks", (o, n) =>
                    {
                        o.Callbacks = n.CreateMap(LoadCallback);
                    }
                },
                {
                    "deprecated", (o, n) =>
                    {
                        o.Deprecated = bool.Parse(n.GetScalarValue());
                    }
                },
                {
                    "security", (o, n) =>
                    {
                        o.Security = n.CreateList(LoadSecurityRequirement);
                    }
                },
                {
                    "servers", (o, n) =>
                    {
                        o.Servers = n.CreateList(LoadServer);
                    }
                },
            };

        private static readonly PatternFieldMap<OpenApiOperation> _operationPatternFields =
            new PatternFieldMap<OpenApiOperation>
            {
                {s => s.StartsWith("x-"), (o, p, n) => o.AddExtension(p, LoadExtension(p,n))},
            };

        internal static OpenApiOperation LoadOperation(ParseNode node)
        {
            var mapNode = node.CheckMapNode("Operation");

            var operation = new OpenApiOperation();

            ParseMap(mapNode, operation, _operationFixedFields, _operationPatternFields);

            return operation;
        }

        private static OpenApiTag LoadTagByReference(
            ParsingContext context,
            OpenApiDiagnostic diagnostic,
            string tagName)
        {
            var tagObject = new OpenApiTag()
            {
                UnresolvedReference = true,
                Reference = new OpenApiReference()
                {
                    Type = ReferenceType.Tag,
                    Id = tagName
                }
            };
            
            return tagObject;
        }
    }
}