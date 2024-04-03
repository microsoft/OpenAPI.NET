// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Reader
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
        /// List of all warnings
        /// </summary>
        public IList<OpenApiError> Warnings { get; set; } = new List<OpenApiError>();

        /// <summary>
        /// Open API specification version of the document parsed.
        /// </summary>
        public OpenApiSpecVersion SpecificationVersion { get; set; }

        /// <summary>
        /// Append another set of diagnostic Errors and Warnings to this one, this may be appended from another external
        /// document's parsing and we want to indicate which file it originated from.
        /// </summary>
        /// <param name="diagnosticToAdd">The diagnostic instance of which the errors and warnings are to be appended to this diagnostic's</param>
        /// <param name="fileNameToAdd">The originating file of the diagnostic to be appended, this is prefixed to each error and warning to indicate the originating file</param>
        public void AppendDiagnostic(OpenApiDiagnostic diagnosticToAdd, string fileNameToAdd = null)
        {
            var fileNameIsSupplied = !string.IsNullOrEmpty(fileNameToAdd);
            foreach (var err in diagnosticToAdd.Errors)
            {
                var errMsgWithFileName = fileNameIsSupplied ? $"[File: {fileNameToAdd}] {err.Message}" : err.Message;
                Errors.Add(new(err.Pointer, errMsgWithFileName));
            }
            foreach (var warn in diagnosticToAdd.Warnings)
            {
                var warnMsgWithFileName = fileNameIsSupplied ? $"[File: {fileNameToAdd}] {warn.Message}" : warn.Message;
                Warnings.Add(new(warn.Pointer, warnMsgWithFileName));
            }
        }
    }
}

/// <summary>
/// Extension class for IList to add the Method "AddRange" used above
/// </summary>
public static class IDiagnosticExtensions
{
    /// <summary>
    /// Extension method for IList so that another list can be added to the current list.
    /// </summary>
    /// <param name="collection"></param>
    /// <param name="enumerable"></param>
    /// <typeparam name="T"></typeparam>
    public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> enumerable)
    {
        if (collection is null || enumerable is null) return;

        foreach (var cur in enumerable)
        {
            collection.Add(cur);
        }
    }
}
