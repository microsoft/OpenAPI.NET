using System.Collections.Generic;

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
        /// Specifies a comparer used to sort string-based collection keys, such as components or tags.
        /// </summary>  
        public IComparer<string>? KeyComparer { get; set; }
    }
}
