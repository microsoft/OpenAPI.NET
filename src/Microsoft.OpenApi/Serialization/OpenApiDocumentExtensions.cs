// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Linq;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;
using System.Collections.Generic;

namespace Microsoft.OpenApi.Serialization
{
    /// <summary>
    /// Extensions method for <see cref="OpenApiDocument"/> serialization.
    /// </summary>
    internal static class OpenApiDocumentExtensions
    {
        /// <summary>
        /// Serialize <see cref="OpenApiDocument"/> to Open Api v3.0
        /// </summary>
        public static void SerializeV3(this OpenApiDocument doc, IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            writer.WriteStartObject();
            if (doc != null)
            {
                writer.WritePropertyName("openapi");
                writer.WriteValue("3.0.0");

                writer.WriteObject("info", doc.Info, (w, i) => i.SerializeV3(w));
                writer.WriteList("servers", doc.Servers, (w, s) => s.SerializeV3(w));
                writer.WritePropertyName("paths");

                writer.WriteStartObject();
                doc.Paths.SerializeV3(writer);
                writer.WriteEndObject();

                writer.WriteList("tags", doc.Tags, (w, t) => t.SerializeV3(w));
                if (!doc.Components.IsEmpty())
                {
                    writer.WriteObject("components", doc.Components, (w, c) => c.SerializeV3(w));
                }
                if (doc.ExternalDocs.Url != null)
                {
                    writer.WriteObject("externalDocs", doc.ExternalDocs, (w, e) => e.SerializeV3(w));
                }
                writer.WriteList("security", doc.SecurityRequirements, (w, s) => s.SerializeV3(w));
            }
            writer.WriteEndObject();
        }

        /// <summary>
        /// Serialize <see cref="OpenApiDocument"/> to Open Api v2.0
        /// </summary>
        public static void SerializeV2(this OpenApiDocument doc, IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            writer.WriteStartObject();
            if (doc != null)
            {
                writer.WritePropertyName("swagger");
                writer.WriteValue("2.0");

                writer.WriteObject("info", doc.Info, (w, i) => i.SerializeV2(w, doc.Servers));
                SerializeHostInfo(writer, doc.Servers);

                writer.WritePropertyName("paths");

                writer.WriteStartObject();
                doc.Paths.SerializeV2(writer);
                writer.WriteEndObject();

                writer.WriteList("tags", doc.Tags, (w, t) => t.SerializeV2(w));
                if (!doc.Components.IsEmpty())
                {
                    doc.Components.SerializeV2(writer);
                }
                if (doc.ExternalDocs.Url != null)
                {
                    writer.WriteObject("externalDocs", doc.ExternalDocs, (w, e) => e.SerializeV2(w));
                }
                writer.WriteList("security", doc.SecurityRequirements, (w, s) => s.SerializeV2(w));
            }
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
