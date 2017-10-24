//---------------------------------------------------------------------
// <copyright file="OpenApiDocument.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OpenApi.Any;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Microsoft.OpenApi
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
