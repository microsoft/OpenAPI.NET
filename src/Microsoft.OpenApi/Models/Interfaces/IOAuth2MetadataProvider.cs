using System;

namespace Microsoft.OpenApi;

/// <summary>
/// TEMPORARY compatibility interface for accessing OAuth2 metadata URL support introduced for OpenAPI 3.2.
/// This exists to avoid adding new members to <see cref="IOpenApiSecurityScheme"/> in a minor release, which is binary breaking for existing compiled consumers.
/// </summary>
// TODO: Remove this temporary interface and collapse this member into IOpenApiSecurityScheme in the next major version.
public interface IOAuth2MetadataProvider
{
    /// <summary>
    /// URL to the OAuth2 Authorization Server Metadata document (RFC 8414).
    /// Note: This field is supported in OpenAPI 3.2.0+ only.
    /// </summary>
    public Uri? OAuth2MetadataUrl { get; }
}
