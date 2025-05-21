using System.Collections.Generic;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Services;

namespace Microsoft.OpenApi.Writers
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
        /// Enables sorting of collections using the default comparer  
        /// </summary>  
        public bool EnableSorting { get; set; }

        /// <summary>  
        /// Custom comparer for sorting collections.  
        /// </summary>  
        public IComparer<string>? KeyComparer { get; set; }
    }
}
