// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers.ParseNodes;
using System;

namespace Microsoft.OpenApi.Readers.Interface
{
    /// <summary>
    /// Interface to a version specific parsing implementations.
    /// </summary>
    internal interface IOpenApiVersionService
    {
        /// <summary>
        /// Load the referenced <see cref="IOpenApiReferenceable"/> object from a <see cref="OpenApiReference"/> object
        /// </summary>
        /// <param name="context">Instance of ParsingContext to use for retrieving references.</param>
        /// <param name="reference">The <see cref="OpenApiReference"/> object.</param>
        /// <param name="referencedObject">The object that is being referenced.</param>
        /// <returns>
        /// If the reference is found, return true and the referenced object in the out parameter.
        /// In the case of tag, it is psosible that the referenced object does not exist. In this case,
        /// a new tag will be returned in the outer parameter and the return value will be false.
        /// If reference is null, no object will be returned and the return value will be false.
        /// </returns>
        bool TryLoadReference(ParsingContext context, OpenApiReference reference, out IOpenApiReferenceable referencedObject);

        /// <summary>
        /// Parse the string to a <see cref="OpenApiReference"/> object.
        /// </summary>
        /// <param name="reference">The reference string.</param>
        /// <param name="type">The type of the reference.</param>
        /// <returns>The <see cref="OpenApiReference"/> object or null.</returns>
        OpenApiReference ConvertToOpenApiReference(string reference, ReferenceType? type);

        /// <summary>
        /// Function that converts a MapNode into a Tag object in a version specific way
        /// </summary>
        Func<MapNode, OpenApiTag> TagLoader { get; }

        /// <summary>
        /// Converts a generic RootNode instance into a strongly typed OpenApiDocument
        /// </summary>
        /// <param name="rootNode"></param>
        /// <returns>Instance of OpenApiDocument populated with data from rootNode</returns>
        OpenApiDocument LoadDocument(RootNode rootNode);
    }
}