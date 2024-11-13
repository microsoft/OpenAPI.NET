// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Reader
{
    /// <summary>
    /// Container object used for returning the result of reading an OpenAPI description.
    /// </summary>
    public class ReadResult
    {
        /// <summary>
        /// The parsed OpenApiDocument.  Null will be returned if the document could not be parsed.
        /// </summary>
        public OpenApiDocument OpenApiDocument { get; set; }
        /// <summary>
        /// OpenApiDiagnostic contains the Errors reported while parsing
        /// </summary>
        public OpenApiDiagnostic OpenApiDiagnostic { get; set; }
    }

    /// <summary>
    /// Container object used for returning the result of reading an OpenAPI fragment
    /// </summary>
    public class ReadFragmentResult
    {
        /// <summary>
        /// The parsed fragment.
        /// </summary>
        public IOpenApiElement Element { get; set; }

        /// <summary>
        /// OpenApiDiagnostic contains the Errors reported while parsing
        /// </summary>
        public OpenApiDiagnostic OpenApiDiagnostic { set; get; }
    }
}
