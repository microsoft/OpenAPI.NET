// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
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
            new(
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

        private static readonly Regex regexPath = new Regex("\\{([^/]+)\\}", RegexOptions.Compiled, TimeSpan.FromMilliseconds(100));
        /// <summary>
        /// A relative path to an individual endpoint. The field name MUST begin with a slash.
        /// </summary>
        public static ValidationRule<OpenApiPaths> PathMustBeUnique =>
            new ValidationRule<OpenApiPaths>(
                (context, item) =>
                {
                    var hashSet = new HashSet<string>();

                    foreach (var path in item.Keys)
                    {
                        context.Enter(path);

                        var pathSignature = regexPath.Replace(path, "{}");
                        
                        if (!hashSet.Add(pathSignature))
                            context.CreateError(nameof(PathMustBeUnique),
                                string.Format(SRResource.Validation_PathSignatureMustBeUnique, pathSignature));

                        context.Exit();
                    }
                });

        // add more rules
    }
}
