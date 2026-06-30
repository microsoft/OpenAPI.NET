// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Microsoft.OpenApi.Reader;

namespace Microsoft.OpenApi
{
    /// <summary>
    /// Enables System.Text.Json serialization and deserialization of <see cref="OpenApiSchema"/>
    /// using the OpenAPI wire format rather than the default reflection-based output.
    /// </summary>
    /// <remarks>
    /// <para>Register this converter via <see cref="JsonSerializerOptions.Converters"/>:</para>
    /// <code>
    /// var options = new JsonSerializerOptions();
    /// options.Converters.Add(new OpenApiSchemaJsonConverter());
    /// var json = JsonSerializer.Serialize(schema, options);
    /// </code>
    /// </remarks>
    public sealed class OpenApiSchemaJsonConverter : JsonConverter<OpenApiSchema>
    {
        private readonly OpenApiSpecVersion _version;

        /// <summary>
        /// Initializes a new instance of <see cref="OpenApiSchemaJsonConverter"/> targeting OpenAPI 3.2.
        /// </summary>
        public OpenApiSchemaJsonConverter() : this(OpenApiSpecVersion.OpenApi3_2) { }

        /// <summary>
        /// Initializes a new instance of <see cref="OpenApiSchemaJsonConverter"/> targeting the specified OpenAPI version.
        /// </summary>
        /// <param name="version">The OpenAPI specification version to use when serializing the schema.</param>
        public OpenApiSchemaJsonConverter(OpenApiSpecVersion version)
        {
            _version = version;
        }

        /// <inheritdoc/>
        /// <remarks>
        /// Deserializes a bare JSON Schema object into an <see cref="OpenApiSchema"/> using
        /// <see cref="OpenApiJsonReader"/> to parse it as a schema fragment.
        /// Only OpenAPI 3.x versions support JSON Schema; deserializing with <see cref="OpenApiSpecVersion.OpenApi2_0"/>
        /// is not supported and will throw <see cref="NotSupportedException"/>.
        /// </remarks>
        public override OpenApiSchema? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (_version == OpenApiSpecVersion.OpenApi2_0)
                throw new NotSupportedException("Deserializing OpenApiSchema is not supported for OpenAPI 2.0.");

            var jsonNode = JsonNode.Parse(ref reader)
                ?? throw new JsonException("Failed to parse the JSON input into a valid JsonNode.");
            var jsonReader = new OpenApiJsonReader();
            return jsonReader.ReadFragment<OpenApiSchema>(jsonNode, _version, new OpenApiDocument(), out _);
        }

        /// <inheritdoc/>
        public override void Write(Utf8JsonWriter writer, OpenApiSchema value, JsonSerializerOptions options)
        {
            Utils.CheckArgumentNull(writer);
            Utils.CheckArgumentNull(value);

            using var stream = new MemoryStream();
            using (var textWriter = new StreamWriter(stream, Encoding.UTF8, bufferSize: 1024, leaveOpen: true))
            {
                var openApiWriter = new OpenApiJsonWriter(textWriter);
                SerializeSchema(value, openApiWriter);
                textWriter.Flush();
            }

            stream.Position = 0;
            using var document = JsonDocument.Parse(stream);
            document.RootElement.WriteTo(writer);
        }

        private void SerializeSchema(OpenApiSchema schema, OpenApiJsonWriter writer)
        {
            switch (_version)
            {
                case OpenApiSpecVersion.OpenApi3_2:
                    schema.SerializeAsV32(writer);
                    break;
                case OpenApiSpecVersion.OpenApi3_1:
                    schema.SerializeAsV31(writer);
                    break;
                case OpenApiSpecVersion.OpenApi3_0:
                    schema.SerializeAsV3(writer);
                    break;
                case OpenApiSpecVersion.OpenApi2_0:
                    schema.SerializeAsV2(writer);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(_version), _version,
                        string.Format(SRResource.OpenApiSpecVersionNotSupported, _version));
            }
        }
    }
}
