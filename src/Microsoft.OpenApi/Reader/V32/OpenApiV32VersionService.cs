// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;

namespace Microsoft.OpenApi.Reader.V32
{
    /// <summary>
    /// The version service for the Open API V3.1.
    /// </summary>
    internal class OpenApiV32VersionService : BaseOpenApiVersionService
    {

        /// <summary>
        /// Create Parsing Context
        /// </summary>
        /// <param name="diagnostic">Provide instance for diagnostic object for collecting and accessing information about the parsing.</param>
        public OpenApiV32VersionService(OpenApiDiagnostic diagnostic):base(diagnostic)
        {
        }

        private readonly Dictionary<Type, Func<ParseNode, OpenApiDocument, object?>> _loaders = new Dictionary<Type, Func<ParseNode, OpenApiDocument, object?>>
        {
            [typeof(JsonNodeExtension)] = OpenApiV32Deserializer.LoadAny,
            [typeof(OpenApiCallback)] = OpenApiV32Deserializer.LoadCallback,
            [typeof(OpenApiComponents)] = OpenApiV32Deserializer.LoadComponents,
            [typeof(OpenApiContact)] = OpenApiV32Deserializer.LoadContact,
            [typeof(OpenApiDiscriminator)] = OpenApiV32Deserializer.LoadDiscriminator,
            [typeof(OpenApiEncoding)] = OpenApiV32Deserializer.LoadEncoding,
            [typeof(OpenApiExample)] = OpenApiV32Deserializer.LoadExample,
            [typeof(OpenApiExternalDocs)] = OpenApiV32Deserializer.LoadExternalDocs,
            [typeof(OpenApiHeader)] = OpenApiV32Deserializer.LoadHeader,
            [typeof(OpenApiInfo)] = OpenApiV32Deserializer.LoadInfo,
            [typeof(OpenApiLicense)] = OpenApiV32Deserializer.LoadLicense,
            [typeof(OpenApiLink)] = OpenApiV32Deserializer.LoadLink,
            [typeof(OpenApiMediaType)] = OpenApiV32Deserializer.LoadMediaType,
            [typeof(OpenApiOAuthFlow)] = OpenApiV32Deserializer.LoadOAuthFlow,
            [typeof(OpenApiOAuthFlows)] = OpenApiV32Deserializer.LoadOAuthFlows,
            [typeof(OpenApiOperation)] = OpenApiV32Deserializer.LoadOperation,
            [typeof(OpenApiParameter)] = OpenApiV32Deserializer.LoadParameter,
            [typeof(OpenApiPathItem)] = OpenApiV32Deserializer.LoadPathItem,
            [typeof(OpenApiPaths)] = OpenApiV32Deserializer.LoadPaths,
            [typeof(OpenApiRequestBody)] = OpenApiV32Deserializer.LoadRequestBody,
            [typeof(OpenApiResponse)] = OpenApiV32Deserializer.LoadResponse,
            [typeof(OpenApiResponses)] = OpenApiV32Deserializer.LoadResponses,
            [typeof(OpenApiSchema)] = OpenApiV32Deserializer.LoadSchema,
            [typeof(OpenApiSecurityRequirement)] = OpenApiV32Deserializer.LoadSecurityRequirement,
            [typeof(OpenApiSecurityScheme)] = OpenApiV32Deserializer.LoadSecurityScheme,
            [typeof(OpenApiServer)] = OpenApiV32Deserializer.LoadServer,
            [typeof(OpenApiServerVariable)] = OpenApiV32Deserializer.LoadServerVariable,
            [typeof(OpenApiTag)] = OpenApiV32Deserializer.LoadTag,
            [typeof(OpenApiXml)] = OpenApiV32Deserializer.LoadXml,
            [typeof(OpenApiSchemaReference)] = OpenApiV32Deserializer.LoadMapping
        };

        public override OpenApiDocument LoadDocument(RootNode rootNode, Uri location)
        {
            return OpenApiV32Deserializer.LoadOpenApi(rootNode, location);
        }
        internal override Dictionary<Type, Func<ParseNode, OpenApiDocument, object?>> Loaders => _loaders;

    }
}

