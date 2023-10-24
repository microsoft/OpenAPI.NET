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
    public static OpenApiEnumValuesDescriptionExtension Parse(IOpenApiAny source)
    {
        if (source is not OpenApiObject rawObject) throw new ArgumentOutOfRangeException(nameof(source));
        var extension = new OpenApiEnumValuesDescriptionExtension();
        if (rawObject.TryGetValue("values", out var values) && values is OpenApiArray valuesArray)
        {
            extension.ValuesDescriptions.AddRange(valuesArray
                                            .OfType<OpenApiObject>()
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
    public EnumDescription(OpenApiObject source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));
        if (source.TryGetValue(nameof(Value).ToFirstCharacterLowerCase(), out var rawValue) && rawValue is OpenApiString value)
            Value = value.Value;
        if (source.TryGetValue(nameof(Description).ToFirstCharacterLowerCase(), out var rawDescription) && rawDescription is OpenApiString description)
            Description = description.Value;
        if (source.TryGetValue(nameof(Name).ToFirstCharacterLowerCase(), out var rawName) && rawName is OpenApiString name)
            Name = name.Value;
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
