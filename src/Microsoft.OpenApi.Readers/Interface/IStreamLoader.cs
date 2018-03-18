// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Threading.Tasks;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Readers.Interface
{
    /// <summary>
    /// Interface for service that translates a URI into an object that can be loaded by a Reader
    /// </summary>
    /// <typeparam name="TInput"></typeparam>
    public interface IInputLoader<TInput>
    {
        /// <summary>
        /// Use Uri to locate data and convert into an input object.
        /// </summary>
        /// <param name="uri">Identifier of some source of an OpenAPI Description</param>
        /// <returns>A data objext that can be processed by a reader to generate an <see cref="OpenApiDocument"/></returns>
        Task<TInput> LoadAsync(Uri uri);
    }
}
