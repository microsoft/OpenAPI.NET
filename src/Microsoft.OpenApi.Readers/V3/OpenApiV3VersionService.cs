// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;
using Microsoft.OpenApi.Exceptions;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers.Interface;
using Microsoft.OpenApi.Readers.ParseNodes;
using Microsoft.OpenApi.Readers.Properties;

namespace Microsoft.OpenApi.Readers.V3
{
    /// <summary>
    /// The version service for the Open API V3.0.
    /// </summary>
    internal class OpenApiV3VersionService : IOpenApiVersionService
    {
        /// <summary>
        /// Return a function that converts a MapNode into a V3 OpenApiTag
        /// </summary>
        public Func<MapNode, OpenApiTag> TagLoader => OpenApiV3Deserializer.LoadTag;

        /// <summary>
        /// Parse the string to a <see cref="OpenApiReference"/> object.
        /// </summary>
        public OpenApiReference ConvertToOpenApiReference(
            string reference,
            ReferenceType? type)
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

        public OpenApiDocument LoadDocument(RootNode rootNode)
        {
            return OpenApiV3Deserializer.LoadOpenApi(rootNode);
        }

        /// <summary>
        /// Load the referenced <see cref="IOpenApiReferenceable"/> object from a <see cref="OpenApiReference"/> object
        /// </summary>
        public bool TryLoadReference(ParsingContext context, OpenApiReference reference, out IOpenApiReferenceable referencedObject)
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
                foreach (var tag in context.Tags)
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

            var componentJsonPointer =
                new JsonPointer("#/components/" + reference.Type.GetDisplayName() + "/" + reference.Id);

            var node = context.RootNode.Find(componentJsonPointer);

            if (node == null)
            {
                throw new OpenApiException(
                    string.Format(
                        SRResource.JsonPointerCannotBeResolved,
                        componentJsonPointer));
            }

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
                            SRResource.ReferenceV3HasInvalidValue,
                            reference.Type,
                            componentJsonPointer));
            }

            return true;
        }

        private OpenApiReference ParseLocalReference(string localReference)
        {
            if (string.IsNullOrWhiteSpace(localReference))
            {
                throw new ArgumentException(string.Format(SRResource.ArgumentNullOrWhiteSpace, nameof(localReference)));
            }

            var segments = localReference.Split('/');

            if (segments.Length == 4) // /components/{type}/pet
            {
                if (segments[1] == "components")
                {
                    var referenceType = segments[2].GetEnumFromDisplayName<ReferenceType>();
                    return new OpenApiReference {Type = referenceType, Id = segments[3]};
                }
            }

            throw new OpenApiException(string.Format(SRResource.ReferenceHasInvalidFormat, localReference));
        }
    }
}