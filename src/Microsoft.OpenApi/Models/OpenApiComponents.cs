// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Models
{
    /// <summary>
    /// Components Object.
    /// </summary>
    public class OpenApiComponents : IOpenApiSerializable, IOpenApiExtensible
    {
        /// <summary>
        /// An object to hold reusable <see cref="OpenApiSchema"/> Objects.
        /// </summary>
        public IDictionary<string, OpenApiSchema> Schemas { get; set; } = new Dictionary<string, OpenApiSchema>();

        /// <summary>
        /// An object to hold reusable <see cref="OpenApiResponse"/> Objects.
        /// </summary>
        public IDictionary<string, OpenApiResponse> Responses { get; set; } = new Dictionary<string, OpenApiResponse>();

        /// <summary>
        /// An object to hold reusable <see cref="OpenApiParameter"/> Objects.
        /// </summary>
        public IDictionary<string, OpenApiParameter> Parameters { get; set; } =
            new Dictionary<string, OpenApiParameter>();

        /// <summary>
        /// An object to hold reusable <see cref="OpenApiExample"/> Objects.
        /// </summary>
        public IDictionary<string, OpenApiExample> Examples { get; set; } = new Dictionary<string, OpenApiExample>();

        /// <summary>
        /// An object to hold reusable <see cref="OpenApiRequestBody"/> Objects.
        /// </summary>
        public IDictionary<string, OpenApiRequestBody> RequestBodies { get; set; } =
            new Dictionary<string, OpenApiRequestBody>();

        /// <summary>
        /// An object to hold reusable <see cref="OpenApiHeader"/> Objects.
        /// </summary>
        public IDictionary<string, OpenApiHeader> Headers { get; set; } = new Dictionary<string, OpenApiHeader>();

        /// <summary>
        /// An object to hold reusable <see cref="OpenApiSecurityScheme"/> Objects.
        /// </summary>
        public IDictionary<string, OpenApiSecurityScheme> SecuritySchemes { get; set; } =
            new Dictionary<string, OpenApiSecurityScheme>();

        /// <summary>
        /// An object to hold reusable <see cref="OpenApiLink"/> Objects.
        /// </summary>
        public IDictionary<string, OpenApiLink> Links { get; set; } = new Dictionary<string, OpenApiLink>();

        /// <summary>
        /// An object to hold reusable <see cref="OpenApiCallback"/> Objects.
        /// </summary>
        public IDictionary<string, OpenApiCallback> Callbacks { get; set; } = new Dictionary<string, OpenApiCallback>();

        /// <summary>
        /// This object MAY be extended with Specification Extensions.
        /// </summary>
        public IDictionary<string, IOpenApiExtension> Extensions { get; set; } = new Dictionary<string, IOpenApiExtension>();

        /// <summary>
        /// Parameter-less constructor
        /// </summary>
        public OpenApiComponents() { }

        /// <summary>
        /// Initializes a copy of an <see cref="OpenApiComponents"/> object
        /// </summary>
        public OpenApiComponents(OpenApiComponents components)
        {
            Schemas = components?.Schemas != null ? new Dictionary<string, OpenApiSchema>(components.Schemas) : null;
            Responses = components?.Responses != null ? new Dictionary<string, OpenApiResponse>(components.Responses) : null;
            Parameters = components?.Parameters != null ? new Dictionary<string, OpenApiParameter>(components.Parameters) : null;
            Examples = components?.Examples != null ? new Dictionary<string, OpenApiExample>(components.Examples) : null;
            RequestBodies = components?.RequestBodies != null ? new Dictionary<string, OpenApiRequestBody>(components.RequestBodies) : null;
            Headers = components?.Headers != null ? new Dictionary<string, OpenApiHeader>(components.Headers) : null;
            SecuritySchemes = components?.SecuritySchemes != null ? new Dictionary<string, OpenApiSecurityScheme>(components.SecuritySchemes) : null;
            Links = components?.Links != null ? new Dictionary<string, OpenApiLink>(components.Links) : null;
            Callbacks = components?.Callbacks != null ? new Dictionary<string, OpenApiCallback>(components.Callbacks) : null;
            Extensions = components?.Extensions != null ? new Dictionary<string, IOpenApiExtension>(components.Extensions) : null;
        }

        /// <summary>
        /// Serialize <see cref="OpenApiComponents"/> to Open Api v3.0.
        /// </summary>
        public void SerializeAsV3(IOpenApiWriter writer)
        {
            Utils.CheckArgumentNull(writer);

            // If references have been inlined we don't need the to render the components section
            // however if they have cycles, then we will need a component rendered
            if (writer.GetSettings().InlineLocalReferences)
            {
                var loops = writer.GetSettings().LoopDetector.Loops;
                writer.WriteStartObject();
                if (loops.TryGetValue(typeof(OpenApiSchema), out var schemas))
                {
                    var openApiSchemas = schemas.Cast<OpenApiSchema>().Distinct().ToList()
                        .ToDictionary<OpenApiSchema, string>(k => k.Reference.Id);

                    writer.WriteOptionalMap(
                       OpenApiConstants.Schemas,
                       Schemas,
                       (w, _, component) => component.SerializeAsV3WithoutReference(w));
                }
                writer.WriteEndObject();
                return;
            }

            writer.WriteStartObject();

            // Serialize each referenceable object as full object without reference if the reference in the object points to itself.
            // If the reference exists but points to other objects, the object is serialized to just that reference.

            // schemas
            writer.WriteOptionalMap(
                OpenApiConstants.Schemas,
                Schemas,
                (w, key, component) =>
                {
                    if (component.Reference is {Type: ReferenceType.Schema} &&
                        component.Reference.Id == key)
                    {
                        component.SerializeAsV3WithoutReference(w);
                    }
                    else
                    {
                        component.SerializeAsV3(w);
                    }
                });

            // responses
            writer.WriteOptionalMap(
                OpenApiConstants.Responses,
                Responses,
                (w, key, component) =>
                {
                    if (component.Reference is {Type: ReferenceType.Response} &&
                        component.Reference.Id == key)
                    {
                        component.SerializeAsV3WithoutReference(w);
                    }
                    else
                    {
                        component.SerializeAsV3(w);
                    }
                });

            // parameters
            writer.WriteOptionalMap(
                OpenApiConstants.Parameters,
                Parameters,
                (w, key, component) =>
                {
                    if (component.Reference is {Type: ReferenceType.Parameter} &&
                        component.Reference.Id == key)
                    {
                        component.SerializeAsV3WithoutReference(w);
                    }
                    else
                    {
                        component.SerializeAsV3(w);
                    }
                });

            // examples
            writer.WriteOptionalMap(
                OpenApiConstants.Examples,
                Examples,
                (w, key, component) =>
                {
                    if (component.Reference is {Type: ReferenceType.Example} &&
                        component.Reference.Id == key)
                    {
                        component.SerializeAsV3WithoutReference(w);
                    }
                    else
                    {
                        component.SerializeAsV3(w);
                    }
                });

            // requestBodies
            writer.WriteOptionalMap(
                OpenApiConstants.RequestBodies,
                RequestBodies,
                (w, key, component) =>
                {
                    if (component.Reference is {Type: ReferenceType.RequestBody} &&
                        component.Reference.Id == key)
                    {
                        component.SerializeAsV3WithoutReference(w);
                    }
                    else
                    {
                        component.SerializeAsV3(w);
                    }
                });

            // headers
            writer.WriteOptionalMap(
                OpenApiConstants.Headers,
                Headers,
                (w, key, component) =>
                {
                    if (component.Reference is {Type: ReferenceType.Header} &&
                        component.Reference.Id == key)
                    {
                        component.SerializeAsV3WithoutReference(w);
                    }
                    else
                    {
                        component.SerializeAsV3(w);
                    }
                });

            // securitySchemes
            writer.WriteOptionalMap(
                OpenApiConstants.SecuritySchemes,
                SecuritySchemes,
                (w, key, component) =>
                {
                    if (component.Reference is {Type: ReferenceType.SecurityScheme} &&
                        component.Reference.Id == key)
                    {
                        component.SerializeAsV3WithoutReference(w);
                    }
                    else
                    {
                        component.SerializeAsV3(w);
                    }
                });

            // links
            writer.WriteOptionalMap(
                OpenApiConstants.Links,
                Links,
                (w, key, component) =>
                {
                    if (component.Reference is {Type: ReferenceType.Link} &&
                        component.Reference.Id == key)
                    {
                        component.SerializeAsV3WithoutReference(w);
                    }
                    else
                    {
                        component.SerializeAsV3(w);
                    }
                });

            // callbacks
            writer.WriteOptionalMap(
                OpenApiConstants.Callbacks,
                Callbacks,
                (w, key, component) =>
                {
                    if (component.Reference is {Type: ReferenceType.Callback} &&
                        component.Reference.Id == key)
                    {
                        component.SerializeAsV3WithoutReference(w);
                    }
                    else
                    {
                        component.SerializeAsV3(w);
                    }
                });

            // extensions
            writer.WriteExtensions(Extensions, OpenApiSpecVersion.OpenApi3_0);

            writer.WriteEndObject();
        }

        /// <summary>
        /// Serialize <see cref="OpenApiComponents"/> to Open Api v2.0.
        /// </summary>
        public void SerializeAsV2(IOpenApiWriter writer)
        {
            // Components object does not exist in V2.
        }
    }
}
