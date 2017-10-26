// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Serialization
{
    /// <summary>
    /// Extensions method for <see cref="OpenApiInfo"/> serialization.
    /// </summary>
    internal static class OpenApiInfoExtensions
    {
        /// <summary>
        /// Serialize <see cref="OpenApiInfo"/> to Open Api v3.0
        /// </summary>
        public static void SerializeV3(this OpenApiInfo info, IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            writer.WriteStartObject();
            if (info != null)
            {
                writer.WriteStringProperty("title", info.Title);
                writer.WriteStringProperty("description", info.Description);
                writer.WriteStringProperty("termsOfService", info.TermsOfService);
                writer.WriteObject("contact", info.Contact, (w, c) => c.SerializeV3(w));
                writer.WriteObject("license", info.License, (w, l) => l.SerializeV3(w));
                writer.WriteStringProperty("version", info.Version.ToString());
            }
            writer.WriteEndObject();
        }

        /// <summary>
        /// Serialize <see cref="OpenApiInfo"/> to Open Api v2.0
        /// </summary>
        public static void SerializeV2(this OpenApiInfo info, IOpenApiWriter writer, IList<OpenApiServer> servers)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            writer.WriteStartObject();
            if (info != null)
            {
                writer.WriteStringProperty("title", info.Title);

                writer.WriteStringProperty("description", info.Description);

                writer.WriteStringProperty("termsOfService", info.TermsOfService);

                writer.WriteObject("contact", info.Contact, (w, c) => c.SerializeV2(w));

                writer.WriteObject("license", info.License, (w, l) => l.SerializeV3(w));

                writer.WriteStringProperty("version", info.Version.ToString());
            }
            writer.WriteEndObject();
        }
    }
}
