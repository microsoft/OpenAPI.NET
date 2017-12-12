// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Validations.Rules
{
    /// <summary>
    /// The validation rules for <see cref="OpenApiDocument"/>.
    /// </summary>
    internal static class OpenApiDocumentRules
    {
        /// <summary>
        /// The Info field is required.
        /// </summary>
        public static readonly ValidationRule<OpenApiDocument> InfoIsRequired =
            new ValidationRule<OpenApiDocument>(
                (context, item) =>
                {
                    if (item.Info == null)
                    {
                        //context.AddError();
                    }
                });

        /// <summary>
        /// The Paths field is required.
        /// </summary>
        public static readonly ValidationRule<OpenApiDocument> PathsIsRequired =
            new ValidationRule<OpenApiDocument>(
                (context, item) =>
                {
                    if (item.Paths == null)
                    {
                        //context.AddError();
                    }
                });
    }
}
