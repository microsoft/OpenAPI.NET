// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Serialization
{
    /// <summary>
    /// Extensions method for <see cref="OpenApiOperation"/> serialization.
    /// </summary>
    internal static class OpenApiOperationExtensions
    {
        /// <summary>
        /// Serialize <see cref="OpenApiOperation"/> to Open Api v3.0
        /// </summary>
        public static void SerializeV3(this OpenApiOperation operation, IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            writer.WriteStartObject();
            if (operation != null)
            {
                writer.WriteList("tags", operation.Tags, (w, t) => t.SerializeV3(w));
                writer.WriteStringProperty("summary", operation.Summary);
                writer.WriteStringProperty("description", operation.Description);
                writer.WriteObject("externalDocs", operation.ExternalDocs, (w, e) => e.SerializeV3(w));

                writer.WriteStringProperty("operationId", operation.OperationId);
                writer.WriteList("parameters", operation.Parameters, (w, p) => p.SerializeV3(w));
                writer.WriteObject("requestBody", operation.RequestBody, (w, r) => r.SerializeV3(w));
                writer.WriteMap("responses", operation.Responses, (w, r) => r.SerializeV3(w));
                writer.WriteMap("callbacks", operation.Callbacks, (w, c) => c.SerializeV3(w));
                writer.WriteBoolProperty("deprecated", operation.Deprecated, OpenApiOperation.DeprecatedDefault);
                writer.WriteList("security", operation.Security, (w, s) => s.SerializeV3(w));
                writer.WriteList("servers", operation.Servers, (w, s) => s.SerializeV3(w));
            }
            writer.WriteEndObject();
        }

        /// <summary>
        /// Serialize <see cref="OpenApiOperation"/> to Open Api v2.0
        /// </summary>
        public static void SerializeV2(this OpenApiOperation operation, IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            writer.WriteStartObject();
            if (operation != null)
            {
                writer.WriteList("tags", operation.Tags, (w, t) => t.SerializeV2(w));
                writer.WriteStringProperty("summary", operation.Summary);
                writer.WriteStringProperty("description", operation.Description);
                writer.WriteObject("externalDocs", operation.ExternalDocs, (w, e) => e.SerializeV2(w));

                writer.WriteStringProperty("operationId", operation.OperationId);

                var parameters = new List<OpenApiParameter>(operation.Parameters);

                OpenApiParameter bodyParameter = null;
                if (operation.RequestBody != null)
                {
                    writer.WritePropertyName("consumes");
                    writer.WriteStartArray();
                    var consumes = operation.RequestBody.Content.Keys.Distinct();
                    foreach (var mediaType in consumes)
                    {
                        writer.WriteValue(mediaType);
                    }
                    writer.WriteEndArray();

                    // Create bodyParameter
                    bodyParameter = new BodyParameter()
                    {
                        Name = "body",
                        Description = operation.RequestBody.Description,
                        Schema = operation.RequestBody.Content.First().Value.Schema
                    };
                    // add to parameters
                    parameters.Add(bodyParameter);
                }

                var produces = operation.Responses.Where(r => r.Value.Content != null).SelectMany(r => r.Value.Content?.Keys).Distinct();
                if (produces.Count() > 0)
                {
                    writer.WritePropertyName("produces");
                    writer.WriteStartArray();
                    foreach (var mediaType in produces)
                    {
                        writer.WriteValue(mediaType);
                    }
                    writer.WriteEndArray();
                }

                writer.WriteList<OpenApiParameter>("parameters", parameters, (w, p) => p.SerializeV2(w));
                writer.WriteMap<OpenApiResponse>("responses", operation.Responses, (w, r) => r.SerializeV2(w));
                writer.WriteBoolProperty("deprecated", operation.Deprecated, OpenApiOperation.DeprecatedDefault);
                writer.WriteList("security", operation.Security, (w, s) => s.SerializeV2(w));
            }
            writer.WriteEndObject();
        }
    }

    internal class BodyParameter : OpenApiParameter
    {
    }
}
