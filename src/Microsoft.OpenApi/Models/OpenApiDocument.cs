// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Models
{
    /// <summary>
    /// Describes an Open API Document. See: https://swagger.io/specification
    /// </summary>
    public class OpenApiDocument : OpenApiElement, IOpenApiExtension
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

        /// <summary>
        /// Serialize <see cref="OpenApiDocument"/> to Open Api v3.0
        /// </summary>
        internal override void WriteAsV3(IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            writer.WriteStartObject();
            writer.WritePropertyName("openapi");
            writer.WriteValue("3.0.0");

            writer.WriteObject("info", Info, (w, i) => i.WriteAsV3(w));
            writer.WriteList("servers", Servers, (w, s) => s.WriteAsV3(w));
            writer.WritePropertyName("paths");

            writer.WriteStartObject();
            Paths.WriteAsV3(writer);
            writer.WriteEndObject();

            writer.WriteList("tags", Tags, (w, t) => t.WriteAsV3(w));
            if (!Components.IsEmpty())
            {
                writer.WriteObject("components", Components, (w, c) => c.WriteAsV3(w));
            }
            if (ExternalDocs.Url != null)
            {
                writer.WriteObject("externalDocs", ExternalDocs, (w, e) => e.WriteAsV3(w));
            }
            writer.WriteList("security", SecurityRequirements, (w, s) => s.WriteAsV3(w));
            writer.WriteEndObject();
        }

        /// <summary>
        /// Serialize <see cref="OpenApiDocument"/> to Open Api v2.0
        /// </summary>
        internal override void WriteAsV2(IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            writer.WriteStartObject();
            writer.WritePropertyName("swagger");
            writer.WriteValue("2.0");

            writer.WriteObject("info", Info, (w, i) => i.WriteAsV2(w));
            SerializeHostInfo(writer, Servers);

            writer.WritePropertyName("paths");

            writer.WriteStartObject();
            Paths.WriteAsV2(writer);
            writer.WriteEndObject();

            writer.WriteList("tags", Tags, (w, t) => t.WriteAsV2(w));
            if (!Components.IsEmpty())
            {
                Components.WriteAsV2(writer);
            }
            if (ExternalDocs.Url != null)
            {
                writer.WriteObject("externalDocs", ExternalDocs, (w, e) => e.WriteAsV2(w));
            }
            writer.WriteList("security", SecurityRequirements, (w, s) => s.WriteAsV2(w));
            writer.WriteEndObject();
        }

        private static void SerializeHostInfo(IOpenApiWriter writer, IList<OpenApiServer> servers)
        {
            if (servers == null || servers.Count == 0)
            {
                return;
            }

            var firstServer = servers.First();

            var url = new Uri(firstServer.Url);

            writer.WriteStringProperty("host", url.GetComponents(UriComponents.Host | UriComponents.Port, UriFormat.SafeUnescaped));

            writer.WriteStringProperty("basePath", url.AbsolutePath);

            var schemes = servers.Select(s => new Uri(s.Url).Scheme).Distinct();

            writer.WritePropertyName("schemes");

            writer.WriteStartArray();

            foreach (var scheme in schemes)
            {
                writer.WriteValue(scheme);
            }

            writer.WriteEndArray();
        }
    }
}
