// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Models
{
    /// <summary>
    /// Security Requirement Object
    /// </summary>
    public class OpenApiSecurityRequirement : OpenApiElement, IOpenApiElement
    {
        public Dictionary<OpenApiSecurityScheme, List<string>> Schemes { get; set; } = new Dictionary<OpenApiSecurityScheme, List<string>>();

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

            foreach (var scheme in Schemes)
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

            foreach (var scheme in Schemes)
            {
                writer.WritePropertyName(scheme.Key.Pointer.TypeName);
                writer.WriteStartArray();

                foreach (var scope in scheme.Value)
                {
                    writer.WriteValue(scope);
                }

                writer.WriteEndArray();
            }
            writer.WriteEndObject();
        }
    }
}
