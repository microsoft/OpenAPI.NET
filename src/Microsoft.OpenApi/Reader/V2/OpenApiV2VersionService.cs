// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;


namespace Microsoft.OpenApi.Reader.V2
{
    /// <summary>
    /// The version specific implementations for OpenAPI V2.0.
    /// </summary>
    internal class OpenApiV2VersionService : BaseOpenApiVersionService
    {
        /// <summary>
        /// Create Parsing Context
        /// </summary>
        /// <param name="diagnostic">Provide instance for diagnostic object for collecting and accessing information about the parsing.</param>
        public OpenApiV2VersionService(OpenApiDiagnostic diagnostic): base(diagnostic)
        {
        }

        private readonly Dictionary<Type, Func<ParseNode, OpenApiDocument, object?>> _loaders = new()
        {
            [typeof(JsonNodeExtension)] = OpenApiV2Deserializer.LoadAny,
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

        public override OpenApiDocument LoadDocument(RootNode rootNode, Uri location)
        {
            return OpenApiV2Deserializer.LoadOpenApi(rootNode, location);
        }
        internal override Dictionary<Type, Func<ParseNode, OpenApiDocument, object?>> Loaders => _loaders;

        public override string GetReferenceScalarValues(MapNode mapNode, string scalarValue)
        {
            throw new InvalidOperationException();
        }
    }
}
