// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Helpers;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Models
{
    /// <summary>
    /// Example Object.
    /// </summary>
    public class OpenApiExample : IOpenApiReferenceable, IOpenApiExtensible
    {
        /// <summary>
        /// Short description for the example.
        /// </summary>
        public virtual string Summary { get; set; }

        /// <summary>
        /// Long description for the example.
        /// CommonMark syntax MAY be used for rich text representation.
        /// </summary>
        public virtual string Description { get; set; }

        /// <summary>
        /// Embedded literal example. The value field and externalValue field are mutually
        /// exclusive. To represent examples of media types that cannot naturally represented
        /// in JSON or YAML, use a string value to contain the example, escaping where necessary.
        /// </summary>
        public virtual JsonNode Value { get; set; }

        /// <summary>
        /// A URL that points to the literal example.
        /// This provides the capability to reference examples that cannot easily be
        /// included in JSON or YAML documents.
        /// The value field and externalValue field are mutually exclusive.
        /// </summary>
        public virtual string ExternalValue { get; set; }

        /// <summary>
        /// This object MAY be extended with Specification Extensions.
        /// </summary>
        public virtual IDictionary<string, IOpenApiExtension> Extensions { get; set; } = new Dictionary<string, IOpenApiExtension>();

        /// <summary>
        /// Reference object.
        /// </summary>
        public virtual OpenApiReference Reference { get; set; }

        /// <summary>
        /// Indicates object is a placeholder reference to an actual object and does not contain valid data.
        /// </summary>
        public virtual bool UnresolvedReference { get; set; } = false;

        /// <summary>
        /// Parameter-less constructor
        /// </summary>
        public OpenApiExample() { }

        /// <summary>
        /// Initializes a copy of <see cref="OpenApiExample"/> object
        /// </summary>
        public OpenApiExample(OpenApiExample example)
        {
            Summary = example?.Summary ?? Summary;
            Description = example?.Description ?? Description;
            Value = example?.Value != null ? JsonNodeCloneHelper.Clone(example.Value) : null;
            ExternalValue = example?.ExternalValue ?? ExternalValue;
            Extensions = example?.Extensions != null ? new Dictionary<string, IOpenApiExtension>(example.Extensions) : null;
            Reference = example?.Reference != null ? new(example.Reference) : null;
            UnresolvedReference = example?.UnresolvedReference ?? UnresolvedReference;
        }

        /// <summary>
        /// Serialize <see cref="OpenApiExample"/> to Open Api v3.1
        /// </summary>
        /// <param name="writer"></param>
        public virtual void SerializeAsV31(IOpenApiWriter writer)
        {
            SerializeInternal(writer, OpenApiSpecVersion.OpenApi3_1);
        }

        /// <summary>
        /// Serialize <see cref="OpenApiExample"/> to Open Api v3.0
        /// </summary>
        /// <param name="writer"></param>
        public virtual void SerializeAsV3(IOpenApiWriter writer)
        {
            SerializeInternal(writer, OpenApiSpecVersion.OpenApi3_0);
        }

        /// <summary>
        /// Writes out existing examples in a mediatype object
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="version"></param>
        public void SerializeInternal(IOpenApiWriter writer, OpenApiSpecVersion version)
        {
            Utils.CheckArgumentNull(writer);

            writer.WriteStartObject();

            // summary
            writer.WriteProperty(OpenApiConstants.Summary, Summary);

            // description
            writer.WriteProperty(OpenApiConstants.Description, Description);

            // value
            writer.WriteOptionalObject(OpenApiConstants.Value, Value, (w, v) => w.WriteAny(v));

            // externalValue
            writer.WriteProperty(OpenApiConstants.ExternalValue, ExternalValue);

            // extensions
            writer.WriteExtensions(Extensions, version);

            writer.WriteEndObject();
        }

        /// <summary>
        /// Serialize <see cref="OpenApiExample"/> to Open Api v2.0
        /// </summary>
        public virtual void SerializeAsV2(IOpenApiWriter writer)
        {
            // Example object of this form does not exist in V2.
            // V2 Example object requires knowledge of media type and exists only
            // in Response object, so it will be serialized as a part of the Response object.
        }
    }
}
