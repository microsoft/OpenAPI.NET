// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OpenApi.Validations.Visitors
{
    /// <summary>
    /// 
    /// </summary>
    internal static class OpenApiVisitorSet
    {
        private static IDictionary<Type, IVisitor> _elementVisitor = new Dictionary<Type, IVisitor>
        {
            { typeof(OpenApiDocument), new DocumentVisitor() },
            { typeof(OpenApiInfo), new InfoVisitor() },
            { typeof(OpenApiTag), new TagVisitor() },
            { typeof(OpenApiLicense), new LicenseVisitor() },
            { typeof(OpenApiContact), new ContactVisitor() },

            // add more
        };

        /// <summary>
        /// Get the element visitor.
        /// </summary>
        /// <param name="elementType">The element type.</param>
        /// <returns>The element visitor or null.</returns>
        public static IVisitor GetVisitor(Type elementType)
        {
            var visitor = _elementVisitor.FirstOrDefault(c => c.Key == elementType).Value;
            if (visitor != null)
            {
                return visitor;
            }

            return null;
        }
    }
}
