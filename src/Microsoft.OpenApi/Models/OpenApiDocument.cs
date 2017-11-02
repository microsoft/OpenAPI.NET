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
        public Version SpecVersion { get; set; } = OpenApiConstants.DefaultVersion;

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
            writer.WriteStringProperty(OpenApiConstants.OpenApi, SpecVersion.ToString());

            // info
            writer.WriteObject(OpenApiConstants.Info, Info, (w, i) => i.WriteAsV3(w));

            // servers
            writer.WriteList(OpenApiConstants.Servers, Servers, (w, s) => s.WriteAsV3(w));

            // paths
            writer.WriteObject(OpenApiConstants.Paths, Paths, (w, p) => p.WriteAsV3(w));

            // components
            if (!Components.IsEmpty())
            {
                writer.WriteObject(OpenApiConstants.Components, Components, (w, c) => c.WriteAsV3(w));
            }

            // security
            writer.WriteList(OpenApiConstants.Security, SecurityRequirements, (w, s) => s.WriteAsV3(w));

            // tags
            writer.WriteList(OpenApiConstants.Tags, Tags, (w, t) => t.WriteAsV3(w));

            // external docs
            if (ExternalDocs.Url != null)
            {
                writer.WriteObject(OpenApiConstants.ExternalDocs, ExternalDocs, (w, e) => e.WriteAsV3(w));
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
            writer.WriteStringProperty(OpenApiConstants.Swagger, SpecVersion.ToString());

            // info
            writer.WriteObject(OpenApiConstants.Info, Info, (w, i) => i.WriteAsV2(w));

            // host, basePath, schemes, consumes, produces
            WriteHostInfoV2(writer, Servers);

            // paths
            writer.WriteObject(OpenApiConstants.Paths, Paths, (w, p) => p.WriteAsV2(w));

            // definitions
            writer.WriteMap(OpenApiConstants.Definitions, Components.Schemas, (w, s) => s.WriteAsV2(w));

            // parameters
            writer.WriteMap(OpenApiConstants.Parameters, Components.Parameters, (w, p) => p.WriteAsV2(w));
            
            // responses
            writer.WriteMap(OpenApiConstants.Responses, Components.Responses, (w, r) => r.WriteAsV2(w));
            
            // securityDefinitions
            writer.WriteMap(OpenApiConstants.SecurityDefinitions, Components.SecuritySchemes, (w, s) => s.WriteAsV2(w));
            
            // security
            writer.WriteList(OpenApiConstants.Security, SecurityRequirements, (w, s) => s.WriteAsV2(w));

            // tags
            writer.WriteList(OpenApiConstants.Tags, Tags, (w, t) => t.WriteAsV2(w));

            // externalDocs
            if (ExternalDocs.Url != null)
            {
                writer.WriteObject(OpenApiConstants.ExternalDocs, ExternalDocs, (w, e) => e.WriteAsV2(w));
            }

            // extensions
            writer.WriteExtensions(Extensions);

            writer.WriteEndObject();
        }

        private static void WriteHostInfoV2(IOpenApiWriter writer, IList<OpenApiServer> servers)
        {
            if (servers == null || !servers.Any())
            {
                return;
            }

            // Arbitrarily choose the first server given that V2 only allows 
            // one host, port, and base path.
            var firstServer = servers.First();
            
            // Divide the URL in the Url property into host and basePath required in OpenAPI V2
            // The Url property cannotcontain path templating to be valid for V2 serialization.
            var firstServerUrl = new Uri(firstServer.Url);

            // host
            writer.WriteStringProperty(
                OpenApiConstants.Host,
                firstServerUrl.GetComponents(UriComponents.Host | UriComponents.Port, UriFormat.SafeUnescaped));

            // basePath
            writer.WriteStringProperty(OpenApiConstants.BasePath, firstServerUrl.AbsolutePath);

            // Consider all schemes of the URLs in the server list that have the same
            // host, port, and base path as the first server.
            var schemes = servers.Select(
                    s =>
                    {
                        Uri.TryCreate(s.Url, UriKind.RelativeOrAbsolute, out var url);
                        return url;
                    })
                .Where(
                    u => Uri.Compare(
                            u,
                            firstServerUrl,
                            UriComponents.Host | UriComponents.Port | UriComponents.Path,
                            UriFormat.SafeUnescaped,
                            StringComparison.OrdinalIgnoreCase) ==
                        0)
                .Select(u => u.Scheme)
                .Distinct()
                .ToList();

            writer.WriteList(OpenApiConstants.Schemes, schemes, (w, s) => w.WriteValue(s));
        }
    }
}