// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Exceptions;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers.Exceptions;
using Microsoft.OpenApi.Readers.Interface;
using Microsoft.OpenApi.Readers.ParseNodes;
using Microsoft.OpenApi.Readers.Properties;


namespace Microsoft.OpenApi.Readers.V2
{
    /// <summary>
    /// The version specific implementations for OpenAPI V2.0.
    /// </summary>
    internal class OpenApiV2VersionService : IOpenApiVersionService
    {
        private IDictionary<Type, Func<ParseNode, object>> _loaders = new Dictionary<Type, Func<ParseNode, object>>
        {
            [typeof(IOpenApiAny)] = OpenApiV2Deserializer.LoadAny,
            [typeof(OpenApiExternalDocs)] = OpenApiV2Deserializer.LoadExternalDocs,
            [typeof(OpenApiHeader)] = OpenApiV2Deserializer.LoadHeader,
            [typeof(OpenApiInfo)] = OpenApiV2Deserializer.LoadInfo,
            [typeof(OpenApiLicense)] = OpenApiV2Deserializer.LoadLicense,
            [typeof(OpenApiOperation)] = OpenApiV2Deserializer.LoadOperation,
            [typeof(OpenApiParameter)] = OpenApiV2Deserializer.LoadParameter,
            [typeof(OpenApiPathItem)] = OpenApiV2Deserializer.LoadPathItem,
            [typeof(OpenApiPaths)] = OpenApiV2Deserializer.LoadPaths,
            [typeof(OpenApiResponse)] = OpenApiV2Deserializer.LoadResponse,
            [typeof(OpenApiResponses)] = OpenApiV2Deserializer.LoadResponses,
            [typeof(OpenApiSchema)] = OpenApiV2Deserializer.LoadSchema,
            [typeof(OpenApiSecurityRequirement)] = OpenApiV2Deserializer.LoadSecurityRequirement,
            [typeof(OpenApiSecurityScheme)] = OpenApiV2Deserializer.LoadSecurityScheme,
            [typeof(OpenApiTag)] = OpenApiV2Deserializer.LoadTag,
            [typeof(OpenApiXml)] = OpenApiV2Deserializer.LoadXml
        };

        private static OpenApiReference ParseLocalReference(string localReference)
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
                    throw new OpenApiReaderException($"Unknown reference type '{referenceTypeName}'");
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
                        // "$ref": "#/definitions/Pet"
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
            return OpenApiV2Deserializer.LoadOpenApi(rootNode);
        }

        public T LoadElement<T>(ParseNode node) where T : IOpenApiElement
        {
            return (T)_loaders[typeof(T)](node);
        }
    }
}