// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models.Interfaces;
using Microsoft.OpenApi.Models.References;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Models
{
    /// <summary>
    /// Components Object.
    /// </summary>
    public class OpenApiComponents : IOpenApiSerializable, IOpenApiExtensible
    {
        /// <summary>
        /// An object to hold reusable <see cref="IOpenApiSchema"/> Objects.
        /// </summary>
        public Dictionary<string, IOpenApiSchema>? Schemas { get; set; }

        /// <summary>
        /// An object to hold reusable <see cref="IOpenApiResponse"/> Objects.
        /// </summary>
        public Dictionary<string, IOpenApiResponse>? Responses { get; set; }

        /// <summary>
        /// An object to hold reusable <see cref="IOpenApiParameter"/> Objects.
        /// </summary>
        public Dictionary<string, IOpenApiParameter>? Parameters { get; set; }

        /// <summary>
        /// An object to hold reusable <see cref="OpenApiExample"/> Objects.
        /// </summary>
        public Dictionary<string, IOpenApiExample>? Examples { get; set; }

        /// <summary>
        /// An object to hold reusable <see cref="IOpenApiRequestBody"/> Objects.
        /// </summary>
        public Dictionary<string, IOpenApiRequestBody>? RequestBodies { get; set; }

        /// <summary>
        /// An object to hold reusable <see cref="IOpenApiHeader"/> Objects.
        /// </summary>
        public Dictionary<string, IOpenApiHeader>? Headers { get; set; }

        /// <summary>
        /// An object to hold reusable <see cref="IOpenApiSecurityScheme"/> Objects.
        /// </summary>
        public Dictionary<string, IOpenApiSecurityScheme>? SecuritySchemes { get; set; }

        /// <summary>
        /// An object to hold reusable <see cref="IOpenApiLink"/> Objects.
        /// </summary>
        public Dictionary<string, IOpenApiLink>? Links { get; set; }

        /// <summary>
        /// An object to hold reusable <see cref="OpenApiCallback"/> Objects.
        /// </summary>
        public Dictionary<string, IOpenApiCallback>? Callbacks { get; set; }

        /// <summary>
        /// An object to hold reusable <see cref="IOpenApiPathItem"/> Object.
        /// </summary>
        public Dictionary<string, IOpenApiPathItem>? PathItems { get; set; }

        /// <summary>
        /// This object MAY be extended with Specification Extensions.
        /// </summary>
        public Dictionary<string, IOpenApiExtension>? Extensions { get; set; }

        /// <summary>
        /// Parameter-less constructor
        /// </summary>
        public OpenApiComponents() { }

        /// <summary>
        /// Initializes a copy of an <see cref="OpenApiComponents"/> object
        /// </summary>
        public OpenApiComponents(OpenApiComponents? components)
        {
            Schemas = components?.Schemas != null ? new Dictionary<string, IOpenApiSchema>(components.Schemas) : null;
            Responses = components?.Responses != null ? new Dictionary<string, IOpenApiResponse>(components.Responses) : null;
            Parameters = components?.Parameters != null ? new Dictionary<string, IOpenApiParameter>(components.Parameters) : null;
            Examples = components?.Examples != null ? new Dictionary<string, IOpenApiExample>(components.Examples) : null;
            RequestBodies = components?.RequestBodies != null ? new Dictionary<string, IOpenApiRequestBody>(components.RequestBodies) : null;
            Headers = components?.Headers != null ? new Dictionary<string, IOpenApiHeader>(components.Headers) : null;
            SecuritySchemes = components?.SecuritySchemes != null ? new Dictionary<string, IOpenApiSecurityScheme>(components.SecuritySchemes) : null;
            Links = components?.Links != null ? new Dictionary<string, IOpenApiLink>(components.Links) : null;
            Callbacks = components?.Callbacks != null ? new Dictionary<string, IOpenApiCallback>(components.Callbacks) : null;
            PathItems = components?.PathItems != null ? new Dictionary<string, IOpenApiPathItem>(components.PathItems) : null;
            Extensions = components?.Extensions != null ? new Dictionary<string, IOpenApiExtension>(components.Extensions) : null;
        }

        /// <summary>
        /// Serialize <see cref="OpenApiComponents"/> to Open API v3.1.
        /// </summary>
        /// <param name="writer"></param>
        public virtual void SerializeAsV31(IOpenApiWriter writer)
        {
            Utils.CheckArgumentNull(writer);

            // If references have been inlined we don't need the to render the components section
            // however if they have cycles, then we will need a component rendered
            if (writer.GetSettings().InlineLocalReferences)
            {
                RenderComponents(writer, (writer, element) => element.SerializeAsV31(writer), OpenApiSpecVersion.OpenApi3_1);                
                return;
            }

            writer.WriteStartObject();

            // pathItems - only present in v3.1
            writer.WriteOptionalMap(
            OpenApiConstants.PathItems,
            PathItems,
            (w, key, component) =>
            {
                if (component is OpenApiPathItemReference reference)
                {
                    reference.SerializeAsV31(w);
                }
                else
                {
                    component.SerializeAsV31(w);
                }
            });

            SerializeInternal(writer, OpenApiSpecVersion.OpenApi3_1, (writer, element) => element.SerializeAsV31(writer),
               (writer, referenceElement) => referenceElement.SerializeAsV31(writer));
        }

        /// <summary>
        /// Serialize <see cref="OpenApiComponents"/> to v3.0
        /// </summary>
        /// <param name="writer"></param>
        public virtual void SerializeAsV3(IOpenApiWriter writer)
        {
            Utils.CheckArgumentNull(writer);

            // If references have been inlined we don't need the to render the components section
            // however if they have cycles, then we will need a component rendered
            if (writer.GetSettings().InlineLocalReferences)
            {
                RenderComponents(writer, (writer, element) => element.SerializeAsV3(writer), OpenApiSpecVersion.OpenApi3_0);
                return;
            }

            writer.WriteStartObject();
            SerializeInternal(writer, OpenApiSpecVersion.OpenApi3_0, (writer, element) => element.SerializeAsV3(writer),
                (writer, referenceElement) => referenceElement.SerializeAsV3(writer));
        }

        /// <summary>
        /// Serialize <see cref="OpenApiComponents"/>.
        /// </summary>
        private void SerializeInternal(IOpenApiWriter writer, OpenApiSpecVersion version,
            Action<IOpenApiWriter, IOpenApiSerializable> callback, Action<IOpenApiWriter, IOpenApiReferenceHolder> action)
        {
            // Serialize each referenceable object as full object without reference if the reference in the object points to itself.
            // If the reference exists but points to other objects, the object is serialized to just that reference.

            // schemas
            writer.WriteOptionalMap(
                OpenApiConstants.Schemas,
                Schemas,
                (w, key, component) =>
                {
                    if (component is OpenApiSchemaReference reference)
                    {
                        action(w, reference);
                    }
                    else
                    {
                        callback(w, component);
                    }
                });

            // responses
            writer.WriteOptionalMap(
                OpenApiConstants.Responses,
                Responses,
                (w, key, component) =>
                {
                    if (component is OpenApiResponseReference reference)
                    {
                        action(w, reference);
                    }
                    else
                    {
                        callback(w, component);
                    }
                });

            // parameters
            writer.WriteOptionalMap(
                OpenApiConstants.Parameters,
                Parameters,
                (w, key, component) =>
                {
                    if (component is OpenApiParameterReference reference)
                    {
                        action(w, reference);
                    }
                    else
                    {
                        callback(w, component);
                    }
                });

            // examples
            writer.WriteOptionalMap(
                OpenApiConstants.Examples,
                Examples,
                (w, key, component) =>
                {
                    if (component is OpenApiExampleReference reference)
                    {
                        action(w, reference);
                    }
                    else
                    {
                        callback(w, component);
                    }
                });

            // requestBodies
            writer.WriteOptionalMap(
                OpenApiConstants.RequestBodies,
                RequestBodies,
                (w, key, component) =>
                {
                    if (component is OpenApiRequestBodyReference reference)
                    {
                        action(w, reference);
                    }
                    else
                    {
                        callback(w, component);
                    }
                });

            // headers
            writer.WriteOptionalMap(
                OpenApiConstants.Headers,
                Headers,
                (w, key, component) =>
                {
                    if (component is OpenApiHeaderReference reference)
                    {
                        action(w, reference);
                    }
                    else
                    {
                        callback(w, component);
                    }
                });

            // securitySchemes
            writer.WriteOptionalMap(
                OpenApiConstants.SecuritySchemes,
                SecuritySchemes,
                (w, key, component) =>
                {
                    if (component is OpenApiSecuritySchemeReference reference)
                    {
                        action(w, reference);
                    }
                    else
                    {
                        callback(w, component);
                    }
                });

            // links
            writer.WriteOptionalMap(
                OpenApiConstants.Links,
                Links,
                (w, key, component) =>
                {
                    if (component is OpenApiLinkReference reference)
                    {
                        action(w, reference);
                    }
                    else
                    {
                        callback(w, component);
                    }
                });

            // callbacks
            writer.WriteOptionalMap(
                OpenApiConstants.Callbacks,
                Callbacks,
                (w, key, component) =>
                {
                    if (component is OpenApiCallbackReference reference)
                    {
                        action(w, reference);
                    }
                    else
                    {
                        callback(w, component);
                    }
                });

            // extensions
            writer.WriteExtensions(Extensions, version);
            writer.WriteEndObject();
        }

        private void RenderComponents(IOpenApiWriter writer, Action<IOpenApiWriter, IOpenApiSerializable> callback, OpenApiSpecVersion version)
        {
            var loops = writer.GetSettings().LoopDetector.Loops;
            writer.WriteStartObject();
            if (loops.TryGetValue(typeof(OpenApiSchema), out _))
            {
                writer.WriteOptionalMap(OpenApiConstants.Schemas, Schemas, callback);
            }
            // always render security schemes as inlining of security requirement objects is not allowed in the spec
            if (SecuritySchemes is not null && SecuritySchemes.Any())
            {
                writer.WriteOptionalMap(
                OpenApiConstants.SecuritySchemes,
                SecuritySchemes,
                (w, key, component) =>
                {
                    if (version is OpenApiSpecVersion.OpenApi3_1)
                        component.SerializeAsV31(writer);
                    component.SerializeAsV3(writer);
                });
            }
            writer.WriteEndObject();
        }

        /// <summary>
        /// Serialize <see cref="OpenApiComponents"/> to Open Api v2.0.
        /// </summary>
        public virtual void SerializeAsV2(IOpenApiWriter writer)
        {
            // Components object does not exist in V2.
        }
    }
}
