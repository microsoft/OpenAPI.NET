// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

namespace Microsoft.OpenApi;
/// <summary>
/// Defines a policy for matching and transforming OpenAPI JSON Pointer path segments
/// between specification versions.
/// </summary>
internal interface IOpenApiPathRepresentationPolicy
{
    /// <summary>
    /// Attempts to transform the given path segments to the equivalent in the target version.
    /// </summary>
    /// <param name="segments">The pre-parsed path segments (without the <c>#/</c> prefix).</param>
    /// <param name="result">
    /// When this method returns <c>true</c>, contains the transformed path or <c>null</c>
    /// if the path has no equivalent in the target version.
    /// </param>
    /// <returns><c>true</c> if this policy handled the path; <c>false</c> to try the next policy.</returns>
    bool TryGetVersionedPath(string[] segments, out string? result);
}
