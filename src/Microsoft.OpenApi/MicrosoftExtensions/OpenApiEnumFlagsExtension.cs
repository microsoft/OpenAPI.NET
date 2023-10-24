﻿// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.MicrosoftExtensions;

/// <summary>
/// Extension element for OpenAPI to add deprecation information. x-ms-enum-flags
/// </summary>
public class OpenApiEnumFlagsExtension : IOpenApiExtension
{
    /// <summary>
    /// Name of the extension as used in the description.
    /// </summary>
    public static string Name => "x-ms-enum-flags";
    /// <summary>
    /// Whether the enum is a flagged enum.
    /// </summary>
    public bool IsFlags
    {
        get; set;
    }
    /// <inheritdoc />
    public void Write(IOpenApiWriter writer, OpenApiSpecVersion specVersion)
    {
        if (writer == null)
            throw new ArgumentNullException(nameof(writer));

        writer.WriteStartObject();
        writer.WriteProperty(nameof(IsFlags).ToFirstCharacterLowerCase(), IsFlags);
        writer.WriteEndObject();
    }
    /// <summary>
    /// Parse the extension from the raw IOpenApiAny object.
    /// </summary>
    /// <param name="source">The source element to parse.</param>
    /// <returns>The <see cref="OpenApiEnumFlagsExtension"/>.</returns>
    /// <exception cref="ArgumentOutOfRangeException">When the source element is not an object</exception>
    public static OpenApiEnumFlagsExtension Parse(IOpenApiAny source)
    {
        if (source is not OpenApiObject rawObject) throw new ArgumentOutOfRangeException(nameof(source));
        var extension = new OpenApiEnumFlagsExtension();
        if (rawObject.TryGetValue(nameof(IsFlags).ToFirstCharacterLowerCase(), out var flagsValue) && flagsValue is OpenApiBoolean isFlags)
        {
            extension.IsFlags = isFlags.Value;
        }
        return extension;
    }
}
