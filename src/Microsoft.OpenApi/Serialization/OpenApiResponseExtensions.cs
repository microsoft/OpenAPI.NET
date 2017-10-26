// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.Linq;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Serialization
{
    /// <summary>
    /// Extensions method for <see cref="OpenApiResponse"/> serialization.
    /// </summary>
    internal static class OpenApiResponseExtensions
    {
        /// <summary>
        /// Serialize <see cref="OpenApiResponse"/> to Open Api v3.0
        /// </summary>
        public static void SerializeV3(this OpenApiResponse response, IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            if (response == null)
            {
                writer.WriteStartObject();
                writer.WriteEndObject();
                return;
            }

            if (response.IsReference())
            {
                response.WriteRef(writer);
            }
            else
            {
                writer.WriteStartObject();

                writer.WriteStringProperty("description", response.Description);
                writer.WriteMap("content", response.Content, (w, c) => c.SerializeV3(w));

                writer.WriteMap("headers", response.Headers, (w, h) => h.SerializeV3(w));
                writer.WriteMap("links", response.Links, (w, l) => l.SerializeV3(w));

                //Links
                writer.WriteEndObject();
            }
        }

        /// <summary>
        /// Serialize <see cref="OpenApiResponse"/> to Open Api v2.0
        /// </summary>
        public static void SerializeV2(this OpenApiResponse response, IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            if (response == null)
            {
                writer.WriteStartObject();
                writer.WriteEndObject();
                return;
            }

            if (response.IsReference())
            {
                response.WriteRef(writer);
            }
            else
            {
                writer.WriteStartObject();

                writer.WriteStringProperty("description", response.Description);
                if (response.Content != null)
                {
                    var mediatype = response.Content.FirstOrDefault();
                    if (mediatype.Value != null)
                    {

                        writer.WriteObject("schema", mediatype.Value.Schema, (w, s) => s.SerializeV2(w));

                        if (mediatype.Value.Example != null)
                        {
                            writer.WritePropertyName("examples");
                            writer.WriteStartObject();
                            writer.WritePropertyName(mediatype.Key);
                            writer.WriteValue(mediatype.Value.Example);
                            writer.WriteEndObject();
                        }
                    }
                }
                writer.WriteMap("headers", response.Headers, (w, h) => h.SerializeV2(w));

                writer.WriteEndObject();
            }
        }
    }
}
