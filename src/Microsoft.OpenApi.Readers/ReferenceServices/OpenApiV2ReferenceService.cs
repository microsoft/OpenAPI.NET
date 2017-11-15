// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.OpenApi.Exceptions;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers.ParseNodes;
using Microsoft.OpenApi.Readers.Properties;
using Microsoft.OpenApi.Readers.V2;
using Microsoft.OpenApi.Readers.V3;

namespace Microsoft.OpenApi.Readers.ReferenceServices
{
    /// <summary>
    /// The reference service for the Open API V2.0.
    /// </summary>
    internal class OpenApiV2ReferenceService : IOpenApiReferenceService
    {
        private readonly RootNode _rootNode;

        private readonly List<OpenApiTag> _tags = new List<OpenApiTag>();

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenApiV2ReferenceService"/> class.
        /// </summary>
        /// <param name="rootNode">The root node.</param>
        public OpenApiV2ReferenceService(RootNode rootNode)
        {
            _rootNode = rootNode ?? throw new ArgumentNullException(nameof(rootNode));

            // Precompute the tags array so that each tag reference does not require a new deserialization.
            var tagListPointer = new JsonPointer("#/tags");

            var tagListNode = _rootNode.Find(tagListPointer);

            if (tagListNode != null && tagListNode is ListNode)
            {
                var tagListNodeAsListNode = (ListNode)tagListNode;
                _tags.AddRange(tagListNodeAsListNode.CreateList(OpenApiV3Deserializer.LoadTag));
            }
        }

        /// <summary>
        /// Load the referenced <see cref="IOpenApiReferenceable"/> object from a <see cref="OpenApiReference"/> object
        /// </summary>
        public bool TryLoadReference(OpenApiReference reference, out IOpenApiReferenceable referencedObject)
        {
            referencedObject = null;

            if (reference == null)
            {
                return false;
            }

            if (reference.IsExternal)
            {
                // TODO: need to read the external document and load the referenced object.
                throw new NotImplementedException(SRResource.LoadReferencedObjectFromExternalNotImplmented);
            }

            if (!reference.Type.HasValue)
            {
                throw new ArgumentException("Local reference must have type specified.");
            }

            // Special case for Tag
            if (reference.Type == ReferenceType.Tag)
            {
                foreach (var tag in _tags)
                {
                    if (tag.Name == reference.Id)
                    {
                        referencedObject = tag;
                        return true;
                    }
                }

                referencedObject = new OpenApiTag {Name = reference.Id};
                return false;
            }

            var jsonPointer =
                new JsonPointer("#/" + GetReferenceTypeV2Name(reference.Type.Value) + "/" + reference.Id);

            var node = _rootNode.Find(jsonPointer);
            
            switch (reference.Type)
            {
                case ReferenceType.Schema:
                    referencedObject = OpenApiV3Deserializer.LoadSchema(node);
                    break;

                case ReferenceType.Response:
                    referencedObject = OpenApiV3Deserializer.LoadResponse(node);
                    break;

                case ReferenceType.Parameter:
                    referencedObject = OpenApiV3Deserializer.LoadParameter(node);
                    break;

                case ReferenceType.Example:
                    referencedObject = OpenApiV3Deserializer.LoadExample(node);
                    break;

                case ReferenceType.RequestBody:
                    referencedObject = OpenApiV3Deserializer.LoadRequestBody(node);
                    break;

                case ReferenceType.Header:
                    referencedObject = OpenApiV3Deserializer.LoadHeader(node);
                    break;

                case ReferenceType.SecurityScheme:
                    referencedObject = OpenApiV3Deserializer.LoadSecurityScheme(node);
                    break;

                case ReferenceType.Link:
                    referencedObject = OpenApiV3Deserializer.LoadLink(node);
                    break;

                case ReferenceType.Callback:
                    referencedObject = OpenApiV3Deserializer.LoadCallback(node);
                    break;

                default:
                    throw new OpenApiException(
                        string.Format(
                            SRResource.ReferenceHasInvalidValue,
                            reference.Type,
                            jsonPointer));
            }

            return true;
        }

        private OpenApiReference ParseLocalReference(string localReference)
        {
            if (string.IsNullOrWhiteSpace(localReference))
            {
                throw new ArgumentException(
                    string.Format(
                        SRResource.ArgumentNullOrWhiteSpace,
                        nameof(localReference)));
            }

            var segments = localReference.Split('/');

            // /definitions/Pet/...
            if (segments.Length >= 3)
            {
                var referenceType = ParseReferenceType(segments[1]);
                var id = localReference.Substring(
                    segments[0].Length + "/".Length + segments[1].Length + "/".Length);

                return new OpenApiReference {Type = referenceType, Id = id};
            }

            throw new OpenApiException(
                string.Format(
                    SRResource.ReferenceHasInvalidFormat,
                    localReference));
        }

        private static ReferenceType ParseReferenceType(string referenceTypeName)
        {
            switch (referenceTypeName)
            {
                case "definitions":
                    return ReferenceType.Schema;

                case "parameters":
                    return ReferenceType.Parameter;

                case "responses":
                    return ReferenceType.Response;

                case "headers":
                    return ReferenceType.Header;

                case "tags":
                    return ReferenceType.Tag;

                case "securityDefinitions":
                    return ReferenceType.SecurityScheme;

                default:
                    throw new ArgumentException();
            }
        }

        private static string GetReferenceTypeV2Name(ReferenceType referenceType)
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

                case ReferenceType.Tag:
                    return "tags";

                case ReferenceType.SecurityScheme:
                    return "securityDefinitions";

                default:
                    throw new ArgumentException();
            }
        }

        /// <summary>
        /// Parse the string to a <see cref="OpenApiReference"/> object.
        /// </summary>
        public OpenApiReference ConvertToOpenApiReference(string reference, ReferenceType? type)
        {
            if (!string.IsNullOrWhiteSpace(reference))
            {
                var segments = reference.Split('#');
                if (segments.Length == 1)
                {
                    // Either this is an external reference as an entire file
                    // or a simple string-style reference for tag and security scheme.
                    if (type == null)
                    {
                        // "$ref": "Pet.json"
                        return new OpenApiReference
                        {
                            ExternalResource = segments[0]
                        };
                    }

                    if (type == ReferenceType.Tag || type == ReferenceType.SecurityScheme)
                    {
                        return new OpenApiReference
                        {
                            Type = type,
                            Id = reference
                        };
                    }
                }
                else if (segments.Length == 2)
                {
                    if (reference.StartsWith("#"))
                    {
                        // "$ref": "#/components/schemas/Pet"
                        return ParseLocalReference(segments[1]);
                    }

                    // $ref: externalSource.yaml#/Pet
                    return new OpenApiReference
                    {
                        ExternalResource = segments[0],
                        Id = segments[1].Substring(1)
                    };
                }
            }

            throw new OpenApiException(string.Format(SRResource.ReferenceHasInvalidFormat, reference));
        }
    }
}