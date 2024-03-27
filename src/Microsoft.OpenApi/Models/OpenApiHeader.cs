// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using Json.Schema;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Helpers;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Models
{
    /// <summary>
    /// Header Object.
    /// The Header Object follows the structure of the Parameter Object.
    /// </summary>
    public class OpenApiHeader : IOpenApiReferenceable, IOpenApiExtensible, IEffective<OpenApiHeader>
    {
        private JsonSchema _schema;

        /// <summary>
        /// Indicates if object is populated with data or is just a reference to the data
        /// </summary>
        public virtual bool UnresolvedReference { get; set; }

        /// <summary>
        /// Reference pointer.
        /// </summary>
        public OpenApiReference Reference { get; set; }

        /// <summary>
        /// A brief description of the header.
        /// </summary>
        public virtual string Description { get; set; }

        /// <summary>
        /// Determines whether this header is mandatory.
        /// </summary>
        public virtual bool Required { get; set; }

        /// <summary>
        /// Specifies that a header is deprecated and SHOULD be transitioned out of usage.
        /// </summary>
        public virtual bool Deprecated { get; set; }

        /// <summary>
        /// Sets the ability to pass empty-valued headers.
        /// </summary>
        public virtual bool AllowEmptyValue { get; set; }

        /// <summary>
        /// Describes how the header value will be serialized depending on the type of the header value.
        /// </summary>
        public virtual ParameterStyle? Style { get; set; }

        /// <summary>
        /// When this is true, header values of type array or object generate separate parameters
        /// for each value of the array or key-value pair of the map.
        /// </summary>
        public virtual bool Explode { get; set; }

        /// <summary>
        /// Determines whether the header value SHOULD allow reserved characters, as defined by RFC3986.
        /// </summary>
        public virtual bool AllowReserved { get; set; }

        /// <summary>
        /// The schema defining the type used for the request body.
        /// </summary>
        public virtual JsonSchema Schema 
        { 
            get => _schema; 
            set => _schema = value;  
        }

        /// <summary>
        /// Example of the media type.
        /// </summary>
        public virtual OpenApiAny Example { get; set; }

        /// <summary>
        /// Examples of the media type.
        /// </summary>
        public virtual IDictionary<string, OpenApiExample> Examples { get; set; } = new Dictionary<string, OpenApiExample>();

        /// <summary>
        /// A map containing the representations for the header.
        /// </summary>
        public virtual IDictionary<string, OpenApiMediaType> Content { get; set; } = new Dictionary<string, OpenApiMediaType>();

        /// <summary>
        /// This object MAY be extended with Specification Extensions.
        /// </summary>
        public virtual IDictionary<string, IOpenApiExtension> Extensions { get; set; } = new Dictionary<string, IOpenApiExtension>();

        /// <summary>
        /// Parameter-less constructor
        /// </summary>
        public OpenApiHeader() { }

        /// <summary>
        /// Initializes a copy of an <see cref="OpenApiHeader"/> object
        /// </summary>
        public OpenApiHeader(OpenApiHeader header)
        {
            UnresolvedReference = header?.UnresolvedReference ?? UnresolvedReference;
            Reference = header?.Reference != null ? new(header?.Reference) : null;
            Description = header?.Description ?? Description;
            Required = header?.Required ?? Required;
            Deprecated = header?.Deprecated ?? Deprecated;
            AllowEmptyValue = header?.AllowEmptyValue ?? AllowEmptyValue;
            Style = header?.Style ?? Style;
            Explode = header?.Explode ?? Explode;
            AllowReserved = header?.AllowReserved ?? AllowReserved;
            _schema = JsonNodeCloneHelper.CloneJsonSchema(header?.Schema);
            Example = JsonNodeCloneHelper.Clone(header?.Example);
            Examples = header?.Examples != null ? new Dictionary<string, OpenApiExample>(header.Examples) : null;
            Content = header?.Content != null ? new Dictionary<string, OpenApiMediaType>(header.Content) : null;
            Extensions = header?.Extensions != null ? new Dictionary<string, IOpenApiExtension>(header.Extensions) : null;
        }

        /// <summary>
        /// Serialize <see cref="OpenApiHeader"/> to Open Api v3.1
        /// </summary>
        public virtual void SerializeAsV31(IOpenApiWriter writer)
        {
            SerializeInternal(writer, (writer, element) => element.SerializeAsV31(writer),
                (writer, element) => element.SerializeAsV31WithoutReference(writer));
        }

        /// <summary>
        /// Serialize <see cref="OpenApiHeader"/> to Open Api v3.0
        /// </summary>
        public virtual void SerializeAsV3(IOpenApiWriter writer)
        {
            SerializeInternal(writer, (writer, element) => element.SerializeAsV3(writer),
                (writer, element) => element.SerializeAsV3WithoutReference(writer));
        }

        private void SerializeInternal(IOpenApiWriter writer, Action<IOpenApiWriter, IOpenApiSerializable> callback,
            Action<IOpenApiWriter, IOpenApiReferenceable> action)
        {
            Utils.CheckArgumentNull(writer);;

            var target = this;
            var isProxyReference = target.GetType().Name.Contains("Reference");

            if (Reference != null && !isProxyReference)
            {
                if (!writer.GetSettings().ShouldInlineReference(Reference))
                {
                    callback(writer, Reference);
                    return;
                }
                else
                {
                    target = GetEffective(Reference.HostDocument);
                }
            }

            action(writer, target);
        }

        /// <summary>
        /// Returns an effective OpenApiHeader object based on the presence of a $ref
        /// </summary>
        /// <param name="doc">The host OpenApiDocument that contains the reference.</param>
        /// <returns>OpenApiHeader</returns>
        public OpenApiHeader GetEffective(OpenApiDocument doc)
        {
            if (Reference != null)
            {
                return doc.ResolveReferenceTo<OpenApiHeader>(Reference);
            }
            else
            {
                return this;
            }
        }

        /// <summary>
        /// Serialize to OpenAPI V31 document without using reference.
        /// </summary>
        public virtual void SerializeAsV31WithoutReference(IOpenApiWriter writer) 
        {
            SerializeInternalWithoutReference(writer, OpenApiSpecVersion.OpenApi3_1,
                (writer, element) => element.SerializeAsV31(writer));
        }

        /// <summary>
        /// Serialize to OpenAPI V3 document without using reference.
        /// </summary>
        public virtual void SerializeAsV3WithoutReference(IOpenApiWriter writer) 
        {
            SerializeInternalWithoutReference(writer, OpenApiSpecVersion.OpenApi3_0,
                (writer, element) => element.SerializeAsV3(writer));
        }

        internal virtual void SerializeInternalWithoutReference(IOpenApiWriter writer, OpenApiSpecVersion version, 
            Action<IOpenApiWriter, IOpenApiSerializable> callback)
        {
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
            writer.WriteOptionalObject(OpenApiConstants.Schema, Schema, (w, s) => writer.WriteJsonSchema(s, version));

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
        /// Serialize <see cref="OpenApiHeader"/> to Open Api v2.0
        /// </summary>
        public void SerializeAsV2(IOpenApiWriter writer)
        {
            Utils.CheckArgumentNull(writer);

            var target = this;
            var isProxyReference = target.GetType().Name.Contains("Reference");

            if (Reference != null && !isProxyReference)
            {
                if (!writer.GetSettings().ShouldInlineReference(Reference))
                {
                    Reference.SerializeAsV2(writer);
                    return;
                }
                else
                {
                    target = GetEffective(Reference.HostDocument);
                }
            }
            target.SerializeAsV2WithoutReference(writer);
        }

        /// <summary>
        /// Serialize to OpenAPI V2 document without using reference.
        /// </summary>
        public void SerializeAsV2WithoutReference(IOpenApiWriter writer)
        {
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
            SchemaSerializerHelper.WriteAsItemsProperties(Schema, writer, Extensions, OpenApiSpecVersion.OpenApi2_0);

            // example
            writer.WriteOptionalObject(OpenApiConstants.Example, Example, (w, s) => w.WriteAny(s));

            // extensions
            writer.WriteExtensions(Extensions, OpenApiSpecVersion.OpenApi2_0);

            writer.WriteEndObject();
        }
    }
}
