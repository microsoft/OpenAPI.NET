﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Any
{
    /// <summary>
    /// Open API null.
    /// </summary>
    public class OpenApiNull : IOpenApiAny
    {
        /// <summary>
        /// The type of <see cref="IOpenApiAny"/>
        /// </summary>
        public AnyType AnyType { get; } = AnyType.Null;

        /// <summary>
        /// Write out null representation
        /// </summary>
        /// <param name="writer"></param>
        public void Write(IOpenApiWriter writer)
        {
            writer.WriteAny(this);
        }
    }
}