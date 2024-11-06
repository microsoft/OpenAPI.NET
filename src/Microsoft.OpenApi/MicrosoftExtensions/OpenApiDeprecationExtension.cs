// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Writers;
using System.Text.Json.Nodes;

namespace Microsoft.OpenApi.MicrosoftExtensions;

/// <summary>
/// Extension element for OpenAPI to add deprecation information. x-ms-deprecation
/// </summary>
public class OpenApiDeprecationExtension : IOpenApiExtension
{
    /// <summary>
    /// Name of the extension as used in the description.
    /// </summary>
    public static string Name => "x-ms-deprecation";
    /// <summary>
    /// The date at which the element has been/will be removed entirely from the service.
    /// </summary>
    public DateTimeOffset? RemovalDate
    {
        get; set;
    }
    /// <summary>
    /// The date at which the element has been/will be deprecated.
    /// </summary>
    public DateTimeOffset? Date
    {
        get; set;
    }
    /// <summary>
    /// The version this revision was introduced.
    /// </summary>
    public string Version
    {
        get; set;
    } = string.Empty;
    /// <summary>
    /// The description of the revision.
    /// </summary>
    public string Description
    {
        get; set;
    } = string.Empty;
    /// <inheritdoc />
    public void Write(IOpenApiWriter writer, OpenApiSpecVersion specVersion)
    {
        if (writer == null)
            throw new ArgumentNullException(nameof(writer));

        if (RemovalDate.HasValue || Date.HasValue || !string.IsNullOrEmpty(Version) || !string.IsNullOrEmpty(Description))
        {
            writer.WriteStartObject();

            if (RemovalDate.HasValue)
                writer.WriteProperty(nameof(RemovalDate).ToFirstCharacterLowerCase(), RemovalDate.Value);
            if (Date.HasValue)
                writer.WriteProperty(nameof(Date).ToFirstCharacterLowerCase(), Date.Value);
            if (!string.IsNullOrEmpty(Version))
                writer.WriteProperty(nameof(Version).ToFirstCharacterLowerCase(), Version);
            if (!string.IsNullOrEmpty(Description))
                writer.WriteProperty(nameof(Description).ToFirstCharacterLowerCase(), Description);

            writer.WriteEndObject();
        }
    }
    /// <summary>
    /// Parses the <see cref="OpenApiAny"/> to <see cref="OpenApiDeprecationExtension"/>.
    /// </summary>
    /// <param name="source">The source object.</param>
    /// <returns>The <see cref="OpenApiDeprecationExtension"/>.</returns>
    /// <exception cref="ArgumentOutOfRangeException">When the source element is not an object</exception>
    public static OpenApiDeprecationExtension Parse(JsonNode source)
    {
        if (source is not JsonObject rawObject) return null;
        var extension = new OpenApiDeprecationExtension();
        if (rawObject.TryGetPropertyValue(nameof(RemovalDate).ToFirstCharacterLowerCase(), out var removalDate) && removalDate is JsonNode removalDateValue)
            extension.RemovalDate = removalDateValue.GetValue<DateTimeOffset>();
        if (rawObject.TryGetPropertyValue(nameof(Date).ToFirstCharacterLowerCase(), out var date) && date is JsonNode dateValue)
            extension.Date = dateValue.GetValue<DateTimeOffset>();
        if (rawObject.TryGetPropertyValue(nameof(Version).ToFirstCharacterLowerCase(), out var version) && version is JsonNode versionValue)
            extension.Version = versionValue.GetValue<string>();
        if (rawObject.TryGetPropertyValue(nameof(Description).ToFirstCharacterLowerCase(), out var description) && description is JsonNode descriptionValue)
            extension.Description = descriptionValue.GetValue<string>();
        return extension;
    }
}
