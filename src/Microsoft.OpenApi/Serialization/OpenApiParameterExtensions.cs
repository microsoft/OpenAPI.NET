// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Serialization
{
    /// <summary>
    /// Extensions method for <see cref="OpenApiParameter"/> serialization.
    /// </summary>
    internal static class OpenApiParameterExtensions
    {
        /// <summary>
        /// Serialize <see cref="OpenApiParameter"/> to Open Api v3.0
        /// </summary>
        public static void SerializeV3(this OpenApiParameter parameter, IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            if (parameter == null)
            {
                writer.WriteStartObject();
                writer.WriteEndObject();
                return;
            }

            if (parameter.IsReference())
            {
                parameter.WriteRef(writer);
            }
            else
            {
                writer.WriteStartObject();
                writer.WriteStringProperty("name", parameter.Name);
                writer.WriteStringProperty("in", parameter.In.ToString());
                writer.WriteStringProperty("description", parameter.Description);
                writer.WriteBoolProperty("required", parameter.Required, false);
                writer.WriteBoolProperty("deprecated", parameter.Deprecated, false);
                writer.WriteBoolProperty("allowEmptyValue", parameter.AllowEmptyValue, false);
                writer.WriteStringProperty("style", parameter.Style);
                writer.WriteBoolProperty("explode", parameter.Explode, false);
                writer.WriteBoolProperty("allowReserved", parameter.AllowReserved, false);
                writer.WriteObject("schema", parameter.Schema, (w, s) => s.SerializeV3(w));
                writer.WriteList("examples", parameter.Examples, (w, e) => e.SerializeV3(w));
                writer.WriteObject("example", parameter.Example, (w, s) => w.WriteRaw(s));
                writer.WriteMap("content", parameter.Content, (w, c) => c.SerializeV3(w));
                writer.WriteEndObject();
            }
        }

        /// <summary>
        /// Serialize <see cref="OpenApiParameter"/> to Open Api v2.0
        /// </summary>
        public static void SerializeV2(this OpenApiParameter parameter, IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            if (parameter == null)
            {
                writer.WriteStartObject();
                writer.WriteEndObject();
                return;
            }

            if (parameter.IsReference())
            {
                parameter.WriteRef(writer);
                return;
            }
            else
            {
                writer.WriteStartObject();
                writer.WriteStringProperty("name", parameter.Name);
                if (parameter is BodyParameter)
                {
                    writer.WriteStringProperty("in", "body");   // form?
                }
                else
                {
                    writer.WriteStringProperty("in", parameter.In.ToString());
                }
                writer.WriteStringProperty("description", parameter.Description);
                writer.WriteBoolProperty("required", parameter.Required, false);
                writer.WriteBoolProperty("deprecated", parameter.Deprecated, false);
                writer.WriteBoolProperty("allowEmptyValue", parameter.AllowEmptyValue, false);

                writer.WriteBoolProperty("allowReserved", parameter.AllowReserved, false);
                if (parameter is BodyParameter)
                {
                    writer.WriteObject("schema", parameter.Schema, (w, s) => s.SerializeV2(w));
                }
                else
                {
                    parameter.Schema.SerializeSchemaProperties(writer);
                }
                //            writer.WriteList("examples", parameter.Examples, AnyNode.Write);
                //            writer.WriteObject("example", parameter.Example, AnyNode.Write);
                writer.WriteEndObject();
            }
        }
    }
}
