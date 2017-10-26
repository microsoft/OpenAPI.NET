// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Serialization
{
    /// <summary>
    /// Extensions method for <see cref="OpenApiLink"/> serialization.
    /// </summary>
    internal static class OpenApiLinkExtensions
    {
        /// <summary>
        /// Serialize <see cref="OpenApiLink"/> to Open Api v3.0
        /// </summary>
        public static void SerializeV3(this OpenApiLink link, IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            if (link == null)
            {
                writer.WriteStartObject();
                writer.WriteEndObject();
                return;
            }

            if (link.IsReference())
            {
                link.WriteRef(writer);
            }
            else
            {
                writer.WriteStartObject();
                if (link != null)
                {
                    writer.WriteStringProperty("href", link.Href);
                    writer.WriteStringProperty("operationId", link.OperationId);
                    writer.WriteMap("parameters", link.Parameters, (w, x) => { w.WriteValue(x.ToString()); });
                }
                writer.WriteEndObject();
            }
        }

        /// <summary>
        /// Serialize <see cref="OpenApiLink"/> to Open Api v3.0
        /// </summary>
        public static void SerializeV2(this OpenApiLink link, IOpenApiWriter writer)
        {
            // nothing here
        }
    }
}
