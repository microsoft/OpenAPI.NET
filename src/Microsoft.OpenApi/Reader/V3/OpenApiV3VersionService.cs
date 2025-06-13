// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;

namespace Microsoft.OpenApi.Reader.V3
{
    /// <summary>
    /// The version service for the Open API V3.0.
    /// </summary>
    internal class OpenApiV3VersionService : BaseOpenApiVersionService
    {

        /// <summary>
        /// Create Parsing Context
        /// </summary>
        /// <param name="diagnostic">Provide instance for diagnostic object for collecting and accessing information about the parsing.</param>
        public OpenApiV3VersionService(OpenApiDiagnostic diagnostic):base(diagnostic)
        {
        }

        private readonly Dictionary<Type, Func<ParseNode, OpenApiDocument, object?>> _loaders = new()
        {
            [typeof(JsonNodeExtension)] = OpenApiV3Deserializer.LoadAny,
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

        internal override Dictionary<Type, Func<ParseNode, OpenApiDocument, object?>> Loaders => _loaders;

        public override OpenApiDocument LoadDocument(RootNode rootNode, Uri location)
        {
            return OpenApiV3Deserializer.LoadOpenApi(rootNode, location);
        }
    }
}
