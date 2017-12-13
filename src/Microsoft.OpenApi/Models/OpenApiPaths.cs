// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using Microsoft.OpenApi.Validations;
using Microsoft.OpenApi.Validations.Validators;

namespace Microsoft.OpenApi.Models
{
    /// <summary>
    /// Paths object.
    /// </summary>
    [OpenApiValidator(typeof(PathsValidator))]
    public class OpenApiPaths : OpenApiExtensibleDictionary<OpenApiPathItem>
    {
    }
}