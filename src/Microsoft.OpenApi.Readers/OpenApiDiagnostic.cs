// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Collections.Generic;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers.Interface;

namespace Microsoft.OpenApi.Readers
{
    /// <summary>
    /// Object containing all diagnostic information related to Open API parsing.
    /// </summary>
    public class OpenApiDiagnostic : IDiagnostic
    {
        /// <summary>
        /// List of all errors.
        /// </summary>
        public IList<OpenApiError> Errors { get; set; } = new List<OpenApiError>();

        /// <summary>
        /// Open API specification version of the document parsed.
        /// </summary>
        public OpenApiSpecVersion SpecificationVersion { get; set; }
    }
}