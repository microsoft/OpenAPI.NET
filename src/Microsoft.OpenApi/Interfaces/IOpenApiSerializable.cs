// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Interfaces
{
    /// <summary>
    /// Represents an Open API element that comes with serialization functionality.
    /// </summary>
    public interface IOpenApiSerializable : IOpenApiElement
    {
        /// <summary>
        /// Serialize OpenAPI element into v3.1
        /// </summary>
        /// <param name="writer"></param>
        void SerializeAsV31(IOpenApiWriter writer);

        /// <summary>
        /// Serialize Open API element to v3.0.
        /// </summary>
        /// <param name="writer">The writer.</param>
        void SerializeAsV3(IOpenApiWriter writer);

        /// <summary>
        /// Serialize Open API element to v2.0.
        /// </summary>
        /// <param name="writer">The writer.</param>
        void SerializeAsV2(IOpenApiWriter writer);
    }
}
