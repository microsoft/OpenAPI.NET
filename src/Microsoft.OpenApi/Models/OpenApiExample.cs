// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.Text.Json.Nodes;

namespace Microsoft.OpenApi
{
    /// <summary>
    /// Example Object.
    /// </summary>
    public class OpenApiExample : IOpenApiExtensible, IOpenApiExample
    {
        /// <inheritdoc/>
        public string? Summary { get; set; }

        /// <inheritdoc/>
        public string? Description { get; set; }

        /// <inheritdoc/>
        public string? ExternalValue { get; set; }

        /// <inheritdoc/>
        public JsonNode? Value { get; set; }

        /// <inheritdoc/>
        public IDictionary<string, IOpenApiExtension>? Extensions { get; set; }

        /// <summary>
        /// Parameter-less constructor
        /// </summary>
        public OpenApiExample() { }

        /// <summary>
        /// Initializes a copy of <see cref="OpenApiExample"/> object
        /// </summary>
        /// <param name="example">The <see cref="IOpenApiExample"/> object</param>
        internal OpenApiExample(IOpenApiExample example)
        {
            Utils.CheckArgumentNull(example);
            Summary = example.Summary ?? Summary;
            Description = example.Description ?? Description;
            Value = example.Value != null ? JsonNodeCloneHelper.Clone(example.Value) : null;
            ExternalValue = example.ExternalValue ?? ExternalValue;
            Extensions = example.Extensions != null ? new Dictionary<string, IOpenApiExtension>(example.Extensions) : null;
        }

        /// <inheritdoc/>
        public virtual void SerializeAsV31(IOpenApiWriter writer)
        {
            SerializeInternal(writer, OpenApiSpecVersion.OpenApi3_1);
        }

        /// <inheritdoc/>
        public virtual void SerializeAsV3(IOpenApiWriter writer)
        {
            SerializeInternal(writer, OpenApiSpecVersion.OpenApi3_0);
        }

        private void SerializeInternal(IOpenApiWriter writer, OpenApiSpecVersion version)
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

        /// <inheritdoc/>
        public virtual void SerializeAsV2(IOpenApiWriter writer)
        {
            SerializeInternal(writer, OpenApiSpecVersion.OpenApi2_0);
        }

        /// <inheritdoc/>
        public IOpenApiExample CreateShallowCopy()
        {
            return new OpenApiExample(this);
        }
    }
}
