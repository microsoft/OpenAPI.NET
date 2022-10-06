﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Models
{
    /// <summary>
    /// Request Body Object
    /// </summary>
    public class OpenApiRequestBody : IOpenApiSerializable, IOpenApiReferenceable, IOpenApiExtensible, IEffective<OpenApiRequestBody>
    {
        /// <summary>
        /// Indicates if object is populated with data or is just a reference to the data
        /// </summary>
        public bool UnresolvedReference { get; set; }

        /// <summary>
        /// Reference object.
        /// </summary>
        public OpenApiReference Reference { get; set; }

        /// <summary>
        /// A brief description of the request body. This could contain examples of use.
        /// CommonMark syntax MAY be used for rich text representation.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Determines if the request body is required in the request. Defaults to false.
        /// </summary>
        public bool Required { get; set; }

        /// <summary>
        /// REQUIRED. The content of the request body. The key is a media type or media type range and the value describes it.
        /// For requests that match multiple keys, only the most specific key is applicable. e.g. text/plain overrides text/*
        /// </summary>
        public IDictionary<string, OpenApiMediaType> Content { get; set; } = new Dictionary<string, OpenApiMediaType>();

        /// <summary>
        /// This object MAY be extended with Specification Extensions.
        /// </summary>
        public IDictionary<string, IOpenApiExtension> Extensions { get; set; } = new Dictionary<string, IOpenApiExtension>();

        /// <summary>
        /// Parameter-less constructor
        /// </summary>
        public OpenApiRequestBody() { }

        /// <summary>
        /// Initializes a copy instance of an <see cref="OpenApiRequestBody"/> object
        /// </summary>
        public OpenApiRequestBody(OpenApiRequestBody requestBody)
        {
            UnresolvedReference = requestBody?.UnresolvedReference ?? UnresolvedReference;
            Reference = requestBody?.Reference != null ? new(requestBody?.Reference) : null;
            Description = requestBody?.Description ?? Description;
            Required = requestBody?.Required ?? Required;
            Content = requestBody?.Content != null ? new Dictionary<string, OpenApiMediaType>(requestBody.Content) : null;
            Extensions = requestBody?.Extensions != null ? new Dictionary<string, IOpenApiExtension>(requestBody.Extensions) : null;
        }

        /// <summary>
        /// Serialize <see cref="OpenApiRequestBody"/> to Open Api v3.0
        /// </summary>
        public void SerializeAsV3(IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            var target = this;

            if (Reference != null)
            {
                if (!writer.GetSettings().ShouldInlineReference(Reference))
                {
                    Reference.SerializeAsV3(writer);
                    return;
                }
                else
                {
                    target = GetEffective(Reference.HostDocument);
                }
            }
            target.SerializeAsV3WithoutReference(writer);
        }

        /// <summary>
        /// Returns an effective OpenApiRequestBody object based on the presence of a $ref 
        /// </summary>
        /// <param name="doc">The host OpenApiDocument that contains the reference.</param>
        /// <returns>OpenApiRequestBody</returns>
        public OpenApiRequestBody GetEffective(OpenApiDocument doc)
        {
            if (this.Reference != null)
            {
                return doc.ResolveReferenceTo<OpenApiRequestBody>(this.Reference);
            }
            else
            {
                return this;
            }
        }

        /// <summary>
        /// Serialize to OpenAPI V3 document without using reference.
        /// </summary>
        public void SerializeAsV3WithoutReference(IOpenApiWriter writer)
        {
            writer.WriteStartObject();

            // description
            writer.WriteProperty(OpenApiConstants.Description, Description);

            // content
            writer.WriteRequiredMap(OpenApiConstants.Content, Content, (w, c) => c.SerializeAsV3(w));

            // required
            writer.WriteProperty(OpenApiConstants.Required, Required, false);

            // extensions
            writer.WriteExtensions(Extensions, OpenApiSpecVersion.OpenApi3_0);

            writer.WriteEndObject();
        }

        /// <summary>
        /// Serialize <see cref="OpenApiRequestBody"/> to Open Api v2.0
        /// </summary>
        public void SerializeAsV2(IOpenApiWriter writer)
        {
            // RequestBody object does not exist in V2.
        }

        /// <summary>
        /// Serialize to OpenAPI V2 document without using reference.
        /// </summary>
        public void SerializeAsV2WithoutReference(IOpenApiWriter writer)
        {
            // RequestBody object does not exist in V2.
        }

        internal OpenApiBodyParameter ConvertToBodyParameter()
        {
            var bodyParameter = new OpenApiBodyParameter
            {
                Description = Description,
                // V2 spec actually allows the body to have custom name.
                // To allow round-tripping we use an extension to hold the name
                Name = "body",
                Schema = Content.Values.FirstOrDefault()?.Schema ?? new OpenApiSchema(),
                Required = Required,
                Extensions = Extensions.ToDictionary(static k => k.Key, static v => v.Value)  // Clone extensions so we can remove the x-bodyName extensions from the output V2 model.
            };
            if (bodyParameter.Extensions.ContainsKey(OpenApiConstants.BodyName))
            {
                bodyParameter.Name = (Extensions[OpenApiConstants.BodyName] as OpenApiString)?.Value ?? "body";
                bodyParameter.Extensions.Remove(OpenApiConstants.BodyName);
            }
            return bodyParameter;
        }

        internal IEnumerable<OpenApiFormDataParameter> ConvertToFormDataParameters()
        {
            if (Content == null || !Content.Any())
                yield break;

            foreach (var property in Content.First().Value.Schema.Properties)
            {
                var paramSchema = property.Value;
                if ("string".Equals(paramSchema.Type, StringComparison.OrdinalIgnoreCase)
                    && ("binary".Equals(paramSchema.Format, StringComparison.OrdinalIgnoreCase)
                    || "base64".Equals(paramSchema.Format, StringComparison.OrdinalIgnoreCase)))
                {
                    paramSchema.Type = "file";
                    paramSchema.Format = null;
                }
                yield return new OpenApiFormDataParameter
                {
                    Description = property.Value.Description,
                    Name = property.Key,
                    Schema = property.Value,
                    Required = Content.First().Value.Schema.Required.Contains(property.Key) 
                };
            }
        }
    }
}
