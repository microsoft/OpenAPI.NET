// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Exceptions;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Properties;
using Microsoft.OpenApi.Reader.ParseNodes;
using Microsoft.OpenApi.Reader.V3;

namespace Microsoft.OpenApi.Reader.V31
{
    /// <summary>
    /// The version service for the Open API V3.1.
    /// </summary>
    internal class OpenApiV31VersionService : IOpenApiVersionService
    {
        public OpenApiDiagnostic Diagnostic { get; }

        /// <summary>
        /// Create Parsing Context
        /// </summary>
        /// <param name="diagnostic">Provide instance for diagnotic object for collecting and accessing information about the parsing.</param>
        public OpenApiV31VersionService(OpenApiDiagnostic diagnostic)
        {
            Diagnostic = diagnostic;
        }

        private readonly IDictionary<Type, Func<ParseNode, OpenApiDocument, object>> _loaders = new Dictionary<Type, Func<ParseNode, OpenApiDocument, object>>
        {
            [typeof(OpenApiAny)] = OpenApiV31Deserializer.LoadAny,
            [typeof(OpenApiCallback)] = OpenApiV31Deserializer.LoadCallback,
            [typeof(OpenApiComponents)] = OpenApiV31Deserializer.LoadComponents,
            [typeof(OpenApiContact)] = OpenApiV31Deserializer.LoadContact,
            [typeof(OpenApiDiscriminator)] = OpenApiV3Deserializer.LoadDiscriminator,
            [typeof(OpenApiEncoding)] = OpenApiV31Deserializer.LoadEncoding,
            [typeof(OpenApiExample)] = OpenApiV31Deserializer.LoadExample,
            [typeof(OpenApiExternalDocs)] = OpenApiV31Deserializer.LoadExternalDocs,
            [typeof(OpenApiHeader)] = OpenApiV31Deserializer.LoadHeader,
            [typeof(OpenApiInfo)] = OpenApiV31Deserializer.LoadInfo,
            [typeof(OpenApiLicense)] = OpenApiV31Deserializer.LoadLicense,
            [typeof(OpenApiLink)] = OpenApiV31Deserializer.LoadLink,
            [typeof(OpenApiMediaType)] = OpenApiV31Deserializer.LoadMediaType,
            [typeof(OpenApiOAuthFlow)] = OpenApiV31Deserializer.LoadOAuthFlow,
            [typeof(OpenApiOAuthFlows)] = OpenApiV31Deserializer.LoadOAuthFlows,
            [typeof(OpenApiOperation)] = OpenApiV31Deserializer.LoadOperation,
            [typeof(OpenApiParameter)] = OpenApiV31Deserializer.LoadParameter,
            [typeof(OpenApiPathItem)] = OpenApiV31Deserializer.LoadPathItem,
            [typeof(OpenApiPaths)] = OpenApiV31Deserializer.LoadPaths,
            [typeof(OpenApiRequestBody)] = OpenApiV31Deserializer.LoadRequestBody,
            [typeof(OpenApiResponse)] = OpenApiV31Deserializer.LoadResponse,
            [typeof(OpenApiResponses)] = OpenApiV31Deserializer.LoadResponses,
            [typeof(OpenApiSchema)] = OpenApiV31Deserializer.LoadSchema,
            [typeof(OpenApiSecurityRequirement)] = OpenApiV31Deserializer.LoadSecurityRequirement,
            [typeof(OpenApiSecurityScheme)] = OpenApiV31Deserializer.LoadSecurityScheme,
            [typeof(OpenApiServer)] = OpenApiV31Deserializer.LoadServer,
            [typeof(OpenApiServerVariable)] = OpenApiV31Deserializer.LoadServerVariable,
            [typeof(OpenApiTag)] = OpenApiV31Deserializer.LoadTag,
            [typeof(OpenApiXml)] = OpenApiV31Deserializer.LoadXml
        };

        /// <summary>
        /// Parse the string to a <see cref="OpenApiReference"/> object.
        /// </summary>
        /// <param name="reference">The URL of the reference</param>
        /// <param name="type">The type of object refefenced based on the context of the reference</param>
        /// <param name="summary">The summary of the reference</param>
        /// <param name="description">A reference description</param>
        public OpenApiReference ConvertToOpenApiReference(
            string reference,
            ReferenceType? type,
            string summary = null,
            string description = null)
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
                            Summary = summary,
                            Description = description,
                            Type = type,
                            Id = reference
                        };
                    }

                    // Either this is an external reference as an entire file
                    // or a simple string-style reference for tag and security scheme.
                    return new OpenApiReference
                    {
                        Summary = summary,
                        Description = description,
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
                            return ParseLocalReference(segments[1], summary, description);
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
                        Summary = summary,
                        Description = description,
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
            return OpenApiV31Deserializer.LoadOpenApi(rootNode);
        }

        public T LoadElement<T>(ParseNode node, OpenApiDocument doc) where T : IOpenApiElement
        {
            return (T)_loaders[typeof(T)](node, doc);
        }

        /// <inheritdoc />
        public string GetReferenceScalarValues(MapNode mapNode, string scalarValue)
        {
            if (mapNode.Any(static x => !"$ref".Equals(x.Name, StringComparison.OrdinalIgnoreCase)))
            {
                var valueNode = mapNode.Where(x => x.Name.Equals(scalarValue))
                .Select(static x => x.Value).OfType<ValueNode>().FirstOrDefault();

                return valueNode?.GetScalarValue();
            }

            return null;
        }

        private OpenApiReference ParseLocalReference(string localReference, string summary = null, string description = null)
        {
            if (string.IsNullOrWhiteSpace(localReference))
            {
                throw new ArgumentException(string.Format(SRResource.ArgumentNullOrWhiteSpace, nameof(localReference)));
            }

            var segments = localReference.Split('/');

            if (segments.Length == 4 && segments[1] == "components") // /components/{type}/pet
            {
                var referenceType = segments[2].GetEnumFromDisplayName<ReferenceType>();
                var refId = segments[3];
                if (segments[2] == "pathItems")
                {
                    refId = "/" + segments[3];
                }

                var parsedReference = new OpenApiReference
                {
                    Summary = summary,
                    Description = description,
                    Type = referenceType,
                    Id = refId
                };

                return parsedReference;
            }

            throw new OpenApiException(string.Format(SRResource.ReferenceHasInvalidFormat, localReference));
        }
    }
}
