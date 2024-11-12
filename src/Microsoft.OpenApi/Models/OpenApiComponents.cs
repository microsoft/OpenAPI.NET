// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models.References;
using Microsoft.OpenApi.Writers;

#nullable enable

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
        public IDictionary<string, OpenApiSchema>? Schemas { get; set; } = new Dictionary<string, OpenApiSchema>();

        /// <summary>
        /// An object to hold reusable <see cref="OpenApiResponse"/> Objects.
        /// </summary>
        public virtual IDictionary<string, OpenApiResponse>? Responses { get; set; } = new Dictionary<string, OpenApiResponse>();

        /// <summary>
        /// An object to hold reusable <see cref="OpenApiParameter"/> Objects.
        /// </summary>
        public virtual IDictionary<string, OpenApiParameter>? Parameters { get; set; } =
            new Dictionary<string, OpenApiParameter>();

        /// <summary>
        /// An object to hold reusable <see cref="OpenApiExample"/> Objects.
        /// </summary>
        public virtual IDictionary<string, OpenApiExample>? Examples { get; set; } = new Dictionary<string, OpenApiExample>();

        /// <summary>
        /// An object to hold reusable <see cref="OpenApiRequestBody"/> Objects.
        /// </summary>
        public virtual IDictionary<string, OpenApiRequestBody>? RequestBodies { get; set; } =
            new Dictionary<string, OpenApiRequestBody>();

        /// <summary>
        /// An object to hold reusable <see cref="OpenApiHeader"/> Objects.
        /// </summary>
        public virtual IDictionary<string, OpenApiHeader>? Headers { get; set; } = new Dictionary<string, OpenApiHeader>();

        /// <summary>
        /// An object to hold reusable <see cref="OpenApiSecurityScheme"/> Objects.
        /// </summary>
        public virtual IDictionary<string, OpenApiSecurityScheme>? SecuritySchemes { get; set; } =
            new Dictionary<string, OpenApiSecurityScheme>();

        /// <summary>
        /// An object to hold reusable <see cref="OpenApiLink"/> Objects.
        /// </summary>
        public virtual IDictionary<string, OpenApiLink>? Links { get; set; } = new Dictionary<string, OpenApiLink>();

        /// <summary>
        /// An object to hold reusable <see cref="OpenApiCallback"/> Objects.
        /// </summary>
        public virtual IDictionary<string, OpenApiCallback>? Callbacks { get; set; } = new Dictionary<string, OpenApiCallback>();

        /// <summary>
        /// An object to hold reusable <see cref="OpenApiPathItem"/> Object.
        /// </summary>
        public virtual IDictionary<string, OpenApiPathItem>? PathItems { get; set; } = new Dictionary<string, OpenApiPathItem>();

        /// <summary>
        /// This object MAY be extended with Specification Extensions.
        /// </summary>
        public virtual IDictionary<string, IOpenApiExtension>? Extensions { get; set; } = new Dictionary<string, IOpenApiExtension>();

        /// <summary>
        /// Parameter-less constructor
        /// </summary>
        public OpenApiComponents() { }

        /// <summary>
        /// Initializes a copy of an <see cref="OpenApiComponents"/> object
        /// </summary>
        public OpenApiComponents(OpenApiComponents? components)
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
            PathItems = components?.PathItems != null ? new Dictionary<string, OpenApiPathItem>(components.PathItems) : null;
            Extensions = components?.Extensions != null ? new Dictionary<string, IOpenApiExtension>(components.Extensions) : null;
        }

        /// <summary>
        /// Serialize <see cref="OpenApiComponents"/> to Open API v3.1.
        /// </summary>
        /// <param name="writer"></param>
        public void SerializeAsV31(IOpenApiWriter writer)
        {
            Utils.CheckArgumentNull(writer);

            // If references have been inlined we don't need the to render the components section
            // however if they have cycles, then we will need a component rendered
            if (writer.GetSettings().InlineLocalReferences)
            {
                RenderComponents(writer, (writer, element) => element.SerializeAsV31(writer));
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
        public void SerializeAsV3(IOpenApiWriter writer)
        {
            Utils.CheckArgumentNull(writer);

            // If references have been inlined we don't need the to render the components section
            // however if they have cycles, then we will need a component rendered
            if (writer.GetSettings().InlineLocalReferences)
            {
                RenderComponents(writer, (writer, element) => element.SerializeAsV3(writer));
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
            Action<IOpenApiWriter, IOpenApiSerializable> callback, Action<IOpenApiWriter, IOpenApiReferenceable> action)
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

        private void RenderComponents(IOpenApiWriter writer, Action<IOpenApiWriter, IOpenApiSerializable> callback)
        {
            var loops = writer.GetSettings().LoopDetector.Loops;
            writer.WriteStartObject();
            if (loops.TryGetValue(typeof(OpenApiSchema), out List<object> schemas))
            {
                writer.WriteOptionalMap(OpenApiConstants.Schemas, Schemas, callback);
            }
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
