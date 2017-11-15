// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System;
using Microsoft.OpenApi.Exceptions;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers.ParseNodes;
using Microsoft.OpenApi.Readers.Properties;
using Microsoft.OpenApi.Readers.V3;

namespace Microsoft.OpenApi.Readers.ReferenceServices
{
    /// <summary>
    /// The reference service for the Open API V2.0.
    /// </summary>
    internal class OpenApiV2ReferenceService : OpenApiReferenceServiceBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OpenApiV2ReferenceService"/> class.
        /// </summary>
        /// <param name="rootNode">The root node.</param>
        public OpenApiV2ReferenceService(RootNode rootNode)
            : base(rootNode)
        {
        }

        /// <inheritdoc/>
        public override IOpenApiReferenceable LoadReference(OpenApiReference reference)
        {
            if (reference == null)
            {
                return null;
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
                var tagListPointer = new JsonPointer("#/tags/");
                var tagListNode = (ListNode)RootNode.Find(tagListPointer);

                var tags = tagListNode.CreateList(LoadTag);

                foreach (var tag in tags)
                {
                    if (tag.Name == reference.Id)
                    {
                        return tag;
                    }
                }

                return new OpenApiTag { Name = reference.Id };
            }

            var jsonPointer =
                new JsonPointer("#/" + GetReferenceTypeV2Name(reference.Type.Value) + "/" + reference.Id);

            var node = RootNode.Find(jsonPointer);

            IOpenApiReferenceable referencedObject = null;
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

            return referencedObject;
        }

        /// <inheritdoc />
        protected override OpenApiReference ParseLocalPointer(string localPointer)
        {
            if (string.IsNullOrWhiteSpace(localPointer))
            {
                throw new ArgumentException(
                    string.Format(
                        SRResource.ArgumentNullOrWhiteSpace,
                        nameof(localPointer)));
            }

            var segments = localPointer.Split('/');

            // /definitions/Pet/...
            if (segments.Length >= 3) 
            {
                ReferenceType referenceType = ParseReferenceType(segments[1]);
                string id = localPointer.Substring(
                    segments[0].Length + "/".Length + segments[1].Length + "/".Length);

                return new OpenApiReference() {Type = referenceType, Id = id};
            }

            throw new OpenApiException(
                string.Format(
                    SRResource.ReferenceHasInvalidFormat, 
                    localPointer));
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

        public override OpenApiReference ConvertToOpenApiReference( string referenceString, ReferenceType? type)
        {
            if (!String.IsNullOrWhiteSpace(referenceString))
            {
                var segments = referenceString.Split('#');
                if (segments.Length == 1)
                {
                    // Either this is an external reference as an entire file
                    // or a simple string-style reference for tag and security scheme.
                    if (type == null)
                    {
                        // "$ref": "Pet.json"
                        return new OpenApiReference()
                        {
                            ExternalResource = segments[0]
                        };
                    }

                    if ( type == ReferenceType.Tag || type == ReferenceType.SecurityScheme )
                    {
                        return new OpenApiReference()
                        {
                            Type = type,
                            Id = referenceString
                        };
                    }
                }
                else if (segments.Length == 2)
                {
                    if (referenceString.StartsWith("#"))
                    {
                        // "$ref": "#/components/schemas/Pet"
                        return ParseLocalPointer(segments[1]);
                    }
                        
                    // $ref: externalSource.yaml#/Pet
                        return new OpenApiReference()
                        {
                            ExternalResource = segments[0],
                            Id = segments[1].Substring(1)
                        };
                    
                }
            }

            throw new OpenApiException(String.Format(SRResource.ReferenceHasInvalidFormat, referenceString));

        }
    }
}



