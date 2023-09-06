// ------------------------------------------------------------
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
/// Extension element for OpenAPI to add pageable information.
/// Based of the AutoRest specification https://github.com/Azure/autorest/blob/main/docs/extensions/readme.md#x-ms-pageable
/// </summary>
public class OpenApiPagingExtension : IOpenApiExtension
{
    /// <summary>
    /// Name of the extension as used in the description.
    /// </summary>
    public static string Name => "x-ms-pageable";

    /// <summary>
    /// The name of the property that provides the collection of pageable items.
    /// </summary>
    public string ItemName
    {
        get; set;
    } = "value";

    /// <summary>
    /// The name of the property that provides the next link (common: nextLink)
    /// </summary>
    public string NextLinkName
    {
        get; set;
    } = "nextLink";

    /// <summary>
    /// The name (operationId) of the operation for retrieving the next page.
    /// </summary>
    public string OperationName
    {
        get; set;
    } = string.Empty;

    /// <inheritdoc />
    public void Write(IOpenApiWriter writer, OpenApiSpecVersion specVersion)
    {
        if (writer is null) throw new ArgumentNullException(nameof(writer));
        writer.WriteStartObject();
        if (!string.IsNullOrEmpty(NextLinkName))
        {
            writer.WriteProperty(nameof(NextLinkName).ToFirstCharacterLowerCase(), NextLinkName);
        }

        if (!string.IsNullOrEmpty(OperationName))
        {
            writer.WriteProperty(nameof(OperationName).ToFirstCharacterLowerCase(), OperationName);
        }

        writer.WriteProperty(nameof(ItemName).ToFirstCharacterLowerCase(), ItemName);

        writer.WriteEndObject();
    }
    /// <summary>
    /// Parse the extension from the raw IOpenApiAny object.
    /// </summary>
    /// <param name="source">The source element to parse.</param>
    /// <returns>The <see cref="OpenApiPagingExtension"/>.</returns>
    /// <exception cref="ArgumentOutOfRangeException">When the source element is not an object</exception>
    public static OpenApiPagingExtension Parse(IOpenApiAny source)
    {
        if (source is not OpenApiObject rawObject) throw new ArgumentOutOfRangeException(nameof(source));
        var extension = new OpenApiPagingExtension();
        if (rawObject.TryGetValue(nameof(NextLinkName).ToFirstCharacterLowerCase(), out var nextLinkName) && nextLinkName is OpenApiString nextLinkNameStr)
        {
            extension.NextLinkName = nextLinkNameStr.Value;
        }

        if (rawObject.TryGetValue(nameof(OperationName).ToFirstCharacterLowerCase(), out var opName) && opName is OpenApiString opNameStr)
        {
            extension.OperationName = opNameStr.Value;
        }

        if (rawObject.TryGetValue(nameof(ItemName).ToFirstCharacterLowerCase(), out var itemName) && itemName is OpenApiString itemNameStr)
        {
            extension.ItemName = itemNameStr.Value;
        }

        return extension;
    }
}
