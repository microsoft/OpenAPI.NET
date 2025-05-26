// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Writers;
using System.Text.Json.Nodes;
using System.Text.Json;
using System.Globalization;

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
    private static readonly DateTimeStyles datesStyle = DateTimeStyles.AssumeUniversal | DateTimeStyles.RoundtripKind;
    private static DateTimeOffset? GetDateTimeOffsetValue(string propertyName, JsonObject rawObject)
    {
        if (!rawObject.TryGetPropertyValue(propertyName.ToFirstCharacterLowerCase(), out var jsonNode) ||
            jsonNode is not JsonValue jsonValue ||
            jsonNode.GetValueKind() is not JsonValueKind.String)
            return null;
            
        if (jsonValue.TryGetValue<string>(out var strValue) &&
            DateTimeOffset.TryParse(strValue, CultureInfo.InvariantCulture, datesStyle, out var parsedValue))
        {
            return parsedValue;
        }
        if (jsonValue.TryGetValue<DateTimeOffset>(out var returnedDto))
        {
            return returnedDto;
        }
        if (jsonValue.TryGetValue<DateTime>(out var returnedDt))
        {
            return new DateTimeOffset(returnedDt, TimeSpan.FromHours(0));
        }
        #if NET6_0_OR_GREATER
        if (jsonValue.TryGetValue<DateOnly>(out var returnedDo))
        {
            return new(returnedDo.Year, returnedDo.Month, returnedDo.Day, 0, 0, 0, TimeSpan.FromHours(0));
        }
        #endif
        return null;
    }
    /// <summary>
    /// Parses the <see cref="JsonNodeExtension"/> to <see cref="OpenApiDeprecationExtension"/>.
    /// </summary>
    /// <param name="source">The source object.</param>
    /// <returns>The <see cref="OpenApiDeprecationExtension"/>.</returns>
    /// <exception cref="ArgumentOutOfRangeException">When the source element is not an object</exception>
    public static OpenApiDeprecationExtension Parse(JsonNode source)
    {
        if (source is not JsonObject rawObject) throw new ArgumentOutOfRangeException(nameof(source));
        var extension = new OpenApiDeprecationExtension
        {
            RemovalDate = GetDateTimeOffsetValue(nameof(RemovalDate), rawObject),
            Date = GetDateTimeOffsetValue(nameof(Date), rawObject)
        };
        if (rawObject.TryGetPropertyValue(nameof(Version).ToFirstCharacterLowerCase(), out var version) && version is JsonValue versionValue && versionValue.TryGetValue<string>(out var versionStr))
            extension.Version = versionStr;
        if (rawObject.TryGetPropertyValue(nameof(Description).ToFirstCharacterLowerCase(), out var description) && description is JsonValue descriptionValue && descriptionValue.TryGetValue<string>(out var descriptionStr))
            extension.Description = descriptionStr;
        return extension;
    }
}
