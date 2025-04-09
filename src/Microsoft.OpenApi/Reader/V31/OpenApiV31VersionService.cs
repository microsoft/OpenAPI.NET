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

        private readonly Dictionary<Type, Func<ParseNode, OpenApiDocument, object>> _loaders = new Dictionary<Type, Func<ParseNode, OpenApiDocument, object>>
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
            [typeof(OpenApiXml)] = OpenApiV31Deserializer.LoadXml,
            [typeof(OpenApiSchemaReference)] = OpenApiV31Deserializer.LoadMapping
        };

        public OpenApiDocument LoadDocument(RootNode rootNode, Uri location)
        {
            return OpenApiV31Deserializer.LoadOpenApi(rootNode, location);
        }

        public T LoadElement<T>(ParseNode node, OpenApiDocument doc) where T : IOpenApiElement
        {
            return (T)_loaders[typeof(T)](node, doc);
        }

        /// <inheritdoc />
        public string? GetReferenceScalarValues(MapNode mapNode, string scalarValue)
        {
            if (mapNode.Any(static x => !"$ref".Equals(x.Name, StringComparison.OrdinalIgnoreCase)))
            {
                var valueNode = mapNode.Where(x => x.Name.Equals(scalarValue))
                .Select(static x => x.Value).OfType<ValueNode>().FirstOrDefault();

                return valueNode?.GetScalarValue();
            }

            return null;
        }
    }
}
