// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Text.Json.Nodes;
using Microsoft.OpenApi.Reader;

namespace Microsoft.OpenApi
{
    /// <summary>
    /// Interface to a version specific parsing implementations.
    /// </summary>
    internal interface IOpenApiVersionService
    {
        /// <summary>
        /// Loads an OpenAPI Element from a document fragment
        /// </summary>
        /// <typeparam name="T">Type of element to load</typeparam>
        /// <param name="node">document fragment node</param>
        /// <param name="doc">A host document instance.</param>
        /// <param name="context">The current parsing context.</param>
        /// <returns>Instance of OpenAPIElement</returns>
        T? LoadElement<T>(JsonNode node, OpenApiDocument doc, ParsingContext context) where T : IOpenApiElement;

        /// <summary>
        /// Converts a generic JsonNode instance into a strongly typed OpenApiDocument
        /// </summary>
        /// <param name="JsonNode">JsonNode containing the information to be converted into an OpenAPI Document</param>
        /// <param name="location">Location of where the document that is getting loaded is saved</param>
        /// <param name="context">The current parsing context.</param>
        /// <returns>Instance of OpenApiDocument populated with data from JsonNode</returns>
        OpenApiDocument LoadDocument(JsonNode JsonNode, Uri location, ParsingContext context);

        /// <summary>
        /// Gets the description and summary scalar values in a reference object for V3.1 support
        /// </summary>
        /// <param name="JsonObject">A Json object.</param>
        /// <param name="scalarValue">The scalar value we're parsing.</param>
        /// <returns>The resulting node value.</returns>
        string? GetReferenceScalarValues(JsonObject JsonObject, string scalarValue);
    }
}
