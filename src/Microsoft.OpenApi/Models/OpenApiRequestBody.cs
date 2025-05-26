// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi
{
    /// <summary>
    /// Request Body Object
    /// </summary>
    public class OpenApiRequestBody : IOpenApiExtensible, IOpenApiRequestBody
    {
        /// <inheritdoc />
        public string? Description { get; set; }

        /// <inheritdoc />
        public bool Required { get; set; }

        /// <inheritdoc />
        public Dictionary<string, OpenApiMediaType>? Content { get; set; }

        /// <inheritdoc />
        public Dictionary<string, IOpenApiExtension>? Extensions { get; set; }

        /// <summary>
        /// Parameter-less constructor
        /// </summary>
        public OpenApiRequestBody() { }

        /// <summary>
        /// Initializes a copy instance of an <see cref="IOpenApiRequestBody"/> object
        /// </summary>
        internal OpenApiRequestBody(IOpenApiRequestBody requestBody)
        {
            Utils.CheckArgumentNull(requestBody);
            Description = requestBody.Description ?? Description;
            Required = requestBody.Required;
            Content = requestBody.Content != null ? new Dictionary<string, OpenApiMediaType>(requestBody.Content) : null;
            Extensions = requestBody.Extensions != null ? new Dictionary<string, IOpenApiExtension>(requestBody.Extensions) : null;
        }

        /// <summary>
        /// Serialize <see cref="OpenApiRequestBody"/> to Open Api v3.1
        /// </summary>
        public virtual void SerializeAsV31(IOpenApiWriter writer)
        {
            SerializeInternal(writer, OpenApiSpecVersion.OpenApi3_1, (writer, element) => element.SerializeAsV31(writer));
        }

        /// <summary>
        /// Serialize <see cref="OpenApiRequestBody"/> to Open Api v3.0
        /// </summary>
        public virtual void SerializeAsV3(IOpenApiWriter writer)
        {
            SerializeInternal(writer, OpenApiSpecVersion.OpenApi3_0, (writer, element) => element.SerializeAsV3(writer));
        }
        
        internal void SerializeInternal(IOpenApiWriter writer, OpenApiSpecVersion version,
            Action<IOpenApiWriter, IOpenApiSerializable> callback)
        {
            Utils.CheckArgumentNull(writer);

            writer.WriteStartObject();

            // description
            writer.WriteProperty(OpenApiConstants.Description, Description);

            // content
            writer.WriteRequiredMap(OpenApiConstants.Content, Content, callback);

            // required
            writer.WriteProperty(OpenApiConstants.Required, Required, false);

            // extensions
            writer.WriteExtensions(Extensions, version);

            writer.WriteEndObject();
        }

        /// <summary>
        /// Serialize <see cref="OpenApiRequestBody"/> to Open Api v2.0
        /// </summary>
        public virtual void SerializeAsV2(IOpenApiWriter writer)
        {
            // RequestBody object does not exist in V2.
        }

        /// <inheritdoc/>
        public IOpenApiParameter ConvertToBodyParameter(IOpenApiWriter writer)
        {
            var bodyParameter = new OpenApiBodyParameter
            {
                Description = Description,
                // V2 spec actually allows the body to have custom name.
                // To allow round-tripping we use an extension to hold the name
                Name = "body",
                Schema = Content?.Values.FirstOrDefault()?.Schema ?? new OpenApiSchema(),
                Examples = Content?.Values.FirstOrDefault()?.Examples,
                Required = Required,
                Extensions = Extensions?.ToDictionary(static k => k.Key, static v => v.Value)
            };
            // Clone extensions so we can remove the x-bodyName extensions from the output V2 model.
            if (bodyParameter.Extensions is not null && 
                bodyParameter.Extensions.TryGetValue(OpenApiConstants.BodyName, out var bodyNameExtension) &&
                bodyNameExtension is JsonNodeExtension bodyName)
            {
                bodyParameter.Name = string.IsNullOrEmpty(bodyName.Node.ToString()) ? "body" : bodyName.Node.ToString();
                bodyParameter.Extensions.Remove(OpenApiConstants.BodyName);
            }
            return bodyParameter;
        }

        /// <inheritdoc/>
        public IEnumerable<IOpenApiParameter> ConvertToFormDataParameters(IOpenApiWriter writer)
        {
            if (Content == null || !Content.Any())
                yield break;
            var properties = Content.First().Value.Schema?.Properties;
            if(properties != null)
            {
                foreach (var property in properties)
                {
                    var paramSchema = property.Value.CreateShallowCopy();
                    if ((paramSchema.Type & JsonSchemaType.String) == JsonSchemaType.String
                        && ("binary".Equals(paramSchema.Format, StringComparison.OrdinalIgnoreCase)
                        || "base64".Equals(paramSchema.Format, StringComparison.OrdinalIgnoreCase)))
                    {
                        var updatedSchema = paramSchema switch
                        {
                            OpenApiSchema s => s, // we already have a copy
                                                  // we have a copy of a reference but don't want to mutate the source schema
                                                  // TODO might need recursive resolution of references here
                            OpenApiSchemaReference r when r.Target is not null => (OpenApiSchema)r.Target.CreateShallowCopy(),
                            OpenApiSchemaReference => throw new InvalidOperationException("Unresolved reference target"),
                            _ => throw new InvalidOperationException("Unexpected schema type")
                        };
                        
                        updatedSchema.Type = "file".ToJsonSchemaType();
                        updatedSchema.Format = null;
                        paramSchema = updatedSchema;
                        
                    }
                    yield return new OpenApiFormDataParameter()
                    {
                        Description = paramSchema.Description,
                        Name = property.Key,
                        Schema = paramSchema,
                        Examples = Content.Values.FirstOrDefault()?.Examples,
                        Required = Content.First().Value.Schema?.Required?.Contains(property.Key) ?? false
                    };
                }
            }            
        }

        /// <inheritdoc/>
        public IOpenApiRequestBody CreateShallowCopy()
        {
            return new OpenApiRequestBody(this);
        }
    }
}
