﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;
using Microsoft.OpenApi.Any;
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
        public OpenApiDiagnostic Diagnostic { get; }

        /// <summary>
        /// Create Parsing Context
        /// </summary>
        /// <param name="diagnostic">Provide instance for diagnotic object for collecting and accessing information about the parsing.</param>
        public OpenApiV3VersionService(OpenApiDiagnostic diagnostic)
        {
            Diagnostic = diagnostic;
        }

        private IDictionary<Type, Func<ParseNode, object>> _loaders = new Dictionary<Type, Func<ParseNode, object>>
        {
            [typeof(IOpenApiAny)] = OpenApiV3Deserializer.LoadAny,
            [typeof(OpenApiCallback)] = OpenApiV3Deserializer.LoadCallback,
            [typeof(OpenApiComponents)] = OpenApiV3Deserializer.LoadComponents,
            [typeof(OpenApiEncoding)] = OpenApiV3Deserializer.LoadEncoding,
            [typeof(OpenApiExample)] = OpenApiV3Deserializer.LoadExample,
            [typeof(OpenApiExternalDocs)] = OpenApiV3Deserializer.LoadExternalDocs,
            [typeof(OpenApiHeader)] = OpenApiV3Deserializer.LoadHeader,
            [typeof(OpenApiInfo)] = OpenApiV3Deserializer.LoadInfo,
            [typeof(OpenApiLicense)] = OpenApiV3Deserializer.LoadLicense,
            [typeof(OpenApiLink)] = OpenApiV3Deserializer.LoadLink,
            [typeof(OpenApiMediaType)] = OpenApiV3Deserializer.LoadMediaType,
            [typeof(OpenApiOAuthFlow)] = OpenApiV3Deserializer.LoadOAuthFlow,
            [typeof(OpenApiOAuthFlows)] = OpenApiV3Deserializer.LoadOAuthFlows,
            [typeof(OpenApiOperation)] = OpenApiV3Deserializer.LoadOperation,
            [typeof(OpenApiParameter)] = OpenApiV3Deserializer.LoadParameter,
            [typeof(OpenApiPathItem)] = OpenApiV3Deserializer.LoadPathItem,
            [typeof(OpenApiPaths)] = OpenApiV3Deserializer.LoadPaths,
            [typeof(OpenApiRequestBody)] = OpenApiV3Deserializer.LoadRequestBody,
            [typeof(OpenApiResponse)] = OpenApiV3Deserializer.LoadResponse,
            [typeof(OpenApiResponses)] = OpenApiV3Deserializer.LoadResponses,
            [typeof(OpenApiSchema)] = OpenApiV3Deserializer.LoadSchema,
            [typeof(OpenApiSecurityRequirement)] = OpenApiV3Deserializer.LoadSecurityRequirement,
            [typeof(OpenApiSecurityScheme)] = OpenApiV3Deserializer.LoadSecurityScheme,
            [typeof(OpenApiServer)] = OpenApiV3Deserializer.LoadServer,
            [typeof(OpenApiServerVariable)] = OpenApiV3Deserializer.LoadServerVariable,
            [typeof(OpenApiTag)] = OpenApiV3Deserializer.LoadTag,
            [typeof(OpenApiXml)] = OpenApiV3Deserializer.LoadXml
        };

        /// <summary>
        /// Parse the string to a <see cref="OpenApiReference"/> object.
        /// </summary>
        /// <param name="reference">The URL of the reference</param>
        /// <param name="type">The type of object refefenced based on the context of the reference</param>
        public OpenApiReference ConvertToOpenApiReference(
            string reference,
            ReferenceType? type)
        {
            if (!string.IsNullOrWhiteSpace(reference))
            {
                var segments = reference.Split('#');
                if (segments.Length == 1)
                {
                    if (type == ReferenceType.Tag || type == ReferenceType.SecurityScheme)
                    {
                        return new OpenApiReference
                        {
                            Type = type,
                            Id = reference
                        };
                    }

                    // Either this is an external reference as an entire file
                    // or a simple string-style reference for tag and security scheme.
                    return new OpenApiReference
                    {
                        Type = type,
                        ExternalResource = segments[0]
                    };
                }
                else if (segments.Length == 2)
                {
                    if (reference.StartsWith("#"))
                    {
                        // "$ref": "#/components/schemas/Pet"
                        try
                        {
                            return ParseLocalReference(segments[1]);
                        }
                        catch (OpenApiException ex)
                        {
                            Diagnostic.Errors.Add(new OpenApiError(ex));
                            return null;
                        }
                    }
                    // Where fragments point into a non-OpenAPI document, the id will be the complete fragment identifier
                    string id = segments[1];
                    // $ref: externalSource.yaml#/Pet
                    if (id.StartsWith("/components/"))
                    {
                        var localSegments = segments[1].Split('/');
                        var referencedType = localSegments[2].GetEnumFromDisplayName<ReferenceType>();
                        if (type == null)
                        {
                            type = referencedType;
                        } 
                        else
                        {
                            if (type != referencedType)
                            {
                                throw new OpenApiException("Referenced type mismatch");
                            }
                        }
                        id = localSegments[3];
                    }

                    return new OpenApiReference
                    {
                        ExternalResource = segments[0],
                        Type = type,
                        Id = id
                    };
                }
            }

            throw new OpenApiException(string.Format(SRResource.ReferenceHasInvalidFormat, reference));
        }

        public OpenApiDocument LoadDocument(RootNode rootNode)
        {
            return OpenApiV3Deserializer.LoadOpenApi(rootNode);
        }

        public T LoadElement<T>(ParseNode node) where T : IOpenApiElement
        {
            return (T)_loaders[typeof(T)](node);
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
                    return new OpenApiReference { Type = referenceType, Id = segments[3] };
                }
            }

            throw new OpenApiException(string.Format(SRResource.ReferenceHasInvalidFormat, localReference));
        }
    }
}
