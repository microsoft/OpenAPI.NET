// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Exceptions;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers.ParseNodes;

namespace Microsoft.OpenApi.Readers.V2
{
    /// <summary>
    /// Class containing logic to deserialize Open API V2 document into
    /// runtime Open API object model.
    /// </summary>
    internal static partial class OpenApiV2Deserializer
    {
        public static OpenApiReference ParseReference(string pointer)
        {
            var pointerbits = pointer.Split('#');

            if (pointerbits.Length == 1)
            {
                return new OpenApiReference
                {
                    ReferenceType = ReferenceType.Schema,
                    TypeName = pointerbits[0]
                };
            }

            var pointerParts = pointerbits[1].Split('/');
            return new OpenApiReference
            {
                ExternalFilePath = pointerbits[0],
                ReferenceType = ParseReferenceTypeName(pointerParts[1]),
                TypeName = pointerParts[2]
            };
        }

        public static JsonPointer GetPointer(OpenApiReference reference)
        {
            return new JsonPointer("#/" + GetReferenceTypeName(reference.ReferenceType) + "/" + reference.TypeName);
        }

        private static ReferenceType ParseReferenceTypeName(string referenceTypeName)
        {
            switch (referenceTypeName)
            {
                case "definitions": return ReferenceType.Schema;
                case "parameters": return ReferenceType.Parameter;
                case "responses": return ReferenceType.Response;
                case "headers": return ReferenceType.Header;
                case "tags": return ReferenceType.Tags;
                case "securityDefinitions": return ReferenceType.SecurityScheme;
                default: throw new ArgumentException();
            }
        }

        private static string GetReferenceTypeName(ReferenceType referenceType)
        {
            switch (referenceType)
            {
                case ReferenceType.Schema:
                    return "definitions";
                case ReferenceType.Parameter:
                    return "parameters";
                case ReferenceType.Response:
                    return "responses";
                case ReferenceType.Header:
                    return "headers";
                case ReferenceType.Tags:
                    return "tags";
                case ReferenceType.SecurityScheme:
                    return "securityDefinitions";
                default: throw new ArgumentException();
            }
        }

        public static IOpenApiReference LoadReference(OpenApiReference reference, object rootNode)
        {
            IOpenApiReference referencedObject = null;
            var node = ((RootNode)rootNode).Find(GetPointer(reference));

            if (node == null && reference.ReferenceType != ReferenceType.Tags)
            {
                return null;
            }

            switch (reference.ReferenceType)
            {
                case ReferenceType.Schema:
                    referencedObject = LoadSchema(node);
                    break;
                case ReferenceType.Parameter:
                    referencedObject = LoadParameter(node);
                    break;
                case ReferenceType.SecurityScheme:
                    referencedObject = LoadSecurityScheme(node);
                    break;
                case ReferenceType.Tags:
                    var list = (ListNode)node;
                    if (list != null)
                    {
                        foreach (var item in list)
                        {
                            var tag = LoadTag(item);

                            if (tag.Name == reference.TypeName)
                            {
                                return tag;
                            }
                        }
                    }
                    else
                    {
                        return new OpenApiTag {Name = reference.TypeName};
                    }

                    break;

                default:
                    throw new OpenApiException($"Unknown $ref {reference.ReferenceType} at {reference}");
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
        }

        private static void ReportMissing(ParseNode node, IList<string> required)
        {
            foreach (var error in required.Select(r => new OpenApiError("", $"{r} is a required property")).ToList())
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