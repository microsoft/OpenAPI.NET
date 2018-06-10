// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Collections.Generic;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Any
{
    /// <summary>
    /// Open API object.
    /// </summary>
    public class OpenApiObject : Dictionary<string, IOpenApiAny>, IOpenApiAny
    {
        /// <summary>
        /// Type of <see cref="IOpenApiAny"/>.
        /// </summary>
        public AnyType AnyType { get; } = AnyType.Object;

        /// <summary>
        /// Serialize OpenApiObject to writer
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="specVersion">Version of the OpenAPI specification that that will be output.</param>
        public void Write(IOpenApiWriter writer, OpenApiSpecVersion specVersion)
        {
            writer.WriteStartObject();

            foreach (var item in this)
            {
                writer.WritePropertyName(item.Key);
                writer.WriteAny(item.Value);
            }

            writer.WriteEndObject();

        }
    }
}