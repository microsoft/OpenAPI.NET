// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;

namespace Microsoft.OpenApi;

/// <summary>
/// Some OpenAPI properties allow 'null' as a valid value, which is different from omitting the
/// property completely. This class holds values that represent an unset value, so that serialization can
/// differentiate between a null value (serialized as null) and an unset value (not serialized at all).
/// </summary>
public static class OpenApiUnsetValues
{
    /// <summary>
    /// Represents an unset string value.
    /// Some OpenAPI string properties does allow 'null' as a valid value, which is different
    /// from when the property is completely omitted.
    /// This UnsetString differentiates that, and is used "by reference" in comparison.
    /// </summary>
    public static readonly string UnsetString = new string(Array.Empty<char>());
}
