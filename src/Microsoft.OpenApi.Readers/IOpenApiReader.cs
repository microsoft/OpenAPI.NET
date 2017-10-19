// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.OpenApi.Readers
{
    /// <summary>
    /// Interface for open API readers.
    /// </summary>
    /// <typeparam name="TInput">The type of input to read from.</typeparam>
    /// <typeparam name="TLog">The type of log for information from reading process.</typeparam>
    public interface IOpenApiReader<TInput, TLog>
    {
        /// <summary>
        /// Reads the input and parses it into an Open API document.
        /// </summary>
        /// <param name="input">The input to read from.</param>
        /// <param name="log">The log or context containing information from the reading process.</param>
        /// <returns>The Open API document.</returns>
        OpenApiDocument Read(TInput input, out TLog log);
    }
}