// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

namespace Microsoft.OpenApi.Reader;
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
    /// The format of the OpenAPI document (e.g., "json", "yaml").
    /// </summary>
    public string? Format { get; set; }
    /// <summary>
    /// Deconstructs the result for easier assignment on the client application.
    /// </summary>
    public void Deconstruct(out OpenApiDocument? document, out OpenApiDiagnostic? diagnostic)
    {
        Deconstruct(out document, out diagnostic, out _);
    }
    /// <summary>
    /// Deconstructs the result for easier assignment on the client application.
    /// </summary>
    public void Deconstruct(out OpenApiDocument? document, out OpenApiDiagnostic? diagnostic, out string? format)
    {
        document = Document;
        diagnostic = Diagnostic;
        format = Format;
    }
}

