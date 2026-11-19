namespace Microsoft.OpenApi
{
    /// <summary>
    /// Configuration settings to control how OpenAPI documents are written
    /// </summary>
    public class OpenApiWriterSettings
    {
        private const int DefaultMaxInlineReferenceCount = 10_000;

        internal LoopDetector LoopDetector { get; } = new();

        /// <summary>
        /// Indicates if local references should be rendered as an inline object
        /// </summary>
        public bool InlineLocalReferences { get; set; }

        /// <summary>
        /// Indicates if external references should be rendered as an inline object
        /// </summary>
        public bool InlineExternalReferences { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of references that can be expanded inline by a writer.
        /// </summary>
        /// <remarks>The default is 10,000.</remarks>
        public int MaxInlineReferenceCount { get; set; } = DefaultMaxInlineReferenceCount;

        internal bool ShouldInlineReference(BaseOpenApiReference reference)
        {
            return (reference.IsLocal && InlineLocalReferences)
                             || (reference.IsExternal && InlineExternalReferences);
        }
    }
}
