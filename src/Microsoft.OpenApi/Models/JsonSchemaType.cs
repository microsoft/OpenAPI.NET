// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;

namespace Microsoft.OpenApi.Models
{
    /// <summary>
    /// Represents the type of a JSON schema.
    /// </summary>
    [Flags]
    public enum JsonSchemaType
    {
        /// <summary>
        /// Represents a null type.
        /// </summary>
        Null = 1,

        /// <summary>
        /// Represents a boolean type.
        /// </summary>
        Boolean = 2,

        /// <summary>
        /// Represents an integer type.
        /// </summary>
        Integer = 4,

        /// <summary>
        /// Represents a number type.
        /// </summary>
        Number = 8,

        /// <summary>
        /// Represents a string type.
        /// </summary>
        String = 16,

        /// <summary>
        /// Represents an object type.
        /// </summary>
        Object = 32,

        /// <summary>
        /// Represents an array type.
        /// </summary>
        Array = 64,
    }
}
