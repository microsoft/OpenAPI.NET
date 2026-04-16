// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

namespace Microsoft.OpenApi;
/// <summary>
/// Defines a policy for matching and transforming OpenAPI JSON Pointer paths
/// between specification versions.
/// </summary>
internal interface IOpenApiPathRepresentationPolicy
{
    /// <summary>
    /// Determines whether this policy can handle the given path.
    /// </summary>
    /// <param name="path">The JSON Pointer path to evaluate.</param>
    /// <returns><c>true</c> if this policy applies to the given path; otherwise, <c>false</c>.</returns>
    bool IsMatch(string path);

    /// <summary>
    /// Transforms the given path to its equivalent in the target specification version.
    /// </summary>
    /// <param name="path">The JSON Pointer path to transform.</param>
    /// <returns>The transformed path, or <c>null</c> if the path has no equivalent in the target version.</returns>
    string? GetVersionedPath(string path);
}
