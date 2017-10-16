//---------------------------------------------------------------------
// <copyright file="OpenApiDocument.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Microsoft.OpenApi
{
    /// <summary>
    /// Describes an Open API Document. See: https://swagger.io/specification
    /// </summary>
    public class OpenApiDocument
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
        public List<OpenApiServer> Servers { get; set; } = new List<OpenApiServer>();
        public List<OpenApiSecurityRequirement> SecurityRequirements { get; set; }
        public OpenApiPaths Paths { get; set; } = new OpenApiPaths();
        public OpenApiComponents Components { get; set; } = new OpenApiComponents();
        public List<OpenApiTag> Tags { get; set; } = new List<OpenApiTag>();
        public OpenApiExternalDocs ExternalDocs { get; set; } = new OpenApiExternalDocs();
        public Dictionary<string, IOpenApiExtension> Extensions { get; set; } = new Dictionary<string, IOpenApiExtension>();

        private static Regex versionRegex = new Regex(@"\d+\.\d+\.\d+");

        public void CreatePath(string key, Action<OpenApiPathItem> configure)
        {
            var pathItem = new OpenApiPathItem();
            configure(pathItem);
            Paths.Add(key, pathItem);
        }

    }
}
