// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Extensions
{
    /// <summary>
    /// Class implementing IOpenApiExtension interface
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ExtensionTypeCaster<T> : IOpenApiExtension
    {
        private readonly T _value;

        /// <summary>
        /// Assigns the value of type T to the x-extension key in an Extensions dictionary
        /// </summary>
        /// <param name="value"></param>
        public ExtensionTypeCaster(T value)
        {
            _value = value;
        }
        
        /// <inheritdoc/>
        public void Write(IOpenApiWriter writer, OpenApiSpecVersion specVersion)
        {
            writer.WriteValue(_value);
        }
    }
}
