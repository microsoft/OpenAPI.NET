// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Exceptions;
using Microsoft.OpenApi.Expressions;
using Microsoft.OpenApi.Interfaces;
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
        public static IOpenApiReference LoadReference(OpenApiReference pointer, object rootNode)
        {
            IOpenApiReference referencedObject = null;

            var node = ((RootNode)rootNode).Find(pointer.GetLocalPointer());
            if (node == null && pointer.ReferenceType != ReferenceType.Tag)
            {
                return null;
            }

            switch (pointer.ReferenceType)
            {
                case ReferenceType.Schema:
                    referencedObject = LoadSchema(node);
                    break;

                case ReferenceType.Parameter:
                    referencedObject = LoadParameter(node);
                    break;

                case ReferenceType.Callback:
                    referencedObject = LoadCallback(node);
                    break;

                case ReferenceType.SecurityScheme:
                    referencedObject = LoadSecurityScheme(node);
                    break;

                case ReferenceType.Link:
                    referencedObject = LoadLink(node);
                    break;

                case ReferenceType.Example:
                    referencedObject = LoadExample(node);
                    break;

                case ReferenceType.Tag:
                    var list = (ListNode)node;
                    if (list != null)
                    {
                        foreach (var item in list)
                        {
                            var tag = LoadTag(item);

                            if (tag.Name == pointer.TypeName)
                            {
                                return tag;
                            }
                        }
                    }
                    else
                    {
                        return new OpenApiTag {Name = pointer.TypeName};
                    }

                    break;

                default:
                    throw new OpenApiException($"Unknown type of $ref {pointer.ReferenceType} at {pointer}");
            }

            return referencedObject;
        }

        private static void ParseMap<T>(
            MapNode mapNode,
            T domainObject,
            FixedFieldMap<T> fixedFieldMap,
            PatternFieldMap<T> patternFieldMap,
            List<string> requiredFields = null)
        {
            if (mapNode == null)
            {
                return;
            }

            foreach (var propertyNode in mapNode)
            {
                propertyNode.ParseField(domainObject, fixedFieldMap, patternFieldMap);
                requiredFields?.Remove(propertyNode.Name);
            }

            ReportMissing(mapNode, requiredFields);
        }

        private static RuntimeExpression LoadRuntimeExpression(ParseNode node)
        {
            var value = node.GetScalarValue();
            return RuntimeExpression.Build(value);
        }

        private static RuntimeExpressionAnyWrapper LoadRuntimeExpressionAnyWrapper(ParseNode node)
        {
            var value = node.GetScalarValue();

            if (value != null && value.StartsWith("$"))
            {
                return new RuntimeExpressionAnyWrapper
                {
                    Expression = RuntimeExpression.Build(value)
                };
            }

            return new RuntimeExpressionAnyWrapper
            {
                Any = node.CreateAny()
            };
        }

        private static void ReportMissing(ParseNode node, IList<string> required)
        {
            if (required == null || !required.Any())
            {
                return;
            }

            foreach (var error in required.Select(
                    r => new OpenApiError("", $"{r} is a required property of {node.Context.GetLocation()}"))
                .ToList())
            {
                node.Diagnostic.Errors.Add(error);
            }
        }

        private static string LoadString(ParseNode node)
        {
            return node.GetScalarValue();
        }
    }
}