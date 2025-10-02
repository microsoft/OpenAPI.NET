// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.Text.Json.Nodes;

namespace Microsoft.OpenApi;

/// <summary>
/// Defines the base properties for the media type object.
/// This interface is provided for type assertions but should not be implemented by package consumers beyond automatic mocking.
/// </summary>
public interface IOpenApiMediaType : IOpenApiReadOnlyExtensible, IShallowCopyable<IOpenApiMediaType>, IOpenApiReferenceable
{
    /// <summary>
    /// The schema defining the type used for the request body.
    /// </summary>
    public IOpenApiSchema? Schema { get; }

    /// <summary>
    /// The schema defining the type used for the items in an array media type.
    /// This property is only applicable for OAS 3.2.0 and later.
    /// </summary>
    public IOpenApiSchema? ItemSchema { get; }

    /// <summary>
    /// Example of the media type.
    /// The example object SHOULD be in the correct format as specified by the media type.
    /// </summary>
    public JsonNode? Example { get; }

    /// <summary>
    /// Examples of the media type.
    /// Each example object SHOULD match the media type and specified schema if present.
    /// </summary>
    public IDictionary<string, IOpenApiExample>? Examples { get; }

    /// <summary>
    /// A map between a property name and its encoding information.
    /// The key, being the property name, MUST exist in the schema as a property.
    /// The encoding object SHALL only apply to requestBody objects
    /// when the media type is multipart or application/x-www-form-urlencoded.
    /// </summary>
    public IDictionary<string, OpenApiEncoding>? Encoding { get; }

    /// <summary>
    /// An encoding object for items in an array schema.
    /// Only applies when the schema is of type array.
    /// </summary>
    public OpenApiEncoding? ItemEncoding { get; }

    /// <summary>
    /// An array of encoding objects for prefixItems in an array schema.
    /// Each element corresponds to a prefixItem in the schema.
    /// </summary>
    public IList<OpenApiEncoding>? PrefixEncoding { get; }
}
