// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Interfaces
{
    /// <summary>
    /// Represents an Open API element that comes with serialzation functionality.
    /// </summary>
    public interface IOpenApiSerializable : IOpenApiElement
    {
        /// <summary>
        /// Serialize Open API element to v3.0.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="specVersion">The OpenApi specification version.</param>
        void SerializeAsV3(IOpenApiWriter writer, OpenApiSpecVersion specVersion = OpenApiSpecVersion.OpenApi3_0);

        /// <summary>
        /// Serialize Open API element to v2.0.
        /// </summary>
        /// <param name="writer">The writer.</param>
        void SerializeAsV2(IOpenApiWriter writer);
    }
}
