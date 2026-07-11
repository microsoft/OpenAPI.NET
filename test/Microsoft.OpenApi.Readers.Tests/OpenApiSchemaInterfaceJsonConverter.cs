// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Microsoft.OpenApi.Readers.Tests
{
    /// <summary>
    /// Serializes <see cref="IOpenApiSchema"/> values via the native OpenAPI writer,
    /// avoiding reflection on computed getters that create cycles for recursive $dynamicRef schemas.
    /// </summary>
    internal sealed class OpenApiSchemaInterfaceJsonConverter(Action<IOpenApiWriter, IOpenApiSchema> serialize) : JsonConverter<IOpenApiSchema>
    {
        public override IOpenApiSchema Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => throw new NotSupportedException();

        public override void Write(Utf8JsonWriter writer, IOpenApiSchema value, JsonSerializerOptions options)
        {
            using var tw = new StringWriter();
            var ow = new OpenApiJsonWriter(tw);
            serialize(ow, value);
            writer.WriteRawValue(tw.ToString(), skipInputValidation: true);
        }
    }
}
