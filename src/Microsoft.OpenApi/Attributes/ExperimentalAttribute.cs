// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

// Polyfill for ExperimentalAttribute which is only available in .NET 8+.
// Since the compiler queries for this attribute by name, having it source-included
// is sufficient for the compiler to recognize it.
#if !NET8_0_OR_GREATER
namespace System.Diagnostics.CodeAnalysis
{
    /// <summary>
    /// Indicates that an API is experimental and it may change in the future.
    /// </summary>
    /// <remarks>
    /// This attribute allows call sites to be flagged with a diagnostic that indicates that an experimental
    /// feature is used. Authors can use this attribute to ship preview features in their assemblies.
    /// </remarks>
    [AttributeUsage(
        AttributeTargets.Assembly | AttributeTargets.Module | AttributeTargets.Class | AttributeTargets.Struct |
        AttributeTargets.Enum | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property |
        AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Interface | AttributeTargets.Delegate,
        Inherited = false)]
    internal sealed class ExperimentalAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExperimentalAttribute"/> class,
        /// specifying the ID that the compiler will use when reporting a use of the API.
        /// </summary>
        /// <param name="diagnosticId">The ID that the compiler will use when reporting a use of the API.</param>
        public ExperimentalAttribute(string diagnosticId)
        {
            DiagnosticId = diagnosticId;
        }

        /// <summary>
        /// Gets the ID that the compiler will use when reporting a use of the API.
        /// </summary>
        public string DiagnosticId { get; }

        /// <summary>
        /// Gets or sets the URL for corresponding documentation.
        /// The API accepts a format string instead of an actual URL, creating a generic URL that includes the diagnostic ID.
        /// </summary>
        public string? UrlFormat { get; set; }
    }
}
#endif
