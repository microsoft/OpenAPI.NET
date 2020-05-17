// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Services
{
    /// <summary>
    /// Difference point between two <see cref="OpenApiDocument"/>.
    /// </summary>
    public class OpenApiDifference
    {
        /// <summary>
        /// The type of the element for which difference found.
        /// </summary>
        public Type OpenApiComparedElementType { get; set; }

        /// <summary>
        /// The open api difference operation.
        /// </summary>
        public OpenApiDifferenceOperation OpenApiDifferenceOperation { get; set; }

        /// <summary>
        /// Pointer to the location of the difference.
        /// </summary>
        public string Pointer { get; set; }

        /// <summary>
        /// The source value.
        /// </summary>
        public object SourceValue { get; set; }

        /// <summary>
        /// The target value.
        /// </summary>
        public object TargetValue { get; set; }
    }
}
