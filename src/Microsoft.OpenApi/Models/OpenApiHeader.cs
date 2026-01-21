// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;

namespace Microsoft.OpenApi
{
    /// <summary>
    /// Header Object.
    /// The Header Object follows the structure of the Parameter Object.
    /// </summary>
    public class OpenApiHeader : IOpenApiHeader, IOpenApiExtensible, IOpenApiContentElement
    {
        /// <inheritdoc/>
        public string? Description { get; set; }

        /// <inheritdoc/>
        public bool Required { get; set; }

        /// <inheritdoc/>
        public bool Deprecated { get; set; }

        /// <inheritdoc/>
        public bool AllowEmptyValue { get; set; }

        /// <inheritdoc/>
        public ParameterStyle? Style { get; set; }

        /// <inheritdoc/>
        public bool Explode { get; set; }

        /// <inheritdoc/>
        public bool AllowReserved { get; set; }

        /// <inheritdoc/>
        public IOpenApiSchema? Schema { get; set; }

        /// <inheritdoc/>
        public JsonNode? Example { get; set; }

        /// <inheritdoc/>
        public IDictionary<string, IOpenApiExample>? Examples { get; set; }

        /// <summary>
        /// A map containing the representations for the header.
        /// </summary>
        public IDictionary<string, IOpenApiMediaType>? Content { get; set; }

        /// <inheritdoc/>
        public IDictionary<string, IOpenApiExtension>? Extensions { get; set; }

        /// <summary>
        /// Parameter-less constructor
        /// </summary>
        public OpenApiHeader() { }

        /// <summary>
        /// Initializes a copy of an <see cref="OpenApiHeader"/> object
        /// </summary>
        internal OpenApiHeader(IOpenApiHeader header)
        {
            Utils.CheckArgumentNull(header);
            Description = header.Description ?? Description;
            Required = header.Required;
            Deprecated = header.Deprecated;
            AllowEmptyValue = header.AllowEmptyValue;
            Style = header.Style ?? Style;
            Explode = header.Explode;
            AllowReserved = header.AllowReserved;
            Schema = header.Schema?.CreateShallowCopy();
            Example = header.Example != null ? JsonNodeCloneHelper.Clone(header.Example) : null;
            Examples = header.Examples != null ? new Dictionary<string, IOpenApiExample>(header.Examples) : null;
            Content = header.Content != null ? new Dictionary<string, IOpenApiMediaType>(header.Content) : null;
            Extensions = header.Extensions != null ? new Dictionary<string, IOpenApiExtension>(header.Extensions) : null;
        }

        /// <summary>
        /// Serialize <see cref="OpenApiHeader"/> to Open Api v3.2
        /// </summary>
        public virtual void SerializeAsV32(IOpenApiWriter writer)
        {
            SerializeInternal(writer, OpenApiSpecVersion.OpenApi3_2, (writer, element) => element.SerializeAsV32(writer));
        }

        /// <summary>
        /// Serialize <see cref="OpenApiHeader"/> to Open Api v3.1
        /// </summary>
        public virtual void SerializeAsV31(IOpenApiWriter writer)
        {
            SerializeInternal(writer, OpenApiSpecVersion.OpenApi3_1, (writer, element) => element.SerializeAsV31(writer));
        }

        /// <summary>
        /// Serialize <see cref="OpenApiHeader"/> to Open Api v3.0
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

            // required
            writer.WriteProperty(OpenApiConstants.Required, Required, false);

            // deprecated
            writer.WriteProperty(OpenApiConstants.Deprecated, Deprecated, false);

            // allowEmptyValue
            writer.WriteProperty(OpenApiConstants.AllowEmptyValue, AllowEmptyValue, false);

            // style
            writer.WriteProperty(OpenApiConstants.Style, Style?.GetDisplayName());

            // explode
            writer.WriteProperty(OpenApiConstants.Explode, Explode, false);

            // allowReserved
            writer.WriteProperty(OpenApiConstants.AllowReserved, AllowReserved, false);

            // schema
            writer.WriteOptionalObject(OpenApiConstants.Schema, Schema, callback);

            // example
            writer.WriteOptionalObject(OpenApiConstants.Example, Example, (w, s) => w.WriteAny(s));

            // examples
            writer.WriteOptionalMap(OpenApiConstants.Examples, Examples, callback);

            // content
            writer.WriteOptionalMap(OpenApiConstants.Content, Content, callback);

            // extensions
            writer.WriteExtensions(Extensions, version);

            writer.WriteEndObject();
        }

        /// <summary>
        /// Serialize to OpenAPI V2 document without using reference.
        /// </summary>
        public virtual void SerializeAsV2(IOpenApiWriter writer)
        {
            Utils.CheckArgumentNull(writer);

            writer.WriteStartObject();

            // description
            writer.WriteProperty(OpenApiConstants.Description, Description);

            // required
            writer.WriteProperty(OpenApiConstants.Required, Required, false);

            // deprecated
            writer.WriteProperty(OpenApiConstants.Deprecated, Deprecated, false);

            // allowEmptyValue
            writer.WriteProperty(OpenApiConstants.AllowEmptyValue, AllowEmptyValue, false);

            // style
            writer.WriteProperty(OpenApiConstants.Style, Style?.GetDisplayName());

            // explode
            writer.WriteProperty(OpenApiConstants.Explode, Explode, false);

            // allowReserved
            writer.WriteProperty(OpenApiConstants.AllowReserved, AllowReserved, false);

            // schema
            var targetSchema = Schema switch
            {
                OpenApiSchemaReference schemaReference => schemaReference.RecursiveTarget,
                OpenApiSchema schema => schema,
                _ => null,
            };
            targetSchema?.WriteAsItemsProperties(writer);

            // example
            writer.WriteOptionalObject(OpenApiConstants.Example, Example, (w, s) => w.WriteAny(s));

            // extensions
            writer.WriteExtensions(Extensions, OpenApiSpecVersion.OpenApi2_0);

            writer.WriteEndObject();
        }

        /// <inheritdoc/>
        public IOpenApiHeader CreateShallowCopy()
        {
            return new OpenApiHeader(this);
        }
    }
}
