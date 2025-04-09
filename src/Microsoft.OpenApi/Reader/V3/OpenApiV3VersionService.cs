// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Models.References;
using Microsoft.OpenApi.Reader.ParseNodes;

namespace Microsoft.OpenApi.Reader.V3
{
    /// <summary>
    /// The version service for the Open API V3.0.
    /// </summary>
    internal class OpenApiV3VersionService : IOpenApiVersionService
    {
        public OpenApiDiagnostic Diagnostic { get; }

        private static readonly char[] _pathSeparator = new char[] { '/' };

        /// <summary>
        /// Create Parsing Context
        /// </summary>
        /// <param name="diagnostic">Provide instance for diagnostic object for collecting and accessing information about the parsing.</param>
        public OpenApiV3VersionService(OpenApiDiagnostic diagnostic)
        {
            Diagnostic = diagnostic;
        }

        private readonly Dictionary<Type, Func<ParseNode, OpenApiDocument, object>> _loaders = new()
        {
            [typeof(OpenApiAny)] = OpenApiV3Deserializer.LoadAny,
            [typeof(OpenApiCallback)] = OpenApiV3Deserializer.LoadCallback,
            [typeof(OpenApiComponents)] = OpenApiV3Deserializer.LoadComponents,
            [typeof(OpenApiContact)] = OpenApiV3Deserializer.LoadContact,
            [typeof(OpenApiDiscriminator)] = OpenApiV3Deserializer.LoadDiscriminator,
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
            [typeof(OpenApiXml)] = OpenApiV3Deserializer.LoadXml,
            [typeof(OpenApiSchemaReference)] = OpenApiV3Deserializer.LoadMapping
        };

        public OpenApiDocument LoadDocument(RootNode rootNode, Uri location)
        {
            return OpenApiV3Deserializer.LoadOpenApi(rootNode, location);
        }

        public T LoadElement<T>(ParseNode node, OpenApiDocument doc) where T : IOpenApiElement
        {
            return (T)_loaders[typeof(T)](node, doc);
        }

        /// <inheritdoc />
        public string? GetReferenceScalarValues(MapNode mapNode, string scalarValue)
        {
            if (mapNode.Any(static x => !"$ref".Equals(x.Name, StringComparison.OrdinalIgnoreCase)) &&
                mapNode
                .Where(x => x.Name.Equals(scalarValue))
                .Select(static x => x.Value)
                .OfType<ValueNode>().FirstOrDefault() is {} valueNode)
            {
                return valueNode.GetScalarValue();
            }

            return null;
        }        
    }
}
