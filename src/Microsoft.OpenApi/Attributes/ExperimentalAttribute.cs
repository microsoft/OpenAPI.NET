// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

#if !NET8_0_OR_GREATER
namespace System.Diagnostics.CodeAnalysis
{
    /// <summary>
    /// Indicates that an API is experimental and may change in the future.
    /// </summary>
    [AttributeUsage(
        AttributeTargets.Assembly |
        AttributeTargets.Module |
        AttributeTargets.Class |
        AttributeTargets.Struct |
        AttributeTargets.Enum |
        AttributeTargets.Constructor |
        AttributeTargets.Method |
        AttributeTargets.Property |
        AttributeTargets.Field |
        AttributeTargets.Event |
        AttributeTargets.Interface |
        AttributeTargets.Delegate,
        Inherited = false)]
    internal sealed class ExperimentalAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExperimentalAttribute"/> class.
        /// </summary>
        /// <param name="diagnosticId">The diagnostic ID reported when the API is used.</param>
        public ExperimentalAttribute(string diagnosticId)
        {
            DiagnosticId = diagnosticId;
        }

        /// <summary>
        /// Gets the diagnostic ID reported when the API is used.
        /// </summary>
        public string DiagnosticId { get; }

        /// <summary>
        /// Gets or sets the URL format used by the diagnostic.
        /// </summary>
        public string? UrlFormat { get; set; }
    }
}
#endif
