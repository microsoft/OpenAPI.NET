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
    public class OpenApiResponse : IReference
    {
        public string Description { get; set; }
        public Dictionary<string, OpenApiMediaType> Content { get; set; }
        public Dictionary<string, OpenApiHeader> Headers { get; set; }
        public Dictionary<string, OpenApiLink> Links { get; set; }
        public Dictionary<string, IOpenApiExtension> Extensions { get; set; }

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