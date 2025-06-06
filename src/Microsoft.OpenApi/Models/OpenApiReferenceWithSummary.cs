// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Linq;
using System.Text.Json.Nodes;
using Microsoft.OpenApi.Reader;

namespace Microsoft.OpenApi;

/// <summary>
/// OpenApiReferenceWithSummary is a reference to an OpenAPI component that includes a summary.
/// </summary>
public class OpenApiReferenceWithSummary : BaseOpenApiReference, IOpenApiSummarizedElement
{
    /// <summary>
    /// A short summary which by default SHOULD override that of the referenced component.
    /// If the referenced object-type does not allow a summary field, then this field has no effect.
    /// </summary>
    public string? Summary { get; set; }

    /// <summary>
    /// Parameterless constructor
    /// </summary>
    public OpenApiReferenceWithSummary() : base() { }

    /// <summary>
    /// Initializes a copy instance of the <see cref="OpenApiReferenceWithSummary"/> object
    /// </summary>
    public OpenApiReferenceWithSummary(OpenApiReferenceWithSummary reference) : base(reference)
    {
        Utils.CheckArgumentNull(reference);
        Summary = reference.Summary;
    }
    /// <inheritdoc/>
    protected override void SerializeAdditionalV31Properties(IOpenApiWriter writer)
    {
        // summary and description are in 3.1 but not in 3.0
        writer.WriteProperty(OpenApiConstants.Summary, Summary);
        base.SerializeAdditionalV31Properties(writer);
    }
    /// <inheritdoc/>
    protected override void SetAdditional31MetadataFromMapNode(JsonObject jsonObject)
    {
        base.SetAdditional31MetadataFromMapNode(jsonObject);
        // Summary and Description
        var summary = GetPropertyValueFromNode(jsonObject, OpenApiConstants.Summary);

        if (!string.IsNullOrEmpty(summary))
        {
            Summary = summary;
        }
    }
}
