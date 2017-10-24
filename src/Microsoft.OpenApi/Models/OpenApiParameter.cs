//---------------------------------------------------------------------
// <copyright file="OpenApiParameter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Microsoft.OpenApi
{
    public enum InEnum
    {
        path = 1,
        query = 2,
        header = 3
    }

    /// <summary>
    /// Parameter Object.
    /// </summary>
    public class OpenApiParameter : IOpenApiReference, IOpenApiExtension
    {
        public OpenApiReference Pointer { get; set; }
        public string Name { get; set; }
        public InEnum In
        {
            get { return @in; }
            set
            {
                @in = value;
                if (@in == InEnum.path)
                {
                    Required = true;
                }
            }
        }
        private InEnum @in;
        public string Description { get; set; }
        public bool Required
        {
            get { return required; }
            set
            {
                if (In == InEnum.path && value == false)
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
