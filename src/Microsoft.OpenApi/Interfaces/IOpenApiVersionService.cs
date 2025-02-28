// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Reader.ParseNodes;

namespace Microsoft.OpenApi.Interfaces
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
        /// <returns>Instance of OpenAPIElement</returns>
        T LoadElement<T>(ParseNode node, OpenApiDocument doc) where T : IOpenApiElement;

        /// <summary>
        /// Converts a generic RootNode instance into a strongly typed OpenApiDocument
        /// </summary>
        /// <param name="rootNode">RootNode containing the information to be converted into an OpenAPI Document</param>
        /// <returns>Instance of OpenApiDocument populated with data from rootNode</returns>
        OpenApiDocument LoadDocument(RootNode rootNode);

        /// <summary>
        /// Gets the description and summary scalar values in a reference object for V3.1 support
        /// </summary>
        /// <param name="mapNode">A YamlMappingNode.</param>
        /// <param name="scalarValue">The scalar value we're parsing.</param>
        /// <returns>The resulting node value.</returns>
        string? GetReferenceScalarValues(MapNode mapNode, string scalarValue);
    }
}
