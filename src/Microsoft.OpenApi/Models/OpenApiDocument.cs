// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;

namespace Microsoft.OpenApi.Models
{
    /// <summary>
    /// Describes an Open API Document. See: https://swagger.io/specification
    /// </summary>
    public class OpenApiDocument : IOpenApiExtension
    {
        string version;
        public string Version { get { return version; }
            set {
                if (versionRegex.IsMatch(value))
                {
                    version = value;
                } else
                {
                    throw new OpenApiException("`openapi` property does not match the required format major.minor.patch");
                }
            } } // Swagger


        public OpenApiInfo Info { get; set; } = new OpenApiInfo();
        public IList<OpenApiServer> Servers { get; set; } = new List<OpenApiServer>();
        public IList<OpenApiSecurityRequirement> SecurityRequirements { get; set; }
        public OpenApiPaths Paths { get; set; } = new OpenApiPaths();
        public OpenApiComponents Components { get; set; } = new OpenApiComponents();
        public IList<OpenApiTag> Tags { get; set; } = new List<OpenApiTag>();
        public OpenApiExternalDocs ExternalDocs { get; set; } = new OpenApiExternalDocs();
        public IDictionary<string, IOpenApiAny> Extensions { get; set; }

        private static Regex versionRegex = new Regex(@"\d+\.\d+\.\d+");

        public void CreatePath(string key, Action<OpenApiPathItem> configure)
        {
            var pathItem = new OpenApiPathItem();
            configure(pathItem);
            Paths.Add(key, pathItem);
        }
    }
}
