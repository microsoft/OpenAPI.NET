﻿// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Models
{
    /// <summary>
    /// Operation Object.
    /// </summary>
    public class OpenApiOperation : OpenApiElement, IOpenApiExtension
    {
        /// <summary>
        /// Default value for <see cref="Deprecated"/>.
        /// </summary>
        public const bool DeprecatedDefault = false;

        /// <summary>
        /// A list of tags for API documentation control.
        /// Tags can be used for logical grouping of operations by resources or any other qualifier.
        /// </summary>
        public IList<OpenApiTag> Tags { get; set; } = new List<OpenApiTag>();

        /// <summary>
        /// A short summary of what the operation does.
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// A verbose explanation of the operation behavior.
        /// CommonMark syntax MAY be used for rich text representation.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Additional external documentation for this operation.
        /// </summary>
        public OpenApiExternalDocs ExternalDocs { get; set; }

        /// <summary>
        /// Unique string used to identify the operation. The id MUST be unique among all operations described in the API.
        /// Tools and libraries MAY use the operationId to uniquely identify an operation, therefore,
        /// it is RECOMMENDED to follow common programming naming conventions.
        /// </summary>
        public string OperationId { get; set; }

        /// <summary>
        /// A list of parameters that are applicable for this operation.
        /// If a parameter is already defined at the Path Item, the new definition will override it but can never remove it.
        /// The list MUST NOT include duplicated parameters. A unique parameter is defined by a combination of a name and location.
        /// The list can use the Reference Object to link to parameters that are defined at the OpenAPI Object's components/parameters.
        /// </summary>
        public IList<OpenApiParameter> Parameters { get; set; } = new List<OpenApiParameter>();

        /// <summary>
        /// The request body applicable for this operation.
        /// The requestBody is only supported in HTTP methods where the HTTP 1.1 specification RFC7231 
        /// has explicitly defined semantics for request bodies.
        /// In other cases where the HTTP spec is vague, requestBody SHALL be ignored by consumers.
        /// </summary>
        public OpenApiRequestBody RequestBody { get; set; }

        /// <summary>
        /// REQUIRED. The list of possible responses as they are returned from executing this operation.
        /// </summary>
        public OpenApiResponses Responses { get; set; }

        /// <summary>
        /// A map of possible out-of band callbacks related to the parent operation. 
        /// The key is a unique identifier for the Callback Object. 
        /// Each value in the map is a Callback Object that describes a request 
        /// that may be initiated by the API provider and the expected responses.
        /// The key value used to identify the callback object is an expression, evaluated at runtime, 
        /// that identifies a URL to use for the callback operation.
        /// </summary>
        public IDictionary<string, OpenApiCallback> Callbacks { get; set; } = new Dictionary<string, OpenApiCallback>();

        /// <summary>
        /// Declares this operation to be deprecated. Consumers SHOULD refrain from usage of the declared operation.
        /// </summary>
        public bool Deprecated { get; set; } = DeprecatedDefault;

        /// <summary>
        /// A declaration of which security mechanisms can be used for this operation. 
        /// The list of values includes alternative security requirement objects that can be used. 
        /// Only one of the security requirement objects need to be satisfied to authorize a request. 
        /// This definition overrides any declared top-level security. 
        /// To remove a top-level security declaration, an empty array can be used.
        /// </summary>
        public IList<OpenApiSecurityRequirement> Security { get; set; } = new List<OpenApiSecurityRequirement>();

        /// <summary>
        /// An alternative server array to service this operation. 
        /// If an alternative server object is specified at the Path Item Object or Root level, 
        /// it will be overridden by this value.
        /// </summary>
        public IList<OpenApiServer> Servers { get; set; } = new List<OpenApiServer>();

        /// <summary>
        /// This object MAY be extended with Specification Extensions.
        /// </summary>
        public IDictionary<string, IOpenApiAny> Extensions { get; set; }

        /// <summary>
        /// Creates a <see cref="OpenApiResponse"/> object and add it to <see cref="Responses"/>.
        /// </summary>
        public void CreateResponse(string key, Action<OpenApiResponse> configure)
        {
            var response = new OpenApiResponse();
            configure(response);
            Responses.Add(key, response);
        }

        /// <summary>
        /// Serialize <see cref="OpenApiOperation"/> to Open Api v3.0
        /// </summary>
        internal override void WriteAsV3(IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            writer.WriteStartObject();

            // tags
            writer.WriteOptionalCollection(OpenApiConstants.OpenApiDocTags, Tags, (w, t) => t.WriteAsV3(w));

            // summary
            writer.WriteStringProperty(OpenApiConstants.OpenApiDocSummary, Summary);

            // description
            writer.WriteStringProperty(OpenApiConstants.OpenApiDocDescription, Description);

            // externalDocs
            writer.WriteOptionalObject(OpenApiConstants.OpenApiDocExternalDocs, ExternalDocs, (w, e) => e.WriteAsV3(w));

            // operationId
            writer.WriteStringProperty(OpenApiConstants.OpenApiDocOperationId, OperationId);

            // parameters
            writer.WriteOptionalCollection(OpenApiConstants.OpenApiDocParameters, Parameters, (w, p) => p.WriteAsV3(w));

            // requestBody
            writer.WriteOptionalObject(OpenApiConstants.OpenApiDocRequestBody, RequestBody, (w, r) => r.WriteAsV3(w));

            // responses
            writer.WriteOptionalObject(OpenApiConstants.OpenApiDocResponses, Responses, (w, r) => r.WriteAsV3(w));

            // callbacks
            writer.WriteOptionalMap(OpenApiConstants.OpenApiDocCallbacks, Callbacks, (w, c) => c.WriteAsV3(w));

            // deprecated
            writer.WriteBoolProperty(OpenApiConstants.OpenApiDocDeprecated, Deprecated, false);

            // security
            writer.WriteOptionalCollection(OpenApiConstants.OpenApiDocSecurity, Security, (w, s) => s.WriteAsV3(w));

            // servers
            writer.WriteOptionalCollection(OpenApiConstants.OpenApiDocServers, Servers, (w, s) => s.WriteAsV3(w));

            // specification extensions
            writer.WriteExtensions(Extensions);

            writer.WriteEndObject();
        }

        /// <summary>
        /// Serialize <see cref="OpenApiOperation"/> to Open Api v2.0
        /// </summary>
        internal override void WriteAsV2(IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            writer.WriteStartObject();

            // tags
            writer.WriteList(OpenApiConstants.OpenApiDocTags, Tags, (w, t) => t.WriteAsV2(w));

            // summary
            writer.WriteStringProperty(OpenApiConstants.OpenApiDocSummary, Summary);

            // description
            writer.WriteStringProperty(OpenApiConstants.OpenApiDocDescription, Description);

            // externalDocs
            writer.WriteObject(OpenApiConstants.OpenApiDocExternalDocs, ExternalDocs, (w, e) => e.WriteAsV2(w));

            // operationId
            writer.WriteStringProperty(OpenApiConstants.OpenApiDocOperationId, OperationId);
            
            var parameters = new List<OpenApiParameter>(Parameters);
            
            if (RequestBody != null)
            {
                // consumes
                writer.WritePropertyName(OpenApiConstants.OpenApiDocConsumes);
                writer.WriteStartArray();
                var consumes = RequestBody.Content.Keys.Distinct().ToList();
                foreach (var mediaType in consumes)
                {
                    writer.WriteValue(mediaType);
                }

                writer.WriteEndArray();

                // Create a parameter as BodyParameter type and add to Parameters.
                // This type will be used to populate the In property as "body" when Parameters is serialized.
                var bodyParameter = new BodyParameter
                {
                    Description = RequestBody.Description,
                    Schema = RequestBody.Content.First().Value.Schema,
                    Format = new List<string>(consumes)
                };
                
                parameters.Add(bodyParameter);
            }

            if (Responses != null)
            {
                var produces = Responses.Where(r => r.Value.Content != null)
                  .SelectMany(r => r.Value.Content?.Keys)
                  .Distinct()
                  .ToList();

                if (produces.Any())
                {
                    // produces
                    writer.WritePropertyName(OpenApiConstants.OpenApiDocProduces);
                    writer.WriteStartArray();
                    foreach (var mediaType in produces)
                    {
                        writer.WriteValue(mediaType);
                    }

                    writer.WriteEndArray();
                }
            }

            // parameters
            // Use the parameters created locally to include request body if exists.
            writer.WriteOptionalCollection(OpenApiConstants.OpenApiDocParameters, parameters, (w, p) => p.WriteAsV2(w));

            // responses
            writer.WriteOptionalMap(OpenApiConstants.OpenApiDocResponses, Responses, (w, r) => r.WriteAsV2(w));

            // schemes
            // All schemes in the Servers are extracted, regardless of whether the host matches
            // the host defined in the outermost Swagger object. This is due to the 
            // inaccessibility of information for that host in the context of an inner object like this Operation.
            var schemes = Servers.Select(
                    s =>
                    {
                        Uri.TryCreate(s.Url, UriKind.RelativeOrAbsolute, out var url);
                        return url?.Scheme;
                    })
                .Where(s => s != null)
                .Distinct()
                .ToList();

            writer.WriteList(OpenApiConstants.OpenApiDocSchemes, schemes, (w, s) => w.WriteValue(s));

            // deprecated
            writer.WriteBoolProperty(OpenApiConstants.OpenApiDocDeprecated, Deprecated, false);

            // security
            writer.WriteOptionalCollection(OpenApiConstants.OpenApiDocSecurity, Security, (w, s) => s.WriteAsV2(w));

            // specification extensions
            writer.WriteExtensions(Extensions);

            writer.WriteEndObject();
        }
    }
}
