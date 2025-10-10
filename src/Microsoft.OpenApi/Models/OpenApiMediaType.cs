// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;

namespace Microsoft.OpenApi
{
    /// <summary>
    /// Media Type Object.
    /// </summary>
    public class OpenApiMediaType : IOpenApiSerializable, IOpenApiExtensible, IOpenApiMediaType
    {
        /// <inheritdoc />
        public IOpenApiSchema? Schema { get; set; }

        /// <inheritdoc />
        public IOpenApiSchema? ItemSchema { get; set; }

        /// <inheritdoc />
        public JsonNode? Example { get; set; }

        /// <inheritdoc />
        public IDictionary<string, IOpenApiExample>? Examples { get; set; }

        /// <inheritdoc />
        public IDictionary<string, OpenApiEncoding>? Encoding { get; set; }

        /// <inheritdoc />
        public OpenApiEncoding? ItemEncoding { get; set; }

        /// <inheritdoc />
        public IList<OpenApiEncoding>? PrefixEncoding { get; set; }

        /// <summary>
        /// Serialize <see cref="OpenApiExternalDocs"/> to Open Api v3.0.
        /// </summary>
        public IDictionary<string, IOpenApiExtension>? Extensions { get; set; }

        /// <summary>
        /// Parameterless constructor
        /// </summary>
        public OpenApiMediaType() { }

        /// <summary>
        /// Initializes a copy of an <see cref="OpenApiMediaType"/> object
        /// </summary>
        public OpenApiMediaType(OpenApiMediaType? mediaType)
        {
            Schema = mediaType?.Schema?.CreateShallowCopy();
            ItemSchema = mediaType?.ItemSchema?.CreateShallowCopy();
            Example = mediaType?.Example != null ? JsonNodeCloneHelper.Clone(mediaType.Example) : null;
            Examples = mediaType?.Examples != null ? new Dictionary<string, IOpenApiExample>(mediaType.Examples, StringComparer.Ordinal) : null;
            Encoding = mediaType?.Encoding != null ? new Dictionary<string, OpenApiEncoding>(mediaType.Encoding, StringComparer.Ordinal) : null;
            ItemEncoding = mediaType?.ItemEncoding != null ? new OpenApiEncoding(mediaType.ItemEncoding) : null;
            PrefixEncoding = mediaType?.PrefixEncoding != null ? new List<OpenApiEncoding>(mediaType.PrefixEncoding.Select(e => new OpenApiEncoding(e))) : null;
            Extensions = mediaType?.Extensions != null ? new Dictionary<string, IOpenApiExtension>(mediaType.Extensions, StringComparer.Ordinal) : null;
        }

        /// <summary>
        /// Initializes a copy of an <see cref="OpenApiMediaType"/> object
        /// </summary>
        internal OpenApiMediaType(IOpenApiMediaType mediaType)
        {
            Schema = mediaType?.Schema?.CreateShallowCopy();
            ItemSchema = mediaType?.ItemSchema?.CreateShallowCopy();
            Example = mediaType?.Example != null ? JsonNodeCloneHelper.Clone(mediaType.Example) : null;
            Examples = mediaType?.Examples != null ? new Dictionary<string, IOpenApiExample>(mediaType.Examples, StringComparer.Ordinal) : null;
            Encoding = mediaType?.Encoding != null ? new Dictionary<string, OpenApiEncoding>(mediaType.Encoding, StringComparer.Ordinal) : null;
            ItemEncoding = mediaType?.ItemEncoding != null ? new OpenApiEncoding(mediaType.ItemEncoding) : null;
            PrefixEncoding = mediaType?.PrefixEncoding != null ? new List<OpenApiEncoding>(mediaType.PrefixEncoding.Select(e => new OpenApiEncoding(e))) : null;
            Extensions = mediaType?.Extensions != null ? new Dictionary<string, IOpenApiExtension>(mediaType.Extensions, StringComparer.Ordinal) : null;
        }

        /// <inheritdoc />
        public IOpenApiMediaType CreateShallowCopy()
        {
            return new OpenApiMediaType(this);
        }

