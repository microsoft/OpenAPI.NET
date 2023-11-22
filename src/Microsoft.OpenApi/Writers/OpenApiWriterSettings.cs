
using System;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Services;

namespace Microsoft.OpenApi.Writers
{
    /// <summary>
    /// Indicates if and when the writer should convert references into complete object renderings
    /// </summary>
    [Obsolete("Use InlineLocalReference and InlineExternalReference settings instead")]
    public enum ReferenceInlineSetting
    {
        /// <summary>
        /// Render all references as $ref.
        /// </summary>
        DoNotInlineReferences,
        /// <summary>
        /// Render all local references as inline objects
        /// </summary>
        InlineLocalReferences,
        /// <summary>
        /// Render all references as inline objects.
        /// </summary>
        InlineAllReferences
    }

    /// <summary>
    /// Configuration settings to control how OpenAPI documents are written
    /// </summary>
    public class OpenApiWriterSettings
    {
        [Obsolete("Use InlineLocalReference and InlineExternalReference settings instead")]
        private ReferenceInlineSetting referenceInline = ReferenceInlineSetting.DoNotInlineReferences;

        internal LoopDetector LoopDetector { get; } = new();
        /// <summary>
        /// Indicates how references in the source document should be handled.
        /// </summary>
        [Obsolete("Use InlineLocalReference and InlineExternalReference settings instead")]
        public ReferenceInlineSetting ReferenceInline
        {
            get { return referenceInline; }
            set
            {
                referenceInline = value;
                switch (referenceInline)
                {
                    case ReferenceInlineSetting.DoNotInlineReferences:
                        InlineLocalReferences = false;
                        InlineExternalReferences = false;
                        break;
                    case ReferenceInlineSetting.InlineLocalReferences:
                        InlineLocalReferences = true;
                        InlineExternalReferences = false;
                        break;
                    case ReferenceInlineSetting.InlineAllReferences:
                        InlineLocalReferences = true;
                        InlineExternalReferences = true;
                        break;
                }
            }
        }
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

        internal bool ShouldInlineReference()
        {
            return InlineLocalReferences || InlineExternalReferences;
        }
    }
}
