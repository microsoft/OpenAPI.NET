// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Properties;

namespace Microsoft.OpenApi
{
    /// <summary>
    /// Extension methods
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Add extension into the Extensions
        /// </summary>
        /// <typeparam name="T"><see cref="IOpenApiExtension"/>.</typeparam>
        /// <param name="element">The extensible Open API element. </param>
        /// <param name="name">The extension name.</param>
        /// <param name="any">The extension value.</param>
        public static void AddExtension<T>(this T element, string name, IOpenApiAny any)
            where T : IOpenApiExtension
        {
            if (element == null)
            {
                throw Error.ArgumentNull(nameof(element));
            }

            VerifyExtensionName(name);

            if (element.Extensions == null)
            {
                element.Extensions = new Dictionary<string, IOpenApiAny>();
            }

            element.Extensions[name] = any ?? throw Error.ArgumentNull(nameof(any));
        }

        private static void VerifyExtensionName(string name)
        {
            if (String.IsNullOrWhiteSpace(name))
            {
                throw Error.ArgumentNullOrWhiteSpace(nameof(name));
            }

            if (!name.StartsWith(OpenApiConstants.ExtensionFieldNamePrefix))
            {
                throw new OpenApiException(String.Format(SRResource.ExtensionFieldNameMustBeginWithXDash, name));
            }
        }
    }
}
