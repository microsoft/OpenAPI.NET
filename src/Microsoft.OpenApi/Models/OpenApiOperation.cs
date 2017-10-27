// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Models
{
    /// <summary>
    /// Operation Object.
    /// </summary>
    public class OpenApiOperation : IOpenApiExtension
    {
        public IList<OpenApiTag> Tags { get; set; } = new List<OpenApiTag>();
        public string Summary { get; set; }
        public string Description { get; set; }
        public OpenApiExternalDocs ExternalDocs { get; set; } 
        public string OperationId { get; set; }
        public IList<OpenApiParameter> Parameters { get; set; } = new List<OpenApiParameter>();
        public OpenApiRequestBody RequestBody { get; set; }
        public IDictionary<string, OpenApiResponse> Responses { get; set; } = new Dictionary<string, OpenApiResponse>();
        public IDictionary<string, OpenApiCallback> Callbacks { get; set; } = new Dictionary<string, OpenApiCallback>();

        public const bool DeprecatedDefault = false;
        public bool Deprecated { get; set; } = DeprecatedDefault;
        public IList<OpenApiSecurityRequirement> Security { get; set; } = new List<OpenApiSecurityRequirement>();
        public IList<OpenApiServer> Servers { get; set; } = new List<OpenApiServer>();
        public IDictionary<string, IOpenApiAny> Extensions { get; set; }

        public void CreateResponse(string key, Action<OpenApiResponse> configure)
        {
            var response = new OpenApiResponse();
            configure(response);
            Responses.Add(key, response);
        }

        /// <summary>
        /// Serialize <see cref="OpenApiOperation"/> to Open Api v3.0
        /// </summary>
        public virtual void WriteAsV3(IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            writer.WriteStartObject();
            writer.WriteList("tags", Tags, (w, t) => t.WriteAsV3(w));
            writer.WriteStringProperty("summary", Summary);
            writer.WriteStringProperty("description", Description);
            writer.WriteObject("externalDocs", ExternalDocs, (w, e) => e.WriteAsV3(w));

            writer.WriteStringProperty("operationId", OperationId);
            writer.WriteList("parameters", Parameters, (w, p) => p.WriteAsV3(w));
            writer.WriteObject("requestBody", RequestBody, (w, r) => r.WriteAsV3(w));
            writer.WriteMap("responses", Responses, (w, r) => r.WriteAsV3(w));
            writer.WriteMap("callbacks", Callbacks, (w, c) => c.WriteAsV3(w));
            writer.WriteBoolProperty("deprecated", Deprecated, OpenApiOperation.DeprecatedDefault);
            writer.WriteList("security", Security, (w, s) => s.WriteAsV3(w));
            writer.WriteList("servers", Servers, (w, s) => s.WriteAsV3(w));
            writer.WriteEndObject();
        }

        /// <summary>
        /// Serialize <see cref="OpenApiOperation"/> to Open Api v2.0
        /// </summary>
        public virtual void WriteAsV2(IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            writer.WriteStartObject();
            writer.WriteList("tags", Tags, (w, t) => t.WriteAsV2(w));
            writer.WriteStringProperty("summary", Summary);
            writer.WriteStringProperty("description", Description);
            writer.WriteObject("externalDocs", ExternalDocs, (w, e) => e.WriteAsV2(w));

            writer.WriteStringProperty("operationId", OperationId);

            var parameters = new List<OpenApiParameter>(Parameters);

            OpenApiParameter bodyParameter = null;
            if (RequestBody != null)
            {
                writer.WritePropertyName("consumes");
                writer.WriteStartArray();
                var consumes = RequestBody.Content.Keys.Distinct();
                foreach (var mediaType in consumes)
                {
                    writer.WriteValue(mediaType);
                }
                writer.WriteEndArray();

                // Create bodyParameter
                bodyParameter = new BodyParameter()
                {
                    Name = "body",
                    Description = RequestBody.Description,
                    Schema = RequestBody.Content.First().Value.Schema
                };
                // add to parameters
                parameters.Add(bodyParameter);
            }

            var produces = Responses.Where(r => r.Value.Content != null).SelectMany(r => r.Value.Content?.Keys).Distinct();
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

            writer.WriteList<OpenApiParameter>("parameters", parameters, (w, p) => p.WriteAsV2(w));
            writer.WriteMap<OpenApiResponse>("responses", Responses, (w, r) => r.WriteAsV2(w));
            writer.WriteBoolProperty("deprecated", Deprecated, OpenApiOperation.DeprecatedDefault);
            writer.WriteList("security", Security, (w, s) => s.WriteAsV2(w));
            writer.WriteEndObject();
        }
    }
}
