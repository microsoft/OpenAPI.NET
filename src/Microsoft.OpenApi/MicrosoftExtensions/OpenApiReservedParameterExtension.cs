// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Text.Json.Nodes;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.MicrosoftExtensions;

/// <summary>
/// Extension element for OpenAPI to add reserved parameters. x-ms-reserved-parameters
/// </summary>
public class OpenApiReservedParameterExtension : IOpenApiExtension
{
    /// <summary>
    /// Name of the extension as used in the description.
    /// </summary>
    public static string Name => "x-ms-reserved-parameter";
    /// <inheritdoc />
    public void Write(IOpenApiWriter writer, OpenApiSpecVersion specVersion)
    {
        if (writer is null) throw new ArgumentNullException(nameof(writer));
        if (IsReserved.HasValue)
            writer.WriteValue(IsReserved.Value);
    }
    /// <summary>
    /// Whether the associated parameter is reserved or not.
    /// </summary>
    public bool? IsReserved
    {
        get; set;
    }
    /// <summary>
    /// Parses the <see cref="OpenApiAny"/> to <see cref="OpenApiReservedParameterExtension"/>.
    /// </summary>
    /// <param name="source">The source object.</param>
    /// <returns>The <see cref="OpenApiReservedParameterExtension"/>.</returns>
    /// <returns></returns>
    public static OpenApiReservedParameterExtension Parse(JsonNode source)
    {
        if (source is not JsonNode rawBoolean) return null;
        return new()
        {
            IsReserved = rawBoolean.GetValue<bool>()
        };
    }
}
