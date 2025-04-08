// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Properties;

namespace Microsoft.OpenApi.Validations.Rules
{
    /// <summary>
    /// The validation rules for <see cref="OpenApiPaths"/>.
    /// </summary>
    [OpenApiRule]
    public static class OpenApiPathsRules
    {
        /// <summary>
        /// A relative path to an individual endpoint. The field name MUST begin with a slash.
        /// </summary>
        public static ValidationRule<OpenApiPaths> PathNameMustBeginWithSlash =>
            new(nameof(PathNameMustBeginWithSlash),
                (context, item) =>
                {
                    foreach (var pathName in item.Keys)
                    {
                        context.Enter(pathName);

                        if (pathName == null || !pathName.StartsWith("/", StringComparison.OrdinalIgnoreCase))
                        {
                            context.CreateError(nameof(PathNameMustBeginWithSlash),
                                string.Format(SRResource.Validation_PathItemMustBeginWithSlash, pathName));
                        }

                        context.Exit();
                    }
                });

        /// <summary>
        /// A relative path to an individual endpoint. The field name MUST begin with a slash.
        /// </summary>
        public static ValidationRule<OpenApiPaths> PathMustBeUnique =>
            new ValidationRule<OpenApiPaths>(nameof(PathMustBeUnique),
                (context, item) =>
                {
                    var hashSet = new HashSet<string>();

                    foreach (var path in item.Keys)
                    {
                        context.Enter(path);

                        var pathSignature = GetPathSignature(path);
                        
                        if (!hashSet.Add(pathSignature))
                            context.CreateError(nameof(PathMustBeUnique),
                                string.Format(SRResource.Validation_PathSignatureMustBeUnique, pathSignature));

                        context.Exit();
                    }
                });

        /// <summary>
        ///  Replaces placeholders in the path with {}, e.g. /pets/{petId} becomes /pets/{} .
        /// </summary>
        /// <param name="path">The input path</param>
        /// <returns>The path signature</returns>
        private static string GetPathSignature(string path)
        {
            for (int openBrace = path.IndexOf('{'); openBrace > -1; openBrace = path.IndexOf('{', openBrace + 2))
            {
                int closeBrace = path.IndexOf('}', openBrace);

                if (closeBrace < 0)
                {
                    return path;
                }

                path = path.Substring(0, openBrace + 1) + path.Substring(closeBrace);
            }

            return path;
        }

        // add more rules
    }
}
