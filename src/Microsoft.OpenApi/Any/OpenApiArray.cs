﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using Microsoft.OpenApi.Writers;
using System.Collections.Generic;

namespace Microsoft.OpenApi.Any
{
    /// <summary>
    /// Open API array.
    /// </summary>
    public class OpenApiArray : List<IOpenApiAny>, IOpenApiAny
    {
        /// <summary>
        /// The type of <see cref="IOpenApiAny"/>
        /// </summary>
        public AnyType AnyType { get; } = AnyType.Array;

        /// <summary>
        /// Write out contents of OpenApiArray to passed writer
        /// </summary>
        /// <param name="writer">Instance of JSON or YAML writer.</param>
        public void Write(IOpenApiWriter writer)
        {
            writer.WriteStartArray();

            foreach (var item in this)
            {
                writer.WriteAny(item);
            }

            writer.WriteEndArray();

        }
    }
}