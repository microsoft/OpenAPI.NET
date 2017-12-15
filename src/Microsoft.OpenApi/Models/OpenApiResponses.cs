// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using Microsoft.OpenApi.Validations;
using Microsoft.OpenApi.Validations.Validators;

namespace Microsoft.OpenApi.Models
{
    /// <summary>
    /// Responses object.
    /// </summary>
    [OpenApiValidator(typeof(ResponsesValidator))]
    public class OpenApiResponses : OpenApiExtensibleDictionary<OpenApiResponse>
    {
    }
}