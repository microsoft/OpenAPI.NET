// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Interfaces
{
    /// <summary>
    /// Interface requuired for implementing any custom extension
    /// </summary>
    public interface IOpenApiExtension
    {
        /// <summary>
        /// Write out contents of custom extension
        /// </summary>
        /// <param name="writer"></param>
        void Write(IOpenApiWriter writer);
    }
}
