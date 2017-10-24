//---------------------------------------------------------------------
// <copyright file="OpenApiResponse.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Microsoft.OpenApi
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