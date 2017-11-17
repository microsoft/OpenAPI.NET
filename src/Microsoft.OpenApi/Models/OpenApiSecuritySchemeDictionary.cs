// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;

namespace Microsoft.OpenApi.Models
{
    /// <summary>
    /// Dictionary with OpenApiSecurityScheme as key and list of scopes as value.
    /// </summary>
    public class OpenApiSecuritySchemeDictionary :
        Dictionary<OpenApiSecurityScheme, List<string>>
    {
        /// <summary>
        /// Initializes the <see cref="OpenApiSecuritySchemeDictionary"/> class.
        /// This constructor ensures that only Reference.Id is considered when two dictionary keys
        /// of type <see cref="OpenApiSecurityScheme"/> are compared.
        /// </summary>
        public OpenApiSecuritySchemeDictionary()
            : base((new OpenApiSecuritySchemeReferenceEqualityComparer()))
        {
        }

        /// <summary>
        /// Comparer for OpenApiSecurityScheme that only considers the Id in the Reference
        /// (i.e. the string that will actually be displayed in the written document)
        /// </summary>
        private class OpenApiSecuritySchemeReferenceEqualityComparer : IEqualityComparer<OpenApiSecurityScheme>
        {
            /// <summary>
            /// Determines whether the specified objects are equal.
            /// </summary>
            public bool Equals(OpenApiSecurityScheme x, OpenApiSecurityScheme y)
            {
                if (x == null && y == null)
                {
                    return true;
                }

                if (x == null || y == null)
                {
                    return false;
                }

                if (x.Reference == null || y.Reference == null)
                {
                    return false;
                }

                return x.Reference.Id == y.Reference.Id;
            }

            /// <summary>
            /// Returns a hash code for the specified object.
            /// </summary>
            public int GetHashCode(OpenApiSecurityScheme obj)
            {
                return obj?.Reference?.Id == null ? 0 : obj.Reference.Id.GetHashCode();
            }
        }
    }
}