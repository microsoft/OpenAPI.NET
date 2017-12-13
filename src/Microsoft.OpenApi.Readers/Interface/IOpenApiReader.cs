// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Readers.Interface
{
    /// <summary>
    /// Interface for Open API readers.
    /// </summary>
    /// <typeparam name="TInput">The type of input to read from.</typeparam>
    /// <typeparam name="TDiagnostic">The type of diagnostic for information from reading process.</typeparam>
    public interface IOpenApiReader<TInput, TDiagnostic> where TDiagnostic : IDiagnostic
    {
        /// <summary>
        /// Reads the input and parses it into an Open API document.
        /// </summary>
        /// <param name="input">The input to read from.</param>
        /// <param name="diagnostic">The diagnostic entity containing information from the reading process.</param>
        /// <returns>The Open API document.</returns>
        OpenApiDocument Read(TInput input, out TDiagnostic diagnostic);
    }
}