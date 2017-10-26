// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Serialization
{
    /// <summary>
    /// Extensions method for <see cref="OpenApiSecurityRequirement"/> serialization.
    /// </summary>
    internal static class OpenApiSecurityRequirementExtensions
    {
        /// <summary>
        /// Serialize <see cref="OpenApiSecurityRequirement"/> to Open Api v3.0
        /// </summary>
        public static void SerializeV3(this OpenApiSecurityRequirement securityRequirement, IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            writer.WriteStartObject();

            if (securityRequirement != null)
            {
                foreach (var scheme in securityRequirement.Schemes)
                {
                    writer.WritePropertyName(scheme.Key.Pointer.TypeName);
                    if (scheme.Value.Count > 0)
                    {
                        writer.WriteStartArray();
                        foreach (var scope in scheme.Value)
                        {
                            writer.WriteValue(scope);
                        }
                        writer.WriteEndArray();
                    }
                    else
                    {
                        writer.WriteValue("[]");
                    }
                }
            }
            writer.WriteEndObject();
        }

        /// <summary>
        /// Serialize <see cref="OpenApiSecurityRequirement"/> to Open Api v2.0
        /// </summary>
        public static void SerializeV2(this OpenApiSecurityRequirement securityRequirement, IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            writer.WriteStartObject();

            if (securityRequirement != null)
            {
                foreach (var scheme in securityRequirement.Schemes)
                {

                    writer.WritePropertyName(scheme.Key.Pointer.TypeName);
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
