// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers.ParseNodes;

namespace Microsoft.OpenApi.Readers.Interface
{
    /// <summary>
    /// Interface to a version specific parsing implementations.
    /// </summary>
    internal interface IOpenApiVersionService
    {

        /// <summary>
        /// Parse the string to a <see cref="OpenApiReference"/> object.
        /// </summary>
        /// <param name="reference">The reference string.</param>
        /// <param name="type">The type of the reference.</param>
        /// <returns>The <see cref="OpenApiReference"/> object or null.</returns>
        OpenApiReference ConvertToOpenApiReference(string reference, ReferenceType? type);

        /// <summary>
        /// Loads an OpenAPI Element from a document fragment
        /// </summary>
        /// <typeparam name="T">Type of element to load</typeparam>
        /// <param name="node">document fragment node</param>
        /// <returns>Instance of OpenAPIElement</returns>
        T LoadElement<T>(ParseNode node) where T : IOpenApiElement;

        /// <summary>
        /// Converts a generic RootNode instance into a strongly typed OpenApiDocument
        /// </summary>
        /// <param name="rootNode">RootNode containing the information to be converted into an OpenAPI Document</param>
        /// <returns>Instance of OpenApiDocument populated with data from rootNode</returns>
        OpenApiDocument LoadDocument(RootNode rootNode);
    }
}