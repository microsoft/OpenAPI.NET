// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Exceptions;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Properties;
using Microsoft.OpenApi.Reader.ParseNodes;


namespace Microsoft.OpenApi.Reader.V2
{
    /// <summary>
    /// The version specific implementations for OpenAPI V2.0.
    /// </summary>
    internal class OpenApiV2VersionService : IOpenApiVersionService
    {
        public OpenApiDiagnostic Diagnostic { get; }

        /// <summary>
        /// Create Parsing Context
        /// </summary>
        /// <param name="diagnostic">Provide instance for diagnostic object for collecting and accessing information about the parsing.</param>
        public OpenApiV2VersionService(OpenApiDiagnostic diagnostic)
        {
            Diagnostic = diagnostic;
        }

        private readonly Dictionary<Type, Func<ParseNode, OpenApiDocument, object>> _loaders = new()
        {
            [typeof(OpenApiAny)] = OpenApiV2Deserializer.LoadAny,
            [typeof(OpenApiContact)] = OpenApiV2Deserializer.LoadContact,
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

                return new() { Type = referenceType, Id = id };
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

        private static ReferenceType GetReferenceTypeV2FromName(string referenceType)
        {
            switch (referenceType)
            {
                case "definitions":
                    return ReferenceType.Schema;

                case "parameters":
                    return ReferenceType.Parameter;

                case "responses":
                    return ReferenceType.Response;

                case "tags":
                    return ReferenceType.Tag;

                case "securityDefinitions":
                    return ReferenceType.SecurityScheme;

                default:
                    throw new ArgumentException();
            }
        }

        /// <summary>
        /// Parse the string to a <see cref="OpenApiReference"/> object.
        /// </summary>
        public OpenApiReference ConvertToOpenApiReference(string reference, ReferenceType? type, string summary = null, string description = null)
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
                        return new()
                        {
                            ExternalResource = segments[0]
                        };
                    }

                    if (type is ReferenceType.Tag or ReferenceType.SecurityScheme)
                    {
                        return new()
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
                        try
                        {
                            return ParseLocalReference(segments[1]);
                        }
                        catch (OpenApiException ex)
                        {
                            Diagnostic.Errors.Add(new(ex));
                            return null;
                        }
                    }

                    // Where fragments point into a non-OpenAPI document, the id will be the complete fragment identifier
                    var id = segments[1];
                    // $ref: externalSource.yaml#/Pet
                    if (id.StartsWith("/definitions/"))
                    {
                        var localSegments = id.Split('/');
                        var referencedType = GetReferenceTypeV2FromName(localSegments[1]);
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
                        id = localSegments[2];
                    }

                    // $ref: externalSource.yaml#/Pet
                    return new()
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
            return OpenApiV2Deserializer.LoadOpenApi(rootNode);
        }

        public T LoadElement<T>(ParseNode node, OpenApiDocument doc) where T : IOpenApiElement
        {
            return (T)_loaders[typeof(T)](node, doc);
        }

        /// <inheritdoc />
        public string GetReferenceScalarValues(MapNode mapNode, string scalarValue)
        {
            throw new InvalidOperationException();
        }
    }
}
