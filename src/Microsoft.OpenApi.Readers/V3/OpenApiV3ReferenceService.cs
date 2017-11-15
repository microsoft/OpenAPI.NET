// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System;
using Microsoft.OpenApi.Commons;
using Microsoft.OpenApi.Exceptions;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers.Interface;
using Microsoft.OpenApi.Readers.ParseNodes;
using Microsoft.OpenApi.Readers.Properties;

namespace Microsoft.OpenApi.Readers.V3
{
    /// <summary>
    /// The reference service for the Open API V3.0.
    /// </summary>
    internal class OpenApiV3ReferenceService : OpenApiReferenceServiceBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OpenApiV3ReferenceService"/> class.
        /// </summary>
        /// <param name="rootNode">The document root node.</param>
        public OpenApiV3ReferenceService(RootNode rootNode)
            : base(rootNode)
        {
        }
        
        /// <inheritdoc/>
        public override OpenApiReference ConvertToOpenApiReference(
            string referenceString, 
            ReferenceType? type)
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

                    if (type == ReferenceType.Tag || type == ReferenceType.SecurityScheme)
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

                if (tagListNode == null)
                {
                    return new OpenApiTag {Name = reference.Id};
                }

                var tags = tagListNode.CreateList(LoadTag);

                foreach (var tag in tags)
                {
                    if (tag.Name == reference.Id)
                    {
                        return tag;
                    }
                }

                return new OpenApiTag {Name = reference.Id};
            }

            var componentJsonPointer =
                new JsonPointer("#/components/" + reference.Type.GetDisplayName() + "/" + reference.Id);
            
            var node = RootNode.Find(componentJsonPointer);

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
                            componentJsonPointer));
            }

            return referencedObject;
        }

        /// <inheritdoc/>
        protected override OpenApiReference ParseLocalPointer(string localPointer)
        {
            if (string.IsNullOrWhiteSpace(localPointer))
            {
                throw new ArgumentException(string.Format(SRResource.ArgumentNullOrWhiteSpace, nameof(localPointer)));
            }

            var segments = localPointer.Split('/');

            if (segments.Length == 4) // /components/{type}/pet
            {
                if (segments[1] == "components")
                {
                    var referenceType = segments[2].GetEnumFromDisplayName<ReferenceType>();
                    return new OpenApiReference {Type = referenceType, Id = segments[3]};
                }
            }

            throw new OpenApiException(string.Format(SRResource.ReferenceHasInvalidFormat, localPointer));
        }
    }
}