        /// <summary>
        /// Serialize <see cref="OpenApiMediaType"/> to Open Api v3.2.
        /// </summary>
        public virtual void SerializeAsV32(IOpenApiWriter writer)
        {
            SerializeInternal(writer, OpenApiSpecVersion.OpenApi3_2, (w, element) => element.SerializeAsV32(w));
        }
        /// <summary>
        /// Serialize <see cref="OpenApiMediaType"/> to Open Api v3.1.
        /// </summary>
        public virtual void SerializeAsV31(IOpenApiWriter writer)
        {
            SerializeInternal(writer, OpenApiSpecVersion.OpenApi3_1, (w, element) => element.SerializeAsV31(w));
        }

        /// <summary>
        /// Serialize <see cref="OpenApiMediaType"/> to Open Api v3.0.
        /// </summary>
        public virtual void SerializeAsV3(IOpenApiWriter writer)
        {
            SerializeInternal(writer, OpenApiSpecVersion.OpenApi3_0, (w, element) => element.SerializeAsV3(w));
        }

        /// <summary>
        /// Serialize <see cref="OpenApiMediaType"/> to Open Api v3.0.
        /// </summary>
        private void SerializeInternal(IOpenApiWriter writer, OpenApiSpecVersion version,
            Action<IOpenApiWriter, IOpenApiSerializable> callback)
        {
            Utils.CheckArgumentNull(writer);

            writer.WriteStartObject();

            // schema
            writer.WriteOptionalObject(OpenApiConstants.Schema, Schema, callback);

            // itemSchema - only for v3.2+
            if (version >= OpenApiSpecVersion.OpenApi3_2)
            {
                writer.WriteOptionalObject(OpenApiConstants.ItemSchema, ItemSchema, callback);
            }
            else if (version < OpenApiSpecVersion.OpenApi3_2 && ItemSchema != null)
            {
                // For v3.1 and earlier, serialize as x-oai-itemSchema extension
                writer.WriteOptionalObject(OpenApiConstants.ExtensionFieldNamePrefix + "oai-" + OpenApiConstants.ItemSchema, ItemSchema, callback);
            }

            // example
            writer.WriteOptionalObject(OpenApiConstants.Example, Example, (w, e) => w.WriteAny(e));

            // examples
            if (Examples != null && Examples.Any())
            {
                writer.WriteOptionalMap(OpenApiConstants.Examples, Examples, callback);
            }

            // encoding
            writer.WriteOptionalMap(OpenApiConstants.Encoding, Encoding, callback);

            // itemEncoding - serialize as native field in v3.2+, as extension in earlier versions
            if (ItemEncoding != null)
            {
                if (version >= OpenApiSpecVersion.OpenApi3_2)
                {
                    writer.WriteOptionalObject(OpenApiConstants.ItemEncoding, ItemEncoding, callback);
                }
                else
                {
                    writer.WriteOptionalObject(OpenApiConstants.ExtensionFieldNamePrefix + "oai-" + OpenApiConstants.ItemEncoding, ItemEncoding, callback);
                }
            }

            // prefixEncoding - serialize as native field in v3.2+, as extension in earlier versions
            if (PrefixEncoding != null)
            {
                if (version >= OpenApiSpecVersion.OpenApi3_2)
                {
                    writer.WriteOptionalCollection(OpenApiConstants.PrefixEncoding, PrefixEncoding, callback);
                }
                else
                {
                    writer.WriteOptionalCollection(OpenApiConstants.ExtensionFieldNamePrefix + "oai-" + OpenApiConstants.PrefixEncoding, PrefixEncoding, callback);
                }
            }

            // extensions
            writer.WriteExtensions(Extensions, version);

            writer.WriteEndObject();
        }

        /// <summary>
        /// Serialize <see cref="OpenApiMediaType"/> to Open Api v2.0.
        /// </summary>
        public virtual void SerializeAsV2(IOpenApiWriter writer)
        {
            // Media type does not exist in V2.
        }
    }
}
