// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Models
{
    /// <summary>
    /// Security Requirement Object
    /// </summary>
    public class OpenApiSecurityRequirement : OpenApiElement
    {
        /// <summary>
        /// Gets or sets the required security schemes along with a list of strings populated with scopes
        /// only when the security scheme is OAuth2 or OpenIdConnect.
        /// For other security scheme types, the array MUST be empty.
        /// </summary>
        public OpenApiSecuritySchemeDictionary Schemes { get; set; }

        /// <summary>
        /// Serialize <see cref="OpenApiSecurityRequirement"/> to Open Api v3.0
        /// </summary>
        internal override void WriteAsV3(IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            writer.WriteStartObject();

            if (Schemes != null)
            {
                foreach (var scheme in Schemes)
                {
                    scheme.Key.WriteAsV3(writer);

                    writer.WriteStartArray();

                    foreach (var scope in scheme.Value)
                    {
                        writer.WriteValue(scope);
                    }

                    writer.WriteEndArray();
                }
            }

            writer.WriteEndObject();
        }

        /// <summary>
        /// Serialize <see cref="OpenApiSecurityRequirement"/> to Open Api v2.0
        /// </summary>
        internal override void WriteAsV2(IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            writer.WriteStartObject();

            if (Schemes != null)
            {
                foreach (var scheme in Schemes)
                {
                    scheme.Key.WriteAsV2(writer);

                    writer.WriteStartArray();

                    foreach (var scope in scheme.Value)
                    {
                        writer.WriteValue(scope);
                    }

                    writer.WriteEndArray();
                }
            }

            writer.WriteEndObject();
        }
    }
}