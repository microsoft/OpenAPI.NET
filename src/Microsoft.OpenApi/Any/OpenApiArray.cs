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
        /// Parameterless constructor
        /// </summary>
        public OpenApiArray() { }

        /// <summary>
        /// Initializes a copy of <see cref="OpenApiArray"/> object
        /// </summary>
        public OpenApiArray(OpenApiArray array)
        {
            AnyType = array.AnyType;
        }

        /// <summary>
        /// Write out contents of OpenApiArray to passed writer
        /// </summary>
        /// <param name="writer">Instance of JSON or YAML writer.</param>
        /// <param name="specVersion">Version of the OpenAPI specification that that will be output.</param>
        public void Write(IOpenApiWriter writer, OpenApiSpecVersion specVersion)
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
