// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Collections.Generic;
using Microsoft.OpenApi.Interfaces;

namespace Microsoft.OpenApi.Models
{
    /// <summary>
    /// Discriminator object.
    /// </summary>
    public class OpenApiDiscriminator : IOpenApiElement
    {
        /// <summary>
        /// REQUIRED. The name of the property in the payload that will hold the discriminator value.
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        /// An object to hold mappings between payload values and schema names or references.
        /// </summary>
        public IDictionary<string, string> Mapping { get; set; } = new Dictionary<string, string>();
    }
}
