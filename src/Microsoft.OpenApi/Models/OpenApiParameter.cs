// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.OpenApi.Any;

namespace Microsoft.OpenApi.Models
{
    /// <summary>
    /// Parameter Object.
    /// </summary>
    public class OpenApiParameter : IOpenApiReference, IOpenApiExtension
    {
        public OpenApiReference Pointer { get; set; }
        public string Name { get; set; }
        public ParameterLocation In
        {
            get { return @in; }
            set
            {
                @in = value;
                if (@in == ParameterLocation.path)
                {
                    Required = true;
                }
            }
        }
        private ParameterLocation @in;
        public string Description { get; set; }
        public bool Required
        {
            get { return required; }
            set
            {
                if (In == ParameterLocation.path && value == false)
                {
                    throw new ArgumentException("Required cannot be set to false when in is path");
                }
                required = value;
            }
        }
        private bool required = false;
        public bool Deprecated { get; set; } = false;
        public bool AllowEmptyValue { get; set; } = false;
        public string Style { get; set; }
        public bool Explode { get; set; }
        public bool AllowReserved { get; set; }
        public OpenApiSchema Schema { get; set; }
        public IList<OpenApiExample> Examples { get; set; } = new List<OpenApiExample>();
        public string Example { get; set; }
        public IDictionary<string, OpenApiMediaType> Content { get; set; }
        public IDictionary<string, IOpenApiAny> Extensions { get; set; }
    }
}
