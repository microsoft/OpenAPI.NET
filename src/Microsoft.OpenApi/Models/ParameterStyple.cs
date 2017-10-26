// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.OpenApi.Models
{
    /// <summary>
    /// The type of the parameter value.
    /// </summary>
    public enum ParameterStyle
    {
        /// <summary>
        /// Path-style parameters.
        /// </summary>
        matrix,

        /// <summary>
        /// Label style parameters.
        /// </summary>
        label,

        /// <summary>
        /// Form style parameters.
        /// </summary>
        form,

        /// <summary>
        /// Simple style parameters.
        /// </summary>
        simple,

        /// <summary>
        /// Space separated array values.
        /// </summary>
        spaceDelimited,

        /// <summary>
        /// Pipe separated array values. 
        /// </summary>
        pipeDelimited,

        /// <summary>
        /// Provides a simple way of rendering nested objects using form parameters.
        /// </summary>
        deepObject
    }
}
