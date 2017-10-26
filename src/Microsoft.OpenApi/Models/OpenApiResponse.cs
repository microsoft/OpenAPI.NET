// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;

namespace Microsoft.OpenApi.Models
{
    /// <summary>
    /// Response object.
    /// </summary>
    public class OpenApiResponse : IOpenApiReference, IOpenApiExtension
    {
        public string Description { get; set; }
        public IDictionary<string, OpenApiMediaType> Content { get; set; }
        public IDictionary<string, OpenApiHeader> Headers { get; set; }
        public IDictionary<string, OpenApiLink> Links { get; set; }
        public IDictionary<string, IOpenApiAny> Extensions { get; set; }

        public OpenApiReference Pointer
        {
            get; set;
        }

        public void CreateContent(string mediatype, Action<OpenApiMediaType> configure)
        {
            var m = new OpenApiMediaType();
            configure(m);
            if (Content == null) {
                Content = new Dictionary<string, OpenApiMediaType>();
            }

            Content.Add(mediatype, m);
        }
    }
}