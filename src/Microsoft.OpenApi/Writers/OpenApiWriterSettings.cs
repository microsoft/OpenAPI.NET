namespace Microsoft.OpenApi
{
    /// <summary>
    /// Configuration settings to control how OpenAPI documents are written
    /// </summary>
    public class OpenApiWriterSettings
    {
        internal LoopDetector LoopDetector { get; } = new();
        /// <summary>
        /// Indicates if local references should be rendered as an inline object
        /// </summary>
        public bool InlineLocalReferences { get; set; }

        /// <summary>
        /// Indicates if external references should be rendered as an inline object
        /// </summary>
        public bool InlineExternalReferences { get; set; }

        internal bool ShouldInlineReference(OpenApiReference reference)
        {
            return (reference.IsLocal && InlineLocalReferences)
                             || (reference.IsExternal && InlineExternalReferences);
        }

        /// <summary>  
        /// Custom comparer for sorting collections.  
        /// </summary>  
        public IComparer<string>? Comparer { get; set; }
    }
}
