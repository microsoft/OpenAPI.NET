// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
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
        /// <summary>
        /// REQUIRED.This string MUST be the semantic version number of the OpenAPI Specification version that the OpenAPI document uses.
        /// </summary>
        public Version Version { get; set; } = OpenApiConstants.OpenApiDocDefaultVersion;

        /// <summary>
        /// REQUIRED. Provides metadata about the API. The metadata MAY be used by tooling as required.
        /// </summary>
        public OpenApiInfo Info { get; set; } = new OpenApiInfo();

        /// <summary>
        /// An array of Server Objects, which provide connectivity information to a target server.
        /// </summary>
        public IList<OpenApiServer> Servers { get; set; } = new List<OpenApiServer>();

        /// <summary>
        /// REQUIRED. The available paths and operations for the API.
        /// </summary>
        public OpenApiPaths Paths { get; set; } = new OpenApiPaths();

        /// <summary>
        /// An element to hold various schemas for the specification.
        /// </summary>
        public OpenApiComponents Components { get; set; } = new OpenApiComponents();

        /// <summary>
        /// A declaration of which security mechanisms can be used across the API.
        /// </summary>
        public IList<OpenApiSecurityRequirement> SecurityRequirements { get; set; }

        /// <summary>
        /// A list of tags used by the specification with additional metadata.
        /// </summary>
        public IList<OpenApiTag> Tags { get; set; } = new List<OpenApiTag>();

        /// <summary>
        /// Additional external documentation.
        /// </summary>
        public OpenApiExternalDocs ExternalDocs { get; set; } = new OpenApiExternalDocs();

        /// <summary>
        /// This object MAY be extended with Specification Extensions.
        /// </summary>
        public IDictionary<string, IOpenApiAny> Extensions { get; set; }

        /// <summary>
        /// Serialize <see cref="OpenApiDocument"/> to Open Api v3.0.
        /// </summary>
        internal override void WriteAsV3(IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            writer.WriteStartObject();

            // openapi
            writer.WriteStringProperty(OpenApiConstants.OpenApiDocOpenApi, "3.0.0");

            // info
            writer.WriteObject(OpenApiConstants.OpenApiDocInfo, Info, (w, i) => i.WriteAsV3(w));

            // servers
            writer.WriteList(OpenApiConstants.OpenApiDocServers, Servers, (w, s) => s.WriteAsV3(w));

            // paths
            writer.WriteObject(OpenApiConstants.OpenApiDocPaths, Paths, (w, p) => p.WriteAsV3(w));

            // components
            if (!Components.IsEmpty())
            {
                writer.WriteObject(OpenApiConstants.OpenApiDocComponents, Components, (w, c) => c.WriteAsV3(w));
            }

            // security
            writer.WriteList(OpenApiConstants.OpenApiDocSecurity, SecurityRequirements, (w, s) => s.WriteAsV3(w));

            // tags
            writer.WriteList(OpenApiConstants.OpenApiDocTags, Tags, (w, t) => t.WriteAsV3(w));

            // external docs
            if (ExternalDocs.Url != null)
            {
                writer.WriteObject(OpenApiConstants.OpenApiDocExternalDocs, ExternalDocs, (w, e) => e.WriteAsV3(w));
            }

            // extensions
            writer.WriteExtensions(Extensions);

            writer.WriteEndObject();
        }

        /// <summary>
        /// Serialize <see cref="OpenApiDocument"/> to Open Api v2.0.
        /// </summary>
        internal override void WriteAsV2(IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            writer.WriteStartObject();

            // swagger
            writer.WriteStringProperty(OpenApiConstants.OpenApiDocSwagger, "2.0");

            // info
            writer.WriteObject("info", Info, (w, i) => i.WriteAsV2(w));

            // host, basePath, schemes, consumes, produces
            SerializeHostInfo(writer, Servers);

            // paths
            writer.WriteObject(OpenApiConstants.OpenApiDocPaths, Paths, (w, p) => p.WriteAsV2(w));

            // definitions
            // parameters
            // responses
            // securityDefinitions

            // security
            writer.WriteList(OpenApiConstants.OpenApiDocSecurity, SecurityRequirements, (w, s) => s.WriteAsV2(w));

            // tags
            writer.WriteList(OpenApiConstants.OpenApiDocTags, Tags, (w, t) => t.WriteAsV2(w));

            // externalDocs
            if (ExternalDocs.Url != null)
            {
                writer.WriteObject(OpenApiConstants.OpenApiDocExternalDocs, ExternalDocs, (w, e) => e.WriteAsV2(w));
            }

            // extensions
            writer.WriteExtensions(Extensions);

            writer.WriteEndObject();
        }

        private static void SerializeHostInfo(IOpenApiWriter writer, IList<OpenApiServer> servers)
        {
            if (servers == null || !servers.Any())
            {
                return;
            }

            var firstServer = servers.First();

            var url = new Uri(firstServer.Url);

            // host
            writer.WriteStringProperty(OpenApiConstants.OpenApiDocHost, url.GetComponents(UriComponents.Host | UriComponents.Port, UriFormat.SafeUnescaped));

            // basePath
            writer.WriteStringProperty(OpenApiConstants.OpenApiDocBasePath, url.AbsolutePath);

            // schemes
            var schemes = servers.Select(s => new Uri(s.Url).Scheme).Distinct();

            writer.WritePropertyName(OpenApiConstants.OpenApiDocSchemes);

            writer.WriteStartArray();

            foreach (var scheme in schemes)
            {
                writer.WriteValue(scheme);
            }

            writer.WriteEndArray();
        }
    }
}
