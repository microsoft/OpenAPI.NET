//---------------------------------------------------------------------
// <copyright file="OpenApiExample.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;

namespace Microsoft.OpenApi
{
    /// <summary>
    /// Example Object.
    /// </summary>
    public class OpenApiExample : IReference
    {
        /// <summary>
        /// Short description for the example.
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// Long description for the example.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Embedded literal example.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// This object MAY be extended with Specification Extensions.
        /// </summary>
        public Dictionary<string, IOpenApiExtension> Extensions { get; set; } = new Dictionary<string, IOpenApiExtension>();

        public OpenApiReference Pointer
        {
            get; set;
        }
     }
}
