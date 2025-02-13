// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

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
        public OpenApiDocument? Document { get; set; }
        /// <summary>
        /// OpenApiDiagnostic contains the Errors reported while parsing
        /// </summary>
        public OpenApiDiagnostic? Diagnostic { get; set; }
        /// <summary>
        /// Deconstructs the result for easier assignment on the client application.
        /// </summary>
        public void Deconstruct(out OpenApiDocument? document, out OpenApiDiagnostic? diagnostic)
        {
            document = Document;
            diagnostic = Diagnostic;
        }
    }
}
