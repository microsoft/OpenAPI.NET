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