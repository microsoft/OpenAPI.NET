// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Serialization
{
    /// <summary>
    /// Extensions method for <see cref="OpenApiHeader"/> serialization.
    /// </summary>
    internal static class OpenApiHeaderExtensions
    {
        /// <summary>
        /// Serialize <see cref="OpenApiHeader"/> to Open Api v3.0
        /// </summary>
        public static void SerializeV3(this OpenApiHeader header, IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            if (header == null)
            {
                writer.WriteStartObject();
                writer.WriteEndObject();
                return;
            }

            if (header.IsReference())
            {
                header.WriteRef(writer);
            }
            else
            {
                writer.WriteStartObject();

                writer.WriteStringProperty("description", header.Description);
                writer.WriteBoolProperty("required", header.Required, false);
                writer.WriteBoolProperty("deprecated", header.Deprecated, false);
                writer.WriteBoolProperty("allowEmptyValue", header.AllowEmptyValue, false);
                writer.WriteStringProperty("style", header.Style);
                writer.WriteBoolProperty("explode", header.Explode, false);
                writer.WriteBoolProperty("allowReserved", header.AllowReserved, false);
                writer.WriteObject("schema", header.Schema, (w, s) => s.SerializeV3(w));
                writer.WriteList("examples", header.Examples, (w, e) => e.SerializeV3(w));
                writer.WriteObject("example", header.Example, (w, s) => w.WriteRaw(s));
                writer.WriteMap("content", header.Content, (w, c) => c.SerializeV3(w));

                writer.WriteEndObject();
            }
        }

        /// <summary>
        /// Serialize <see cref="OpenApiHeader"/> to Open Api v2.0
        /// </summary>
        public static void SerializeV2(this OpenApiHeader header, IOpenApiWriter writer)
        {
            if(writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            if (header == null)
            {
                writer.WriteStartObject();
                writer.WriteEndObject();
                return;
            }

            if (header.IsReference())
            {
                header.WriteRef(writer);
            }
            else
            {
                writer.WriteStartObject();

                writer.WriteStringProperty("description", header.Description);
                writer.WriteBoolProperty("required", header.Required, false);
                writer.WriteBoolProperty("deprecated", header.Deprecated, false);
                writer.WriteBoolProperty("allowEmptyValue", header.AllowEmptyValue, false);
                writer.WriteStringProperty("style", header.Style);
                writer.WriteBoolProperty("explode", header.Explode, false);
                writer.WriteBoolProperty("allowReserved", header.AllowReserved, false);
                writer.WriteObject("schema", header.Schema, (w, s) => s.SerializeV2(w));
                writer.WriteStringProperty("example", header.Example);

                writer.WriteEndObject();
            }
        }
    }
}
