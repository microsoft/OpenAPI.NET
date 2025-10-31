// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

namespace Microsoft.OpenApi
{
    /// <summary>
    /// The type of the XML node
    /// </summary>
    public enum OpenApiXmlNodeType
    {
        /// <summary>
        /// Element node type
        /// </summary>
        [Display("element")] Element,

        /// <summary>
        /// Attribute node type
        /// </summary>
        [Display("attribute")] Attribute,

        /// <summary>
        /// Text node type
        /// </summary>
        [Display("text")] Text,

        /// <summary>
        /// CDATA node type
        /// </summary>
        [Display("cdata")] Cdata,

        /// <summary>
        /// None node type
        /// </summary>
        [Display("none")] None
    }
}
