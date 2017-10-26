// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Serialization
{
    /// <summary>
    /// Extensions method for <see cref="OpenApiServerVariable"/> serialization.
    /// </summary>
    internal static class OpenApiServerVariableExtensions
    {
        /// <summary>
        /// Serialize <see cref="OpenApiServerVariable"/> to Open Api v3.0
        /// </summary>
        public static void SerializeV3(this OpenApiServerVariable serverVariable, IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            writer.WriteStartObject();
            if (serverVariable != null)
            {
                writer.WriteStringProperty("default", serverVariable.Default);
                writer.WriteStringProperty("description", serverVariable.Description);
                writer.WriteList("enum", serverVariable.Enum, (w, s) => w.WriteValue(s));
            }
            writer.WriteEndObject();
        }

        /// <summary>
        /// Serialize <see cref="OpenApiServerVariable"/> to Open Api v2.0
        /// </summary>
        public static void SerializeV2(this OpenApiServerVariable serverVariable, IOpenApiWriter writer)
        {
            // nothing here
        }
    }
}
