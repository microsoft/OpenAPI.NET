// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Writers;
using System.Text.Json.Nodes;

namespace Microsoft.OpenApi.MicrosoftExtensions;

/// <summary>
/// Extension element for OpenAPI to add enum values descriptions.
/// Based of the AutoRest specification https://github.com/Azure/autorest/blob/main/docs/extensions/readme.md#x-ms-enum
/// </summary>
public class OpenApiEnumValuesDescriptionExtension : IOpenApiExtension
{
    /// <summary>
    /// Name of the extension as used in the description.
    /// </summary>
    public static string Name => "x-ms-enum";

    /// <summary>
    /// The of the enum.
    /// </summary>
    public string EnumName { get; set; } = string.Empty;

    /// <summary>
    /// Descriptions for the enum symbols, where the value MUST match the enum symbols in the main description
    /// </summary>
    public List<EnumDescription> ValuesDescriptions { get; set; } = new();

    /// <inheritdoc />
    public void Write(IOpenApiWriter writer, OpenApiSpecVersion specVersion)
    {
        if (writer is null) throw new ArgumentNullException(nameof(writer));
        if (specVersion is OpenApiSpecVersion.OpenApi2_0 or OpenApiSpecVersion.OpenApi3_0 &&
            !string.IsNullOrEmpty(EnumName) &&
            ValuesDescriptions.Any())
        { // when we upgrade to 3.1, we don't need to write this extension as JSON schema will support writing enum values
            writer.WriteStartObject();
            writer.WriteProperty(nameof(Name).ToFirstCharacterLowerCase(), EnumName);
            writer.WriteProperty("modelAsString", false);
            writer.WriteRequiredCollection("values", ValuesDescriptions, (w, x) =>
            {
                w.WriteStartObject();
                w.WriteProperty(nameof(x.Value).ToFirstCharacterLowerCase(), x.Value);
                w.WriteProperty(nameof(x.Description).ToFirstCharacterLowerCase(), x.Description);
                w.WriteProperty(nameof(x.Name).ToFirstCharacterLowerCase(), x.Name);
                w.WriteEndObject();
            });
            writer.WriteEndObject();
        }
    }
    /// <summary>
    /// Parse the extension from the raw IOpenApiAny object.
    /// </summary>
    /// <param name="source">The source element to parse.</param>
    /// <returns>The <see cref="OpenApiEnumValuesDescriptionExtension"/>.</returns>
    /// <exception cref="ArgumentOutOfRangeException">When the source element is not an object</exception>
    public static OpenApiEnumValuesDescriptionExtension Parse(JsonNode source)
    {
        if (source is not JsonObject rawObject) return null;
        var extension = new OpenApiEnumValuesDescriptionExtension();
        if (rawObject.TryGetPropertyValue("values", out var values) && values is JsonArray valuesArray)
        {
            extension.ValuesDescriptions.AddRange(valuesArray
                                            .OfType<JsonObject>()
                                            .Select(x => new EnumDescription(x)));
        }
        return extension;
    }
}

/// <summary>
/// Description of an enum symbol
/// </summary>
public class EnumDescription : IOpenApiElement
{
    /// <summary>
    /// Default constructor
    /// </summary>
    public EnumDescription()
    {
    }

    /// <summary>
    /// Constructor from a raw OpenApiObject
    /// </summary>
    /// <param name="source">The source object</param>
    public EnumDescription(JsonObject source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));
        if (source.TryGetPropertyValue(nameof(Value).ToFirstCharacterLowerCase(), out var rawValue) && rawValue is JsonNode value)
            Value = value.GetValue<string>();
        if (source.TryGetPropertyValue(nameof(Description).ToFirstCharacterLowerCase(), out var rawDescription) && rawDescription is JsonNode description)
            Description = description.GetValue<string>();
        if (source.TryGetPropertyValue(nameof(Name).ToFirstCharacterLowerCase(), out var rawName) && rawName is JsonNode name)
            Name = name.GetValue<string>();
    }
    /// <summary>
    /// The description for the enum symbol
    /// </summary>
    public string Description { get; set; } = string.Empty;
    /// <summary>
    /// The symbol for the enum symbol to use for code-generation
    /// </summary>
    public string Name { get; set; } = string.Empty;
    /// <summary>
    /// The symbol as described in the main enum schema.
    /// </summary>
    public string Value { get; set; } = string.Empty;
}
