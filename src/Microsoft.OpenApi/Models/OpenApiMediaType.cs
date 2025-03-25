﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using Microsoft.OpenApi.Helpers;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models.Interfaces;
using Microsoft.OpenApi.Writers;

#nullable enable

namespace Microsoft.OpenApi.Models
{
    /// <summary>
    /// Media Type Object.
    /// </summary>
    public class OpenApiMediaType : IOpenApiSerializable, IOpenApiExtensible
    {
        /// <summary>
        /// The schema defining the type used for the request body.
        /// </summary>
        public IOpenApiSchema? Schema { get; set; }

        /// <summary>
        /// Example of the media type.
        /// The example object SHOULD be in the correct format as specified by the media type.
        /// </summary>
        public JsonNode? Example { get; set; }

        private Lazy<IDictionary<string, IOpenApiExample>>? _examples = new(() => new Dictionary<string, IOpenApiExample>(StringComparer.Ordinal));
        /// <summary>
        /// Examples of the media type.
        /// Each example object SHOULD match the media type and specified schema if present.
        /// </summary>
        public IDictionary<string, IOpenApiExample>? Examples
        {
            get => _examples?.Value;
            set => _examples = value is null ? null : new(() => value);
        }

        private Lazy<IDictionary<string, OpenApiEncoding>>? _encoding = new(() => new Dictionary<string, OpenApiEncoding>(StringComparer.Ordinal));
        /// <summary>
        /// A map between a property name and its encoding information.
        /// The key, being the property name, MUST exist in the schema as a property.
        /// The encoding object SHALL only apply to requestBody objects
        /// when the media type is multipart or application/x-www-form-urlencoded.
        /// </summary>
        public IDictionary<string, OpenApiEncoding>? Encoding
        {
            get => _encoding?.Value;
            set => _encoding = value is null ? null : new(() => value);
        }

        private Lazy<IDictionary<string, IOpenApiExtension>>? _extensions = new(() => new Dictionary<string, IOpenApiExtension>(StringComparer.Ordinal));
        /// <summary>
        /// This object MAY be extended with Specification Extensions.
        /// </summary>
        public IDictionary<string, IOpenApiExtension>? Extensions
        {
            get => _extensions?.Value;
            set => _extensions = value is null ? null : new(() => value);
        }

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
            Example = mediaType?.Example != null ? JsonNodeCloneHelper.Clone(mediaType.Example) : null;
            Examples = mediaType?.Examples != null ? new Dictionary<string, IOpenApiExample>(mediaType.Examples) : null;
            Encoding = mediaType?.Encoding != null ? new Dictionary<string, OpenApiEncoding>(mediaType.Encoding) : null;
            Extensions = mediaType?.Extensions != null ? new Dictionary<string, IOpenApiExtension>(mediaType.Extensions) : null;
        }

        /// <summary>
        /// Serialize <see cref="OpenApiMediaType"/> to Open Api v3.1.
        /// </summary>
        public void SerializeAsV31(IOpenApiWriter writer)
        {
            SerializeInternal(writer, OpenApiSpecVersion.OpenApi3_1, (w, element) => element.SerializeAsV31(w));
        }

        /// <summary>
        /// Serialize <see cref="OpenApiMediaType"/> to Open Api v3.0.
        /// </summary>
        public void SerializeAsV3(IOpenApiWriter writer)
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

            // example
            writer.WriteOptionalObject(OpenApiConstants.Example, Example, (w, e) => w.WriteAny(e));

            // examples
            if (Examples != null && Examples.Any())
            {
                SerializeExamples(writer, Examples);
            }

            // encoding
            writer.WriteOptionalMap(OpenApiConstants.Encoding, Encoding, callback);

            // extensions
            writer.WriteExtensions(Extensions, version);

            writer.WriteEndObject();
        }

        /// <summary>
        /// Serialize <see cref="OpenApiMediaType"/> to Open Api v2.0.
        /// </summary>
        public void SerializeAsV2(IOpenApiWriter writer)
        {
            // Media type does not exist in V2.
        }

        private static void SerializeExamples(IOpenApiWriter writer, IDictionary<string, IOpenApiExample> examples)
        {
            /* Special case for writing out empty arrays as valid response examples
            * Check if there is any example with an empty array as its value and set the flag `hasEmptyArray` to true
            * */
            var hasEmptyArray = examples.Values.Any( static example =>
                example.Value is JsonArray arr && arr.Count == 0
            );

            if (hasEmptyArray)
            {
                writer.WritePropertyName(OpenApiConstants.Examples);
                writer.WriteStartObject();
                foreach (var kvp in examples.Where(static kvp => kvp.Value.Value is JsonArray arr && arr.Count == 0))
                {
                    writer.WritePropertyName(kvp.Key);
                    writer.WriteStartObject();
                    writer.WriteRequiredObject(OpenApiConstants.Value, kvp.Value.Value, (w, v) => w.WriteAny(v));
                    writer.WriteEndObject();
                }
                writer.WriteEndObject();
            }
            else
            {
                writer.WriteOptionalMap(OpenApiConstants.Examples, examples, (w, e) => e.SerializeAsV3(w));
            }
        }
    }
}
