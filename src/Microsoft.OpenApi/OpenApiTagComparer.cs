using System;
using System.Collections.Generic;
using Microsoft.OpenApi.Models.Interfaces;

namespace Microsoft.OpenApi;

/// <summary>
/// This comparer is used to maintain a globally unique list of tags encountered
/// in a particular OpenAPI document.
/// </summary>
internal sealed class OpenApiTagComparer : IEqualityComparer<IOpenApiTag>
{
    private static readonly Lazy<OpenApiTagComparer> _lazyInstance = new(() => new OpenApiTagComparer());
    /// <summary>
    /// Default instance for the comparer.
    /// </summary>
    internal static OpenApiTagComparer Instance { get => _lazyInstance.Value; }

    /// <inheritdoc/>
    public bool Equals(IOpenApiTag? x, IOpenApiTag? y)
    {
        if (x is null && y is null)
        {
            return true;
        }
        if (x is null || y is null)
        {
            return false;
        }
        if (ReferenceEquals(x, y))
        {
            return true;
        }
        return StringComparer.Equals(x.Name, y.Name);
    }

    // Tag comparisons are case-sensitive by default. Although the OpenAPI specification
    // only outlines case sensitivity for property names, we extend this principle to
    // property values for tag names as well.
    // See https://spec.openapis.org/oas/v3.1.0#format.
    internal static readonly StringComparer StringComparer = StringComparer.Ordinal;

    /// <inheritdoc/>
    public int GetHashCode(IOpenApiTag obj) => string.IsNullOrEmpty(obj?.Name) ? 0 : StringComparer.GetHashCode(obj!.Name);
}
